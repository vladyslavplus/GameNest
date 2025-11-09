using Pulumi;
using System.Collections.Generic;
using Keycloak = Pulumi.Keycloak;

return await Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config("gamenest");
    var realmName = config.Require("realmName");

    var realm = new Keycloak.Realm("gamenest-realm", new Keycloak.RealmArgs
    {
        RealmName = realmName,
        Enabled = true,
        DisplayName = "GameNest Platform",
        RegistrationAllowed = true,
        RegistrationEmailAsUsername = true,
        VerifyEmail = true,
        LoginWithEmailAllowed = true,
        ResetPasswordAllowed = true,
        RememberMe = true,
        SsoSessionIdleTimeout = "30m",
        SsoSessionMaxLifespan = "10h",
        AccessTokenLifespan = "5m",
        AccessTokenLifespanForImplicitFlow = "15m",
    });

    // ===== CREATE REALM ROLES =====
    var adminRole = new Keycloak.Role("admin-role", new Keycloak.RoleArgs
    {
        RealmId = realm.Id,
        Name = "admin",
        Description = "Administrator with full access",
    });

    var managerRole = new Keycloak.Role("manager-role", new Keycloak.RoleArgs
    {
        RealmId = realm.Id,
        Name = "manager",
        Description = "Manager with approval rights",
    });

    var userRole = new Keycloak.Role("user-role", new Keycloak.RoleArgs
    {
        RealmId = realm.Id,
        Name = "user",
        Description = "Regular user",
    });

    // ===== CREATE CLIENT SCOPE FOR API =====
    var gameNestApiScope = new Keycloak.OpenId.ClientScope("gamenest-api-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "gamenest_api",
        Description = "GameNest API Access",
        IncludeInTokenScope = true,
    });

    var audienceMapper = new Keycloak.OpenId.AudienceProtocolMapper("api-audience-mapper", new Keycloak.OpenId.AudienceProtocolMapperArgs
    {
        RealmId = realm.Id,
        ClientScopeId = gameNestApiScope.Id,
        Name = "api-audience",
        IncludedClientAudience = "gamenest_api",
        AddToAccessToken = true,
    });

    var userIdMapper = new Keycloak.OpenId.UserPropertyProtocolMapper("user-id-mapper", new Keycloak.OpenId.UserPropertyProtocolMapperArgs
    {
        RealmId = realm.Id,
        ClientScopeId = gameNestApiScope.Id,
        Name = "user-id",
        UserProperty = "id",
        ClaimName = "sub",
        ClaimValueType = "String",
        AddToIdToken = true,
        AddToAccessToken = true,
        AddToUserinfo = true,
    });

    // ===== CREATE CLIENT SCOPES FOR CATALOG PERMISSIONS =====
    var catalogWriteScope = new Keycloak.OpenId.ClientScope("catalog-write-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "catalog:write",
        Description = "Write to catalog",
        IncludeInTokenScope = true,
    });

    var catalogDeleteScope = new Keycloak.OpenId.ClientScope("catalog-delete-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "catalog:delete",
        Description = "Delete from catalog",
        IncludeInTokenScope = true,
    });

    // ===== CREATE CLIENT SCOPES FOR ORDER PERMISSIONS =====
    var ordersReadScope = new Keycloak.OpenId.ClientScope("orders-read-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "orders:read",
        Description = "Read user's orders",
        IncludeInTokenScope = true,
    });

    var ordersCreateScope = new Keycloak.OpenId.ClientScope("orders-create-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "orders:create",
        Description = "Create new orders",
        IncludeInTokenScope = true,
    });

    var ordersUpdateScope = new Keycloak.OpenId.ClientScope("orders-update-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "orders:update",
        Description = "Update existing orders (status, etc.)",
        IncludeInTokenScope = true,
    });

    var ordersDeleteScope = new Keycloak.OpenId.ClientScope("orders-delete-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "orders:delete",
        Description = "Delete existing orders",
        IncludeInTokenScope = true,
    });

    // ===== CREATE CLIENT SCOPES FOR PAYMENT PERMISSIONS =====
    var paymentsReadScope = new Keycloak.OpenId.ClientScope("payments-read-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "payments:read",
        Description = "Read user's payments",
        IncludeInTokenScope = true,
    });

    var paymentsCreateScope = new Keycloak.OpenId.ClientScope("payments-create-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "payments:create",
        Description = "Create payment records",
        IncludeInTokenScope = true,
    });

    var paymentsUpdateScope = new Keycloak.OpenId.ClientScope("payments-update-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "payments:update",
        Description = "Update payment records",
        IncludeInTokenScope = true,
    });

    var paymentsDeleteScope = new Keycloak.OpenId.ClientScope("payments-delete-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "payments:delete",
        Description = "Delete payment records",
        IncludeInTokenScope = true,
    });

    var reviewsWriteScope = new Keycloak.OpenId.ClientScope("reviews-write-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "reviews:write",
        Description = "Write and update reviews",
        IncludeInTokenScope = true,
    });

    var reviewsUpdateScope = new Keycloak.OpenId.ClientScope("reviews-update-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "reviews:update",
        Description = "Update reviews or comments",
        IncludeInTokenScope = true,
    });

    var reviewsDeleteScope = new Keycloak.OpenId.ClientScope("reviews-delete-scope", new Keycloak.OpenId.ClientScopeArgs
    {
        RealmId = realm.Id,
        Name = "reviews:delete",
        Description = "Delete reviews",
        IncludeInTokenScope = true,
    });

    // ===== CREATE SWAGGER CLIENT =====
    var swaggerClient = new Keycloak.OpenId.Client("swagger-client", new Keycloak.OpenId.ClientArgs
    {
        RealmId = realm.Id,
        ClientId = "swagger",
        Name = "Swagger UI",
        Enabled = true,
        AccessType = "PUBLIC",
        StandardFlowEnabled = true,
        DirectAccessGrantsEnabled = false,
        ImplicitFlowEnabled = false,
        ValidRedirectUris = new[]
        {
            // Catalog Service
            "https://localhost:7048/swagger/oauth2-redirect.html",
            "https://localhost:7046/swagger/oauth2-redirect.html",

            // Review Service
            "https://localhost:7047/swagger/oauth2-redirect.html",
            "https://localhost:7260/swagger/oauth2-redirect.html",

            // Order Service
            "https://localhost:7045/swagger/oauth2-redirect.html",
            "https://localhost:7122/swagger/oauth2-redirect.html",

            // Cart Service
            "https://localhost:7050/swagger/oauth2-redirect.html",
            "https://localhost:7162/swagger/oauth2-redirect.html",

            // Aggregator Service
            "https://localhost:7049/swagger/oauth2-redirect.html",
            "https://localhost:7111/swagger/oauth2-redirect.html",
        },
        WebOrigins = new[] { "*" },
        PkceCodeChallengeMethod = "S256",
    });

    var swaggerDefaultScopes = new Keycloak.OpenId.ClientDefaultScopes("swagger-default-scopes", new Keycloak.OpenId.ClientDefaultScopesArgs
    {
        RealmId = realm.Id,
        ClientId = swaggerClient.Id,
        DefaultScopes = new InputList<string>
        {
            "openid",
            "profile",
            "email",
            "gamenest_api"
        },
    });

    var swaggerOptionalScopes = new Keycloak.OpenId.ClientOptionalScopes("swagger-optional-scopes", new Keycloak.OpenId.ClientOptionalScopesArgs
    {
        RealmId = realm.Id,
        ClientId = swaggerClient.Id,
        OptionalScopes = new InputList<string>
        {
            "catalog:write", "catalog:delete",
            "orders:read", "orders:create", "orders:update", "orders:delete",
            "payments:read", "payments:create", "payments:update", "payments:delete",
            "reviews:write", "reviews:update", "reviews:delete"
        },
    });

    var postmanClient = new Keycloak.OpenId.Client("postman-client", new Keycloak.OpenId.ClientArgs
    {
        RealmId = realm.Id,
        ClientId = "postman",
        Name = "Postman Client",
        Enabled = true,
        AccessType = "CONFIDENTIAL",
        StandardFlowEnabled = false,
        DirectAccessGrantsEnabled = true, // Resource Owner Password Flow for Postman
        ImplicitFlowEnabled = false,
        ServiceAccountsEnabled = false,
        ClientSecret = "postman-secret-123",
    });

    var postmanDefaultScopes = new Keycloak.OpenId.ClientDefaultScopes("postman-default-scopes", new Keycloak.OpenId.ClientDefaultScopesArgs
    {
        RealmId = realm.Id,
        ClientId = postmanClient.Id,
        DefaultScopes = new InputList<string>
        {
            "openid",
            "profile",
            "email",
            "gamenest_api"
        },
    });

    var postmanOptionalScopes = new Keycloak.OpenId.ClientOptionalScopes("postman-optional-scopes", new Keycloak.OpenId.ClientOptionalScopesArgs
    {
        RealmId = realm.Id,
        ClientId = postmanClient.Id,
        OptionalScopes = new InputList<string>
        {
            "catalog:write", "catalog:delete",
            "orders:read", "orders:create", "orders:update", "orders:delete",
            "payments:read", "payments:create", "payments:update", "payments:delete",
            "reviews:write", "reviews:update", "reviews:delete"
        },
    });

    // ===== CREATE API CLIENT =====
    var apiClient = new Keycloak.OpenId.Client("api-client", new Keycloak.OpenId.ClientArgs
    {
        RealmId = realm.Id,
        ClientId = "gamenest_api",
        Name = "GameNest API",
        Enabled = true,
        AccessType = "CONFIDENTIAL",
        StandardFlowEnabled = false,
        DirectAccessGrantsEnabled = false,
        ServiceAccountsEnabled = true,
        ClientSecret = "gamenest-api-secret-change-me",
    });

    // ===== CREATE TEST USERS =====
    var adminUser = new Keycloak.User("admin-user", new Keycloak.UserArgs
    {
        RealmId = realm.Id,
        Username = "admin@gamenest.local",
        Email = "admin@gamenest.local",
        EmailVerified = true,
        Enabled = true,
        FirstName = "Admin",
        LastName = "User",
        InitialPassword = new Keycloak.Inputs.UserInitialPasswordArgs
        {
            Value = "Admin123!",
            Temporary = false,
        },
    });

    var adminUserRoles = new Keycloak.UserRoles("admin-user-roles", new Keycloak.UserRolesArgs
    {
        RealmId = realm.Id,
        UserId = adminUser.Id,
        RoleIds = new[] { adminRole.Id },
    });

    var managerUser = new Keycloak.User("manager-user", new Keycloak.UserArgs
    {
        RealmId = realm.Id,
        Username = "manager@gamenest.local",
        Email = "manager@gamenest.local",
        EmailVerified = true,
        Enabled = true,
        FirstName = "Manager",
        LastName = "User",
        InitialPassword = new Keycloak.Inputs.UserInitialPasswordArgs
        {
            Value = "Manager123!",
            Temporary = false,
        },
    });

    var managerUserRoles = new Keycloak.UserRoles("manager-user-roles", new Keycloak.UserRolesArgs
    {
        RealmId = realm.Id,
        UserId = managerUser.Id,
        RoleIds = new[] { managerRole.Id },
    });

    var regularUser = new Keycloak.User("regular-user", new Keycloak.UserArgs
    {
        RealmId = realm.Id,
        Username = "user@gamenest.local",
        Email = "user@gamenest.local",
        EmailVerified = true,
        Enabled = true,
        FirstName = "Regular",
        LastName = "User",
        InitialPassword = new Keycloak.Inputs.UserInitialPasswordArgs
        {
            Value = "User123!",
            Temporary = false,
        },
    });

    var regularUserRoles = new Keycloak.UserRoles("regular-user-roles", new Keycloak.UserRolesArgs
    {
        RealmId = realm.Id,
        UserId = regularUser.Id,
        RoleIds = new[] { userRole.Id },
    });

    return new Dictionary<string, object?>
    {
        ["RealmName"] = realm.RealmName,
        ["RealmUrl"] = Output.Format($"http://localhost:8080/realms/{realm.RealmName}"),
        ["AdminConsoleUrl"] = "http://localhost:8080/admin/master/console/#/GameNest",
        ["SwaggerClientId"] = swaggerClient.ClientId,
        ["ApiClientId"] = apiClient.ClientId,
        ["TestUsers"] = @"
        Admin:   admin@gamenest.local / Admin123!
        Manager: manager@gamenest.local / Manager123!
        User:    user@gamenest.local / User123!
        ",
    };
});