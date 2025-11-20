using FDK;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI;

internal class CStageSongLoading : CStage
{
    // コンストラクタ

    public CStageSongLoading()
    {
        base.eStageID = CStage.EStage.SongLoading;
        base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
    }

    // CStage 実装

    public override void On活性化()
    {
        Trace.TraceInformation("曲読み込みステージを活性化します。");
        Trace.Indent();
        try
        {
            this.strTitle = "";

            this.nBGM再生開始時刻 = -1;
            this.nBGMの総再生時間ms = 0;

            var 譜面情報 = TJAPlayerPI.app.r確定されたスコア.譜面情報;
            this.strTitle = 譜面情報.Title;
            this.strSubTitle = 譜面情報.SubTitle;



            // For the moment, detect that we are performing
            // calibration via there being an actual single
            // player and the special song title and subtitle
            // of the .tja used to perform input calibration
            TJAPlayerPI.IsPerformingCalibration =
                !TJAPlayerPI.app.ConfigToml.PlayOption.AutoPlay[0] &&
                TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount == 1 &&
                strTitle == "Input Calibration" &&
                strSubTitle == "TJAPlayer3 Developers";


            this.ct待機 = new CCounter(0, 600, 5, TJAPlayerPI.app.Timer);
            this.ct曲名表示 = new CCounter(1, 30, 30, TJAPlayerPI.app.Timer);
            try
            {
                // When performing calibration, inform the player that
                // calibration is about to begin, rather than
                // displaying the song title and subtitle as usual.

                var タイトル = TJAPlayerPI.IsPerformingCalibration
                    ? "Input calibration is about to begin."
                    : this.strTitle;

                var サブタイトル = TJAPlayerPI.IsPerformingCalibration
                    ? "Please play as accurately as possible."
                    : this.strSubTitle;

                this.txTitle?.Dispose();
                this.txTitle = null;
                this.txSubTitle?.Dispose();
                this.txSubTitle = null;

                if (!string.IsNullOrEmpty(タイトル))
                {
                    using (CFontRenderer pfTITLE = HFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleFontSize))
                    {
                        using (var bmpSongTitle = pfTITLE.DrawText(タイトル, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                        {
                            this.txTitle = TJAPlayerPI.app.tCreateTexture(bmpSongTitle);
                            this.txTitle.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txTitle, 710);
                        }
                    }

                    if (!string.IsNullOrEmpty(サブタイトル))
                    {
                        using (CFontRenderer pfSUBTITLE = HFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleFontSize))
                        {
                            using (var bmpSongSubTitle = pfSUBTITLE.DrawText(サブタイトル, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                            {
                                this.txSubTitle = TJAPlayerPI.app.tCreateTexture(bmpSongSubTitle);
                            }
                        }
                    }
                }

            }
            catch (CTextureCreateFailedException e)
            {
                Trace.TraceError(e.ToString());
                this.txTitle?.Dispose();
                this.txTitle = null;
                this.txSubTitle?.Dispose();
                this.txSubTitle = null;
            }

            base.On活性化();
        }
        finally
        {
            Trace.TraceInformation("曲読み込みステージの活性化を完了しました。");
            Trace.Unindent();
        }
    }
    public override void On非活性化()
    {
        Trace.TraceInformation("曲読み込みステージを非活性化します。");
        Trace.Indent();
        try
        {
            TJAPlayerPI.t安全にDisposeする(ref this.txTitle);
            TJAPlayerPI.t安全にDisposeする(ref this.txSubTitle);
            base.On非活性化();
        }
        finally
        {
            Trace.TraceInformation("曲読み込みステージの非活性化を完了しました。");
            Trace.Unindent();
        }
    }
    public override int On進行描画()
    {
        if (base.b活性化してない)
            return 0;

        #region [ 初めての進行描画 ]
        //-----------------------------
        if (base.b初めての進行描画)
        {
            if (TJAPlayerPI.app.n確定された曲の難易度[0] != (int)Difficulty.Dan)
            {
                TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND曲読込開始音].t再生する();
                this.nBGM再生開始時刻 = CSoundManager.rc演奏用タイマ.n現在時刻ms;
                this.nBGMの総再生時間ms = TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND曲読込開始音].n長さ_現在のサウンド;
            }
            //this.actFI.tFadeIn開始();							// #27787 2012.3.10 yyagi 曲読み込み画面のFadeInの省略
            base.eフェーズID = CStage.Eフェーズ.共通_FadeIn;
            base.b初めての進行描画 = false;

            nWAVcount = 1;
        }
        //-----------------------------
        #endregion

