using NAudio.Wave;
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

        public ESoundDeviceType eOutputDevice { get => ESoundDeviceType.OpenAL; }

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
                alSample.t再生位置を取得する(out _, out double dbTime);
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

        private CSoundImplOpenAL alSample;
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
            this.tmSystemTimer = new CTimer();

            Device = ALC.OpenDevice(null);

            if (Device == null)
            {
                throw new Exception();
            }

            Context = ALC.CreateContext(Device, null);
            ALC.MakeContextCurrent(Context);

            using Stream stream = new MemoryStream();

            WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            using WaveFileWriter waveFileWriter = new WaveFileWriter(stream, waveFormat);
            int length = waveFileWriter.WaveFormat.SampleRate * dbSampleIntervalMs / 1000;
            for (int i = 0; i < length; i++)
            {
                waveFileWriter.WriteSample(0);
            }
            waveFileWriter.Flush();
            stream.Position = 0;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes);

            alSample = new CSoundImplOpenAL(this, bytes, ESoundGroup.Unknown);
            alSample.tサウンドを再生する(true);
        }

        public unsafe void Dispose()
        {
            alSample.tサウンドを停止する();
            alSample.Dispose();
            tmSystemTimer.Dispose();

            ALC.DestroyContext(Context);
            ALC.CloseDevice(Device);

            GC.SuppressFinalize(this);
        }
    }
}
