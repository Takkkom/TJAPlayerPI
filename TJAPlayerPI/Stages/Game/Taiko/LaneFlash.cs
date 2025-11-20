using FDK;

namespace TJAPlayerPI;

/// <summary>
/// レーンフラッシュのクラス。
/// </summary>
public class LaneFlash : CActivity
{

    public LaneFlash(ref CTexture? texture, int player)
    {
        Texture = texture;
        Player = player;
    }

    public void Start()
    {
        Counter = new CCounter(0, 100, 2, TJAPlayerPI.app.Timer);
    }

    public override void On活性化()
    {
        Counter = new CCounter();
        base.On活性化();
    }

    public override void On非活性化()
    {
        Counter = null;
        base.On非活性化();
    }

    public override int On進行描画()
    {
        if (Texture is null || Counter is null) return base.On進行描画();
        if (!Counter.b停止中)
        {
            Counter.t進行();
            if (Counter.b終了値に達した) Counter.t停止();

            float value = Counter.n現在の値 / 100.0f;
            float opacity = MathF.Cos(value * MathF.PI);
            opacity *= 0.2f;

            Texture.Opacity = (int)(opacity * 255);


            Texture.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[Player], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGY[Player]);
        }
        return base.On進行描画();
    }

    private CTexture? Texture;
    private CCounter? Counter;
    private int Player;
}
