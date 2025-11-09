using FDK;

namespace TJAPlayerPI;

internal class Dan_Cert : CActivity
{
    /// <summary>
    /// 段位認定
    /// </summary>
    public Dan_Cert()
    {
    }

    //
    Dan_C[] Challenge;
    Dan_C Gauge;
    private bool IsVer2 = false;
    //

    public void Start(int number)
    {
        NowShowingNumber = number;

        for (int i = 0; i < 3; i++)
        {
            if (Challenge[i] is not null)
                if (Challenge[i].IsEnable)
                    Challenge[i].SetNowSongNum(number);
        }
        if (Gauge is not null)
            if (Gauge.IsEnable)
                Gauge.SetNowSongNum(number);

        Counter_In = new CCounter(0, 999, 1, TJAPlayerPI.app.Timer);
        ScreenPoint = new double[] { TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] - (TJAPlayerPI.app.Tx.DanC_Screen?.szTextureSize.Width ?? 1280) / 2, 1280 }; //2020.06.06 Mr-Ojii twopointzero氏のソースコードをもとに改良
        TJAPlayerPI.stage演奏ドラム画面.ReSetScore(TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].ScoreInit, TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].ScoreDiff, 0);
        IsAnimating = true;

        string subtitle = (TJAPlayerPI.app.ConfigToml.Game._SubtitleDispMode == ESubtitleDispMode.On || (TJAPlayerPI.app.ConfigToml.Game._SubtitleDispMode == ESubtitleDispMode.Compliant && TJAPlayerPI.DTX[0].SUBTITLEDisp)) ? TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitle : null;

        TJAPlayerPI.stage演奏ドラム画面.actPanel.SetPanelString(TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].Title, subtitle, TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].Genre, 1 + NowShowingNumber + "曲目");
        Sound_Section?.t再生を開始する();
    }

    public override void On活性化()
    {
        NowShowingNumber = 0;
        Challenge = new Dan_C[3];
        Gauge = new Dan_C();
        for (int i = 0; i < 3; i++)
        {
            if (TJAPlayerPI.DTX[0].Dan_C[i] is not null) Challenge[i] = new Dan_C(TJAPlayerPI.DTX[0].Dan_C[i]);
        }
        if (TJAPlayerPI.DTX[0].Dan_C_Gauge is not null) Gauge = new Dan_C(TJAPlayerPI.DTX[0].Dan_C_Gauge);
        // 始点を決定する。
        ExamCount = 0;
        this.IsVer2 = false;
        for (int i = 0; i < 3; i++)
        {
            if (Challenge[i] is not null)
            {
                if (Challenge[i].IsEnable == true)
                    this.ExamCount++;
                if (Challenge[i].IsForEachSongs)
                    this.IsVer2 = true;
            }
        }

        if (Gauge.IsEnable == true)
        {
            this.IsVer2 = true;
        }

        for (int i = 0; i < 3; i++)
        {
            Status[i] = new ChallengeStatus();
            Status[i].Timer_Amount = new CCounter();
            Status[i].Timer_Gauge = new CCounter();
            Status[i].Timer_Failed = new CCounter();
        }
        IsEnded = false;

        if (TJAPlayerPI.stage選曲.n確定された曲の難易度[0] == (int)Difficulty.Dan)
            IsAnimating = true;

        Dan_Plate = TJAPlayerPI.app.tCreateTexture(Path.GetDirectoryName(TJAPlayerPI.DTX[0].strFilenameの絶対パス) + @"/Dan_Plate.png");
        Sound_Section = TJAPlayerPI.SoundManager.tCreateSound(CSkin.Path(@"Sounds/Dan/Section.ogg"), ESoundGroup.SoundEffect);
        Sound_Failed = TJAPlayerPI.SoundManager.tCreateSound(CSkin.Path(@"Sounds/Dan/Failed.ogg"), ESoundGroup.SoundEffect);
        base.On活性化();
    }

    public void Update()
    {
        if (Gauge is not null)
            if (Gauge.IsEnable)
            {
                Gauge.Update((int)TJAPlayerPI.stage演奏ドラム画面.actGauge.db現在のゲージ値[0]);
                var notesRemain = TJAPlayerPI.DTX[0].nノーツ数[3] - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Perfect) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Good) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Bad) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Miss);
                // 残り音符数が0になったときに判断されるやつ
                if (notesRemain <= 0)
                {
                    if (Gauge.GetAmount() < Gauge.GetValue(false)) Gauge.SetReached(true);
                }
            }

        for (int i = 0; i < 3; i++)
        {
            if (Challenge[i] is null || !Challenge[i].IsEnable) return;
            var oldReached = Challenge[i].GetReached();
            var isChangedAmount = false;
            switch (Challenge[i].Type)
            {
                case Exam.Type.Gauge:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.actGauge.db現在のゲージ値[0]);
                    break;
                case Exam.Type.JudgePerfect:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Perfect);
                    break;
                case Exam.Type.JudgeGood:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Good);
                    break;
                case Exam.Type.JudgeBad:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Miss + TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Bad);
                    break;
                case Exam.Type.Score:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.actScore.GetScore(0));
                    break;
                case Exam.Type.Roll:
                    isChangedAmount = Challenge[i].Update((int)(TJAPlayerPI.stage演奏ドラム画面.GetRoll(0)));
                    break;
                case Exam.Type.Hit:
                    isChangedAmount = Challenge[i].Update((int)(TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Perfect + TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Good + TJAPlayerPI.stage演奏ドラム画面.GetRoll(0)));
                    break;
                case Exam.Type.Combo:
                    isChangedAmount = Challenge[i].Update((int)TJAPlayerPI.stage演奏ドラム画面.actCombo.n現在のコンボ数.Max[0]);
                    break;
                default:
                    break;
            }

            // 値が変更されていたらアニメーションを行う。
            if (isChangedAmount)
            {
                if (Status[i].Timer_Amount is not null && Status[i].Timer_Amount.b終了値に達してない)
                {
                    Status[i].Timer_Amount = new CCounter(0, 11, 12, TJAPlayerPI.app.Timer);
                    Status[i].Timer_Amount.n現在の値 = 1;
                }
                else
                {
                    Status[i].Timer_Amount = new CCounter(0, 11, 12, TJAPlayerPI.app.Timer);
                }
            }

            // 条件の達成見込みがあるかどうか判断する。
            if (Challenge[i].Range == Exam.Range.Less)
            {
                Challenge[i].SetReached(!Challenge[i].GetNowCleared(false));
            }
            else
            {
                var notesRemain = TJAPlayerPI.DTX[0].nノーツ数[3] - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Perfect) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Good) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Bad) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Miss);
                // 残り音符数が0になったときに判断されるやつ
                if (notesRemain <= 0)
                {
                    // 残り音符数ゼロ
                    switch (Challenge[i].Type)
                    {
                        case Exam.Type.Gauge:
                            if (Challenge[i].GetAmount() < Challenge[i].GetValue(false)) Challenge[i].SetReached(true);
                            break;
                        case Exam.Type.Score:
                            if (Challenge[i].GetAmount() < Challenge[i].GetValue(false)) Challenge[i].SetReached(true);
                            break;
                        default:
                            // 何もしない
                            break;
                    }
                }
                // 常に監視されるやつ。
                switch (Challenge[i].Type)
                {
                    case Exam.Type.JudgePerfect:
                    case Exam.Type.JudgeGood:
                    case Exam.Type.JudgeBad:
                        if (notesRemain < (Challenge[i].GetValue(false) - Challenge[i].GetAmount())) Challenge[i].SetReached(true);
                        break;
                    case Exam.Type.Combo:
                        if (notesRemain + TJAPlayerPI.stage演奏ドラム画面.actCombo.n現在のコンボ数[0] < ((Challenge[i].GetValue(false))) && TJAPlayerPI.stage演奏ドラム画面.actCombo.n現在のコンボ数.Max[0] < (Challenge[i].GetValue(false))) Challenge[i].SetReached(true);
                        break;
                    default:
                        break;
                }


                // 音源が終了したやつの分岐。
                if (!IsEnded)
                {
                    if (TJAPlayerPI.DTX[0].listChip.Count <= 0) continue;

                    //次の音符が段位幕かENDだったらtrue
                    //次の音符がドン・カッ・連打・風船だったら、false
                    bool bNotesFin = true;
                    for (int index = 0; index < TJAPlayerPI.DTX[0].listChip.Count; index++)
                    {
                        if (TJAPlayerPI.DTX[0].listChip[index].n発声時刻ms > (long)(CSoundManager.rc演奏用タイマ.n現在時刻ms * (((double)TJAPlayerPI.app.ConfigToml.PlayOption.PlaySpeed) / 20.0)))
                        {
                            if (TJAPlayerPI.DTX[0].listChip[index].nチャンネル番号 == 0xff || (Challenge[i].IsForEachSongs && TJAPlayerPI.DTX[0].listChip[index].nチャンネル番号 == 0x9B))
                                break;
                            else if (TJAPlayerPI.DTX[0].listChip[index].nチャンネル番号 >= 0x10 && TJAPlayerPI.DTX[0].listChip[index].nチャンネル番号 <= 0x1f)
                            {
                                bNotesFin = false;
                                break;
                            }
                        }
                    }

                    if (bNotesFin)
                    {
                        switch (Challenge[i].Type)
                        {
                            case Exam.Type.Score:
                            case Exam.Type.Roll:
                            case Exam.Type.Hit:
                                if (Challenge[i].GetAmount() < Challenge[i].GetValue(false)) Challenge[i].SetReached(true);
                                break;
                            default:
                                break;
                        }
                        IsEnded = true;
                    }
                }
            }
            if (oldReached == false && Challenge[i].GetReached() == true)
            {
                Sound_Failed?.t再生を開始する();
            }
        }
    }

    public override void On非活性化()
    {
        for (int i = 0; i < 3; i++)
        {
            Challenge[i] = null;
        }

        for (int i = 0; i < 3; i++)
        {
            Status[i].Timer_Amount = null;
            Status[i].Timer_Gauge = null;
            Status[i].Timer_Failed = null;
        }
        IsEnded = false;

        TJAPlayerPI.t安全にDisposeする(ref Dan_Plate);
        Sound_Section?.t解放する();
        Sound_Failed?.t解放する();
        base.On非活性化();
    }

    public override int On進行描画()
    {
        if (TJAPlayerPI.stage選曲.n確定された曲の難易度[0] != (int)Difficulty.Dan) return base.On進行描画();
        Counter_In?.t進行();
        Counter_Wait?.t進行();
        Counter_Out?.t進行();
        Counter_Text?.t進行();

        if (Counter_Text is not null)
        {
            if (Counter_Text.n現在の値 >= 2000)
            {
                for (int i = Counter_Text_Old; i < Counter_Text.n現在の値; i++)
                {
                    if (i % 2 == 0)
                    {
                        if (TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].TitleTex is not null)
                        {
                            TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].TitleTex.Opacity--;
                        }
                        if (TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitleTex is not null)
                        {
                            TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitleTex.Opacity--;
                        }
                    }
                }
            }
            else
            {
                if (TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].TitleTex is not null)
                {
                    TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].TitleTex.Opacity = 255;
                }
                if (TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitleTex is not null)
                {
                    TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitleTex.Opacity = 255;
                }
            }
            Counter_Text_Old = Counter_Text.n現在の値;
        }

        for (int i = 0; i < 3; i++)
        {
            Status[i].Timer_Amount?.t進行();
        }


        if (TJAPlayerPI.app.ConfigToml.EnableSkinV2 || this.IsVer2)
        {
            // 背景を描画する。
            TJAPlayerPI.app.Tx.DanC_V2_Background?.t2D描画(TJAPlayerPI.app.Device, 0, 0);

            // 段プレートを描画する。
            Dan_Plate?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2DanPlateXY[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2DanPlateXY[1]);

            DrawExamV2(Challenge, Gauge);
        }
        else
        {
            // 背景を描画する。
            TJAPlayerPI.app.Tx.DanC_Background?.t2D描画(TJAPlayerPI.app.Device, 0, 0);

            // 残り音符数を描画する。
            var notesRemain = TJAPlayerPI.DTX[0].nノーツ数[3] - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Perfect) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Good) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Bad) - (TJAPlayerPI.stage演奏ドラム画面.nヒット数[0].Miss);

            DrawNumber(notesRemain, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberXY[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberXY[1], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberPadding);

            // 段プレートを描画する。
            Dan_Plate?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.DanPlateXY[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.DanPlateXY[1]);
            DrawExam(Challenge);
        }

        // 幕のアニメーション
        if (Counter_In is not null)
        {
            if (Counter_In.b終了値に達してない)
            {
                for (int i = Counter_In_Old; i < Counter_In.n現在の値; i++)
                {
                    ScreenPoint[0] += (TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] - ScreenPoint[0]) / 180.0;
                    ScreenPoint[1] += ((1280 / 2 + TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] / 2) - ScreenPoint[1]) / 180.0;
                }
                Counter_In_Old = Counter_In.n現在の値;
                TJAPlayerPI.app.Tx.DanC_Screen?.t2D描画(TJAPlayerPI.app.Device, (int)ScreenPoint[0], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0], new Rectangle(0, 0, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Height));
                TJAPlayerPI.app.Tx.DanC_Screen?.t2D描画(TJAPlayerPI.app.Device, (int)ScreenPoint[1], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0], new Rectangle(TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, 0, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Height));
                //CDTXMania.act文字コンソール.tPrint(0, 420, C文字コンソール.EFontType.白, String.Format("{0} : {1}", ScreenPoint[0], ScreenPoint[1]));
            }
            if (Counter_In.b終了値に達した)
            {
                Counter_In = null;
                Counter_Wait = new CCounter(0, 2299, 1, TJAPlayerPI.app.Timer);
            }
        }
        if (Counter_Wait is not null)
        {
            if (Counter_Wait.b終了値に達してない)
            {
                TJAPlayerPI.app.Tx.DanC_Screen?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0]);
            }
            if (Counter_Wait.b終了値に達した)
            {
                Counter_Wait = null;
                Counter_Out = new CCounter(0, 499, 1, TJAPlayerPI.app.Timer);
                Counter_Text = new CCounter(0, 2899, 1, TJAPlayerPI.app.Timer);
            }
        }
        if (Counter_Text is not null)
        {
            if (Counter_Text.b終了値に達してない)
            {
                var title = TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].TitleTex;
                var subTitle = TJAPlayerPI.DTX[0].List_DanSongs[NowShowingNumber].SubTitleTex;
                if (subTitle is null)
                    title?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, 1280 / 2 + TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] / 2, TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0] + 65);
                else
                {
                    title?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, 1280 / 2 + TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] / 2, TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0] + 45);
                    subTitle?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, 1280 / 2 + TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldBGX[0] / 2, TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0] + 85);
                }
            }
            if (Counter_Text.b終了値に達した)
            {
                Counter_Text = null;
                IsAnimating = false;
            }
        }
        if (Counter_Out is not null)
        {
            if (Counter_Out.b終了値に達してない)
            {
                for (int i = Counter_Out_Old; i < Counter_Out.n現在の値; i++)
                {
                    ScreenPoint[0] += -3;
                    ScreenPoint[1] += 3;
                }
                Counter_Out_Old = Counter_Out.n現在の値;
                TJAPlayerPI.app.Tx.DanC_Screen?.t2D描画(TJAPlayerPI.app.Device, (int)ScreenPoint[0], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0], new Rectangle(0, 0, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Height));
                TJAPlayerPI.app.Tx.DanC_Screen?.t2D描画(TJAPlayerPI.app.Device, (int)ScreenPoint[1], TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldY[0], new Rectangle(TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, 0, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Width / 2, TJAPlayerPI.app.Tx.DanC_Screen.szTextureSize.Height));
                //CDTXMania.act文字コンソール.tPrint(0, 420, C文字コンソール.EFontType.白, String.Format("{0} : {1}", ScreenPoint[0], ScreenPoint[1]));
            }
            if (Counter_Out.b終了値に達した)
            {
                Counter_Out = null;
            }
        }
        return base.On進行描画();
    }

    public void DrawExam(Dan_C[] dan_C)
    {
        var count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (dan_C[i] is not null && dan_C[i].IsEnable == true)
                count++;
        }
        for (int i = 0; i < count; i++)
        {
            float PanelOffset = (count - 1) / 2.0f;
            int PanelY = 500 + 90 * i;

            #region ゲージの土台を描画する。
            if (TJAPlayerPI.app.Tx.DanC_Base is not null)
            {
                PanelY = (int)(TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.Y - (TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.YPadding * PanelOffset) + (TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.YPadding * i) - (TJAPlayerPI.app.Tx.DanC_Base.szTextureSize.Height / 2));
                TJAPlayerPI.app.Tx.DanC_Base?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1], PanelY);
            }
            #endregion

            #region ゲージを描画する。
            var drawGaugeType = 0;
            if (dan_C[i].Range == Exam.Range.More)
            {
                if (dan_C[i].GetAmountToPercent() >= 100)
                    drawGaugeType = 2;
                else if (dan_C[i].GetAmountToPercent() >= 70)
                    drawGaugeType = 1;
                else
                    drawGaugeType = 0;
            }
            else
            {
                if (dan_C[i].GetAmountToPercent() >= 100)
                    drawGaugeType = 2;
                else if (dan_C[i].GetAmountToPercent() > 70)
                    drawGaugeType = 1;
                else
                    drawGaugeType = 0;
            }
            TJAPlayerPI.app.Tx.DanC_Gauge[drawGaugeType]?.t2D描画(TJAPlayerPI.app.Device,
                TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.Offset[0], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.Offset[1], new Rectangle(0, 0, (int)(dan_C[i].GetAmountToPercent() * (TJAPlayerPI.app.Tx.DanC_Gauge[drawGaugeType].szTextureSize.Width / 100.0)), TJAPlayerPI.app.Tx.DanC_Gauge[drawGaugeType].szTextureSize.Height));
            #endregion

            #region 現在の値を描画する。
            var nowAmount = 0;
            if (dan_C[i].Range == Exam.Range.Less)
            {
                nowAmount = dan_C[i].GetValue(false) - dan_C[i].GetAmount();
            }
            else
            {
                nowAmount = dan_C[i].GetAmount();
            }
            if (nowAmount < 0) nowAmount = 0;

            DrawNumber(nowAmount, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[0], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[1], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallScale, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallScale, (Status[i].Timer_Amount is not null ? ScoreScale[Status[i].Timer_Amount.n現在の値] : 0f));

            if (TJAPlayerPI.app.Tx.DanC_Number is not null)
            {
                // 単位(あれば)
                switch (dan_C[i].Type)
                {
                    case Exam.Type.Gauge:
                        // パーセント
                        TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[0], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));
                        break;
                    case Exam.Type.Score:
                        TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[2], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 2, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));

                        // 点
                        break;
                    case Exam.Type.Roll:
                    case Exam.Type.Hit:
                        TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[1], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 1, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));

                        // 打
                        break;
                    case Exam.Type.Combo:
                        TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[3], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 3, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));
                        //コンボ
                        break;
                    default:
                        // 何もしない
                        break;
                }
            }

            #endregion

            #region 条件の文字を描画する。
            var offset = TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[0];
            //offset -= CDTXMania.Skin.Game_DanC_ExamRange_Padding;
            // 条件の範囲
            if (TJAPlayerPI.app.Tx.DanC_ExamRange is not null)
            {
                TJAPlayerPI.app.Tx.DanC_ExamRange?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset - TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Width, PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1] + (TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Height / 2), new Rectangle(0, (TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Height / 2) * (int)dan_C[i].Range, TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Width, (TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Height / 2)));
                offset -= TJAPlayerPI.app.Tx.DanC_ExamRange.szTextureSize.Width;
            }

            // 単位(あれば)
            switch (dan_C[i].Type)
            {
                case Exam.Type.Gauge:
                    // パーセント
                    TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[0], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));
                    offset -= TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[0];
                    break;
                case Exam.Type.Score:
                    TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[2], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 2, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));
                    offset -= TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[2];

                    // 点
                    break;
                case Exam.Type.Roll:
                case Exam.Type.Hit:
                    TJAPlayerPI.app.Tx.DanC_ExamUnit?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[1], PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1] * 1, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamUnitSize[1]));
                    offset -= TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.PercentHitScorePadding[1];

                    // 打
                    break;
                default:
                    // 何もしない
                    break;
            }

            // 条件の数字
            DrawNumber(dan_C[i].GetValue(false), TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset, PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallScale, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallScale);
            offset -= TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.NumberSmallPadding * (dan_C[i].GetValue(false).ToString().Length);

            // 条件の種類
            TJAPlayerPI.app.Tx.DanC_ExamType?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1] + offset - TJAPlayerPI.app.Tx.DanC_ExamType.szTextureSize.Width, PanelY - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamTypeSize[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamTypeSize[1] * (int)dan_C[i].Type, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamTypeSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.ExamTypeSize[1]));
            #endregion

            #region 条件達成失敗の画像を描画する。
            if (dan_C[i].GetReached())
            {
                TJAPlayerPI.app.Tx.DanC_Failed?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.X[count - 1], PanelY);
            }
            #endregion
        }
    }

    public void DrawExamV2(Dan_C[] dan_C, Dan_C DanCGauge)
    {
        if (Gauge is not null)
            if (Gauge.IsEnable)
            {
                int soulgaugeboxx = (int)((TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxX[1] - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxX[0]) * DanCGauge.GetValue(false) / 100.0) + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxX[0];
                TJAPlayerPI.app.Tx.DanC_V2_SoulGauge_Box.t2D描画(TJAPlayerPI.app.Device, soulgaugeboxx, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxY);

                if (TJAPlayerPI.app.Tx.DanC_V2_ExamRange is not null)
                    TJAPlayerPI.app.Tx.DanC_V2_ExamRange?.t2D描画(TJAPlayerPI.app.Device, soulgaugeboxx + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamRangeOffset[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamRangeOffset[1], new Rectangle(0, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Height / 2 * (int)Gauge.Range, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Width, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Height / 2));

                // 条件の種類
                if (TJAPlayerPI.app.Tx.DanC_V2_ExamType is not null)
                {
                    if (TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box is not null)
                    {
                        TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box.vcScaling.X = 0.5f;
                        TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, soulgaugeboxx + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamTypeOffset[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamTypeOffset[1]);
                    }
                    TJAPlayerPI.app.Tx.DanC_V2_ExamType?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, soulgaugeboxx + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamTypeOffset[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamTypeOffset[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[1] * (int)Gauge.Type, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[1]));
                }

                DrawNumberV2(DanCGauge.GetValue(false), soulgaugeboxx + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamRangeOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeNumOffset[0] - TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxPersentWidth, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SoulGaugeBoxExamRangeOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeNumOffset[1], true, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2NumberSmallScale, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2NumberSmallScale);
            }

        var count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (dan_C[i] is not null && dan_C[i].IsEnable == true)
                count++;
        }
        for (int i = 0; i < count; i++)
        {
            float PanelOffset = (count - 1) / 2.0f;
            int PanelY = 500 + 90 * i;
            #region[パネルを描画する]
            if (TJAPlayerPI.app.Tx.DanC_V2_Panel is not null)
            {
                PanelY = TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelY[i];
                TJAPlayerPI.app.Tx.DanC_V2_Panel.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i], PanelY);
            }
            #endregion

            float GaugeXRatio = 1f;
            if (dan_C[i].IsForEachSongs)
                GaugeXRatio = 0.675f;
            else
                GaugeXRatio = 1.0f;

            #region ゲージの土台を描画する。
            if (TJAPlayerPI.app.Tx.DanC_V2_Base is not null)
            {
                TJAPlayerPI.app.Tx.DanC_V2_Base.vcScaling.X = GaugeXRatio;
                TJAPlayerPI.app.Tx.DanC_V2_Base.vcScaling.Y = 1.0f;
                TJAPlayerPI.app.Tx.DanC_V2_Base?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1]);
            }
            #endregion

            #region ゲージを描画する。
            var drawGaugeType = 0;
            if (dan_C[i].Range == Exam.Range.More)
            {
                if (dan_C[i].GetAmountToPercent() >= 100)
                    drawGaugeType = 2;
                else if (dan_C[i].GetAmountToPercent() >= 70)
                    drawGaugeType = 1;
                else
                    drawGaugeType = 0;
            }
            else
            {
                if (dan_C[i].GetAmountToPercent() >= 100)
                    drawGaugeType = 2;
                else if (dan_C[i].GetAmountToPercent() > 70)
                    drawGaugeType = 1;
                else
                    drawGaugeType = 0;
            }
            if (TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType] is not null)
            {
                TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].vcScaling.X = GaugeXRatio;
                TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].vcScaling.Y = 1.0f;
                TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0] + (int)(TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2GaugeOffset[0] * GaugeXRatio), PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2GaugeOffset[1], new Rectangle(0, 0, (int)(dan_C[i].GetAmountToPercent() * (TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].szTextureSize.Width / 100.0)), TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].szTextureSize.Height));
            }
            #endregion

            #region 現在の値を描画する。
            var nowAmount = 0;
            if (dan_C[i].Range == Exam.Range.Less)
            {
                nowAmount = dan_C[i].GetValue(false) - dan_C[i].GetAmount();
            }
            else
            {
                nowAmount = dan_C[i].GetAmount();
            }
            if (nowAmount < 0) nowAmount = 0;

            DrawNumberV2(nowAmount, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountOffset[1],
                false, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountScale, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountScale, (Status[i].Timer_Amount is not null ? ScoreScale[Status[i].Timer_Amount.n現在の値] : 0f));
            #endregion

            #region 条件の文字を描画する。
            // 条件の範囲
            if (TJAPlayerPI.app.Tx.DanC_V2_ExamRange is not null)
                TJAPlayerPI.app.Tx.DanC_V2_ExamRange?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeOffset[1], new Rectangle(0, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Height / 2 * (int)dan_C[i].Range, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Width, TJAPlayerPI.app.Tx.DanC_V2_ExamRange.szTextureSize.Height / 2));

            // 条件の数字
            DrawNumberV2(dan_C[i].GetValue(false), TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeNumOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamRangeNumOffset[1], true, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2NumberSmallScale, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2NumberSmallScale);
            // 条件の種類
            if (TJAPlayerPI.app.Tx.DanC_V2_ExamType is not null)
            {
                if (TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box is not null)
                {
                    TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box.vcScaling.X = 1f;
                    TJAPlayerPI.app.Tx.DanC_V2_ExamType_Box?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeOffset[1]);
                }
                TJAPlayerPI.app.Tx.DanC_V2_ExamType?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeOffset[1], new Rectangle(0, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[1] * (int)dan_C[i].Type, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[0], TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2ExamTypeSize[1]));
            }
            #endregion

            #region 条件達成失敗の画像を描画する。
            if (dan_C[i].GetReached())
            {
                if (TJAPlayerPI.app.Tx.DanC_V2_Failed_Cover is not null)
                {
                    TJAPlayerPI.app.Tx.DanC_V2_Failed_Cover.vcScaling.X = GaugeXRatio;
                    TJAPlayerPI.app.Tx.DanC_V2_Failed_Cover.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1]);

                    if (TJAPlayerPI.app.Tx.DanC_V2_Failed_Text is not null)
                    {
                        int textx = TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0] + (int)(TJAPlayerPI.app.Tx.DanC_V2_Failed_Cover.szTextureSize.Width * GaugeXRatio / 2) - (TJAPlayerPI.app.Tx.DanC_V2_Failed_Text.szTextureSize.Width / 2);
                        int texty = PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + (int)(TJAPlayerPI.app.Tx.DanC_V2_Failed_Cover.szTextureSize.Height / 2) - (TJAPlayerPI.app.Tx.DanC_V2_Failed_Text.szTextureSize.Height / 2);
                        TJAPlayerPI.app.Tx.DanC_V2_Failed_Text.t2D描画(TJAPlayerPI.app.Device, textx, texty);
                    }
                }
            }
            #endregion


            #region[過去ゲージの描画]
            if (dan_C[i].IsForEachSongs)
            {
                GaugeXRatio = 0.675f * 0.36f;
                float GaugeYRatio = 0.36f;
                for (int examindex = 0; examindex < this.NowShowingNumber; examindex++)
                {
                    //ベース
                    if (TJAPlayerPI.app.Tx.DanC_V2_Base is not null)
                    {
                        TJAPlayerPI.app.Tx.DanC_V2_Base.vcScaling.X = GaugeXRatio;
                        TJAPlayerPI.app.Tx.DanC_V2_Base.vcScaling.Y = GaugeYRatio;
                        TJAPlayerPI.app.Tx.DanC_V2_Base?.t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[1] + (examindex * TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffsetYPadding));
                    }
                    //ゲージ
                    drawGaugeType = 0;
                    if (dan_C[i].Range == Exam.Range.More)
                    {
                        if (dan_C[i].GetAmountToPercent(examindex) >= 100)
                            drawGaugeType = 2;
                        else if (dan_C[i].GetAmountToPercent(examindex) >= 70)
                            drawGaugeType = 1;
                        else
                            drawGaugeType = 0;
                    }
                    else
                    {
                        if (dan_C[i].GetAmountToPercent(examindex) >= 100)
                            drawGaugeType = 2;
                        else if (dan_C[i].GetAmountToPercent(examindex) > 70)
                            drawGaugeType = 1;
                        else
                            drawGaugeType = 0;
                    }
                    if (TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType] is not null)
                    {
                        TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].vcScaling.X = GaugeXRatio;
                        TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].vcScaling.Y = GaugeYRatio;
                        TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].t2D描画(TJAPlayerPI.app.Device, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0] + (int)(TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2GaugeOffset[0] * GaugeXRatio), PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[1] + (int)(TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2GaugeOffset[1] * GaugeYRatio) + (examindex * TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffsetYPadding), new Rectangle(0, 0, (int)(dan_C[i].GetAmountToPercent(examindex) * (TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].szTextureSize.Width / 100.0)), TJAPlayerPI.app.Tx.DanC_V2_Gauge[drawGaugeType].szTextureSize.Height));
                    }
                    //値
                    nowAmount = 0;
                    if (dan_C[i].Range == Exam.Range.Less)
                    {
                        nowAmount = dan_C[i].GetValue(false, examindex) - dan_C[i].GetAmount(examindex);
                    }
                    else
                    {
                        nowAmount = dan_C[i].GetAmount(examindex);
                    }
                    if (nowAmount < 0) nowAmount = 0;

                    DrawNumberV2(nowAmount, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2PanelX[i] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[0] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountOffset[0], PanelY + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2BaseOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffset[1] + TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2AmountOffset[1] + (examindex * TJAPlayerPI.app.Skin.SkinConfig.Game.DanC.v2SmallGaugeOffsetYPadding),
                        false, 0.37f, 0.37f, 0f);
                }
            }
            #endregion
        }
    }


    /// <summary>
    /// 段位チャレンジの数字フォントで数字を描画します。
    /// </summary>
    /// <param name="value">値。</param>
    /// <param name="x">一桁目のX座標。</param>
    /// <param name="y">一桁目のY座標</param>
    /// <param name="padding">桁数間の字間</param>
    /// <param name="scaleX">拡大率X</param>
    /// <param name="scaleY">拡大率Y</param>
    /// <param name="scaleJump">アニメーション用拡大率(Yに加算される)。</param>
    private static void DrawNumber(int value, int x, int y, int padding, float scaleX = 1.0f, float scaleY = 1.0f, float scaleJump = 0.0f)
    {
        for (int i = 0; i < value.ToString().Length; i++)
        {
            if (TJAPlayerPI.app.Tx.DanC_Number is not null)
            {
                var number = (int)(value / Math.Pow(10, i) % 10);
                Rectangle rectangle = new Rectangle(TJAPlayerPI.app.Tx.DanC_Number.szTextureSize.Width / 10 * number, 0, TJAPlayerPI.app.Tx.DanC_Number.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.DanC_Number.szTextureSize.Height);
                TJAPlayerPI.app.Tx.DanC_Number.vcScaling.X = scaleX;
                TJAPlayerPI.app.Tx.DanC_Number.vcScaling.Y = scaleY + scaleJump;
                TJAPlayerPI.app.Tx.DanC_Number.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownRight, x + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 20) - ((i + 1) * padding), y + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Height * scaleY), rectangle);
            }
        }
    }


    /// <summary>
    /// 段位チャレンジ(ニジイロ)の数字フォントで数字を描画します。
    /// </summary>
    /// <param name="value">値。</param>
    /// <param name="x">一桁目のX座標。</param>
    /// <param name="y">一桁目のY座標</param>
    /// <param name="padding">桁数間の字間</param>
    /// <param name="IsRPRight">右揃えか</param>
    /// <param name="scaleX">拡大率X</param>
    /// <param name="scaleY">拡大率Y</param>
    /// <param name="scaleJump">アニメーション用拡大率(Yに加算される)。</param>
    private static void DrawNumberV2(int value, int x, int y, bool IsRPRight, float scaleX = 1.0f, float scaleY = 1.0f, float scaleJump = 0.0f)
    {
        if (TJAPlayerPI.app.Tx.DanC_V2_Number is not null)
        {
            int padding = (int)(TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 10f * scaleX);
            if (IsRPRight)
            {
                for (int i = 0; i < value.ToString().Length; i++)
                {
                    var number = (int)(value / Math.Pow(10, i) % 10);
                    Rectangle rect = new Rectangle(TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 10 * number, 0, TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Height);
                    TJAPlayerPI.app.Tx.DanC_V2_Number.vcScaling.X = scaleX;
                    TJAPlayerPI.app.Tx.DanC_V2_Number.vcScaling.Y = scaleY + scaleJump;
                    TJAPlayerPI.app.Tx.DanC_V2_Number.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 20) - ((i + 1) * padding), y + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Height * scaleY), rect);
                }
            }
            else
            {
                for (int i = 0; i < value.ToString().Length; i++)
                {
                    var number = (int)(value / Math.Pow(10, value.ToString().Length - 1 - i) % 10);
                    Rectangle rect = new Rectangle(TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 10 * number, 0, TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Height);
                    TJAPlayerPI.app.Tx.DanC_V2_Number.vcScaling.X = scaleX;
                    TJAPlayerPI.app.Tx.DanC_V2_Number.vcScaling.Y = scaleY + scaleJump;
                    TJAPlayerPI.app.Tx.DanC_V2_Number.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Width / 20) + (i * padding), y + (TJAPlayerPI.app.Tx.DanC_V2_Number.szTextureSize.Height * scaleY), rect);
                }

            }
        }
    }

    /// <summary>
    /// n個の条件がひとつ以上達成失敗しているかどうかを返します。
    /// </summary>
    /// <returns>n個の条件がひとつ以上達成失敗しているか。</returns>
    public bool GetFailedAllChallenges()
    {
        var isFailed = false;
        for (int i = 0; i < this.ExamCount; i++)
        {
            if (Challenge[i].IsEnable && Challenge[i].GetReached()) isFailed = true;
        }
        if (Gauge.IsEnable && Gauge.GetReached()) isFailed = true;
        return isFailed;
    }

    /// <summary>
    /// n個の条件で段位認定モードのステータスを返します。
    /// </summary>
    /// <param name="dan_C">条件。</param>
    /// <returns>ExamStatus。</returns>
    public Exam.Status GetExamStatus(Dan_C[] dan_C, Dan_C Gauge)
    {
        var status = Exam.Status.Better_Success;
        var count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (dan_C[i] is not null && dan_C[i].IsEnable == true)
                count++;
        }
        for (int i = 0; i < count; i++)
        {
            if (!dan_C[i].GetCleared(true)) status = Exam.Status.Success;
        }
        if (Gauge.IsEnable)
            if (!Gauge.GetCleared(true))
                status = Exam.Status.Success;
        for (int i = 0; i < count; i++)
        {
            if (!dan_C[i].GetCleared(false)) status = Exam.Status.Failure;
        }
        if (Gauge.IsEnable)
            if (!Gauge.GetCleared(false))
                status = Exam.Status.Failure;
        return status;
    }

    public Dan_C[] GetExam()
    {
        return Challenge;
    }

    public Dan_C GetGaugeExam()
    {
        return Gauge;
    }

    private readonly float[] ScoreScale = new float[]
    {
        0.000f,
        0.111f, // リピート
        0.222f,
        0.185f,
        0.148f,
        0.129f,
        0.111f,
        0.074f,
        0.065f,
        0.033f,
        0.015f,
        0.000f
    };

    [StructLayout(LayoutKind.Sequential)]
    struct ChallengeStatus
    {
        public CCounter Timer_Gauge;
        public CCounter Timer_Amount;
        public CCounter Timer_Failed;
    }

    #region[ private ]
    //-----------------
    private int ExamCount;
    private ChallengeStatus[] Status = new ChallengeStatus[3];
    private CTexture Dan_Plate;
    private bool IsEnded;

    // アニメ関連
    private int NowShowingNumber;
    private CCounter Counter_In, Counter_Wait, Counter_Out, Counter_Text;
    private double[] ScreenPoint;
    private int Counter_In_Old, Counter_Out_Old, Counter_Text_Old;
    public bool IsAnimating;

    //音声関連
    private CSound Sound_Section;
    private CSound Sound_Failed;


    //-----------------
    #endregion
}
