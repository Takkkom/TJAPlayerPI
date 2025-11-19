using FDK;
using Tomlyn;
using static TJAPlayerPI.CSkin;
using static TJAPlayerPI.CSkin.CSkinConfig;
using static TJAPlayerPI.CSkin.CSkinConfig.CGame;

namespace TJAPlayerPI;

internal class CActRunner : CActivity
{
    /// <summary>
    /// ランナー
    /// </summary>
    public CActRunner()
    {
    }

    public void Start(int Player, bool IsMiss, CDTX.CChip pChip)
    {
        if (pChip.nチャンネル番号 < 0x15 || (pChip.nチャンネル番号 >= 0x1A))
        {
            for (int i = 0; i < 128; i++)
            {
                if (!stRunners[i].b使用中)
                {
                    stRunners[i].b使用中 = true;
                    stRunners[i].nPlayer = Player;

                    /*
                    if (IsMiss)
                        stRunners[i].nType = 0;
                    else
                        stRunners[i].nType = random.Next(1, Type + 1);
                    */

                    if (IsMiss)
                    {
                        stRunners[i].cRunnerType = cRunnerMiss;
                    }
                    else
                    {
                        stRunners[i].cRunnerType = cRunnerHit;
                    }

                    stRunners[i].nType = random.Next(0, stRunners[i].cRunnerType?.Config.Variant ?? 0);
                    stRunners[i].fValue = 0;
                    //stRunners[i].nOldValue = 0;
                    //stRunners[i].nNowPtn = 0;
                    break;
                }
            }
        }
    }

    public override void On活性化()
    {
        for (int i = 0; i < 128; i++)
        {
            stRunners[i] = new STRunner();
            stRunners[i].b使用中 = false;
        }

        string runnersPath = CSkin.Path($"{TextureLoader.BASE}{TextureLoader.GAME}{TextureLoader.RUNNER}");
        cRunnerMiss = new CRunnerType(System.IO.Path.Combine(runnersPath, "Miss"));
        cRunnerHit = new CRunnerType(System.IO.Path.Combine(runnersPath, "Paper_Crane"));

        // フィールド上で代入してたためこちらへ移動。
        /*
        Size = TJAPlayerPI.app.Skin.SkinConfig.Game.Runner.Size;
        Ptn = TJAPlayerPI.app.Skin.SkinConfig.Game.Runner.Ptn;
        Type = TJAPlayerPI.app.Skin.SkinConfig.Game.Runner.Type;
        StartPoint_X = TJAPlayerPI.app.Skin.SkinConfig.Game.Runner.StartPointX;
        StartPoint_Y = TJAPlayerPI.app.Skin.SkinConfig.Game.Runner.StartPointY;
        */
        base.On活性化();
    }

    public override void On非活性化()
    {
        TJAPlayerPI.t安全にDisposeする(ref cRunnerMiss);
        TJAPlayerPI.t安全にDisposeする(ref cRunnerHit);
        base.On非活性化();
    }

    public override int On進行描画()
    {
        for (int i = 0; i < 128; i++)
        {
            if (stRunners[i].b使用中)
            {
                //stRunners[i].nOldValue = stRunners[i].ct進行.n現在の値;
                if (stRunners[i].fValue > 1.0f)
                {
                    stRunners[i].b使用中 = false;
                }


                /*
                for (int n = stRunners[i].nOldValue; n < stRunners[i].ct進行.n現在の値; n++)
                {
                    //AkasokoPullyou様のソースコードを参考にして、ランナーの逆流を防止
                    double dbBPM = Math.Abs(TJAPlayerPI.stage演奏ドラム画面.actPlayInfo.dbBPM);
                    stRunners[i].fX += (float)dbBPM / 18;
                    int Width = TJAPlayerPI.app.LogicalSize.Width / Ptn;
                    stRunners[i].nNowPtn = (int)stRunners[i].fX / Width;
                }
                */

                //int x = (int)(StartPoint_X[stRunners[i].nPlayer] + stRunners[i].fX);
                //int y = StartPoint_Y[stRunners[i].nPlayer];
                //stRunners[i].txRunner?.t2D描画(TJAPlayerPI.app.Device, x, y, new Rectangle(stRunners[i].nNowPtn * Size[0], stRunners[i].nType * Size[1], Size[0], Size[1]));
                stRunners[i].cRunnerType?.Draw(ref stRunners[i]);
            }
        }
        return base.On進行描画();
    }

    #region[ private ]
    //-----------------
    [StructLayout(LayoutKind.Sequential)]
    private struct STRunner
    {
        public bool b使用中;
        public int nPlayer;
        public int nType;
        //public int nOldValue;
        //public int nNowPtn;
        public float fValue;
        public CRunnerType? cRunnerType;
    }
    private STRunner[] stRunners = new STRunner[128];
    Random random = new Random();

    private CRunnerType? cRunnerMiss;
    private CRunnerType? cRunnerHit;

