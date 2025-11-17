using FFmpeg.AutoGen;
using ManagedBass.Fx;
using NAudio.Wave;
using SDL;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Sound
{
    internal class CSoundImplSDL : CSoundImpl
    {
        private int _nDurationms;
        public override int nDurationms => _nDurationms;
        public unsafe override double dbPlaySpeed
        {
            get
            {
                if (pTrack is null)
                {
                    return 1.0f;
                }
                return SDL3_mixer.MIX_GetTrackFrequencyRatio(pTrack);
            }
            set
            {
                if (pTrack is null)
                {
                    return;
                }
                SDL3_mixer.MIX_SetTrackFrequencyRatio(pTrack, (float)value);
            }
        }
        public unsafe override Lufs lufsVolume
        {
            set
            {
                float gain = Math.Clamp(((float)value.ToDouble() / 100.0f) + 1.0f, 0, 1);
                SDL3_mixer.MIX_SetTrackGain(pTrack, gain);
            }
        }
        public unsafe override int nPanning
        {
            get
            {
                MIX_Point3D pos;
                SDL3_mixer.MIX_GetTrack3DPosition(pTrack, &pos);

                return (int)(pos.x * 100);
            }
            set
            {
                MIX_Point3D pos = new MIX_Point3D()
                {
                    x = value * 0.01f
                };
                SDL3_mixer.MIX_SetTrack3DPosition(pTrack, &pos);
            }
        }

        public unsafe override bool b一時停止中
        {
            get
            {
                if (pTrack is null)
                {
                    return false;
                }

                return SDL3_mixer.MIX_TrackPaused(pTrack);
            }
        }
        public unsafe override bool bPlaying
        {
            get
            {
                if (pTrack is null)
                {
                    return false;
                }

                return SDL3_mixer.MIX_TrackPlaying(pTrack);
            }
        }

        private CSoundDeviceSDL device;

        public CSoundImplSDL(CSoundDeviceSDL device, string strFilename, ESoundGroup soundGroup) : base(soundGroup)
        {
            this.device = device;
            this.eMakeType = CSound.EMakeType.File;
            this.strFilename = strFilename;

            using Stream stream = File.OpenRead(strFilename);
            using WaveStream? waveStream = AudioFile.GetWaveStream(stream);
            if (waveStream is null) return;

            tCreateSound(waveStream);
        }
        public CSoundImplSDL(CSoundDeviceSDL device, byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup) : base(soundGroup)
        {
            this.device = device;
            this.eMakeType = CSound.EMakeType.WAVFileImage;
            this.byArrWAVファイルイメージ = byArrWAVファイルイメージ;

            using Stream stream = new MemoryStream(byArrWAVファイルイメージ);
            using WaveStream waveStream = new WaveFileReader(stream);

            tCreateSound(waveStream);
        }

        private unsafe void tCreateSound(WaveStream waveStream)
        {
            _nDurationms = (int)waveStream.TotalTime.TotalMilliseconds;

            SDL_AudioSpec mixerSpec = new SDL_AudioSpec()
            {
                format = SDL3_mixer.MIX_DEFAULT_FORMAT,
                channels = 2,
                freq = 44100
            };
            SDL_AudioSpec audioSpec = new SDL_AudioSpec()
            {
                format = BitUtil.GetSDLAudioFormat(waveStream),
                channels = waveStream.WaveFormat.Channels,
                freq = waveStream.WaveFormat.SampleRate
            };

            byte[] bytes = new byte[waveStream.Length];
            waveStream.Read(bytes);
            if (waveStream.WaveFormat.BitsPerSample == 24)
            {
                bytes = BitUtil.Bit24ToBit16(bytes);
            }

            fixed (void* data = bytes)
            {
                pAudio = SDL3_mixer.MIX_LoadRawAudio(this.device.pMixer, (nint)data, (nuint)bytes.Length, &audioSpec);
            }

            pTrack = SDL3_mixer.MIX_CreateTrack(this.device.pMixer);
            SDL3_mixer.MIX_SetTrackAudio(pTrack, pAudio);
        }

        public unsafe override void tサウンドを停止する()
        {
            if (pTrack is null)
            {
                return;
            }
            SDL3_mixer.MIX_PauseTrack(pTrack);
        }

        public unsafe override void tサウンドを再生する(bool bループする)
        {
            if (pTrack is null)
            {
                return;
            }

            SDL_PropertiesID id = SDL3.SDL_CreateProperties();
            SDL3.SDL_SetNumberProperty(id, SDL3_mixer.MIX_PROP_PLAY_LOOPS_NUMBER, bループする ? -1 : 0);
            SDL3_mixer.MIX_PlayTrack(pTrack, id);

            SDL3.SDL_DestroyProperties(id);
        }

        public unsafe override void t再生位置を取得する(out long n位置byte, out double db位置ms)
        {
            if (pTrack is null)
            {
                db位置ms = 0;
                n位置byte = 0;
                return;
            }

            long trackPos = SDL3_mixer.MIX_GetTrackPlaybackPosition(pTrack);
            n位置byte = trackPos;
            db位置ms = SDL3_mixer.MIX_TrackFramesToMS(pTrack, trackPos);
        }

        public unsafe override void t再生位置を変更する(long n位置ms)
        {
            if (pTrack is null)
            {
                return;
            }

            long trackPos = SDL3_mixer.MIX_TrackMSToFrames(pTrack, n位置ms);
            SDL3_mixer.MIX_SetTrackPlaybackPosition(pTrack, trackPos);
        }

        public unsafe override void Dispose(bool bManagedも解放する)
        {
            if (pAudio is null)
            {
                SDL3_mixer.MIX_DestroyAudio(pAudio);
            }
            if (pTrack is null)
            {
                SDL3_mixer.MIX_DestroyTrack(pTrack);
            }

            base.Dispose(bManagedも解放する);
        }

        private unsafe MIX_Audio* pAudio = null;
        private unsafe MIX_Track* pTrack = null;
    }
}
