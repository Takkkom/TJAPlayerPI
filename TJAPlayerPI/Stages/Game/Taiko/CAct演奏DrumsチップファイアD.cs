using FDK;
using Silk.NET.OpenAL;

namespace TJAPlayerPI;

internal class CAct演奏DrumsチップファイアD : CActivity
{
    // コンストラクタ

    public CAct演奏DrumsチップファイアD()
    {
    }


    // メソッド

    public virtual void Start(int nLane, EJudge judge, int player)
    {
        for (int j = 0; j < 3 * 4; j++)
        {
            if (!this.st状態[j].b使用中)
            //for( int n = 0; n < 1; n++ )
            {
                this.st状態[j].b使用中 = true;
                //this.st状態[ n ].ct進行 = new CCounter( 0, 9, 20, CDTXMania.Timer );
                this.st状態[j].ct進行 = new CCounter(0, 3 + nOutLength, 25, TJAPlayerPI.app.Timer);
                this.st状態[j].judge = judge;
                this.st状態[j].nPlayer = player;
                this.st状態_大[j].nPlayer = player;

                switch (nLane)
                {
                    case 0x11:
                    case 0x12:
                        this.st状態[j].nIsBig = 0;
                        break;
                    case 0x13:
                    case 0x14:
                    case 0x1A:
                    case 0x1B:
                        this.st状態_大[j].ct進行 = new CCounter(0, 240, 1, TJAPlayerPI.app.Timer);//20
                        this.st状態_大[j].judge = judge;
                        this.st状態_大[j].nIsBig = 1;
                        break;
                }
                break;
            }
        }
    }

    // CActivity 実装

