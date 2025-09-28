using Bogus;
using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNest.CatalogService.DAL.Data.Seed
{
    public static class CatalogDbSeeder
    {
        private static readonly Guid RpgId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid ShooterId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid StrategyId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid AdventureId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        private static readonly Guid PcId = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        private static readonly Guid PlayStationId = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
        private static readonly Guid XboxId = Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
        private static readonly Guid SwitchId = Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4");

        private static readonly Guid ProgrammerId = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
        private static readonly Guid DesignerId = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        private static readonly Guid ArtistId = Guid.Parse("bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
        private static readonly Guid ProducerId = Guid.Parse("bbbbbbb4-bbbb-bbbb-bbbb-bbbbbbbbbbb4");

        public static async Task SeedAsync(CatalogDbContext context)
        {
            var now = DateTime.UtcNow;
            var random = new Random();

            if (!await context.Genres.AnyAsync())
            {
                var genres = new List<Genre>
                {
                    new Genre { Id = RpgId, Name = "RPG", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = ShooterId, Name = "Shooter", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = StrategyId, Name = "Strategy", CreatedAt = now, UpdatedAt = now },
                    new Genre { Id = AdventureId, Name = "Adventure", CreatedAt = now, UpdatedAt = now }
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            if (!await context.Platforms.AnyAsync())
            {
                var platforms = new List<Platform>
                {
                    new Platform { Id = PcId, Name = "PC", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = PlayStationId, Name = "PlayStation", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = XboxId, Name = "Xbox", CreatedAt = now, UpdatedAt = now },
                    new Platform { Id = SwitchId, Name = "Nintendo Switch", CreatedAt = now, UpdatedAt = now }
                };
                await context.Platforms.AddRangeAsync(platforms);
                await context.SaveChangesAsync();
            }

            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Id = ProgrammerId, Name = "Programmer", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = DesignerId, Name = "Designer", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = ArtistId, Name = "Artist", CreatedAt = now, UpdatedAt = now },
                    new Role { Id = ProducerId, Name = "Producer", CreatedAt = now, UpdatedAt = now }
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
                    .RuleFor(p => p.Type, f => f.PickRandom("Indie", "AAA" ))
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
                if (!publishers.Any()) throw new InvalidOperationException("Publishers list is empty!");

                var gameFaker = new Faker<Game>()
                    .RuleFor(g => g.Id, f => Guid.NewGuid())
                    .RuleFor(g => g.Title, f => f.Commerce.ProductName())
                    .RuleFor(g => g.Description, f => f.Lorem.Sentence(10))
                    .RuleFor(g => g.ReleaseDate, f => f.Date.Past(5).ToUniversalTime())
                    .RuleFor(g => g.Price, f => f.Finance.Amount(10, 100))
                    .RuleFor(g => g.PublisherId, f => f.PickRandom(publishers).Id)
                    .FinishWith((f, g) => { g.CreatedAt = now; g.UpdatedAt = now; });

                var games = gameFaker.Generate(15);
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