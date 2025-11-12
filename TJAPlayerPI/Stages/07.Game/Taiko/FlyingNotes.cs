using FDK;
using System;

namespace TJAPlayerPI;

internal class FlyingNotes : CActivity
{
    // コンストラクタ

    public FlyingNotes()
    {
    }


    // メソッド
    public virtual void Start(int nLane, int nPlayer, bool isRoll = false)
    {
        if (TJAPlayerPI.app.Tx.Notes is null)
            return;

        for (int i = 0; i < 128; i++)
        {
            if (!Flying[i].IsUsing)
            {
                // 初期化
                Flying[i].IsUsing = true;
                Flying[i].Lane = nLane;
                Flying[i].Player = nPlayer;
                Flying[i].X = StartPointX[nPlayer];
                Flying[i].Y = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[nPlayer];
                Flying[i].StartPointX = StartPointX[nPlayer];
                Flying[i].StartPointY = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[nPlayer];
                Flying[i].EndPointX = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[nPlayer];
                Flying[i].EndPointY = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[nPlayer];
                Flying[i].OldValue = 0;
                Flying[i].IsRoll = isRoll;
                // 角度の決定
                Flying[i].Height = Math.Abs(TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[nPlayer] - TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[nPlayer]);
                Flying[i].Width = Math.Abs((TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[nPlayer] - StartPointX[nPlayer])) / 2;
                //Console.WriteLine("{0}, {1}", width2P, height2P);
                Flying[i].Counter = new CCounter(0, (180), TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.Timer, TJAPlayerPI.app.Timer);
                //Flying[i].Counter = new CCounter(0, 200000, CDTXMania.Skin.Game_Effect_FlyingNotes_Timer, CDTXMania.Timer);

                Flying[i].IncreaseX = (1.00 * Math.Abs((TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[nPlayer] - StartPointX[nPlayer]))) / (180);
                Flying[i].IncreaseY = (1.00 * Math.Abs((TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[nPlayer] - TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[nPlayer]))) / (180);
                break;
            }
        }
    }

    // CActivity 実装