    public override void On活性化()
    {
        for (int i = 0; i < 3 * 4; i++)
        {
            this.st状態[i].ct進行 = new CCounter();
            this.st状態[i].b使用中 = false;
            this.st状態_大[i].ct進行 = new CCounter();
        }
        base.On活性化();
    }
    public override void On非活性化()
    {
        for (int i = 0; i < 3 * 4; i++)
        {
            this.st状態[i].ct進行 = null;
            this.st状態_大[i].ct進行 = null;
        }
        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (!base.b活性化してない)
        {
            for (int i = 0; i < 3 * 4; i++)
            {
                if (this.st状態[i].b使用中)
                {
                    if (!this.st状態[i].ct進行.b停止中)
                    {
                        this.st状態[i].ct進行.t進行();
                        if (this.st状態[i].ct進行.b終了値に達した)
                        {
                            this.st状態[i].ct進行.t停止();
                            this.st状態[i].b使用中 = false;
                        }

                        // (When performing calibration, reduce visual distraction
                        // and current judgment feedback near the judgment position.)
                        if (TJAPlayerPI.app.Tx.Effects_Hit_Explosion is not null && !TJAPlayerPI.IsPerformingCalibration)
                        {
                            int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.HitExplosion.Width;
                            int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.HitExplosion.Height;

                            int n = this.st状態[i].nIsBig == 1 ? (height * 2) : 0;
                            int nX = (TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldX[this.st状態[i].nPlayer]) - (width / 2);
                            int nY = (TJAPlayerPI.app.Skin.SkinConfig.Game.JudgePointY[this.st状態[i].nPlayer]) - (height / 2);

                            float value = Math.Max(this.st状態[i].ct進行.n現在の値 - 3, 0);
                            value /= nOutLength;

                            float opacity = 1.0f - value;
                            TJAPlayerPI.app.Tx.Effects_Hit_Explosion.Opacity = (int)(opacity * 255);

                            int index = Math.Min(this.st状態[i].ct進行.n現在の値, 3);

                            switch (st状態[i].judge)
                            {
                                case EJudge.Perfect:
                                case EJudge.AutoPerfect:
                                    if (!this.st状態_大[i].ct進行.b停止中 && TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big is not null && this.st状態_大[i].nIsBig == 1)
                                    {
                                        TJAPlayerPI.app.Tx.Effects_Hit_Explosion.t2D描画(TJAPlayerPI.app.Device, nX, nY, new Rectangle(index * width, n + (height * 2), width, height));
                                    }
                                    else
                                    {
                                        TJAPlayerPI.app.Tx.Effects_Hit_Explosion.t2D描画(TJAPlayerPI.app.Device, nX, nY, new Rectangle(index * width, n, height, height));
                                    }
                                    break;
                                case EJudge.Good:
                                    if (!this.st状態_大[i].ct進行.b停止中 && TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big is not null && this.st状態_大[i].nIsBig == 1)
                                    {
                                        TJAPlayerPI.app.Tx.Effects_Hit_Explosion.t2D描画(TJAPlayerPI.app.Device, nX, nY, new Rectangle(index * width, n + (height * 3), width, height));
                                    }
                                    else
                                    {
                                        TJAPlayerPI.app.Tx.Effects_Hit_Explosion.t2D描画(TJAPlayerPI.app.Device, nX, nY, new Rectangle(index * width, n + height, width, height));
                                    }
                                    break;
                                case EJudge.Miss:
                                case EJudge.Bad:
                                    break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 3 * 4; i++)
            {
                if (!this.st状態_大[i].ct進行.b停止中)
                {
                    this.st状態_大[i].ct進行.t進行();
                    if (this.st状態_大[i].ct進行.b終了値に達した)
                    {
                        this.st状態_大[i].ct進行.t停止();
                    }
                    if (TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big is not null && this.st状態_大[i].nIsBig == 1)
                    {
                        int x = TJAPlayerPI.app.Skin.SkinConfig.Game.ScrollFieldX[this.st状態_大[i].nPlayer];
                        int y = TJAPlayerPI.app.Skin.SkinConfig.Game.JudgePointY[this.st状態[i].nPlayer];

                        float value = this.st状態_大[i].ct進行.n現在の値 / 120.0f;

                        DrawExpBig(x, y, value, 0.65f, 1.15f, 1.4f, st状態_大[i].judge, false);
                        if (value >= 1)
                        {
                            DrawExpBig(x, y, value - 1.0f, 0.4f, 0.95f, 1.0f, st状態_大[i].judge, true);
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
    protected STSTATUS[] st状態 = new STSTATUS[3 * 4];
    protected STSTATUS_B[] st状態_大 = new STSTATUS_B[3 * 4];

    protected int[] nX座標 = new int[] { 450, 521, 596, 686, 778, 863, 970, 1070, 1150 };
    protected int[] nY座標 = new int[] { 172, 108, 50, 8, -10, -60, -5, 30, 90 };
    protected int[] nY座標P2 = new int[] { 172, 108, 50, 8, -10, -60, -5, 30, 90 };

    private int nOutLength = 2;

    [StructLayout(LayoutKind.Sequential)]
    protected struct STSTATUS
    {
        public bool b使用中;
        public CCounter ct進行;
        public EJudge judge;
        public int nIsBig;
        public int n透明度;
        public int nPlayer;
    }
    [StructLayout(LayoutKind.Sequential)]
    protected struct STSTATUS_B
    {
        public CCounter ct進行;
        public EJudge judge;
        public int nIsBig;
        public int n透明度;
        public int nPlayer;
    }

    private void DrawExpBig(int x, int y, float value, float scaleBegin, float scaleEnd, float length, EJudge eJudge, bool is2nd)
    {
        if (TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big is null) return;
        if (value > length) return;

        int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.HitExplosion.BigWidth;
        int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.HitExplosion.BigHeight;

        value /= length;

        float endLength = 0.5f;
        if (is2nd)
        {
            endLength = 0.0f;
        }
        else
        {
            value *= 1.0f + endLength;
        }

            float scaleValue = MathF.Min(value, 1.0f);
        scaleValue = MathF.Sin(scaleValue * MathF.PI * 0.5f);
        float scale = float.Lerp(scaleBegin, scaleEnd, scaleValue);

        float opacityValue = 0.0f;
        if (is2nd)
        {
            opacityValue = 1.0f - MathF.Cos(value * MathF.PI * 0.5f);
        }
        else
        {
            opacityValue = MathF.Max(value - 1.0f, 0.0f) / endLength;
        }
        float opacity = 1.0f - opacityValue;

        TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big.Opacity = (int)(opacity * 255);
        TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big.vcScaling = new Vector2(scale);

        switch (eJudge)
        {
            case EJudge.Perfect:
            case EJudge.AutoPerfect:
                {
                    TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, new Rectangle(width, 0, width, height));
                }
                break;

            case EJudge.Good:
                {
                    TJAPlayerPI.app.Tx.Effects_Hit_Explosion_Big.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, new Rectangle(0, 0, width, height));
                }
                break;

            case EJudge.Miss:
            case EJudge.Bad:
                break;
        }
    }

    //-----------------
    #endregion
}