    private class CRunnerConfig
    {
        public int[] X { get; set; } = new int[2];
        public int[] Y { get; set; } = new int[2];
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] InJumpWave { get; set; } = new int[2];
        public int Wave { get; set; }
        public int InPtn { get; set; }
        public int LoopPtn { get; set; }
        public float InSpeed { get; set; }
        public float LoopSpeed { get; set; }
        public int Variant { get; set; }
        public string Style { get; set; } = "";
    }
    private class CRunnerType : IDisposable
    {
        public CRunnerConfig Config { get; init; } = new CRunnerConfig();
        public CTexture? txIn;
        public CTexture? txLoop;

        public CRunnerType(string path)
        {
            string? strToml = CJudgeTextEncoding.ReadTextFile(System.IO.Path.Combine(path, "Config.toml"));
            if (string.IsNullOrEmpty(strToml))
            {
                return;
            }

            TomlModelOptions tomlModelOptions = new()
            {
                ConvertPropertyName = (x) => x,
                ConvertFieldName = (x) => x,
            };
            this.Config = Toml.ToModel<CRunnerConfig>(strToml, null, tomlModelOptions);

            txIn = TJAPlayerPI.app.tCreateTexture(System.IO.Path.Combine(path, "In.png"));
            txLoop = TJAPlayerPI.app.tCreateTexture(System.IO.Path.Combine(path, "Loop.png"));
        }

        public void Dispose()
        {
            TJAPlayerPI.t安全にDisposeする(ref this.txIn);
            TJAPlayerPI.t安全にDisposeする(ref this.txLoop);
        }

        public void Draw(ref STRunner stRunner)
        {
            float x = Config.X[stRunner.nPlayer];
            float y = Config.Y[stRunner.nPlayer];

            if (TJAPlayerPI.app.Timer.b停止していない)
            {
                float delta = TJAPlayerPI.app.FPS.fDelta;
                float speedMul = TJAPlayerPI.app.ConfigToml.PlayOption.PlaySpeed / 20.0f;
                float valueSpeed = (float)TJAPlayerPI.stage演奏ドラム画面.actPlayInfo.dbBPM[0] * speedMul / (60.0f * 4);
                stRunner.fValue += valueSpeed * delta;
            }


            float inTimeLength = 0.15f;
            float inValue = CConvert.InverseLerpClamp(0.0f, inTimeLength, stRunner.fValue);
            float loopValue = stRunner.fValue;
            bool isMiss = Config.Style == "Miss";
            bool isIn = stRunner.fValue < inTimeLength;

            if (!isMiss)
            {
                x += Config.InJumpWave[0] * MathF.Sin(inValue * MathF.PI * 0.5f);
                loopValue -= inTimeLength;
                if (isIn)
                {
                    y += MathF.Sin(inValue * MathF.PI) * Config.InJumpWave[1];
                }
            }

            if (!isIn || isMiss)
            {
                x += loopValue * TJAPlayerPI.app.LogicalSize.Width * 0.9f;
            }
            loopValue *= Config.LoopSpeed;

            int inFrame = (int)(inValue * Config.InPtn);
            inFrame = Math.Min(inFrame, Config.InPtn);

            float scale = float.Lerp(0.7f, 1.0f, CConvert.InverseLerpClamp(0.0f, inTimeLength, stRunner.fValue));

            switch (Config.Style)
            {
                case "Miss":
                    {
                        float waveValue = (-1 + MathF.Cos(loopValue * MathF.PI));
                        y += waveValue * Config.Wave;
                    }
                    break;
                case "Paper_Crane":
                    {
                        if (!isIn)
                        {
                            float waveValue = (1.0f - CConvert.ZigZagWave(loopValue + 0.25f)) * 0.5f;
                            y += waveValue * Config.Wave;
                        }
                    }
                    break;
            }

            int rectY = Config.Width * stRunner.nType;
            if (isIn)
            {
                if (this.txIn is not null)
                {
                    Rectangle rectangle = new Rectangle(Config.Width * inFrame, rectY, Config.Width, Config.Height);
                    this.txIn.vcScaling = new Vector2(scale);
                    this.txIn.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, rectangle);
                }
            }
            else
            {
                if (this.txLoop is not null)
                {
                    int frame = (int)(stRunner.fValue * 20) % Config.LoopPtn;
                    Rectangle rectangle = new Rectangle(Config.Width * frame, rectY, Config.Width, Config.Height);
                    this.txLoop.vcScaling = new Vector2(scale);
                    this.txLoop.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, rectangle);
                }
            }
        }
    }

    /*
    // ランナー画像のサイズ。 X, Y
    private int[] Size;
    // ランナーのコマ数
    private int Ptn;
    // ランナーのキャラクターのバリエーション(ミス時を含まない)。
    private int Type;
    // スタート地点のX座標 1P, 2P
    private int[] StartPoint_X;
    // スタート地点のY座標 1P, 2P
    private int[] StartPoint_Y;
    */
    //-----------------
    #endregion
}
