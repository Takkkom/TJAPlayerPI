using FDK;

namespace TJAPlayerPI;

internal class CActMob : CActivity
{
    /// <summary>
    /// 踊り子
    /// </summary>
    public CActMob(CAct演奏ゲージ共通 actGauge)
    {
        this.actGauge = actGauge;
    }

    public override void On活性化()
    {
        ctMob = new CCounter();
        ctMobPtn = new CCounter();
        base.On活性化();
    }

    public override void On非活性化()
    {
        ctMob = null;
        ctMobPtn = null;
        base.On非活性化();
    }

    public override int On進行描画()
    {
        if (TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount != 1)
            return base.On進行描画();

        if (this.ctMob is not null && this.ctMobPtn is not null && TJAPlayerPI.app.Skin.Game_Mob_Ptn != 0)
        {
            ctMob.t進行LoopDb();
            ctMobPtn.t進行LoopDb();
            if (actGauge.db現在のゲージ値[0] >= 100)
            {
                TJAPlayerPI.app.Tx.Mob[(int)ctMobPtn.db現在の値]?.t2D描画(TJAPlayerPI.app.Device, 0, (720 - (TJAPlayerPI.app.Tx.Mob[0].szTextureSize.Height - 70)) + -((float)Math.Sin((float)this.ctMob.db現在の値 * (Math.PI / 180)) * 70));
            }
        }
        return base.On進行描画();
    }
    #region[ private ]
    //-----------------
    private CAct演奏ゲージ共通 actGauge;
    public CCounter? ctMob;
    public CCounter? ctMobPtn;
    //-----------------
    #endregion
}
