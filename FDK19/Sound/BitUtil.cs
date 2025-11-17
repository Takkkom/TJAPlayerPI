using NAudio.Wave;
using SDL;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Sound
{
    public static class BitUtil
    {
        public static BufferFormat GetBufferFormat(WaveStream waveStream)
        {
            switch (waveStream.WaveFormat.BitsPerSample)
            {
                case 8:
                    return waveStream.WaveFormat.Channels == 1 ? BufferFormat.Mono8 : BufferFormat.Stereo8;
                case 16:
                    return waveStream.WaveFormat.Channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16;
                case 24:
                    return waveStream.WaveFormat.Channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16;
                case 32:
                    return waveStream.WaveFormat.Channels == 1 ? (BufferFormat)FloatBufferFormat.Mono : (BufferFormat)FloatBufferFormat.Stereo;
            }
            return BufferFormat.Mono8;
        }
        public static SDL_AudioFormat GetSDLAudioFormat(WaveStream waveStream)
        {
            switch (waveStream.WaveFormat.BitsPerSample)
            {
                case 8:
                    return SDL_AudioFormat.SDL_AUDIO_S8;
                case 16:
                    return SDL_AudioFormat.SDL_AUDIO_S16LE;
                case 24:
                    return SDL_AudioFormat.SDL_AUDIO_S16LE;
                case 32:
                    return SDL_AudioFormat.SDL_AUDIO_F32LE;
            }
            return SDL_AudioFormat.SDL_AUDIO_S8;
        }

        public static byte[] Bit24ToBit16(byte[] bytes)
        {
            int baseLength = bytes.Length / 3;
            byte[] newBytes = new byte[baseLength * 2];

            for (int i = 0; i < baseLength; i++)
            {
                int shift24 = i * 3;
                int shift16 = i * 2;
                byte[] byteArray = new byte[3] { bytes[shift24 + 0], bytes[shift24 + 1], bytes[shift24 + 2] };

                int og = byteArray[0] | (byteArray[1] << 8) | (byteArray[2] << 16);
                short val = (short)(og / 256);

                byte[] newB = BitConverter.GetBytes(val);
                newBytes[shift16 + 0] = newB[0];
                newBytes[shift16 + 1] = newB[1];
            }

            return newBytes;
        }
    }
}
