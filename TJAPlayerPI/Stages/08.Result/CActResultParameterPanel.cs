using FDK;

namespace TJAPlayerPI;

internal class CActResultParameterPanel : CActivity
{
    // コンストラクタ

    public CActResultParameterPanel()
    {
    }


    // メソッド

    public void tアニメを完了させる()
    {
        this.ephase = EPhase.Loop;
        this.ephase_v2 = EPhaseV2.Loop;
        this.ctGauge.n現在の値 = this.ctGauge.n終了値;
        this.ctCrown用.n現在の値 = this.ctCrown用.n終了値;
    }


    // CActivity 実装

    public override void On活性化()
    {
        base.On活性化();
        this.ephase = EPhase.Start;
        this.ctCrown用 = new CCounter(0, 255, 2, TJAPlayerPI.app.Timer);
        this.ct文字V2用 = new CCounter(0, 180, 3, TJAPlayerPI.app.Timer);
        this.ctGauge = new CCounter(0, 100, 35, TJAPlayerPI.app.Timer);
        for (int index = 0; index < 4; index++)
        {
            this.ct文字アニメ用[index] = new CCounter(0, 15, 50, TJAPlayerPI.app.Timer);

            this.ToNextPhase[index] = false;
            this.n表示された桁数[index] = 0;
        }
        this.AllPlayerCannotGetCrown = true;
        for (int index = 0; index < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; index++)
        {
            if (TJAPlayerPI.stage選曲.n確定された曲の難易度[index] == (int)Difficulty.Dan)
            {
                switch (TJAPlayerPI.stage演奏ドラム画面.actDan.GetExamStatus(TJAPlayerPI.stageResult.cRecords[index].DanC, TJAPlayerPI.stageResult.cRecords[index].DanCGauge))
                {
                    case Exam.Status.Failure:
                        this.CrownState[index] = 0;
                        break;
                    case Exam.Status.Success:
                        this.CrownState[index] = 1;
                        break;
                    case Exam.Status.Better_Success:
                        this.CrownState[index] = 2;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (TJAPlayerPI.stageResult.cRecords[index].Gauge < 80)
                {
                    this.CrownState[index] = 0;
                }
                else if (TJAPlayerPI.stageResult.cRecords[index].MissCount != 0 && TJAPlayerPI.stageResult.cRecords[index].BadCount != 0)
                {
                    this.CrownState[index] = 1;
                    this.AllPlayerCannotGetCrown = false;
                }
                else if (TJAPlayerPI.stageResult.cRecords[index].GoodCount != 0)
                {
                    this.CrownState[index] = 2;
                    this.AllPlayerCannotGetCrown = false;
                }
                else
                {
                    this.CrownState[index] = 3;
                    this.AllPlayerCannotGetCrown = false;
                }
            }
        }

        Dan_Plate = TJAPlayerPI.app.tCreateTexture(Path.GetDirectoryName(TJAPlayerPI.DTX[0].strFilenameの絶対パス) + @"/Dan_Plate.png");
        this.ephase_v2 = EPhaseV2.Start;
    }
    public override void On非活性化()
    {
        TJAPlayerPI.t安全にDisposeする(ref Dan_Plate);
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
            base.b初めての進行描画 = false;
        }
        if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
        {
            this.ct文字V2用.t進行();

            #region[phaseの進行]
            if (ephase_v2 == EPhaseV2.Start)
            {
                this.ctGauge.n現在の値 = 0;
                this.ctGauge.t時間Reset();
                this.NextPhaseV2();

            }
            if (ephase_v2 == EPhaseV2.Gauge)
            {
                if (this.ctGauge.b終了値に達した)
                    this.NextPhaseV2();
            }
            else if (ephase_v2 == EPhaseV2.Skill)
            {
                this.ctCrown用.n現在の値 = 0;
                this.ctCrown用.t時間Reset();
                this.NextPhaseV2();
            }
            else if (ephase_v2 == EPhaseV2.Crown)
            {
                if (this.ctCrown用.b終了値に達した)
                    this.NextPhaseV2();
            }
            else if (ephase_v2 != EPhaseV2.Loop)
            {
                if (this.ct文字V2用.b終了値に達した)
                    this.NextPhaseV2();
            }
            #endregion

            if (ephase_v2 == EPhaseV2.Crown)
                this.ctCrown用.t進行();
            else if (ephase_v2 == EPhaseV2.Gauge)
                ctGauge.t進行();

            for (int i = 0; i < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; i++)
            {
                if (TJAPlayerPI.app.Tx.Result_v2_Panel is not null)
                {
                    var paneli = TJAPlayerPI.app.Tx.Result_v2_Panel[i];
                    if (paneli is not null)
                        paneli.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2PanelX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2PanelY[i]);
                }
                if (TJAPlayerPI.app.Tx.Result_v2_GaugeBack is not null)
                {
                    TJAPlayerPI.app.Tx.Result_v2_GaugeBack.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBackX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBackY[i]);
                }
                if (TJAPlayerPI.app.Tx.Result_v2_GaugeBase is not null && TJAPlayerPI.app.Tx.Result_v2_Gauge is not null)
                {
                    int width = (int)(TJAPlayerPI.app.Tx.Result_v2_Gauge.szTextureSize.Width * (Math.Min((TJAPlayerPI.stageResult.cRecords[i].Gauge / 100f), this.ctGauge.n現在の値 / 100f))) / (TJAPlayerPI.app.Tx.Result_v2_Gauge.szTextureSize.Width / 50) * (TJAPlayerPI.app.Tx.Result_v2_Gauge.szTextureSize.Width / 50);// 2020/10/13 Mr-Ojii 最後の意味が無いように見える乗算、除算には意味があります。消さないで。
                    Rectangle rec = new Rectangle(0, 0, width, TJAPlayerPI.app.Tx.Result_v2_Gauge.szTextureSize.Height);
                    TJAPlayerPI.app.Tx.Result_v2_GaugeBase.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBodyX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBodyY[i]);
                    TJAPlayerPI.app.Tx.Result_v2_Gauge.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBodyX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2GaugeBodyY[i], rec);
                }

                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2PerfectX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2PerfectY[i], TJAPlayerPI.stageResult.cRecords[i].PerfectCount, false, EPhaseV2.Perfect, i);
                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2GoodX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2GoodY[i], TJAPlayerPI.stageResult.cRecords[i].GoodCount, false, EPhaseV2.Good, i);
                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2BadX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2BadY[i], TJAPlayerPI.stageResult.cRecords[i].MissCount + TJAPlayerPI.stageResult.cRecords[i].BadCount, false, EPhaseV2.Bad, i);
                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2RollX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2RollY[i], TJAPlayerPI.stageResult.cRecords[i].RollCount, false, EPhaseV2.Roll, i);
                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2ComboX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2ComboY[i], TJAPlayerPI.stageResult.cRecords[i].MaxCombo, false, EPhaseV2.Combo, i);

                this.t小文字表示V2(TJAPlayerPI.app.Skin.SkinConfig.Result.v2ScoreX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2ScoreY[i], TJAPlayerPI.stageResult.cRecords[i].Score, true, EPhaseV2.Score, i);

                #region 段位認定モード用+王冠
                if (TJAPlayerPI.stage選曲.n確定された曲の難易度[i] == (int)Difficulty.Dan)
                {
                    TJAPlayerPI.stage演奏ドラム画面.actDan.DrawExam(TJAPlayerPI.stageResult.cRecords[i].DanC);

                    TJAPlayerPI.app.Tx.Result_Dan?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.DanXY[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanXY[1], new Rectangle(TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[0] * CrownState[i], 0, TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[1]));
                    // Dan_Plate
                    Dan_Plate?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Result.DanPlateXY[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanPlateXY[1]);
                }
                else
                {
                    if (CrownState[i] != 0 && TJAPlayerPI.app.Tx.Crown_t is not null)
                    {
                        TJAPlayerPI.app.Tx.Crown_t.Opacity = this.ctCrown用.n現在の値;
                        TJAPlayerPI.app.Tx.Crown_t.vcScaling.X = ((this.ctCrown用.n終了値 - this.ctCrown用.n現在の値) / 255f) * 2f + 1.0f;
                        TJAPlayerPI.app.Tx.Crown_t.vcScaling.Y = ((this.ctCrown用.n終了値 - this.ctCrown用.n現在の値) / 255f) * 2f + 1.0f;
                        TJAPlayerPI.app.Tx.Crown_t.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Result.v2CrownX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.v2CrownY[i], new Rectangle(CrownState[i] * 100, 0, 100, 100));
                    }
                }
                #endregion

            }
            return (ephase_v2 == EPhaseV2.Loop) ? 1 : 0;
        }
        else
        {
            for (int i = 0; i < this.ct文字アニメ用.Length; i++)
                if (!this.ToNextPhase[i])
                    this.ct文字アニメ用[i].t進行();
            this.ctCrown用.t進行();

            #region[phaseの進行]
            if (ephase == EPhase.Start)
            {
                ctCrown用.n現在の値 = 0;
                ctCrown用.t時間Reset();
                this.NextPhase();
            }
            else if (ephase == EPhase.Crown)
            {
                if (this.ctCrown用.b終了値に達した || this.AllPlayerCannotGetCrown)
                {
                    this.NextPhase();
                }
            }
            else if (ephase == EPhase.HighScore)
            {
                this.NextPhase();
            }
            else if (ephase != EPhase.Loop)
            {
                bool bIstrue = true;
                for (int index = 0; index < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; index++)
                {
                    if (!this.ToNextPhase[index])
                        bIstrue = false;
                }

                if (bIstrue)
                {
                    this.NextPhase();
                }
            }
            #endregion

            for (int i = 0; i < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; i++)
            {
                /*
                if (TJAPlayerPI.app.Tx.Result_Panel is not null)
                {
                    TJAPlayerPI.app.Tx.Result_Panel.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.PanelX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.PanelY[i]);
                }
                if (TJAPlayerPI.app.Tx.Result_Score_Text is not null)
                {
                    int[] s_y = { 249, 543 };
                    TJAPlayerPI.app.Tx.Result_Score_Text.t2D描画(TJAPlayerPI.app.Device, 753, s_y[i]); //点
                }
                if (TJAPlayerPI.app.Tx.Result_Judge is not null)
                {
                    TJAPlayerPI.app.Tx.Result_Judge.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.JudgeX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.JudgeY[i]);
                }
                if (TJAPlayerPI.app.Tx.Result_Gauge_Base is not null && TJAPlayerPI.app.Tx.Result_Gauge is not null)
                {
                    double Rate = TJAPlayerPI.stageResult.cRecords[i].Gauge;
                    TJAPlayerPI.app.Tx.Result_Gauge_Base.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.GaugeBaseX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.GaugeBaseY[i], new Rectangle(0, 0, 691, 47));
                    #region[ ゲージ本体 ]
                    int[] y_tmp = { 145, 439 };
                    int y = y_tmp[i];
                    if (Rate > 2)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 4)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 12, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 6)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 24, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 8)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 36, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 10)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 49, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 12)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 62, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 14)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 74, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 16)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 86, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 18)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 99, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 20)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 112, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 22)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 125, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 24)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 138, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 26)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 150, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 28)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 162, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 30)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 175, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 32)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 187, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 34)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 200, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 36)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 212, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 38)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 225, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 40)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 238, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 42)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 251, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 44)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 263, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 46)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 276, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 48)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 288, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 50)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 301, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 52)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 313, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 54)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 326, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 56)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 339, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 58)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 352, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 60)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 364, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 62)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 377, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 64)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 390, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 66)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 402, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 68)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 415, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 70)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 427, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 72)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 440, y, new Rectangle(0, 20, 12, 20));
                    if (Rate > 74)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 452, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 76)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 465, y, new Rectangle(12, 20, 13, 20));
                    if (Rate > 78)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 478, y, new Rectangle(12, 20, 13, 20));

                    if (Rate > 80)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 491, y - 20, new Rectangle(25, 0, 12, 40));
                    if (Rate > 82)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 503, y - 20, new Rectangle(49, 0, 13, 40));
                    if (Rate > 84)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 516, y - 20, new Rectangle(37, 0, 12, 40));
                    if (Rate > 86)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 528, y - 20, new Rectangle(49, 0, 13, 40));
                    if (Rate > 88)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 541, y - 20, new Rectangle(37, 0, 12, 40));
                    if (Rate > 90)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 553, y - 20, new Rectangle(49, 0, 13, 40));
                    if (Rate > 92)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 566, y - 20, new Rectangle(49, 0, 13, 40));
                    if (Rate > 94)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 579, y - 20, new Rectangle(37, 0, 12, 40));
                    if (Rate > 96)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 591, y - 20, new Rectangle(49, 0, 13, 40));
                    if (Rate > 98)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 604, y - 20, new Rectangle(37, 0, 12, 40));
                    if (Rate >= 100)
                        TJAPlayerPI.app.Tx.Result_Gauge.t2D描画(TJAPlayerPI.app.Device, 559 + 616, y - 20, new Rectangle(49, 0, 10, 40));

                    #endregion
                }
                if (TJAPlayerPI.app.Tx.Gauge_Soul is not null)
                {
                    int[] y_Fire = { 34, 328 };
                    int[] y_Soul = { 107, 401 };
                    if (TJAPlayerPI.app.Tx.Gauge_Soul_Fire is not null && TJAPlayerPI.stageResult.cRecords[i].Gauge >= 100.0f)
                        TJAPlayerPI.app.Tx.Gauge_Soul_Fire.t2D描画(TJAPlayerPI.app.Device, 1100, y_Fire[i], new Rectangle(0, 0, 230, 230));
                    TJAPlayerPI.app.Tx.Gauge_Soul.t2D描画(TJAPlayerPI.app.Device, 1174, y_Soul[i], new Rectangle(0, 0, 80, 80));
                }
                */

                //演奏中のやつ使いまわせなかった。ファック。
                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.ScoreX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.ScoreY[i], TJAPlayerPI.stageResult.cRecords[i].Score, true, EPhase.Score, i);
                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.PerfectX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.PerfectY[i], TJAPlayerPI.stageResult.cRecords[i].PerfectCount, false, EPhase.Perfect, i);
                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.GoodX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.GoodY[i], TJAPlayerPI.stageResult.cRecords[i].GoodCount, false, EPhase.Good, i);
                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.BadX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.BadY[i], TJAPlayerPI.stageResult.cRecords[i].MissCount + TJAPlayerPI.stageResult.cRecords[i].BadCount, false, EPhase.Bad, i);

                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.ComboX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.ComboY[i], TJAPlayerPI.stageResult.cRecords[i].MaxCombo, false, EPhase.Combo, i);
                this.t小文字表示(TJAPlayerPI.app.Skin.SkinConfig.Result.RollX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.RollY[i], TJAPlayerPI.stageResult.cRecords[i].RollCount, false, EPhase.Roll, i);

                #region 段位認定モード用+王冠
                if (TJAPlayerPI.stage選曲.n確定された曲の難易度[i] == (int)Difficulty.Dan)
                {
                    TJAPlayerPI.stage演奏ドラム画面.actDan.DrawExam(TJAPlayerPI.stageResult.cRecords[i].DanC);

                    TJAPlayerPI.app.Tx.Result_Dan?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Result.DanXY[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanXY[1], new Rectangle(TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[0] * CrownState[i], 0, TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanWH[1]));
                    // Dan_Plate
                    Dan_Plate?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Result.DanPlateXY[0], TJAPlayerPI.app.Skin.SkinConfig.Result.DanPlateXY[1]);
                }
                else
                {
                    if (CrownState[i] != 0 && TJAPlayerPI.app.Tx.Crown_t is not null)
                    {
                        TJAPlayerPI.app.Tx.Crown_t.Opacity = this.ctCrown用.n現在の値;
                        TJAPlayerPI.app.Tx.Crown_t.vcScaling.X = ((this.ctCrown用.n終了値 - this.ctCrown用.n現在の値) / 255f) * 2f + 1.0f;
                        TJAPlayerPI.app.Tx.Crown_t.vcScaling.Y = ((this.ctCrown用.n終了値 - this.ctCrown用.n現在の値) / 255f) * 2f + 1.0f;
                        TJAPlayerPI.app.Tx.Crown_t.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Result.CrownX[i], TJAPlayerPI.app.Skin.SkinConfig.Result.CrownY[i], new Rectangle(CrownState[i] * 100, 0, 100, 100));
                    }
                }
                #endregion
            }

            return (this.ephase == EPhase.Loop) ? 1 : 0;
        }
    }


    // その他

    #region [ private ]
    //-----------------
    private int[] CrownState = new int[4] { 0, 0, 0, 0 };
    private bool[] ToNextPhase = new bool[4] { false, false, false, false };
    private bool AllPlayerCannotGetCrown = false;
    private CCounter ctCrown用 = new CCounter();

    private CTexture? Dan_Plate;
    #region[V1]
    private CCounter[] ct文字アニメ用 = new CCounter[4];
    private int[] n表示された桁数 = new int[4] { 0, 0, 0, 0 };

    private EPhase ephase;

    private void t小文字表示(int x, int y, long n, bool score, EPhase phase, int nPlayer)
    {
        if (phase > ephase)
            return;

        for (int index = 0; index < n.ToString().Length; index++)
        {
            int Num = (int)(n / Math.Pow(10, index) % 10);
            bool IsDigit = false;

            if (ephase == phase && !this.ToNextPhase[nPlayer] && index == this.n表示された桁数[nPlayer])
            {
                if (TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND回転音] is not null && !TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND回転音].b再生中)
                    TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND回転音].t再生する();
                Num = this.ct文字アニメ用[nPlayer].n現在の値 % 10;
                IsDigit = true;
            }

            if (score)
            {
                if (TJAPlayerPI.app.Tx.Result_Score_Number is not null)
                {
                    Rectangle rectangle = new Rectangle(24 * Num, 0, 24, TJAPlayerPI.app.Tx.Result_Score_Number.szTextureSize.Height);
                    if (TJAPlayerPI.app.Tx.Result_Score_Number is not null)
                    {
                        TJAPlayerPI.app.Tx.Result_Score_Number.t2D描画(TJAPlayerPI.app.Device, x, y, rectangle);
                    }
                    x -= 24;
                }
            }
            else
            {
                if (TJAPlayerPI.app.Tx.Result_Number is not null)
                {
                    Rectangle rectangle = new Rectangle(32 * Num, 0, 32, TJAPlayerPI.app.Tx.Result_Number.szTextureSize.Height / 2);
                    if (TJAPlayerPI.app.Tx.Result_Number is not null)
                    {
                        TJAPlayerPI.app.Tx.Result_Number.t2D描画(TJAPlayerPI.app.Device, x, y, rectangle);
                    }
                    x -= 22;
                }
            }
            if (IsDigit)
            {
                if (this.ct文字アニメ用[nPlayer].b終了値に達した)
                {
                    this.n表示された桁数[nPlayer]++;
                    this.ct文字アニメ用[nPlayer].n現在の値 = 0;
                    if (this.n表示された桁数[nPlayer] == n.ToString().Length)
                    {
                        TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND決定音]?.t再生する();
                        this.n表示された桁数[nPlayer] = 0;
                        this.ToNextPhase[nPlayer] = true;
                    }
                }
                break;
            }
        }
    }

    private void NextPhase()
    {
        ephase += 1;
        for (int index = 0; index < this.ToNextPhase.Length; index++)
        {
            this.ct文字アニメ用[index].n現在の値 = 0;
            this.ct文字アニメ用[index].t時間Reset();
            this.ToNextPhase[index] = false;
        }
    }
    private enum EPhase : int
    {
        Start,
        Crown,
        Score,
        HighScore,
        Perfect,
        Good,
        Bad,
        Combo,
        Roll,
        Loop
    }
    #endregion
    #region[V2]
    private EPhaseV2 ephase_v2;
    private CCounter ct文字V2用 = new CCounter();
    private CCounter ctGauge = new CCounter();

    private void NextPhaseV2()
    {
        ephase_v2 += 1;
        for (int index = 0; index < this.ToNextPhase.Length; index++)
        {
            this.ct文字V2用.n現在の値 = 0;
            this.ct文字V2用.t時間Reset();
            this.ToNextPhase[index] = false;
        }
    }

    private void t小文字表示V2(int x, int y, long n, bool score, EPhaseV2 phase_v2, int nPlayer)
    {
        if (phase_v2 > ephase_v2 || TJAPlayerPI.app.Tx.Result_v2_Number is null)
            return;

        else if (phase_v2 == ephase_v2)
        {
            if (score)
            {
                float ratio = (float)Math.Sin(this.ct文字V2用.n現在の値 / 180f * Math.PI) * 0.4f + 1f;
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.X = ratio;
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.Y = ratio;
            }
            else
            {
                float ratio = Math.Max(1f, (float)Math.Cos(this.ct文字V2用.n現在の値 / 180f * Math.PI) * 0.4f + 1f);
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.X = ratio / 1.6f;
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.Y = ratio / 1.6f;
            }
        }
        else
        {
            if (score)
            {
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.X = 1f;
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.Y = 1f;
            }
            else
            {
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.X = 0.625f;
                TJAPlayerPI.app.Tx.Result_v2_Number.vcScaling.Y = 0.625f;
            }
        }

        for (int index = 0; index < n.ToString().Length; index++)
        {
            int Num = (int)(n / Math.Pow(10, index) % 10);

            Rectangle rectangle = new Rectangle(TJAPlayerPI.app.Tx.Result_v2_Number.szTextureSize.Width / 10 * Num, 0, TJAPlayerPI.app.Tx.Result_v2_Number.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.Result_v2_Number.szTextureSize.Height);

            TJAPlayerPI.app.Tx.Result_v2_Number.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, rectangle);

            if (score)
                x -= TJAPlayerPI.app.Tx.Result_v2_Number.szTextureSize.Width / 10;
            else
                x -= TJAPlayerPI.app.Tx.Result_v2_Number.szTextureSize.Width / 16;
        }
    }
    private enum EPhaseV2 : int
    {
        Start,
        Gauge,
        Perfect,
        Good,
        Bad,
        Roll,
        Combo,
        Score,
        Skill,
        Crown,
        Loop
    }
    #endregion

    //-----------------
    #endregion
}
