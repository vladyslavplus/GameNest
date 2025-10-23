namespace GameNest.Shared.TestData
{
    public static class SharedSeedData
    {
        public static class Games
        {
            public const string TheWitcher3 = "11111111-1111-1111-1111-111111111111";
            public const string DoomEternal = "33333333-3333-3333-3333-333333333333";
            public const string StardewValley = "55555555-5555-5555-5555-555555555555";
            public const string Cyberpunk2077 = "77777777-7777-7777-7777-777777777777";
        }

        public static class Customers
        {
            public const string JohnDoe = "22222222-2222-2222-2222-222222222222";
            public const string AliceWonder = "44444444-4444-4444-4444-444444444444";
            public const string MarkSmith = "66666666-6666-6666-6666-666666666666";
            public const string BobMarley = "88888888-8888-8888-8888-888888888888";
        }

        public static class ReviewTexts
        {
            public static readonly List<string> Default =
            [
                "Amazing game! Highly recommend.",
                "Good, but could be better.",
                "Perfect game! Best I've ever played.",
                "Not worth the money. Too many bugs."
            ];
        }

        public static class Comments
        {
            public static readonly List<string> Default =
            [
                "I totally agree with this review!",
                "I had a different experience.",
                "Thanks for sharing your opinion!",
                "Very helpful review, thank you.",
                "I'm thinking about buying this game too."
            ];
        }

        public static class Media
        {
            public static readonly List<string> Urls =
            [
                "https://example.com/screenshot1.png",
                "https://example.com/screenshot2.jpg",
                "https://example.com/gameplay.mp4",
                "https://example.com/review_image.png",
                "https://example.com/game_photo.jpg"
            ];
        }

        public static class Users
        {
            public const string Admin = "99999999-9999-9999-9999-999999999999";
            public const string JohnDoe = "22222222-2222-2222-2222-222222222222";
            public const string AliceWonder = "44444444-4444-4444-4444-444444444444";
            public const string MarkSmith = "66666666-6666-6666-6666-666666666666";
        }
    }
}