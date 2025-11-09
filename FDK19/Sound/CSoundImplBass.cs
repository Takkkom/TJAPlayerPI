using FDK.BassMixExtension;
using FDK.Sound;
using FFmpeg.AutoGen;
using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;
using NAudio.Wave;
using SDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FDK
{
    internal class CSoundImplBass : CSoundImpl
    {
        private SyncProcedure? _cbEndofStream;  // ストリームの終端まで再生されたときに呼び出されるコールバック

        private int _nDurationms;
        public override int nDurationms => _nDurationms;

        private double _dbPlaySpeed = 1.0;
        public override double dbPlaySpeed
        {
            get
            {
                return _dbPlaySpeed;
            }
            set
            {
                if (_dbPlaySpeed != value)
                {
                    _dbPlaySpeed = value;

                    if (_hTempoStream != 0 && _dbPlaySpeed != 1.000f)   // PlaySpeedがx1.000のときは、TempoStreamを用いないようにして高速化する
                    {
                        this.hBassStream = _hTempoStream;
                    }
                    else
                    {
                        this.hBassStream = _hBassStream;
                    }

                    if (CSoundManager.bIsTimeStretch)
                    {
                        Bass.ChannelSetAttribute(this.hBassStream, ChannelAttribute.Tempo, (float)(_dbPlaySpeed * 100 - 100));
                    }
                    else
                    {
                        Bass.ChannelSetAttribute(this.hBassStream, ChannelAttribute.Frequency, (float)(_dbPlaySpeed * nFrequency));
                    }
                }
            }
        }

        public override Lufs lufsVolume
        {
            set
            {
                var dbVolume = Math.Clamp((value.ToDouble() / 100.0) + 1.0, 0, 1);
                Bass.ChannelSetAttribute(this.hBassStream, ChannelAttribute.Volume, (float)dbVolume);
            }
        }

        /// <summary>
        /// <para>左:-100～中央:0～100:右。set のみ。</para>
        /// </summary>
        public override int nPanning
        {
            get
            {
                if (!Bass.ChannelGetAttribute(this.hBassStream, ChannelAttribute.Pan, out var fPan))
                    return 0;
                return (int)(fPan * 100);
            }
            set
            {
                float fPan = Math.Min(Math.Max(value, -100), 100) / 100.0f; // -100～100 → -1.0～1.0
                Bass.ChannelSetAttribute(this.hBassStream, ChannelAttribute.Pan, fPan);
            }
        }

        public override bool b一時停止中
        {
            get
            {
                bool ret = (!BassMixExtensions.ChannelIsPlaying(this.hBassStream)) &
                            (BassMix.ChannelGetPosition(this.hBassStream) > 0);
                return ret;
            }
        }
        public override bool bPlaying
        {
            get
            {
                // 基本的にはBASS_ACTIVE_PLAYINGなら再生中だが、最後まで再生しきったchannelも
                // BASS_ACTIVE_PLAYINGのままになっているので、小細工が必要。
                bool ret = (BassMixExtensions.ChannelIsPlaying(this.hBassStream));
                if (BassMix.ChannelGetPosition(this.hBassStream) >= nBytes)
                {
                    ret = false;
                }
                return ret;
            }
        }

        public CSoundImplBass(string strFilename, int hMixer, ESoundGroup soundGroup) : base(soundGroup)
        {
            this._dbPlaySpeed = 1.0;
            this._hTempoStream = 0;

            tBASSサウンドを作成する(strFilename, hMixer);
        }

        public CSoundImplBass(byte[] byArrWAVファイルイメージ, int hMixer, ESoundGroup soundGroup) : base(soundGroup)
        {
            this._dbPlaySpeed = 1.0;
            this._hTempoStream = 0;

            tBASSサウンドを作成する(byArrWAVファイルイメージ, hMixer);
        }

        private void tBASSサウンドを作成する(string strFilename, int hMixer)
        {
            this.eMakeType = CSound.EMakeType.File;
            this.strFilename = strFilename;

            // BASSファイルストリームを作成。

            this._hBassStream = Bass.CreateStream(strFilename, 0, 0, BassFlags.Decode);
            if (this._hBassStream == 0)
            {
                //ファイルからのサウンド生成に失敗した場合にデコードする。(時間がかかるのはしょうがないね)
                //CAudioDecoder.AudioDecode(strFilename, out byArrWAVファイルイメージ, out _, out _, true);
                using Stream stream = File.OpenRead(strFilename);
                using WaveStream? waveStream = AudioFile.GetWaveStream(stream);
                if (waveStream == null) return;

                using Stream newStream = new MemoryStream();
                NAudio.Wave.WaveFileWriter.WriteWavFileToStream(newStream, waveStream);
                newStream.Position = 0;

                byte[] byArrWAVファイルイメージ = new byte[newStream.Length];
                newStream.Read(byArrWAVファイルイメージ);

                tBASSサウンドを作成する(byArrWAVファイルイメージ, hMixer);
                return;
            }

            nBytes = Bass.ChannelGetLength(this._hBassStream);

            tBASSサウンドを作成する_ストリーム生成後の共通処理(hMixer);
        }

        private void tBASSサウンドを作成する(byte[] byArrWAVファイルイメージ, int hMixer)
        {
            this.eMakeType = CSound.EMakeType.WAVFileImage;
            this.byArrWAVファイルイメージ = byArrWAVファイルイメージ;

            this.hGC = GCHandle.Alloc(byArrWAVファイルイメージ, GCHandleType.Pinned);       // byte[] をピン留め

            // BASSファイルストリームを作成。

            this._hBassStream = Bass.CreateStream(hGC.AddrOfPinnedObject(), 0, byArrWAVファイルイメージ.Length, BassFlags.Decode | BassFlags.Float);

            if (this._hBassStream == 0)
                throw new Exception(string.Format("サウンドストリームの生成に失敗しました。(BASS_StreamCreateFile)[{0}]", Bass.LastError.ToString()));

            nBytes = Bass.ChannelGetLength(this._hBassStream);

            tBASSサウンドを作成する_ストリーム生成後の共通処理(hMixer);
        }

        private void tBASSサウンドを作成する_ストリーム生成後の共通処理(int hMixer)
        {
            CSoundManager.nStreams++;

            // 個々のストリームの出力をテンポ変更のストリームに入力する。テンポ変更ストリームの出力を、Mixerに出力する。

            //			if ( CSoundManager.bIsTimeStretch )	// TimeStretchのON/OFFに関わりなく、テンポ変更のストリームを生成する。後からON/OFF切り替え可能とするため。
            {
                this._hTempoStream = BassFx.TempoCreate(this._hBassStream, BassFlags.Decode | BassFlags.FxFreeSource);
                if (this._hTempoStream == 0)
                {
                    hGC.Free();
                    throw new Exception(string.Format("サウンドストリームの生成に失敗しました。(BASS_FX_TempoCreate)[{0}]", Bass.LastError.ToString()));
                }
                else
                {
                    Bass.ChannelSetAttribute(this._hTempoStream, ChannelAttribute.TempoUseQuickAlgorithm, 1f);  // 高速化(音の品質は少し落ちる)
                }
            }

            if (_hTempoStream != 0 && _dbPlaySpeed != 1.000f)   // PlaySpeedがx1.000のときは、TempoStreamを用いないようにして高速化する
            {
                this.hBassStream = _hTempoStream;
            }
            else
            {
                this.hBassStream = _hBassStream;
            }

            // #32248 再生終了時に発火するcallbackを登録する (演奏終了後に再生終了するチップを非同期的にミキサーから削除するため。)
            _cbEndofStream = new SyncProcedure(CallbackEndofStream);
            Bass.ChannelSetSync(hBassStream, SyncFlags.End | SyncFlags.Mixtime, 0, _cbEndofStream, IntPtr.Zero);

            // n総演奏時間の取得; DTXMania用に追加。
            double seconds = Bass.ChannelBytes2Seconds(this._hBassStream, nBytes);
            this._nDurationms = (int)(seconds * 1000);
            //this.pos = 0;
            this.hMixer = hMixer;
            float freq = 0.0f;
            if (!Bass.ChannelGetAttribute(this._hBassStream, ChannelAttribute.Frequency, out freq))
            {
                hGC.Free();
                throw new Exception(string.Format("サウンドストリームの周波数取得に失敗しました。(BASS_ChannelGetAttribute)[{0}]", Bass.LastError.ToString()));
            }
            this.nFrequency = (int)freq;
        }

        public override void t解放する()
        {
            tBASSサウンドをミキサーから削除する();
            _cbEndofStream = null;

            base.t解放する();
        }

        public override void tサウンドを再生する(bool bループする)
        {
            if (bループする)
            {
                Bass.ChannelFlags(this.hBassStream, BassFlags.Loop, BassFlags.Loop);
            }
            else
            {
                Bass.ChannelFlags(this.hBassStream, BassFlags.Default, BassFlags.Default);
            }
            // BASSサウンド時のループ処理は、t再生を開始する()側に実装。ここでは「bループする」は未使用。

            //Debug.WriteLine( "再生中?: " +  System.IO.Path.GetFileName(this.strFilename) + " status=" + BassMix.BASS_Mixer_ChannelIsActive( this.hBassStream ) + " current=" + BassMix.BASS_Mixer_ChannelGetPosition( this.hBassStream ) + " nBytes=" + nBytes );
            bool b = BassMixExtensions.ChannelPlay(this.hBassStream);
            if (!b)
            {
                //Debug.WriteLine( "再生しようとしたが、Mixerに登録されていなかった: " + Path.GetFileName( this.strFilename ) + ", stream#=" + this.hBassStream + ", ErrCode=" + Bass.BASS_ErrorGetCode() );

                if (!tBASSサウンドをミキサーに追加する())
                {
                    Debug.WriteLine("Mixerへの登録に失敗: " + Path.GetFileName(this.strFilename) + ", ErrCode=" + Bass.LastError);
                }

                if (!BassMixExtensions.ChannelPlay(this.hBassStream))
                {
                    Debug.WriteLine("更に再生に失敗: " + Path.GetFileName(this.strFilename) + ", ErrCode=" + Bass.LastError);
                }
            }
            else
            {
                //Debug.WriteLine( "再生成功: " + Path.GetFileName( this.strFilename ) + " (" + hBassStream + ")" );
            }
        }

        public void tサウンドを停止してMixerからも削除する()
        {
            tサウンドを停止する();
            tBASSサウンドをミキサーから削除する();
        }

        public override void tサウンドを停止する()
        {
            BassMixExtensions.ChannelPause(this.hBassStream);
        }

        public override void t再生位置を変更する(long n位置ms)
        {
            bool b = true;
            try
            {
                b = BassMix.ChannelSetPosition(this.hBassStream, Bass.ChannelSeconds2Bytes(this.hBassStream, n位置ms * _dbPlaySpeed / 1000.0), PositionFlags.Bytes);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Trace.TraceInformation(Path.GetFileName(this.strFilename) + ": Seek error: " + e.ToString() + ": " + n位置ms + "ms");
            }
            finally
            {
                if (!b)
                {
                    Errors be = Bass.LastError;
                    Trace.TraceInformation(Path.GetFileName(this.strFilename) + ": Seek error: " + be.ToString() + ": " + n位置ms + "MS");
                }
            }
        }

        /// <summary>
        /// デバッグ用
        /// </summary>
        /// <param name="n位置byte"></param>
        /// <param name="db位置ms"></param>
        public override void t再生位置を取得する(out long n位置byte, out double db位置ms)
        {
            n位置byte = BassMix.ChannelGetPosition(this.hBassStream);
            db位置ms = Bass.ChannelBytes2Seconds(this.hBassStream, n位置byte);
        }


        /// <summary>
        /// ストリームの終端まで再生したときに呼び出されるコールバック
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void CallbackEndofStream(int handle, int channel, int data, IntPtr user)    // #32248 2013.10.14 yyagi
        {
            if (b演奏終了後も再生が続くチップである)         // 演奏終了後に再生終了するチップ音のミキサー削除は、再生終了のコールバックに引っ掛けて、自前で行う。
            {                                                   // そうでないものは、ミキサー削除予定時刻に削除する。
                tBASSサウンドをミキサーから削除する(channel);
            }
        }


        // mixerからの削除

        public bool tBASSサウンドをミキサーから削除する()
        {
            return tBASSサウンドをミキサーから削除する(this.hBassStream);
        }
        private static bool tBASSサウンドをミキサーから削除する(int channel)
        {
            bool b = BassMix.MixerRemoveChannel(channel);
            if (b)
            {
                Interlocked.Decrement(ref CSoundManager.nMixing);
            }
            return b;
        }


        // mixer への追加
        public bool tBASSサウンドをミキサーに追加する()
        {
            if (BassMix.ChannelGetMixer(hBassStream) == 0)
            {
                BassFlags bf = BassFlags.SpeakerFront | BassFlags.MixerChanNoRampin | BassFlags.MixerChanPause;
                Interlocked.Increment(ref CSoundManager.nMixing);

                // preloadされることを期待して、敢えてflagからはBASS_MIXER_PAUSEを外してAddChannelした上で、すぐにPAUSEする
                // -> ChannelUpdateでprebufferできることが分かったため、BASS_MIXER_PAUSEを使用することにした
                bool b1 = BassMix.MixerAddChannel(this.hMixer, this.hBassStream, bf);
                t再生位置を変更する(0);	// StreamAddChannelの後で再生位置を戻さないとダメ。逆だと再生位置が変わらない。
                Bass.ChannelUpdate(this.hBassStream, 0);	// pre-buffer
                return b1;	// &b2;
            }
            return true;
        }

        public override void Dispose(bool bManagedも解放する)
        {
            #region [ Stream の解放 ]
            //-----------------
            if (_hTempoStream != 0)
            {
                BassMix.MixerRemoveChannel(this._hTempoStream);
                Bass.StreamFree(this._hTempoStream);
            }
            BassMix.MixerRemoveChannel(this._hBassStream);
            Bass.StreamFree(this._hBassStream);
            this.hBassStream = -1;
            this._hBassStream = -1;
            this._hTempoStream = 0;
            //-----------------
            #endregion
            
                if (this.hGC.IsAllocated)
                {
                    this.hGC.Free();
                    this.hGC = default(GCHandle);
                }
            base.Dispose(bManagedも解放する);

        }


        protected int _hTempoStream = 0;
        protected int _hBassStream = -1;                    // ASIO, WASAPI 用
        protected int hBassStream = 0;                      // #31076 2013.4.1 yyagi; プロパティとして実装すると動作が低速になったため、
                                                            // tBASSサウンドを作成する_ストリーム生成後の共通処理()のタイミングと、
                                                            // PlaySpeedを変更したタイミングでのみ、
                                                            // hBassStreamを更新するようにした。
        protected int hMixer = -1;  // 設計壊してゴメン Mixerに後で登録するときに使う

        private long nBytes = 0;
        private int nFrequency = 0;
        protected GCHandle hGC;
    }
}
