using FDK;

namespace TJAPlayerPI;

internal class CActFIFOStart : CActivity
{
    // メソッド

    public void tFadeOut開始()
    {
        this.mode = EFIFOMode.FadeOut;

        this.counter = new CCounter(0, 1500, 1, TJAPlayerPI.app.Timer);
    }
    public void tFadeIn開始()
    {
        this.mode = EFIFOMode.FadeIn;
        this.counter = new CCounter(0, 1500, 1, TJAPlayerPI.app.Timer);
    }

    // CActivity 実装

    public override void On非活性化()
    {
        if (!base.b活性化してない)
        {
            base.On非活性化();
        }
    }
    public override int On進行描画()
    {
        if (base.b活性化してない || (this.counter is null))
        {
            return 0;
        }
        this.counter.t進行();

        if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
        {
            if (TJAPlayerPI.app.Tx.SongLoading_v2_BG is not null)
            {
                if (this.mode == EFIFOMode.FadeOut)
                {
                    int x = Math.Max(1000 - this.counter.n現在の値, 0);
                    int num = Math.Min(100, x);
                    TJAPlayerPI.app.Tx.SongLoading_v2_BG.t2D幕用描画(TJAPlayerPI.app.Device, -x, 0, new Rectangle(0, 0, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Height), true, num);
                    TJAPlayerPI.app.Tx.SongLoading_v2_BG.t2D幕用描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2) + x, 0, new Rectangle(TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, 0, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Height), false, num);
                }
                else
                {
                    int x = Math.Max(this.counter.n現在の値 - 500, 0);
                    int num = Math.Min(100, x);
                    TJAPlayerPI.app.Tx.SongLoading_v2_BG.t2D幕用描画(TJAPlayerPI.app.Device, -x, 0, new Rectangle(0, 0, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Height), true, num);
                    TJAPlayerPI.app.Tx.SongLoading_v2_BG.t2D幕用描画(TJAPlayerPI.app.Device, (TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2) + x, 0, new Rectangle(TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, 0, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.SongLoading_v2_BG.szTextureSize.Height), false, num);
                }
            }
        }
        else
        {
            if (this.mode == EFIFOMode.FadeOut)
            {
                if (TJAPlayerPI.app.Tx.SongLoading_BG is not null)
                {
                    int y = this.counter.n現在の値 >= 840 ? 840 : this.counter.n現在の値;
                    TJAPlayerPI.app.Tx.SongLoading_BG.t2D描画(TJAPlayerPI.app.Device, 0, TJAPlayerPI.app.LogicalSize.Height - y);
                }
            }
            else
            {
                if (TJAPlayerPI.app.Tx.SongLoading_BG is not null)
                {
                    int y = this.counter.n現在の値;
                    int sa = (TJAPlayerPI.app.Tx.SongLoading_BG.szTextureSize.Height - TJAPlayerPI.app.LogicalSize.Height) / 2;
                    TJAPlayerPI.app.Tx.SongLoading_BG.t2D描画(TJAPlayerPI.app.Device, 0, -sa - y);
                }
            }
        }

        if (this.mode == EFIFOMode.FadeOut)
        {
            if (this.counter.b終了値に達してない)
            {
                return 0;
            }
        }
        else if (this.mode == EFIFOMode.FadeIn)
        {
            if (this.counter.b終了値に達してない)
            {
                return 0;
            }
        }
        return 1;
    }


    // その他

    #region [ private ]
    //-----------------
    private CCounter? counter = null;
    private EFIFOMode mode;
    //-----------------
    #endregion
}
