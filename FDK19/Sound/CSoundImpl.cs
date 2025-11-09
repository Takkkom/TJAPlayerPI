using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK
{
    internal abstract class CSoundImpl : IDisposable
    {
        public readonly ESoundGroup SoundGroup;

        #region [ DTXMania用拡張 ]

        public abstract int nDurationms { get; }
        public abstract double dbPlaySpeed { get; set; }
        #endregion
        public bool b演奏終了後も再生が続くチップである = false; // これがtrueなら、本サウンドの再生終了のコールバック時に自動でミキサーから削除する

        public abstract Lufs lufsVolume { set; }

        /// <summary>
        /// <para>左:-100～中央:0～100:右。set のみ。</para>
        /// </summary>
        public abstract int nPanning { get; set; }

        public CSoundImpl(ESoundGroup soundGroup)
        {
            SoundGroup = soundGroup;
            this.nPanning = 0;
        }

        #region [ DTXMania用の変換 ]

        public void t再生を開始する(bool bループする)
        {
            t再生位置を変更する(0);
            tサウンドを再生する(bループする);
        }
        public abstract bool b一時停止中 { get; }
        public abstract bool bPlaying { get; }
        #endregion


        public virtual void t解放する()
        {
            this.Dispose(true);   // CSoundの再初期化時は、インスタンスは存続する。
        }

        public abstract void tサウンドを再生する(bool bループする);

        public abstract void tサウンドを停止する();

        public abstract void t再生位置を変更する(long n位置ms);

        /// <summary>
        /// デバッグ用
        /// </summary>
        /// <param name="n位置byte"></param>
        /// <param name="db位置ms"></param>
        public abstract void t再生位置を取得する(out long n位置byte, out double db位置ms);



        #region [ Dispose-Finalizeパターン実装 ]
        //-----------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool bManagedも解放する)
        {
            if (bManagedも解放する)
            {
                if (this.byArrWAVファイルイメージ is not null)
                {
                    this.byArrWAVファイルイメージ = null;
                }
            }
        }
        ~CSoundImpl()
        {
            this.Dispose(false);
        }
        //-----------------
        #endregion

        #region [ protected ]
        //-----------------
        public string? strFilename = null;
        public CSound.EMakeType eMakeType = CSound.EMakeType.Unknown;
        public byte[]? byArrWAVファイルイメージ = null;  // WAVファイルイメージ、もしくはchunkのDATA部のみ
        //-----------------
        #endregion
    }
}
