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

        [HttpGet("{index}/cover")]
        public IActionResult GetCover(int index, [FromQuery] long seed = 1)
        {
            var random = new Random((int)((seed + index * 7919) % int.MaxValue));

            string Color() =>
                $"{random.Next(50, 200):X2}{random.Next(50, 200):X2}{random.Next(50, 200):X2}";

            var color1 = Color();
            var color2 = Color();

            var svg = $@"
                <svg xmlns='http://www.w3.org/2000/svg' width='300' height='300'>
                <defs>
                <linearGradient id='g' x1='0' y1='0' x2='1' y2='1'>
                <stop offset='0%' stop-color='#{color1}'/>
                <stop offset='100%' stop-color='#{color2}'/>
                </linearGradient>
                </defs>
                <rect width='300' height='300' fill='url(#g)'/>
                <text x='150' y='175'
                    font-size='96'
                    text-anchor='middle'
                    fill='white'
                    font-family='Arial, sans-serif'
                    opacity='0.85'>
                {index}
                </text>
            </svg>";

            return Content(svg, "image/svg+xml");
        }

    }
}
