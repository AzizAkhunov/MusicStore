using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Services;

namespace MusicStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly SongGenerator _generator = new();

        [HttpGet]
        public IActionResult Get(
            string language = "en",
            long seed = 1,
            int page = 1,
            int pageSize = 20,
            double avgLikes = 0)
        {
            var songs = Enumerable.Range(1, pageSize)
                .Select(i =>
                {
                    int index = (page - 1) * pageSize + i;
                    var song = _generator.Generate(index, page, language, seed, avgLikes);
                    song.AudioUrl = $"/api/songs/{index}/audio";
                    return song;
                });
            return Ok(new
            {
                page,
                pageSize,
                songs
            });
        }

        [HttpGet("{id}/audio")]
        public IActionResult GetAudio(int id, string language = "en", long seed = 1)
        {
            var generator = new MusicGenerator();
            var wav = generator.GenerateWav(seed, id);

            return File(wav, "audio/wav", $"song_{id}.wav");
        }
    }
}
