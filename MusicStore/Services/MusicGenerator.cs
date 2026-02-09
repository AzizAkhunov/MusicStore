using System.Text;

namespace MusicStore.Services
{
    public class MusicGenerator
    {
        public byte[] GenerateWav(long seed, int songIndex)
        {
            const int sampleRate = 44100;
            const int durationSeconds = 3;
            const short bitsPerSample = 16;
            const short channels = 1;

            int samples = sampleRate * durationSeconds;
            var random = new Random((int)((seed + songIndex * 997) % int.MaxValue));

            double frequency = 220 + random.Next(0,400);
            short[] buffer = new short[samples];

            for (int i = 0; i< samples; i++)
            {
                double t = (double)i / sampleRate;
                buffer[i] = (short) (Math.Sin(2 * Math.PI * frequency * t) * short.MaxValue * 0.3);
            }

            return BuildWav(buffer, sampleRate, bitsPerSample, channels);
        }
        private byte[] BuildWav(short[] samples, int sampleRate, short bitsPerSample, short channels)
        {
            int byteRate = sampleRate * channels * bitsPerSample / 8;
            short blockAlign = (short)(channels * bitsPerSample / 8);
            int dataSize = samples.Length * 2;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.ASCII);

            bw.Write("RIFF".ToCharArray());
            bw.Write(36 + dataSize);
            bw.Write("WAVE".ToCharArray());

            bw.Write("fmt ".ToCharArray());
            bw.Write(16);
            bw.Write((short)1);
            bw.Write(channels);
            bw.Write(sampleRate);
            bw.Write(byteRate);
            bw.Write(blockAlign);
            bw.Write(bitsPerSample);

            bw.Write("data".ToCharArray());
            bw.Write(dataSize);

            foreach (var sample in samples)
                bw.Write(sample);

            return ms.ToArray();
        }
    }
}
