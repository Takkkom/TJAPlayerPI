using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Sound
{
    public class CSoundDeviceOpenAL : ISoundDevice
    {
        public static readonly AL AL = AL.GetApi();
        public static readonly ALContext ALC = ALContext.GetApi();

        public bool bValid
        {
            get;
            private set;
        }

        private int _nMasterVolume;
        public int nMasterVolume
        {
            get => _nMasterVolume;
            set
            {
                _nMasterVolume = value;
                foreach (CSound sound in CSound.listインスタンス)
                {
                    if (sound.SoundImpl is CSoundImplOpenAL soundOpenAL)
                    {
                        soundOpenAL.tUpdateVolume();
                    }
                }
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

                while (systemIntervalMs >= dbSampleIntervalMs)        // 前回から単位繰り上げ間隔以上経過してるなら確実にループしている。誤差は大きくないだろうから無視。
                {
                    this.nLoopCount++;
                    systemIntervalMs -= dbSampleIntervalMs;
                }

                if (dbTime < this.dbPreviousTime)                            // 単位繰り上げ間隔以内であっても、現在位置が前回より手前にあるなら1回ループしている。
                    this.nLoopCount++;


                // 経過時間を算出。

                double nextTime = (this.nLoopCount * dbSampleIntervalMs) + dbTime;


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


        internal readonly unsafe Silk.NET.OpenAL.Device* Device;
        internal readonly unsafe Silk.NET.OpenAL.Context* Context;

        private CSound sample;
        private long nPreviousSystemTime;
        private double dbSampleInterval = 1.0;
        private int dbSampleIntervalMs => (int)(dbSampleInterval * 1000.0);
        private int nLoopCount;
        private double dbPreviousTime;

        public unsafe CSound tCreateSound(string strFilename, ESoundGroup soundGroup)
        {
            CSound sound = new CSound(soundGroup);
            sound.SoundImpl = new CSoundImplOpenAL(this, strFilename, sound.SoundGroup);
            return sound;
        }

        public unsafe CSound tCreateSound(byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup)
        {
            CSound sound = new CSound(soundGroup);
            sound.SoundImpl = new CSoundImplOpenAL(this, byArrWAVファイルイメージ, sound.SoundGroup);
            return sound;
        }

        public unsafe void tCreateSound(string strFilename, CSound sound)
        {
            sound.SoundImpl = new CSoundImplOpenAL(this, strFilename, sound.SoundGroup);
        }

        public unsafe void tCreateSound(byte[] byArrWAVファイルイメージ, CSound sound)
        {
            sound.SoundImpl = new CSoundImplOpenAL(this, byArrWAVファイルイメージ, sound.SoundGroup);
        }

        public unsafe CSoundDeviceOpenAL()
        {
            this.bValid = false;

            this.tmSystemTimer = new CTimer();

            Device = ALC.OpenDevice(null);

            if (Device == null)
            {
                throw new Exception();
            }

            Context = ALC.CreateContext(Device, null);
            ALC.MakeContextCurrent(Context);

            this.bValid = true;

            sample = CSampleSound.tCreateSample(this);
            sample.t再生を開始する(true);
        }

        public unsafe void Dispose()
        {
            this.bValid = false;

            sample.tサウンドを停止する();
            sample.Dispose();
            tmSystemTimer.Dispose();

            ALC.DestroyContext(Context);
            ALC.CloseDevice(Device);

            GC.SuppressFinalize(this);
        }
    }
}