    public override void On活性化()
    {
        for (int i = 0; i < 128; i++)
        {
            Flying[i] = new Status();
            Flying[i].IsUsing = false;
            Flying[i].Counter = new CCounter();
        }
        for (int i = 0; i < 2; i++)
        {
            StartPointX[i] = TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointX[i];
        }
        base.On活性化();
    }
    public override void On非活性化()
    {
        for (int i = 0; i < 128; i++)
        {
            Flying[i].Counter = null;
        }
        base.On非活性化();
    }
    public override int On進行描画()
    {
        if (!base.b活性化してない)
        {
            for (int i = 0; i < 128; i++)
            {
                if (Flying[i].IsUsing)
                {
                    Flying[i].OldValue = Flying[i].Counter.n現在の値;
                    Flying[i].Counter.t進行();
                    if (Flying[i].Counter.b終了値に達した)
                    {
                        Flying[i].Counter.t停止();
                        Flying[i].IsUsing = false;
                        TJAPlayerPI.stage演奏ドラム画面.actChipEffects.Start(Flying[i].Player, Flying[i].Lane);
                    }
                    for (int n = Flying[i].OldValue; n < Flying[i].Counter.n現在の値; n++)
                    {
                        /*
                        if (TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.IsUsingEasing)
                        {
                            Flying[i].X = Flying[i].StartPointX + Flying[i].Width + ((-Math.Cos(Flying[i].Counter.n現在の値 * (Math.PI / 180)) * Flying[i].Width));
                        }
                        else
                        {
                            Flying[i].X += Flying[i].IncreaseX;
                        }
                        */

                        if (n % TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FireWorks.Timing == 0 && !Flying[i].IsRoll && Flying[i].Counter.n現在の値 > 18)
                        {
                            if (Flying[i].Lane == 3 || Flying[i].Lane == 4)
                            {
                                TJAPlayerPI.stage演奏ドラム画面.FireWorks.Start(Flying[i].Lane, Flying[i].Player, Flying[i].X, Flying[i].Y);
                            }
                        }

                        float value = Flying[i].Counter.n現在の値 / 180.0f;

                        Vector2 begin = new Vector2(Flying[i].StartPointX, Flying[i].StartPointY);
                        Vector2 end = new Vector2(Flying[i].EndPointX, Flying[i].EndPointY);

                        //Vector2 position = GetArcPoint(begin, end, 230.0f, value);

                        float radius = Flying[i].Player == 0 ? -480 : 480;

                        Vector2 position = MoveAlongArc(begin, end, radius, value);
                        Flying[i].X = position.X;
                        Flying[i].Y = position.Y;


                        /*
                        if (Flying[i].Player == 0)
                        {
                            Flying[i].Y = (TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[Flying[i].Player]) - Math.Sin(Flying[i].Counter.n現在の値 * (Math.PI / 180)) * TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.Sine;
                            Flying[i].Y -= Flying[i].IncreaseY * Flying[i].Counter.n現在の値;
                        }
                        else
                        {
                            Flying[i].Y = (TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.StartPointY[Flying[i].Player]) + Math.Sin(Flying[i].Counter.n現在の値 * (Math.PI / 180)) * TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.Sine;
                            Flying[i].Y += Flying[i].IncreaseY * Flying[i].Counter.n現在の値;
                        }
                        */

                    }

                    TJAPlayerPI.app.Tx.Notes?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, (int)Flying[i].X, (int)Flying[i].Y, new Rectangle(Flying[i].Lane * 130, 0, 130, 130));

                }
            }
        }
        return base.On進行描画();
    }


    #region [ private ]
    //-----------------

    [StructLayout(LayoutKind.Sequential)]
    private struct Status
    {
        public int Lane;
        public int Player;
        public bool IsUsing;
        public CCounter Counter;
        public int OldValue;
        public double X;
        public double Y;
        public int Height;
        public int Width;
        public double IncreaseX;
        public double IncreaseY;
        public bool IsRoll;
        public int StartPointX;
        public int StartPointY;
        public int EndPointX;
        public int EndPointY;
    }

    private Status[] Flying = new Status[128];

    public readonly int[] StartPointX = new int[2];


    public static Vector2 MoveAlongArc(Vector2 startPoint, Vector2 endPoint, float radius, float t)
    {
        float dx = endPoint.X - startPoint.X;
        float dy = endPoint.Y - startPoint.Y;
        float L = MathF.Sqrt(dx * dx + dy * dy);

        float R_abs = MathF.Abs(radius);

        if (L / 2.0 > R_abs)
        {
            return new Vector2(
                startPoint.X + dx * t,
                startPoint.Y + dy * t
            );
        }

        Vector2 midPoint = new Vector2(
            (startPoint.X + endPoint.X) / 2.0f,
            (startPoint.Y + endPoint.Y) / 2.0f
        );

        float H_abs = MathF.Sqrt(R_abs * R_abs - (L / 2.0f) * (L / 2.0f));
        float H = (radius >= 0) ? H_abs : -H_abs;

        float unitPerpX = dy / L;
        float unitPerpY = -dx / L;

        Vector2 centerPoint = new Vector2(
            midPoint.X + H * unitPerpX,
            midPoint.Y + H * unitPerpY
        );

        float theta_s = MathF.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
        float theta_e = MathF.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);

        float deltaTheta = theta_e - theta_s;

        if (deltaTheta > MathF.PI)
        {
            deltaTheta -= 2 * MathF.PI;
        }
        else if (deltaTheta < -MathF.PI)
        {
            deltaTheta += 2 * MathF.PI;
        }

        float theta_t = theta_s + t * deltaTheta;

        Vector2 currentPoint = new Vector2(
            centerPoint.X + R_abs * MathF.Cos(theta_t),
            centerPoint.Y + R_abs * MathF.Sin(theta_t)
        );

        return currentPoint;
    }

    //-----------------
    #endregion
}
