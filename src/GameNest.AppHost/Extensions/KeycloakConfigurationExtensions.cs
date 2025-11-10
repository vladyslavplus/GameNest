using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GameNest.AppHost.Extensions
{
    public static class KeycloakConfigurationExtensions
    {
        private const string KeycloakBaseUrl = "http://localhost:8080";
        private const string KeycloakAdminUsername = "admin";
        private const string KeycloakAdminPassword = "admin";
        private const string KeycloakAdminClient = "admin-cli";
        private const string RealmName = "GameNest";

        private const string PulumiStackName = "dev";
        private const string InfrastructureFolderName = "infrastructure";
        private const string PulumiConfigFileName = "Pulumi.dev.yaml";

        private const string ConfigureScriptName = "configure-keycloak.ps1";

        private const int PostDeploymentDelayMs = 3000;
        private const int MaxParentDirectoryLevels = 5;

        private const string LoggerCategoryName = "KeycloakAutoConfig";

        /// <summary>
        /// Automatically configures Keycloak realm using Pulumi when the container starts.
        /// Only runs if configuration doesn't exist yet (checks for realm existence).
        /// </summary>
        public static IResourceBuilder<T> WithAutoConfiguration<T>(
            this IResourceBuilder<T> builder) where T : ContainerResource
        {
            var solutionRoot = FindSolutionRoot(builder.ApplicationBuilder.Environment.ContentRootPath);

            if (solutionRoot == null)
            {
                LogWarning(builder, "Could not find infrastructure folder - skipping auto-configuration");
                return builder;
            }

            builder.ApplicationBuilder.Eventing.Subscribe<ResourceReadyEvent>(builder.Resource,
                async (@event, ct) => await ConfigureKeycloakAsync(builder, solutionRoot, ct));

            return builder;
        }

        private static async Task ConfigureKeycloakAsync<T>(
            IResourceBuilder<T> builder,
            string solutionRoot,
            CancellationToken ct) where T : ContainerResource
        {
            ILogger? logger = null;
            try
            {
                logger = CreateLogger(builder);

                logger.LogInformation("Waiting for Keycloak to become available...");

                var ready = await WaitForKeycloakAsync(logger, ct);
                if (!ready)
                {
                    logger.LogWarning("Keycloak did not become ready in time. Skipping auto configuration.");
                    return;
                }

                if (await IsKeycloakAlreadyConfiguredAsync(logger, ct))
                {
                    logger.LogInformation("Keycloak is already configured - skipping auto-configuration");
                    return;
                }

                logger.LogInformation("Keycloak not configured yet - starting auto-configuration...");

                var infrastructurePath = Path.Combine(solutionRoot, InfrastructureFolderName);

                await EnsurePulumiStackExistsAsync(infrastructurePath, logger, ct);

                logger.LogInformation("Deploying Keycloak configuration via Pulumi...");
                var success = await RunProcessAsync("pulumi", "up --yes --skip-preview",
                    infrastructurePath, logger, ct);

                if (!success)
                {
                    logger.LogWarning("Pulumi deployment failed — check logs above.");
                    return;
                }

                await Task.Delay(PostDeploymentDelayMs, ct);

                logger.LogInformation("Running configure-keycloak.ps1 for role-to-scope mappings...");
                var scriptPath = Path.Combine(solutionRoot, ConfigureScriptName);
                var scriptArgs = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"";

                var scriptSuccess = await RunProcessAsync("powershell", scriptArgs, solutionRoot, logger, ct);

                if (!scriptSuccess)
                {
                    logger.LogWarning("configure-keycloak.ps1 execution failed.");
                    return;
                }

                logger.LogInformation("Keycloak auto-configuration completed successfully!");
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Keycloak auto-config failed.");
            }
        }

        private static async Task EnsurePulumiStackExistsAsync(
            string infrastructurePath,
            ILogger logger,
            CancellationToken ct)
        {
            if (await CheckPulumiStackExistsAsync(infrastructurePath, ct))
            {
                return;
            }

            logger.LogInformation("Initializing Pulumi stack...");

            // Initialize stack
            await RunProcessAsync("pulumi", $"stack init {PulumiStackName}",
                infrastructurePath, logger, ct);

            // Create config
            var configPath = Path.Combine(infrastructurePath, PulumiConfigFileName);
            var configContent = $@"config:
                  keycloak:url: {KeycloakBaseUrl}
                  keycloak:username: {KeycloakAdminUsername}
                  keycloak:password: {KeycloakAdminPassword}
                  keycloak:clientId: {KeycloakAdminClient}
                  gamenest:realmName: {RealmName}
                ";
            await File.WriteAllTextAsync(configPath, configContent, ct);
        }

        private static async Task<bool> CheckPulumiStackExistsAsync(
            string infrastructurePath,
            CancellationToken ct)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pulumi",
                    Arguments = "stack ls --json",
                    WorkingDirectory = infrastructurePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            return output.Contains($"\"name\":\"{PulumiStackName}\"");
        }

        /// <summary>
        /// Checks if Keycloak realm is already configured by attempting to fetch realm info.
        /// </summary>
        private static async Task<bool> IsKeycloakAlreadyConfiguredAsync(ILogger logger, CancellationToken ct)
        {
            try
            {
                using var httpClient = new HttpClient();
                var realmUrl = $"{KeycloakBaseUrl}/realms/{RealmName}";

                var response = await httpClient.GetAsync(realmUrl, ct);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogDebug($"{RealmName} realm exists - configuration present");
                    return true;
                }

                logger.LogDebug($"{RealmName} realm not found - needs configuration");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Could not check realm existence - assuming not configured");
                return false;
            }
        }

        /// <summary>
        /// Finds solution root by looking for infrastructure folder in parent directories.
        /// </summary>
        private static string? FindSolutionRoot(string startPath)
        {
            var currentDir = new DirectoryInfo(startPath);

            for (int i = 0; i < MaxParentDirectoryLevels && currentDir != null; i++)
            {
                var infrastructurePath = Path.Combine(currentDir.FullName, InfrastructureFolderName);
                if (Directory.Exists(infrastructurePath))
                {
                    return currentDir.FullName;
                }
                currentDir = currentDir.Parent;
            }

            return null;
        }

        private static async Task<bool> RunProcessAsync(
            string fileName,
            string arguments,
            string workingDirectory,
            ILogger logger,
            CancellationToken ct)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    logger.LogDebug(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    logger.LogWarning(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(ct);

            return process.ExitCode == 0;
        }

        private static ILogger CreateLogger<T>(IResourceBuilder<T> builder) where T : ContainerResource
        {
            var loggerFactory = builder.ApplicationBuilder.Services
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();

            return loggerFactory.CreateLogger(LoggerCategoryName);
        }

        private static void LogWarning<T>(IResourceBuilder<T> builder, string message) where T : ContainerResource
        {
            var logger = CreateLogger(builder);
            logger.LogWarning(message);
        }

        private static async Task<bool> WaitForKeycloakAsync(ILogger logger, CancellationToken ct)
        {
            using var httpClient = new HttpClient();
            var healthUrl = $"{KeycloakBaseUrl}/realms/master/.well-known/openid-configuration";

            const int maxAttempts = 30;
            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    var response = await httpClient.GetAsync(healthUrl, ct);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Keycloak is ready");
                        return true;
                    }
                }
                catch
                {
                    // ignore exceptions (Keycloak not up yet)
                }

                logger.LogInformation($"Waiting for Keycloak... attempt {i}/{maxAttempts}");
                await Task.Delay(2000, ct);
            }

            return false;
        }
    }
}