using FDK;
using TJAPlayerPI.Common;

namespace TJAPlayerPI;

internal class CActPanel : CActivity
{

    // コンストラクタ

    public CActPanel()
    {
        this.Start();
    }


    // メソッド

    /// <summary>
    /// 右上の曲名、曲数表示の更新を行います。
    /// </summary>
    /// <param name="songName">曲名</param>
    /// <param name="genreName">ジャンル名</param>
    /// <param name="stageText">曲数</param>
    public void SetPanelString(string songName, string subtitle, string genreName, string? stageText = null)
    {
        if (!base.b活性化してる)
            return;

        TJAPlayerPI.t安全にDisposeする(ref this.txPanel);
        if (!string.IsNullOrEmpty(songName) && this.pfMusicName is not null)
        {
            try
            {
                TJAPlayerPI.t安全にDisposeする(ref txMusicName);
                TJAPlayerPI.t安全にDisposeする(ref txSubTitleName);
                using (var bmpSongTitle = pfMusicName.DrawText(songName, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                {
                    this.txMusicName = TJAPlayerPI.app.tCreateTexture(bmpSongTitle);
                }
                if (txMusicName is not null)
                {
                    this.txMusicName.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txMusicName);
                }
                if (!string.IsNullOrEmpty(subtitle) && this.pfSubTitleName is not null)
                {
                    using (var bmpSubTitle = pfSubTitleName.DrawText(subtitle, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                    {
                        this.txSubTitleName = TJAPlayerPI.app.tCreateTexture(bmpSubTitle);
                    }
                    if (txSubTitleName is not null)
                    {
                        this.txSubTitleName.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txSubTitleName, 520);
                    }
                }
                if (!string.IsNullOrEmpty(stageText))
                {
                    using (var bmpDiff = pfMusicName.DrawText(stageText, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._StageTextForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._StageTextBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                    {
                        this.tx難易度とステージ数 = TJAPlayerPI.app.tCreateTexture(bmpDiff);
                    }
                }
            }
            catch (CTextureCreateFailedException e)
            {
                Trace.TraceError(e.ToString());
                Trace.TraceError("パネル文字列テクスチャの生成に失敗しました。");
                this.txPanel = null;
            }
        }
        if (!string.IsNullOrEmpty(genreName))
        {
            this.txGENRE = TJAPlayerPI.app.Tx.TxCGen(TJAPlayerPI.app.Skin.nStrジャンルtoNum(genreName).ToString());
        }

        this.ct進行用 = new CCounter(0, 2000, 2, TJAPlayerPI.app.Timer);
        this.Start();
    }

    public void Stop()
    {
        this.bMute = true;
    }
    public void Start()
    {
        this.bMute = false;
    }


    // CActivity 実装

    public override void On活性化()
    {
        this.pfMusicName = CFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameFontSize);
        this.pfSubTitleName = CFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameFontSize);

        this.txPanel = null;
        this.ct進行用 = new CCounter();
        this.Start();
        this.bFirst = true;
        base.On活性化();
    }
    public override void On非活性化()
    {
        this.ct進行用 = null;

        TJAPlayerPI.t安全にDisposeする(ref this.txPanel);
        TJAPlayerPI.t安全にDisposeする(ref this.txMusicName);
        TJAPlayerPI.t安全にDisposeする(ref this.txSubTitleName);
        TJAPlayerPI.t安全にDisposeする(ref this.txGENRE);
        TJAPlayerPI.t安全にDisposeする(ref this.txPanel);
        TJAPlayerPI.t安全にDisposeする(ref this.pfMusicName);
        TJAPlayerPI.t安全にDisposeする(ref this.pfSubTitleName);
        TJAPlayerPI.t安全にDisposeする(ref this.tx難易度とステージ数);
        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (TJAPlayerPI.stage演奏ドラム画面.actDan.IsAnimating) return 0;
        if (!base.b活性化してない && !this.bMute && this.ct進行用 is not null)
        {
            this.ct進行用.t進行Loop();
            if (this.bFirst)
            {
                this.ct進行用.n現在の値 = 300;
            }
            if (this.txGENRE is not null)
                this.txGENRE.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.GenreX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.GenreY);

            if (!TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.StageTextDisp)
            {
                if (this.txMusicName is not null)
                {
                    float fRate = 660.0f / this.txMusicName.szTextureSize.Width;
                    if (this.txMusicName.szTextureSize.Width <= 660.0f)
                        fRate = 1.0f;
                    this.txMusicName.vcScaling.X = fRate;
                    if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Center)
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX - ((this.txMusicName.szTextureSize.Width * fRate) / 2), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    else
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX - (this.txMusicName.szTextureSize.Width * fRate), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    if (this.txSubTitleName is not null)
                    {
                        fRate = 600.0f / this.txSubTitleName.szTextureSize.Width;
                        if (this.txSubTitleName.szTextureSize.Width <= 600.0f)
                            fRate = 1.0f;
                        this.txSubTitleName.vcScaling.X = fRate;
                        if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._SubTitleNameReferencePoint == CSkin.EReferencePoint.Center)
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX - ((this.txSubTitleName.szTextureSize.Width * fRate) / 2), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                        else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._SubTitleNameReferencePoint == CSkin.EReferencePoint.Left)
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                        else
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX - (this.txSubTitleName.szTextureSize.Width * fRate), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                    }
                }
            }
            else
            {
                #region[ 透明度制御 ]

                if (this.txMusicName is not null)
                {
                    if (this.ct進行用.n現在の値 < 745)
                    {
                        this.bFirst = false;
                        this.txMusicName.Opacity = 255;
                        if (this.txSubTitleName is not null)
                            this.txSubTitleName.Opacity = 255;
                        if (this.txGENRE is not null)
                            this.txGENRE.Opacity = 255;
                        if (this.tx難易度とステージ数 is not null)
                            this.tx難易度とステージ数.Opacity = 0;
                    }
                    else if (this.ct進行用.n現在の値 >= 745 && this.ct進行用.n現在の値 < 1000)
                    {
                        this.txMusicName.Opacity = 255 - (this.ct進行用.n現在の値 - 745);
                        if (this.txSubTitleName is not null)
                            this.txSubTitleName.Opacity = 255 - (this.ct進行用.n現在の値 - 745);
                        if (this.txGENRE is not null)
                            this.txGENRE.Opacity = 255 - (this.ct進行用.n現在の値 - 745);
                        if (this.tx難易度とステージ数 is not null)
                            this.tx難易度とステージ数.Opacity = this.ct進行用.n現在の値 - 745;
                    }
                    else if (this.ct進行用.n現在の値 >= 1000 && this.ct進行用.n現在の値 <= 1745)
                    {
                        this.txMusicName.Opacity = 0;
                        if (this.txSubTitleName is not null)
                            this.txSubTitleName.Opacity = 0;
                        if (this.txGENRE is not null)
                            this.txGENRE.Opacity = 0;
                        if (this.tx難易度とステージ数 is not null)
                            this.tx難易度とステージ数.Opacity = 255;
                    }
                    else if (this.ct進行用.n現在の値 >= 1745)
                    {
                        this.txMusicName.Opacity = this.ct進行用.n現在の値 - 1745;
                        if (this.txSubTitleName is not null)
                            this.txSubTitleName.Opacity = this.ct進行用.n現在の値 - 1745;
                        if (this.txGENRE is not null)
                            this.txGENRE.Opacity = this.ct進行用.n現在の値 - 1745;
                        if (this.tx難易度とステージ数 is not null)
                            this.tx難易度とステージ数.Opacity = 255 - (this.ct進行用.n現在の値 - 1745);
                    }
                    #endregion
                    if (this.b初めての進行描画)
                    {
                        b初めての進行描画 = false;
                    }
                    if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Center)
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX - ((this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X) / 2), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Left)
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    else
                    {
                        this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX - (this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
                    }
                    if (this.txSubTitleName is not null)
                    {
                        if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._SubTitleNameReferencePoint == CSkin.EReferencePoint.Center)
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX - ((this.txSubTitleName.szTextureSize.Width * this.txSubTitleName.vcScaling.X) / 2), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                        else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._SubTitleNameReferencePoint == CSkin.EReferencePoint.Left)
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                        else
                        {
                            this.txSubTitleName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameX - (this.txSubTitleName.szTextureSize.Width * this.txSubTitleName.vcScaling.X), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.SubTitleNameY);
                        }
                    }
                }
                CTexture.RefPnt stageRefPoint;
                if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Center)
                {
                    stageRefPoint = CTexture.RefPnt.Up;
                }
                else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._MusicNameReferencePoint == CSkin.EReferencePoint.Left)
                {
                    stageRefPoint = CTexture.RefPnt.UpLeft;
                }
                else
                {
                    stageRefPoint = CTexture.RefPnt.UpRight;
                }
                this.tx難易度とステージ数?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, stageRefPoint, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameX, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.MusicNameY);
            }
        }
        return base.On進行描画();
    }


    // その他

    #region [ private ]
    //-----------------
    private CCounter? ct進行用;
    private bool bMute;
    private bool bFirst;

    private CTexture? txPanel,
                    txMusicName,
                    txSubTitleName,
                    tx難易度とステージ数,
                    txGENRE;
    private CCachedFontRenderer? pfMusicName,
                                pfSubTitleName;
    //-----------------
    #endregion
}
