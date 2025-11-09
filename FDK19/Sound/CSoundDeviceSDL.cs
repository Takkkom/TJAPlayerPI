using NAudio.Wave;
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
        public ESoundDeviceType eOutputDevice { get => ESoundDeviceType.SDL; }

        public int nMasterVolume
        {
            get;
            set;
        }

        public long nElapsedTimems
        {
            get
            {
                return (long)SDL3.SDL_GetTicks();
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
            sound.SoundImpl = new CSoundImplSDL(strFilename, sound.SoundGroup);
            return sound;
        }

        public void tCreateSound(string strFilename, CSound sound)
        {
            sound.SoundImpl = new CSoundImplSDL(strFilename, sound.SoundGroup);
        }

        public void tCreateSound(byte[] byArrWAVファイルイメージ, CSound sound)
        {
            sound.SoundImpl = new CSoundImplSDL(byArrWAVファイルイメージ, sound.SoundGroup);
        }

        public CSoundDeviceSDL()
        {
            this.tmSystemTimer = new CTimer();
        }

        public void Dispose()
        {
            tmSystemTimer.Dispose();
        }
    }
}
