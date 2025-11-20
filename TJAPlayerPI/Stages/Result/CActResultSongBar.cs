using FDK;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI;

internal class CActResultSongBar : CActivity
{
    // コンストラクタ

    public CActResultSongBar()
    {
    }


    // メソッド

    public void tアニメを完了させる()
    {
        this.ct登場用.n現在の値 = this.ct登場用.n終了値;
    }


    // CActivity 実装

    public override void On活性化()
    {
        // After performing calibration, inform the player that
        // calibration has been completed, rather than
        // displaying the song title as usual.


        var title = TJAPlayerPI.IsPerformingCalibration
            ? $"Calibration complete. InputAdjustTime is now {TJAPlayerPI.app.ConfigToml.PlayOption.InputAdjustTimeMs}ms"
            : TJAPlayerPI.DTX[0].TITLE;

        using (var pfMusicName = HFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameFontSize))
        {

            using (var bmpSongTitle = pfMusicName.DrawText(title, TJAPlayerPI.app.Skin.SkinConfig.Result._MusicNameForeColor, TJAPlayerPI.app.Skin.SkinConfig.Result._MusicNameBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
            {
                this.txMusicName = TJAPlayerPI.app.tCreateTexture(bmpSongTitle);
                txMusicName.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txMusicName);
            }
        }

        using (var pfStageText = HFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextFontSize))
        {
            using (var bmpStageText = pfStageText.DrawText(TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.StageText, TJAPlayerPI.app.Skin.SkinConfig.Result._StageTextForeColor, TJAPlayerPI.app.Skin.SkinConfig.Result._StageTextBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
            {
                this.txStageText = TJAPlayerPI.app.tCreateTexture(bmpStageText);
            }
        }

        base.On活性化();
    }
    public override void On非活性化()
    {
        if (this.ct登場用 is not null)
        {
            this.ct登場用 = null;
        }
        TJAPlayerPI.t安全にDisposeする(ref this.txMusicName);

        TJAPlayerPI.t安全にDisposeする(ref this.txStageText);
        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (base.b活性化してない)
        {
            return 0;
        }
        if (base.b初めての進行描画)
        {
            this.ct登場用 = new CCounter(0, 270, 4, TJAPlayerPI.app.Timer);
            base.b初めての進行描画 = false;
        }
        this.ct登場用.t進行();

        if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
        {
            if (TJAPlayerPI.app.Skin.SkinConfig.Result._v2MusicNameReferencePoint == CSkin.EReferencePoint.Center)
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameX - ((this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X) / 2), TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameY);
            }
            else if (TJAPlayerPI.app.Skin.SkinConfig.Result._v2MusicNameReferencePoint == CSkin.EReferencePoint.Left)
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameX, TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameY);
            }
            else
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameX - this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X, TJAPlayerPI.app.Skin.SkinConfig.Result.v2MusicNameY);
            }
        }
        else
        {
            if (TJAPlayerPI.app.Skin.SkinConfig.Result._MusicNameReferencePoint == CSkin.EReferencePoint.Center)
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameX - ((this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X) / 2), TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameY);
            }
            else if (TJAPlayerPI.app.Skin.SkinConfig.Result._MusicNameReferencePoint == CSkin.EReferencePoint.Left)
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameX, TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameY);
            }
            else
            {
                this.txMusicName.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameX - this.txMusicName.szTextureSize.Width * txMusicName.vcScaling.X, TJAPlayerPI.app.Skin.SkinConfig.Result.MusicNameY);
            }

            if (TJAPlayerPI.app.n確定された曲の難易度[0] != (int)Difficulty.Dan)
            {
                if (TJAPlayerPI.app.Skin.SkinConfig.Result._StageTextReferencePoint == CSkin.EReferencePoint.Center)
                {
                    this.txStageText.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextX - ((this.txStageText.szTextureSize.Width * txStageText.vcScaling.X) / 2), TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextY);
                }
                else if (TJAPlayerPI.app.Skin.SkinConfig.Result._StageTextReferencePoint == CSkin.EReferencePoint.Right)
                {
                    this.txStageText.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextX - this.txStageText.szTextureSize.Width, TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextY);
                }
                else
                {
                    this.txStageText.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextX, TJAPlayerPI.app.Skin.SkinConfig.Result.StageTextY);
                }
            }
        }


        if (!this.ct登場用.b終了値に達した)
        {
            return 0;
        }
        return 1;
    }


    // その他

    #region [ private ]
    //-----------------
    private CCounter ct登場用;

    private CTexture txMusicName;

    private CTexture txStageText;
    //-----------------
    #endregion
}
