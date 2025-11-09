using FDK.Sound;
using ManagedBass.Fx;
using NAudio.Wave;
using Silk.NET.OpenAL;
using SkiaSharp;

namespace FDK
{
    internal class CSoundImplOpenAL : CSoundImpl
    {
        public static AL AL = CSoundDeviceOpenAL.AL;
        public static ALContext ALC = CSoundDeviceOpenAL.ALC;

        private int _nDurationms;
        public override int nDurationms => _nDurationms;
        public override double dbPlaySpeed
        {
            get
            {
                AL.GetSourceProperty(Source, SourceFloat.Pitch, out float value);
                return value;
            }
            set => AL.SetSourceProperty(Source, SourceFloat.Pitch, (float)value);
        }

        public override Lufs lufsVolume
        {
            set
            {
                volume = Math.Clamp(((float)value.ToDouble() / 100.0f) + 1.0f, 0, 1);
                tUpdateVolume();
            }
        }

        public override int nPanning
        {
            get
            {
                AL.GetSourceProperty(Source, SourceVector3.Position, out Vector3 value);
                return (int)(value.X * 100);
            }
            set
            {
                float val = value * 0.01f;
                float z = MathF.Sqrt(1 - val * val);
                AL.SetSourceProperty(Source, SourceVector3.Position, value, 0.0f, z);
            }
        }

        public override bool b一時停止中 => !bPlaying;

        public override bool bPlaying
        {
            get
            {
                AL.GetSourceProperty(Source, GetSourceInteger.BuffersProcessed, out int processed);
                return processed == 0;
            }
        }

        private float volume = 1.0f;
        public float Gain
        {
            get
            {
                AL.GetSourceProperty(Source, SourceFloat.Gain, out float value);
                return value;
            }
            set => AL.SetSourceProperty(Source, SourceFloat.Gain, value);
        }

        public void tUpdateVolume()
        {
            Gain = volume * (device.nMasterVolume * 0.01f);
        }

        public uint Buffer { get; private set; }
        public uint Source { get; private set; }

        private CSoundDeviceOpenAL device;

        public CSoundImplOpenAL(CSoundDeviceOpenAL device, string strFilename, ESoundGroup soundGroup) : base(soundGroup)
        {
            this.eMakeType = CSound.EMakeType.File;
            this.device = device;
            this.strFilename = strFilename;

            using Stream stream = File.OpenRead(strFilename);
            using WaveStream? waveStream = AudioFile.GetWaveStream(stream);

            if (waveStream is not null)
            {
                tCreateSound(waveStream);
            }

        }
        public CSoundImplOpenAL(CSoundDeviceOpenAL device, byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup) : base(soundGroup)
        {
            this.eMakeType = CSound.EMakeType.WAVFileImage;
            this.device = device;
            this.byArrWAVファイルイメージ = byArrWAVファイルイメージ;

            using Stream stream = new MemoryStream(byArrWAVファイルイメージ);
            using WaveStream waveStream = new WaveFileReader(stream);

            tCreateSound(waveStream);
        }

        internal void tCreateSound(WaveStream waveStream)
        {
            Buffer = AL.GenBuffer();

            byte[] bytes = new byte[waveStream.Length];
            waveStream.Read(bytes);

            BufferFormat bufferFormat = BitUtil.GetBufferFormat(waveStream);

            if (waveStream.WaveFormat.BitsPerSample == 24)
            {
                bytes = BitUtil.Bit24ToBit16(bytes);
            }

            unsafe
            {
                fixed (void* data = bytes)
                {
                    AL.BufferData(Buffer, bufferFormat, data, bytes.Length, waveStream.WaveFormat.SampleRate);
                }
            }

            Source = AL.GenSource();
            AL.SetSourceProperty(Source, SourceInteger.Buffer, Buffer);

            _nDurationms = (int)waveStream.TotalTime.TotalMilliseconds;

            tUpdateVolume();
        }

        public override void tサウンドを停止する()
        {
            AL.SourceStop(Source);
        }

        public override void tサウンドを再生する(bool bループする)
        {
            AL.SetSourceProperty(Source, SourceBoolean.Looping, bループする);
            AL.SourcePlay(Source);
        }

        public override void t再生位置を取得する(out long n位置byte, out double db位置ms)
        {
            AL.GetSourceProperty(Source, SourceFloat.SecOffset, out float posSec);
            AL.GetSourceProperty(Source, GetSourceInteger.ByteOffset, out int posBytes);
            db位置ms = posSec * 1000.0;
            n位置byte = posBytes;
        }

        public override void t再生位置を変更する(long n位置ms)
        {
            AL.SetSourceProperty(Source, SourceFloat.SecOffset, n位置ms * 0.001f);
        }

        public override void Dispose(bool bManagedも解放する)
        {
            AL.DeleteSource(Source);
            AL.DeleteBuffer(Buffer);

            base.Dispose(bManagedも解放する);
        }
    }
}
