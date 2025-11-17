using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Sound
{
    internal static class CSampleSound
    {
        public const double dbSampleInterval = 1.0;
        public const int nSampleIntervalMs = (int)(dbSampleInterval * 1000.0);

        public static CSound tCreateSample(ISoundDevice soundDevice)
        {
            using Stream stream = new MemoryStream();

            WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            using WaveFileWriter waveFileWriter = new WaveFileWriter(stream, waveFormat);
            int length = waveFileWriter.WaveFormat.SampleRate * nSampleIntervalMs / 1000;
            for (int i = 0; i < length; i++)
            {
                waveFileWriter.WriteSample(0);
            }
            waveFileWriter.Flush();
            stream.Position = 0;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes);

            CSound sound = soundDevice.tCreateSound(bytes, ESoundGroup.Unknown);
            CSound.listインスタンス.Remove(sound);
            return sound;
        }
    }
}