        #region [ ESC押下時は選曲画面に戻る ]
        if (this.ct待機 is null || this.ct曲名表示 is null || TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.Escape))
        {
            return (int)E曲読込画面の戻り値.読込中止;
        }
        #endregion

        this.ct待機.t進行();

        #region [ 背景、音符＋タイトル表示 ]
        //-----------------------------
        this.ct曲名表示.t進行();
        if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
        {
            if (TJAPlayerPI.app.Tx.SongLoading_v2_BG is not null)
                TJAPlayerPI.app.Tx.SongLoading_v2_BG.t2D描画(TJAPlayerPI.app.Device, 0, 0);
        }
        else
        {
            if (TJAPlayerPI.app.Tx.SongLoading_BG is not null)
                TJAPlayerPI.app.Tx.SongLoading_BG.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.LogicalSize.Width / 2, TJAPlayerPI.app.LogicalSize.Height / 2);
        }

        if (TJAPlayerPI.app.n確定された曲の難易度[0] != (int)Difficulty.Dan)
        {
            if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
            {
                if (TJAPlayerPI.app.Tx.SongLoading_v2_Plate is not null)
                {
                    TJAPlayerPI.app.Tx.SongLoading_v2_Plate.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2PlateReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        TJAPlayerPI.app.Tx.SongLoading_v2_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateY - (TJAPlayerPI.app.Tx.SongLoading_v2_Plate.szTextureSize.Height / 2));
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2PlateReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        TJAPlayerPI.app.Tx.SongLoading_v2_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateX - TJAPlayerPI.app.Tx.SongLoading_v2_Plate.szTextureSize.Width, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateY - (TJAPlayerPI.app.Tx.SongLoading_v2_Plate.szTextureSize.Height / 2));
                    }
                    else
                    {
                        TJAPlayerPI.app.Tx.SongLoading_v2_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateX - (TJAPlayerPI.app.Tx.SongLoading_v2_Plate.szTextureSize.Width / 2), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateY - (TJAPlayerPI.app.Tx.SongLoading_v2_Plate.szTextureSize.Height / 2));
                    }
                }

                if (this.txTitle is not null)
                {
                    int nサブタイトル補正 = string.IsNullOrEmpty(TJAPlayerPI.app.r確定されたスコア.譜面情報.SubTitle) ? 15 : 0;

                    this.txTitle.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2TitleReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2TitleReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleX - (this.txTitle.szTextureSize.Width * txTitle.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                    else
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleX - ((this.txTitle.szTextureSize.Width * txTitle.vcScaling.X) / 2)), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                }
                if (this.txSubTitle is not null)
                {
                    this.txSubTitle.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2SubTitleReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._v2SubTitleReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleX - (this.txSubTitle.szTextureSize.Width * txSubTitle.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                    else
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleX - ((this.txSubTitle.szTextureSize.Width * txSubTitle.vcScaling.X) / 2)), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                }
            }
            else
            {
                if (TJAPlayerPI.app.Tx.SongLoading_Plate is not null)
                {
                    TJAPlayerPI.app.Tx.SongLoading_Plate.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._PlateReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        TJAPlayerPI.app.Tx.SongLoading_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateY - (TJAPlayerPI.app.Tx.SongLoading_Plate.szTextureSize.Height / 2));
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._PlateReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        TJAPlayerPI.app.Tx.SongLoading_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateX - TJAPlayerPI.app.Tx.SongLoading_Plate.szTextureSize.Width, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateY - (TJAPlayerPI.app.Tx.SongLoading_Plate.szTextureSize.Height / 2));
                    }
                    else
                    {
                        TJAPlayerPI.app.Tx.SongLoading_Plate.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateX - (TJAPlayerPI.app.Tx.SongLoading_Plate.szTextureSize.Width / 2), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateY - (TJAPlayerPI.app.Tx.SongLoading_Plate.szTextureSize.Height / 2));
                    }
                }

                if (this.txTitle is not null)
                {
                    int nサブタイトル補正 = string.IsNullOrEmpty(TJAPlayerPI.app.r確定されたスコア.譜面情報.SubTitle) ? 15 : 0;

                    this.txTitle.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleX - (this.txTitle.szTextureSize.Width * txTitle.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                    else
                    {
                        this.txTitle.t2D描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleX - ((this.txTitle.szTextureSize.Width * txTitle.vcScaling.X) / 2)), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleY - (this.txTitle.szTextureSize.Height / 2) + nサブタイトル補正);
                    }
                }
                if (this.txSubTitle is not null)
                {
                    this.txSubTitle.Opacity = CConvert.nParsentTo255((this.ct曲名表示.n現在の値 / 30.0));
                    if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleReferencePoint == CSkin.EReferencePoint.Right)
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleX - (this.txSubTitle.szTextureSize.Width * txSubTitle.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                    else
                    {
                        this.txSubTitle.t2D描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleX - ((this.txSubTitle.szTextureSize.Width * txSubTitle.vcScaling.X) / 2)), TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleY - (this.txSubTitle.szTextureSize.Height / 2));
                    }
                }
            }
        }
        //-----------------------------
        #endregion

        switch (base.eフェーズID)
        {
            case CStage.Eフェーズ.共通_FadeIn:
                //if( this.actFI.On進行描画() != 0 )			    // #27787 2012.3.10 yyagi 曲読み込み画面のFadeInの省略
                // 必ず一度「CStaeg.Eフェーズ.共通_FadeIn」フェーズを経由させること。
                // さもないと、曲読み込みが完了するまで、曲読み込み画面が描画されない。
                base.eフェーズID = CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む;
                return (int)E曲読込画面の戻り値.継続;

            case CStage.Eフェーズ.NOWLOADING_DTXファイルを読み込む:
                {
                    timeBeginLoad = DateTime.Now;
                    string str = TJAPlayerPI.app.r確定されたスコア.FileInfo.FileAbsolutePath;

                    CScoreJson json = CScoreJson.Load(str + ".score.json");

                    if ((TJAPlayerPI.DTX[0] is not null) && TJAPlayerPI.DTX[0].b活性化してる)
                        TJAPlayerPI.DTX[0].On非活性化();

                    //if( CDTXMania.DTX is null )
                    {
                        bool bSession = TJAPlayerPI.app.ConfigToml.PlayOption.Session &&
                                        TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount == 2 &&
                                        TJAPlayerPI.app.n確定された曲の難易度[0] == TJAPlayerPI.app.n確定された曲の難易度[1];

                        for (int i = 0; i < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; i++)
                            TJAPlayerPI.DTX[i] = new CDTX(str, false, json.BGMAdjust, i, bSession);

                        if (TJAPlayerPI.app.ConfigToml.OverrideScrollMode == false)
                            TJAPlayerPI.app.ConfigToml.ScrollMode = TJAPlayerPI.DTX[0].eScrollMode;

                        Trace.TraceInformation("----曲情報-----------------");
                        Trace.TraceInformation("TITLE: {0}", TJAPlayerPI.DTX[0].TITLE);
                        Trace.TraceInformation("FILE: {0}", TJAPlayerPI.DTX[0].strFilenameの絶対パス);
                        Trace.TraceInformation("---------------------------");

                        TimeSpan span = (TimeSpan)(DateTime.Now - timeBeginLoad);
                        Trace.TraceInformation("DTX読込所要時間:           {0}", span.ToString());

                        // 段位認定モード用。
                        if (TJAPlayerPI.app.n確定された曲の難易度[0] == (int)Difficulty.Dan && TJAPlayerPI.DTX[0].List_DanSongs is not null)
                        {
                            for (int i = 0; i < TJAPlayerPI.DTX[0].List_DanSongs.Count; i++)
                            {
                                if (!string.IsNullOrEmpty(TJAPlayerPI.DTX[0].List_DanSongs[i].Title))
                                {
                                    using (var pfTitle = HFontHelper.tCreateFont(32))
                                    {
                                        using (var bmpSongTitle = pfTitle.DrawText(TJAPlayerPI.DTX[0].List_DanSongs[i].Title, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._TitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._TitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                                        {
                                            TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex = TJAPlayerPI.app.tCreateTexture(bmpSongTitle);
                                            TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex, 710);
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitle))
                                {
                                    using (var pfSubTitle = HFontHelper.tCreateFont(19))
                                    {
                                        using (var bmpSongSubTitle = pfSubTitle.DrawText(TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitle, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._SubTitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._SubTitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                                        {
                                            TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex = TJAPlayerPI.app.tCreateTexture(bmpSongSubTitle);
                                            TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex, 710);
                                        }
                                    }
                                }

                            }
                        }
                    }

                    base.eフェーズID = CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む;
                    timeBeginLoadWAV = DateTime.Now;
                    return (int)E曲読込画面の戻り値.継続;
                }

            case CStage.Eフェーズ.NOWLOADING_WAVファイルを読み込む:
                {
                    int looptime = (TJAPlayerPI.app.ConfigToml.Window.VSyncWait) ? 3 : 1;	// VSyncWait=ON時は1frame(1/60s)あたり3つ読むようにする
                    for (int i = 0; i < looptime && nWAVcount <= TJAPlayerPI.DTX[0].listWAV.Count; i++)
                    {
                        if (TJAPlayerPI.DTX[0].listWAV[nWAVcount].bUse)	// #28674 2012.5.8 yyagi
                        {
                            TJAPlayerPI.DTX[0].tWAVの読み込み(TJAPlayerPI.DTX[0].listWAV[nWAVcount]);
                        }
                        nWAVcount++;
                    }
                    if (nWAVcount > TJAPlayerPI.DTX[0].listWAV.Count)
                    {
                        TimeSpan span = (TimeSpan)(DateTime.Now - timeBeginLoadWAV);
                        Trace.TraceInformation("WAV読込所要時間({0,4}):     {1}", TJAPlayerPI.DTX[0].listWAV.Count, span.ToString());
                        timeBeginLoadWAV = DateTime.Now;

                        TJAPlayerPI.DTX[0].PlanToAddMixerChannel();

                        for (int nPlayer = 0; nPlayer < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; nPlayer++)
                        {
                            TJAPlayerPI.DTX[nPlayer].t太鼓チップのランダム化(TJAPlayerPI.app.ConfigToml.PlayOption._Random[nPlayer]);
                        }

                        //TJAPlayerPI.stage演奏ドラム画面.On活性化();

                        span = (TimeSpan)(DateTime.Now - timeBeginLoadWAV);

                        base.eフェーズID = CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む;
                    }
                    return (int)E曲読込画面の戻り値.継続;
                }

            case CStage.Eフェーズ.NOWLOADING_BMPファイルを読み込む:
                {
                    if (TJAPlayerPI.app.ConfigToml.Game.Background.Movie)
                        TJAPlayerPI.DTX[0].tAVIの読み込み();

                    TimeSpan span = (TimeSpan)(DateTime.Now - timeBeginLoad);
                    Trace.TraceInformation("総読込時間:                {0}", span.ToString());

                    TJAPlayerPI.app.Timer.t更新();

                    base.eフェーズID = CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ;
                    return (int)E曲読込画面の戻り値.継続;
                }

            case CStage.Eフェーズ.NOWLOADING_システムサウンドBGMの完了を待つ:
                {
                    long nCurrentTime = TJAPlayerPI.app.Timer.n現在時刻ms;
                    if (nCurrentTime < this.nBGM再生開始時刻)
                        this.nBGM再生開始時刻 = nCurrentTime;

                    //						if ( ( nCurrentTime - this.nBGM再生開始時刻 ) > ( this.nBGMの総再生時間ms - 1000 ) )
                    if ((nCurrentTime - this.nBGM再生開始時刻) >= (this.nBGMの総再生時間ms))	// #27787 2012.3.10 yyagi 1000ms == FadeIn分の時間
                    {
                        base.eフェーズID = CStage.Eフェーズ.共通_FadeOut;
                    }
                    return (int)E曲読込画面の戻り値.継続;
                }

            case CStage.Eフェーズ.共通_FadeOut:
                if (this.ct待機.b終了値に達してない)
                    return (int)E曲読込画面の戻り値.継続;

                return (int)E曲読込画面の戻り値.読込完了;
        }
        return (int)E曲読込画面の戻り値.継続;
    }

    // その他

    #region [ private ]
    //-----------------
    private long nBGMの総再生時間ms;
    private long nBGM再生開始時刻;
    private string? strTitle;
    private string? strSubTitle;
    private CTexture? txTitle;
    private CTexture? txSubTitle;
    private DateTime timeBeginLoad;
    private DateTime timeBeginLoadWAV;
    private int nWAVcount;
    private CCounter? ct待機;
    private CCounter? ct曲名表示;
    //-----------------
    #endregion
}
