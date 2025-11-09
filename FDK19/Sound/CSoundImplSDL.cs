using NAudio.Wave;
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
        public override double dbPlaySpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Lufs lufsVolume { set => throw new NotImplementedException(); }
        public override int nPanning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool b一時停止中 => throw new NotImplementedException();

        public override bool bPlaying => throw new NotImplementedException();

        public CSoundImplSDL(string strFilename, ESoundGroup soundGroup) : base(soundGroup)
        {
            this.strFilename = strFilename;
        }
        public CSoundImplSDL(byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup) : base(soundGroup)
        {
            //this.byArrWAVファイルイメージ = waveStream;
        }

        public override void tサウンドを停止する()
        {
            throw new NotImplementedException();
        }

        public override void tサウンドを再生する(bool bループする)
        {
            throw new NotImplementedException();
        }

        public override void t再生位置を取得する(out long n位置byte, out double db位置ms)
        {
            throw new NotImplementedException();
        }

        public override void t再生位置を変更する(long n位置ms)
        {
            throw new NotImplementedException();
        }
    }
}
