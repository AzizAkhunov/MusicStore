namespace MusicStore.Services
{
    public class SeedService
    {
        public static Random Create(long userSeed, int page, int index)
        {
            long seed = userSeed + page * 31L + index * 131L;
            return new Random((int)(seed % int.MaxValue));
        }
    }
}
