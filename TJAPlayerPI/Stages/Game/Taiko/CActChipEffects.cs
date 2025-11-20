using FDK;
using Newtonsoft.Json.Linq;

namespace TJAPlayerPI;

internal class CActChipEffects : CActivity
{
    // コンストラクタ

    public CActChipEffects()
    {
    }

    // メソッド
    public virtual void Start(int nPlayer, int Lane)
    {
        //if (TJAPlayerPI.app.Tx.Gauge_Soul_Explosion is null)
        //    return;

        for (int i = 0; i < 128; i++)
        {
            var si = chipEffects[i];
            if (!si.b使用中)
            {
                si.b使用中 = true;
                si.ct進行 = new CCounter(0, 240, 1, TJAPlayerPI.app.Timer);
                si.nプレイヤー = nPlayer;
                si.Lane = Lane;
                break;
            }
        }
    }

    // CActivity 実装

    public override void On活性化()
    {
        for (int i = 0; i < 128; i++)
        {
            chipEffects[i] = new CChipEffect
            {
                b使用中 = false,
                ct進行 = new CCounter()
            };
        }
        base.On活性化();
    }
    public override void On非活性化()
    {
        for (int i = 0; i < 128; i++)
        {
            chipEffects[i].ct進行 = null;
            chipEffects[i].b使用中 = false;
        }
        base.On非活性化();
    }
    public override int On進行描画()
    {
        for (int i = 0; i < 128; i++)
        {
            var si = chipEffects[i];
            if (!si.b使用中 || si.ct進行 is null)
                continue;

            si.ct進行.t進行();
            if (si.ct進行.b終了値に達した)
            {
                si.ct進行.t停止();
                si.b使用中 = false;
            }

            float value = si.ct進行.n現在の値 / (float)si.ct進行.n終了値;

            if (TJAPlayerPI.app.Tx.Notes_White is not null)//2020.05.15 Mr-Ojii ノーツを白くするために追加。
            {
                Color nextColor = Color.White;
                if (value < 0.5f)
                {
                    float inValue = CConvert.InverseLerpClamp(0.0f, 0.5f, value);
                    Vector3 colorVec3 = Vector3.Lerp(new Vector3(BeginColor.R, BeginColor.G, BeginColor.B), new Vector3(nextColor.R, nextColor.G, nextColor.B), inValue);

                    TJAPlayerPI.app.Tx.Notes_White.color = Color.FromArgb((int)colorVec3.X, (int)colorVec3.Y, (int)colorVec3.Z);
                    TJAPlayerPI.app.Tx.Notes_White.Opacity = (int)(inValue * 255);
                }
                else
                {
                    float outValue = CConvert.InverseLerpClamp(0.5f, 1.0f, value);
                    TJAPlayerPI.app.Tx.Notes_White.color = nextColor;
                    TJAPlayerPI.app.Tx.Notes_White.Opacity = 255 - (int)(outValue * 255);
                }
            }

            if (TJAPlayerPI.app.Tx.Gauge_Explosion is not null)
            {
                Color nextColor = P1Color;
                switch (si.nプレイヤー)
                {
                    case 0:
                        nextColor = P1Color;
                        break;
                    case 1:
                        nextColor = P2Color;
                        break;
                }

                int width = 152;
                int height = 152;
                Rectangle rectangle = new Rectangle(0, 0, width, height);
                float scale = 1.0f;
                float angle = 0.0f;

                if (value > 0.8f)
                {
                    rectangle.X = width * 2;

                    float outValue = CConvert.InverseLerp(0.8f, 1.0f, value);
                    scale = float.Lerp(1.0f, 1.2f, outValue);
                    angle = float.Lerp(-60f, -90f, outValue);

                    float opacity = 1.0f - CConvert.InverseLerp(0.8f, 1.0f, value);
                    TJAPlayerPI.app.Tx.Gauge_Explosion.Opacity = (int)(opacity * 255);
                    TJAPlayerPI.app.Tx.Gauge_Explosion.color = nextColor;
                }
                else if (value > 0.1f)
                {
                    rectangle.X = width * 2;

                    float rotationValue = CConvert.InverseLerp(0.1f, 0.8f, value);
                    scale = float.Lerp(0.7f, 1.0f, rotationValue);
                    angle = float.Lerp(-15f, -60f, rotationValue);

                    Vector3 colorVec3 = Vector3.Lerp(new Vector3(BeginColor.R, BeginColor.G, BeginColor.B), new Vector3(nextColor.R, nextColor.G, nextColor.B), rotationValue);

                    TJAPlayerPI.app.Tx.Gauge_Explosion.Opacity = 255;
                    TJAPlayerPI.app.Tx.Gauge_Explosion.color = Color.FromArgb((int)colorVec3.X, (int)colorVec3.Y, (int)colorVec3.Z);
                }
                else if (value > 0.05f)
                {
                    rectangle.X = width * 1;
                    TJAPlayerPI.app.Tx.Gauge_Explosion.Opacity = 255;
                    TJAPlayerPI.app.Tx.Gauge_Explosion.color = Color.White;
                }
                else
                {
                    TJAPlayerPI.app.Tx.Gauge_Explosion.Opacity = 255;
                    TJAPlayerPI.app.Tx.Gauge_Explosion.color = Color.White;
                }

                TJAPlayerPI.app.Tx.Gauge_Explosion.fRotation = angle / 180.0f * MathF.PI;
                TJAPlayerPI.app.Tx.Gauge_Explosion.vcScaling = new Vector2(scale);
                TJAPlayerPI.app.Tx.Gauge_Explosion.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[si.nプレイヤー], TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[si.nプレイヤー], rectangle);
            }

            if (value < 0.5f)
            {
                TJAPlayerPI.app.Tx.Notes?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[si.nプレイヤー], TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[si.nプレイヤー], new Rectangle(si.Lane * 130, 0, 130, 130));
            }
            TJAPlayerPI.app.Tx.Notes_White?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointX[si.nプレイヤー], TJAPlayerPI.app.Skin.SkinConfig.Game.Effect.FlyingNotes.EndPointY[si.nプレイヤー], new Rectangle(si.Lane * 130, 0, 130, 130));

            if (TJAPlayerPI.app.Tx.Notes is not null)
                TJAPlayerPI.app.Tx.Notes.Opacity = 255; //2020.05.15 Mr-Ojii これ書いとかないと、流れるノーツも半透明化されてしまう。
        }
        return 0;
    }


    // その他

    #region [ private ]
    //-----------------
    private class CChipEffect
    {
        public bool b使用中;
        public CCounter? ct進行;
        public int nプレイヤー;
        public int Lane;
    }
    private CChipEffect[] chipEffects = new CChipEffect[128];
    public static readonly Color BeginColor = Color.FromArgb(240, 240, 5);
    public static readonly Color P1Color = Color.FromArgb(255, 68, 0);
    public static readonly Color P2Color = Color.FromArgb(36, 255, 210);
    //-----------------
    #endregion
}
