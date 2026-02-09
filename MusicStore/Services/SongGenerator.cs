using MusicStore.Models;
using Bogus;
namespace MusicStore.Services
{
    public class SongGenerator
    {
        public SongDto Generate(int index, int page, string language, long seed, double avgLikes)
        {
            var random = SeedService.Create(seed, page, index);
            var faker = new Faker(language == "ru" ? "ru" : "en");

            return new SongDto
            {
                Index = index,
                Title = faker.Lorem.Sentence(faker.Random.Int(1, 3))
                    .Replace(".", "")
                    .Replace(",", ""),
                Artist = faker.Name.FullName(),
                Album = random.Next(0, 2) == 0 ? "Single" : faker.Commerce.ProductName(),
                Genre = faker.Music.Genre(),
                Likes = GenerateLikes(avgLikes, random)
            };
        }
        private int GenerateLikes (double avg, Random random)
        {
            int baseLikes = (int)Math.Floor(avg);
            double chance = avg - baseLikes;
            return baseLikes + (random.NextDouble() < chance ? 1 : 0);
        }
    }
}
