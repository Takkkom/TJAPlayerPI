using SDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Sound
{
    public class CSoundDeviceSDL : ISoundDevice
    {
        public bool bValid
        {
            get;
            private set;
        }

        public unsafe int nMasterVolume
        {
            get
            {
                float gain = SDL3_mixer.MIX_GetMasterGain(pMixer);
                return (int)(gain * 100);
            }
            set
            {
                float gain = value * 0.01f;
                SDL3_mixer.MIX_SetMasterGain(pMixer, gain);
            }
        }

        public double dbElapsedTimems
        {
            get
            {
                sample.t再生位置を取得する(out _, out double dbTime);
                long systemTime = this.tmSystemTimer.nシステム時刻ms;


                // ループ回数を調整。

                long systemIntervalMs = systemTime - this.nPreviousSystemTime;

                while (systemIntervalMs >= CSampleSound.nSampleIntervalMs)        // 前回から単位繰り上げ間隔以上経過してるなら確実にループしている。誤差は大きくないだろうから無視。
                {
                    this.nLoopCount++;
                    systemIntervalMs -= CSampleSound.nSampleIntervalMs;
                }

                if (dbTime < this.dbPreviousTime)                            // 単位繰り上げ間隔以内であっても、現在位置が前回より手前にあるなら1回ループしている。
                    this.nLoopCount++;


                // 経過時間を算出。

                double nextTime = (this.nLoopCount * CSampleSound.nSampleIntervalMs) + dbTime;


                // 今回の値を次回に向けて保存。

                this.nPreviousSystemTime = systemTime;
                this.dbPreviousTime = dbTime;

                return nextTime;
            }
        }

        public long nElapsedTimems
        {
            get
            {
                return (long)dbElapsedTimems;
            }
        }

        public long SystemTimemsWhenUpdatingElapsedTime
        {
            get
            {
                return this.tmSystemTimer is null ? 0 : this.tmSystemTimer.nシステム時刻ms;
            }
        }

        public CTimer tmSystemTimer { get; private set; }

        public CSound tCreateSound(string strFilename, ESoundGroup soundGroup)
        {
            CSound sound = new CSound(soundGroup);
            sound.SoundImpl = new CSoundImplSDL(this, strFilename, sound.SoundGroup);
            return sound;
        }

        public CSound tCreateSound(byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup)
        {
            CSound sound = new CSound(soundGroup);
            sound.SoundImpl = new CSoundImplSDL(this, byArrWAVファイルイメージ, sound.SoundGroup);
            return sound;
        }

        public void tCreateSound(string strFilename, CSound sound)
        {
            sound.SoundImpl = new CSoundImplSDL(this, strFilename, sound.SoundGroup);
        }

        public void tCreateSound(byte[] byArrWAVファイルイメージ, CSound sound)
        {
            sound.SoundImpl = new CSoundImplSDL(this, byArrWAVファイルイメージ, sound.SoundGroup);
        }

        public unsafe CSoundDeviceSDL()
        {
            this.tmSystemTimer = new CTimer();

            SDL3_mixer.MIX_Init();

            SDL_AudioSpec spec = new SDL_AudioSpec()
            {
                format = SDL3_mixer.MIX_DEFAULT_FORMAT,
                channels = 2,
                freq = 44100
            };
            AudioDevice = SDL3.SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK;
            pMixer = SDL3_mixer.MIX_CreateMixerDevice(AudioDevice, &spec);

            bValid = true;

            sample = CSampleSound.tCreateSample(this);
            sample.t再生を開始する(true);
        }

        public unsafe void Dispose()
        {
            bValid = false;

            sample.Dispose();

            if (pMixer is null)
            {
                SDL3_mixer.MIX_DestroyMixer(pMixer);
            }

            SDL3_mixer.MIX_Quit();

            tmSystemTimer.Dispose();
        }

        internal SDL_AudioDeviceID AudioDevice;
        internal unsafe MIX_Mixer* pMixer = null;


        private CSound sample;
        private long nPreviousSystemTime;
        private int nLoopCount;
        private double dbPreviousTime;
    }
}
