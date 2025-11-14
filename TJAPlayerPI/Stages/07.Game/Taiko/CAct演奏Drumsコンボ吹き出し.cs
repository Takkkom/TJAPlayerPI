using FDK;
using System.Numerics;

namespace TJAPlayerPI;

internal class CAct演奏Drumsコンボ吹き出し : CActivity
{
    // コンストラクタ

    /// <summary>
    /// 100コンボごとに出る吹き出し。
    /// 本当は「10000点」のところも動かしたいけど、技術不足だし保留。
    /// </summary>
    public CAct演奏Drumsコンボ吹き出し()
    {
    }


    // メソッド
    public virtual void Start(int nCombo, int player)
    {
        this.ct進行[player] = new CCounter(1, 103, 20, TJAPlayerPI.app.Timer);
        this.nCombo_渡[player] = nCombo;
        tSetState(EComboState.In, player);
    }

    // CActivity 実装

    public override void On活性化()
    {
        Dictionary<char, Rectangle> dict = new Dictionary<char, Rectangle>();
        int width = 56;
        int height = 64;
        for (int i = 0; i < 10; i++)
        {
            dict.Add(i.ToString()[0], new Rectangle(width * i, 0, width, height));
        }
        st小文字位置 = dict.ToFrozenDictionary();

        for (int i = 0; i < 2; i++)
        {
            this.nCombo_渡[i] = 0;
            this.ct進行[i] = new CCounter();
            this.ctIn[i] = new CCounter();
            this.ctWait[i] = new CCounter();
            this.ctOut[i] = new CCounter();
        }

        base.On活性化();
    }
    public override void On非活性化()
    {
        for (int i = 0; i < 2; i++)
        {
            this.ct進行[i] = null;
            this.ctIn[i] = null;
            this.ctOut[i] = null;
        }

        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (base.b活性化してない)
            return 0;

        for (int i = 0; i < 2; i++)
        {
            if (!this.ct進行[i].b停止中)
            {
                this.ct進行[i].t進行();
                if (this.ct進行[i].b終了値に達した)
                {
                    this.ct進行[i].t停止();
                }
            }

            ctIn[i].t進行();
            ctWait[i].t進行();
            ctOut[i].t進行();

            CTexture? combo = TJAPlayerPI.app.Tx.Balloon_Combo[i];
            CTexture? num_combo = TJAPlayerPI.app.Tx.Balloon_Number_Combo;
            CTexture? num_combo_text = TJAPlayerPI.app.Tx.Balloon_Number_Combo_Text;
            CTexture? num_score = TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score;
            CTexture? num_score_flash = TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score_Flash;
            CTexture? num_score_text = TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score_Text;

            if (TJAPlayerPI.app.ConfigToml.PlayOption.Shinuchi[i])
            {
                combo = TJAPlayerPI.app.Tx.Balloon_Combo_Shin[i];
            }
            if (combo is not null && num_combo is not null)
            {
                //半透明4f
                /*
                if (this.ct進行[i].n現在の値 == 1 || this.ct進行[i].n現在の値 == 103)
                {
                    combo.Opacity = 64;
                    num_combo.Opacity = 64;
                }
                else if (this.ct進行[i].n現在の値 == 2 || this.ct進行[i].n現在の値 == 102)
                {
                    combo.Opacity = 128;
                    num_combo.Opacity = 128;
                }
                else if (this.ct進行[i].n現在の値 == 3 || this.ct進行[i].n現在の値 == 101)
                {
                    combo.Opacity = 192;
                    num_combo.Opacity = 192;
                }
                else if (this.ct進行[i].n現在の値 >= 4 && this.ct進行[i].n現在の値 <= 100)
                {
                    combo.Opacity = 255;
                    num_combo.Opacity = 255;
                }
                */

                int baseOpacity = 0;
                int flashOpacity = 0;
                int scoreOutValue = 0;

                switch (this.eComboStates[i])
                {
                    case EComboState.In:
                        {
                            baseOpacity = (int)(CConvert.InverseLerpClamp(0, 100, this.ctIn[i].n現在の値) * 255);

                            float flashValue = CConvert.InverseLerpClamp(150, 500, this.ctIn[i].n現在の値);
                            flashOpacity = (int)(CConvert.ZigZagWave(flashValue * 0.5f) * 255);

                            if (this.ctIn[i].n現在の値 == this.ctIn[i].n終了値)
                            {
                                tSetState(EComboState.Wait, i);
                            }
                        }
                        break;
                    case EComboState.Wait:
                        {
                            baseOpacity = 255;
                            flashOpacity = 0;

                            if (this.ctWait[i].n現在の値 == this.ctWait[i].n終了値)
                            {
                                tSetState(EComboState.Out, i);
                            }
                        }
                        break;
                    case EComboState.Out:
                        {
                            baseOpacity = 255 - (int)(CConvert.InverseLerpClamp(320, 420, this.ctOut[i].n現在の値) * 255);
                            flashOpacity = 0;

                            scoreOutValue = this.ctOut[i].n現在の値;

                            if (this.ctOut[i].n現在の値 == this.ctOut[i].n終了値)
                            {
                                tSetState(EComboState.None, i);
                            }
                        }
                        break;
                }

                combo.Opacity = baseOpacity;
                num_combo.Opacity = baseOpacity;
                if (num_combo_text is not null)
                    num_combo_text.Opacity = baseOpacity;
                if (num_score is not null)
                    num_score.Opacity = baseOpacity;
                if (num_score_flash is not null)
                    num_score_flash.Opacity = flashOpacity;
                if (num_score_text is not null)
                    num_score_text.Opacity = baseOpacity;

                if (eComboStates[i] != EComboState.None)
                {
                    int comboX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboX[i];
                    int comboY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboY[i];
                    int comboNumberX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberX[i];
                    int comboNumberY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberY[i];
                    int comboTextX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextX[i];
                    int comboTextY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextY[i];
                    if (TJAPlayerPI.app.ConfigToml.PlayOption.Shinuchi[i])
                    {
                        comboX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinX[i];
                        comboY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinY[i];
                        comboNumberX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinNumberX[i];
                        comboNumberY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinNumberY[i];
                        comboTextX = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinTextX[i];
                        comboTextY = TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboShinTextY[i];
                    }

                    combo.t2D描画(TJAPlayerPI.app.Device, comboX, comboY);

                    int offset = this.t小文字表示(comboNumberX, comboNumberY, this.nCombo_渡[i].ToString());
                    num_combo_text?.t2D描画(TJAPlayerPI.app.Device, comboTextX + offset, comboTextY);

                    if (!TJAPlayerPI.app.ConfigToml.PlayOption.Shinuchi[i])
                    {
                        if (num_score_text is not null)
                        {
                            num_score_text.Opacity = tGetScoreOutOpacity(scoreOutValue, 0);
                            num_score_text.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScoreTextX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScoreTextY[i]);
                        }
                        tDrawScore(TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScoreX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScoreY[i], 10000, scoreOutValue);
                    }

                    /*
                    if (this.nCombo_渡[i] < 1000) //2016.08.23 kairera0467 仮実装。
                    {
                        this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberY[i], string.Format("{0,4:###0}", this.nCombo_渡[i]));
                        TJAPlayerPI.app.Tx.Balloon_Number_Combo_Text?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextY[i]);
                    }
                    else
                    {
                        this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberExX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboNumberExY[i], string.Format("{0,4:###0}", this.nCombo_渡[i]));
                        TJAPlayerPI.app.Tx.Balloon_Number_Combo_Text?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextExX[i], TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboTextExY[i], new Rectangle(0, 54, 77, 32));
                    }
                    */
                }
            }
        }
        return 0;
    }


    // その他

    #region [ private ]
    //-----------------
    private enum EComboState
    {
        None,
        In,
        Wait,
        Out
    }

    private const int nScoreJumpTimePadding = 3;

    private CCounter[] ct進行 = new CCounter[2];
    private CCounter[] ctIn = new CCounter[2];
    private CCounter[] ctWait = new CCounter[2];
    private CCounter[] ctOut = new CCounter[2];
    private int[] nCombo_渡 = new int[2];
    private EComboState[] eComboStates = new EComboState[2];

    private void tSetState(EComboState eComboState, int nPlayer)
    {
        const int interval = 1;
        switch (eComboState)
        {
            case EComboState.In:
                this.ctIn[nPlayer] = new CCounter(0, 500, interval, TJAPlayerPI.app.Timer);
                break;
            case EComboState.Wait:
                this.ctWait[nPlayer] = new CCounter(0, 900, interval, TJAPlayerPI.app.Timer);
                break;
            case EComboState.Out:
                this.ctOut[nPlayer] = new CCounter(0, 500, interval, TJAPlayerPI.app.Timer);
                break;
        }
        this.eComboStates[nPlayer] = eComboState;
    }

    private FrozenDictionary<char, Rectangle> st小文字位置;

    private int t小文字表示(int x, int y, string str)
    {
        int padding = 40;
        int offset = 0;
        offset -= (str.Length - 1) * padding / 2;


        foreach (char ch in str)
        {
            if (this.st小文字位置.TryGetValue(ch, out var pt))
            {
                TJAPlayerPI.app.Tx.Balloon_Number_Combo?.t2D描画(TJAPlayerPI.app.Device, x + offset, y, pt);
            }
            offset += padding;
        }

        return offset;
    }


    private void tDrawScore(int x, int y, int number, int scoreOutValue)
    {
        int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Score.Size[0];
        int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Score.Size[1];

        int jumpHeight = -8;

        string str = number.ToString();
        x -= (str.Length - 1) * TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScorePadding;

        int digitIndex = str.Length - 1;

        foreach (char ch in str)
        {
            if (int.TryParse(ch.ToString(), out int value))
            {
                Rectangle rectangle = new Rectangle(width * value, 0, width, height);

                int offsetY = 0;
                if (scoreOutValue != 0)
                {
                    int timeOffset = digitIndex * 2;
                    float jumpValue = CConvert.InverseLerpClamp(0 + timeOffset, 60 + timeOffset, scoreOutValue);
                    jumpValue = MathF.Sin(jumpValue * MathF.PI * 0.5f);
                    offsetY = (int)(jumpValue * jumpHeight);
                }

                if (TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score is not null)
                {
                    TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score.Opacity = tGetScoreOutOpacity(scoreOutValue, digitIndex + 1);
                    TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score.t2D描画(TJAPlayerPI.app.Device, x, y + offsetY, rectangle);
                }
                TJAPlayerPI.app.Tx.Balloon_Number_Combo_Score_Flash?.t2D描画(TJAPlayerPI.app.Device, x, y + offsetY, rectangle);

                x += TJAPlayerPI.app.Skin.SkinConfig.Game.Balloon.ComboScorePadding;
                digitIndex--;
            }
        }
    }

    private int tGetScoreOutOpacity(int scoreOutValue, int index)
    {
        int timeOffset = index * 16;
        float value = CConvert.InverseLerpClamp(50 + timeOffset, 90 + timeOffset, scoreOutValue);
        float opacity = 1.0f - value;
        return (int)(opacity * 255);
    }
    //-----------------
    #endregion
}
