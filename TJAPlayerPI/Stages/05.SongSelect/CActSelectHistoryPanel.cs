using FDK;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI;

internal class CActSelectHistoryPanel : CActivity
{
    // メソッド

    public CActSelectHistoryPanel(CStage選曲 stage選曲)
    {
        this.stage選曲 = stage選曲;
    }

    // CActivity 実装

    public override void On活性化()
    {
        this.Font = HFontHelper.tCreateFont(40);
        tSongChange(null, null);
        base.On活性化();
    }
    public override void On非活性化()
    {
        this.ct登場アニメ用 = null;

        for (int i = 0; i < (int)Difficulty.Total; i++)
            for (int j = 0; j < 3; j++)
                TJAPlayerPI.t安全にDisposeする(ref this.Names[i, j]);
        if (Font is not null)
        {
            Font.Dispose();
            Font = null;
        }
        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (!base.b活性化してない)
        {
            if (base.b初めての進行描画)
            {
                this.ct登場アニメ用 = new CCounter(0, 3000, 1, TJAPlayerPI.app.Timer);
                base.b初めての進行描画 = false;
            }
            if (ct登場アニメ用 is null)
                return 0;
            this.ct登場アニメ用.t進行();
            int[] x = TJAPlayerPI.app.Skin.SkinConfig.SongSelect.ScoreWindowX;
            int[] y = TJAPlayerPI.app.Skin.SkinConfig.SongSelect.ScoreWindowY;
            int xdiff = 170;
            for (int i = 0; i < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; i++)
            {
                if (this.r現在選択中のスコア is not null && r現在選択中の曲 is not null && this.ct登場アニメ用.b終了値に達した && r現在選択中の曲.eNodeType == C曲リストノード.ENodeType.SCORE)
                {
                    int diff = stage選曲.n現在選択中の曲の難易度[i];
                    if (TJAPlayerPI.app.Tx.SongSelect_ScoreWindow[diff] is not null && TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text is not null)
                    {
                        TJAPlayerPI.app.Tx.SongSelect_ScoreWindow[diff]?.t2D描画(TJAPlayerPI.app.Device, x[i], y[i]);
                        for (int j = 0; j < 3; j++)
                        {
                            this.Names[diff, j]?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.UpRight, x[i] + xdiff + 50, y[i] + 65 + j * 70);
                            this.t小文字表示(x[i] + xdiff, y[i] + 90 + j * 70, this.r現在選択中のスコア.譜面情報.nHiScore[diff][j]);
                            TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.t2D描画(TJAPlayerPI.app.Device, x[i] + xdiff + 15, y[i] + 95 + j * 70, new Rectangle(0, 36, 32, 30));
                        }
                    }
                }
            }
        }
        return 0;
    }

    // その他

    #region [ private ]
    //-----------------
    private CCounter? ct登場アニメ用;
    private CTexture?[,] Names = new CTexture?[(int)Difficulty.Total, 3];
    private CCachedFontRenderer? Font;
    private Cスコア? r現在選択中のスコア;
    private C曲リストノード? r現在選択中の曲;
    private CStage選曲 stage選曲;
    //-----------------

    private void t小文字表示(int x, int y, long n)
    {
        if (TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text is null)
            return;

        for (int index = 0; index < n.ToString().Length; index++)
        {
            int Num = (int)(n / Math.Pow(10, index) % 10);
            Rectangle rectangle = new Rectangle((TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.szTextureSize.Width / 10) * Num, 0, TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.szTextureSize.Height / 2);
            TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.t2D描画(TJAPlayerPI.app.Device, x, y, rectangle);
            x -= TJAPlayerPI.app.Tx.SongSelect_ScoreWindow_Text.szTextureSize.Width / 10;
        }
    }

    public void tSongChange(C曲リストノード? song, Cスコア? score)
    {
        this.r現在選択中の曲 = song;
        this.r現在選択中のスコア = score;
        this.ct登場アニメ用 = new CCounter(0, 2000, 1, TJAPlayerPI.app.Timer);

        //Dispose
        for (int i = 0; i < (int)Difficulty.Total; i++)
            for (int j = 0; j < 3; j++)
                TJAPlayerPI.t安全にDisposeする(ref this.Names[i, j]);

        if (this.r現在選択中のスコア is not null)
        {
            string[][] HiScorerName = this.r現在選択中のスコア.譜面情報.strHiScorerName;

            if (Font is not null)
                for (int index = 0; index < (int)Difficulty.Total; index++)
                {
                    for (int j = 0; j < 3; j++)
                        if (!string.IsNullOrEmpty(HiScorerName[index][j]))
                        {
                            var name = this.Names[index, j] = TJAPlayerPI.app.tCreateTexture(Font.DrawText(HiScorerName[index][j], Color.Black));
                            if (name is not null)
                                name.vcScaling = new Vector2(0.5f);
                        }
                }
        }
    }


    #endregion
}
