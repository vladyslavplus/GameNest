using Bogus;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.TestData;
using Microsoft.EntityFrameworkCore;

namespace GameNest.CatalogService.DAL.Data.Seed
{
    public static class CatalogDbSeeder
    {
        public static async Task SeedAsync(CatalogDbContext context)
        {
            var now = DateTime.UtcNow;
            var random = new Random();

            if (!await context.Genres.AnyAsync())
            {
                var genres = new List<Genre>
                {
                    new Genre { Id = Guid.Parse(SharedSeedData.Games.TheWitcher3), Name = "RPG", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = Guid.Parse(SharedSeedData.Games.DoomEternal), Name = "Shooter", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = Guid.Parse(SharedSeedData.Games.StardewValley), Name = "Strategy", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = Guid.Parse(SharedSeedData.Games.Cyberpunk2077), Name = "Adventure", CreatedAt = now, UpdatedAt = now }
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            if (!await context.Platforms.AnyAsync())
            {
                var platforms = new List<Platform>
                {
                    new Platform { Id = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "PC", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "PlayStation", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Xbox", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Nintendo Switch", CreatedAt = now, UpdatedAt = now }
                };
                await context.Platforms.AddRangeAsync(platforms);
                await context.SaveChangesAsync();
            }

            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Id = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), Name = "Programmer", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), Name = "Designer", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = Guid.Parse("bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), Name = "Artist", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = Guid.Parse("bbbbbbb4-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), Name = "Producer", CreatedAt = now, UpdatedAt = now }
                };
                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            if (!await context.Developers.AnyAsync())
            {
                var developerFaker = new Faker<Developer>()
                    .RuleFor(d => d.Id, f => Guid.NewGuid())
                    .RuleFor(d => d.FullName, f => f.Name.FullName())
                    .RuleFor(d => d.Email, f => f.Internet.Email())
                    .RuleFor(d => d.Country, f => f.Address.Country())
                    .FinishWith((f, d) => { d.CreatedAt = now; d.UpdatedAt = now; });

                var developers = developerFaker.Generate(20);
                await context.Developers.AddRangeAsync(developers);
                await context.SaveChangesAsync();
            }

            if (!await context.Publishers.AnyAsync())
            {
                var publisherFaker = new Faker<Publisher>()
                    .RuleFor(p => p.Id, f => Guid.NewGuid())
                    .RuleFor(p => p.Name, f => f.Company.CompanyName())
                    .RuleFor(p => p.Type, f => f.PickRandom("Indie", "AAA"))
                    .RuleFor(p => p.Country, f => f.Address.Country())
                    .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber())
                    .FinishWith((f, p) => { p.CreatedAt = now; p.UpdatedAt = now; });

                var publishers = publisherFaker.Generate(5);
                await context.Publishers.AddRangeAsync(publishers);
                await context.SaveChangesAsync();
            }

            if (!await context.Games.AnyAsync())
            {
                var publishers = await context.Publishers.ToListAsync();
                var games = new List<Game>
                {
                    new Game
                    {
                        Id = Guid.Parse(SharedSeedData.Games.TheWitcher3),
                        Title = "The Witcher 3",
                        Description = "RPG with vast open world",
                        Price = 39.99m,
                        ReleaseDate = DateTime.UtcNow.AddYears(-5),
                        PublisherId = publishers[random.Next(publishers.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Game
                    {
                        Id = Guid.Parse(SharedSeedData.Games.DoomEternal),
                        Title = "Doom Eternal",
                        Description = "Fast-paced FPS",
                        Price = 59.99m,
                        ReleaseDate = DateTime.UtcNow.AddYears(-2),
                        PublisherId = publishers[random.Next(publishers.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Game
                    {
                        Id = Guid.Parse(SharedSeedData.Games.StardewValley),
                        Title = "Stardew Valley",
                        Description = "Relaxing farming simulator",
                        Price = 14.99m,
                        ReleaseDate = DateTime.UtcNow.AddYears(-4),
                        PublisherId = publishers[random.Next(publishers.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Game
                    {
                        Id = Guid.Parse(SharedSeedData.Games.Cyberpunk2077),
                        Title = "Cyberpunk 2077",
                        Description = "Futuristic open-world RPG",
                        Price = 49.99m,
                        ReleaseDate = DateTime.UtcNow.AddYears(-1),
                        PublisherId = publishers[random.Next(publishers.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };
                await context.Games.AddRangeAsync(games);
                await context.SaveChangesAsync();
            }

            if (!await context.GameDeveloperRoles.AnyAsync())
            {
                var games = await context.Games.ToListAsync();
                var developers = await context.Developers.ToListAsync();
                var roles = await context.Roles.ToListAsync();

                if (!games.Any() || !developers.Any() || !roles.Any())
                    throw new InvalidOperationException("Cannot create GameDeveloperRoles: one of the lists is empty!");

                var gdrList = new List<GameDeveloperRole>();
                foreach (var game in games)
                {
                    var devsForGame = developers.OrderBy(_ => random.Next()).Take(3).ToList();
                    foreach (var dev in devsForGame)
                    {
                        gdrList.Add(new GameDeveloperRole
                        {
                            Id = Guid.NewGuid(),
                            GameId = game.Id,
                            DeveloperId = dev.Id,
                            RoleId = roles[random.Next(roles.Count)].Id,
                            Seniority = random.Next(0, 2) == 0 ? "Junior" : "Senior",
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                    }
                }
                await context.GameDeveloperRoles.AddRangeAsync(gdrList);
                await context.SaveChangesAsync();
            }

            if (!await context.GameGenres.AnyAsync())
            {
                var games = await context.Games.ToListAsync();
                var genres = await context.Genres.ToListAsync();

                if (!games.Any() || !genres.Any())
                    throw new InvalidOperationException("Cannot create GameGenres: one of the lists is empty!");

                var ggList = new List<GameGenre>();
                foreach (var game in games)
                {
                    ggList.Add(new GameGenre
                    {
                        Id = Guid.NewGuid(),
                        GameId = game.Id,
                        GenreId = genres[random.Next(genres.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
                await context.GameGenres.AddRangeAsync(ggList);
                await context.SaveChangesAsync();
            }

            if (!await context.GamePlatforms.AnyAsync())
            {
                var games = await context.Games.ToListAsync();
                var platforms = await context.Platforms.ToListAsync();

                if (!games.Any() || !platforms.Any())
                    throw new InvalidOperationException("Cannot create GamePlatforms: one of the lists is empty!");

                var gpList = new List<GamePlatform>();
                foreach (var game in games)
                {
                    gpList.Add(new GamePlatform
                    {
                        Id = Guid.NewGuid(),
                        GameId = game.Id,
                        PlatformId = platforms[random.Next(platforms.Count)].Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
                await context.GamePlatforms.AddRangeAsync(gpList);
                await context.SaveChangesAsync();
            }
        }
    }
}