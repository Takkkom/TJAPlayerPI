using FDK;
using SkiaSharp;

namespace TJAPlayerPI;

internal class CActLyric : CActivity
{
    // コンストラクタ
    public CActLyric()
    {
    }

    public void tSetLyricTexture(SKBitmap bmpLyric)
    {
        TJAPlayerPI.t安全にDisposeする(ref this.txLyricTexture);
        this.txLyricTexture = TJAPlayerPI.app.tCreateTexture(bmpLyric);
    }
    public void tDeleteLyricTexture()
    {
        TJAPlayerPI.t安全にDisposeする(ref this.txLyricTexture);
    }
    public override void On活性化()
    {
        if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._LyricReferencePoint == CSkin.EReferencePoint.Left)
            refRatio = 0;
        else if (TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont._LyricReferencePoint == CSkin.EReferencePoint.Right)
            refRatio = 1;
        else
            refRatio = 0.5f;

        base.On活性化();
    }
    public override void On非活性化()
    {
        TJAPlayerPI.t安全にDisposeする(ref this.txLyricTexture);
        base.On非活性化();
    }
    public override int On進行描画()
    {
        this.txLyricTexture?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.LyricX - (this.txLyricTexture.szTextureSize.Width * refRatio), TJAPlayerPI.app.Skin.SkinConfig.Game.PanelFont.LyricY);

        return base.On進行描画();
    }

    private float refRatio;
    private CTexture? txLyricTexture;
}
