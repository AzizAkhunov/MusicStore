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

            int totalSamples = sampleRate * durationSeconds;

            var random = new Random((int)((seed + songIndex * 997) % int.MaxValue));

            // 🎵 Набор нот (простая гамма)
            double[] scale = new[]
            {
                220.0, // A3
                246.94, // B3
                261.63, // C4
                293.66, // D4
                329.63, // E4
                392.00  // G4
            };

            double[] melody = new double[6];
            for (int i = 0; i < melody.Length; i++)
            {
                melody[i] = scale[random.Next(scale.Length)];
            }

            int noteLength = totalSamples / melody.Length;
            short[] buffer = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                int noteIndex = Math.Min(i / noteLength, melody.Length - 1);
                double freq = melody[noteIndex];

                double t = (double)i / sampleRate;
                buffer[i] = (short)(
                    Math.Sin(2 * Math.PI * freq * t) *
                    short.MaxValue *
                    0.35
                );
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
