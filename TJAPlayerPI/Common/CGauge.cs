using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TJAPlayerPI.CSkin.CSkinConfig.CGame.CEffect;

namespace TJAPlayerPI.Common
{
    internal class CGauge
    {
        public EventHandler<GaugeEventArgs> ClearIn;
        public EventHandler<GaugeEventArgs> ClearOut;
        public EventHandler<GaugeEventArgs> MaxIn;
        public EventHandler<GaugeEventArgs> MaxOut;

        private double _dbValue;
        public double dbValue
        {
            get => _dbValue;
            set
            {
                if (_dbValue == value) return;

                if (value >= dbClearLine && !bIsCleared)
                {
                    tClearIn();
                }
                else if (value < dbClearLine && bIsCleared)
                {
                    tClearOut();
                }

                if (value >= 100 && !bIsMaxed)
                {
                    tMaxIn();
                }
                else if (value < 100 && bIsMaxed)
                {
                    tMaxOut();
                }

                _dbValue = value;

            }
        }

        private double _dbClearLine;
        public double dbClearLine
        {
            get => _dbClearLine;
            set
            {
                if (_dbClearLine == value) return;
                _dbClearLine = value;
            }
        }

        private bool bIsDan;
        public bool bIsCleared => dbValue >= dbClearLine;
        public bool bIsMaxed => dbValue >= 100;

        public CGauge(int player, bool isDan)
        {
            this.nPlayer = player;
            this.bIsDan = isDan;
        }

        public void Update()
        {
            ctAddFlash.t進行();
            ctClearFlash.t進行Loop();
            ctRainbow.t進行();
            ctRainbowIn.t進行();
            ctRainbowOut.t進行();
            ctSoulFire.t進行Loop();
            ctSoulFireIn.t進行();
            ctSoulFireOut.t進行();
            ctSoulTextFlash.t進行Loop();

            nPreviousLine = nLine;
            nLine = (int)(dbValue / 2);
            nClearLine = (int)(dbClearLine / 2);

            if (nPreviousLine != nLine && nLine > nPreviousLine)
            {
                ctAddFlash = new CCounter(0, ctAddFlash.n終了値, ctAddFlash._n間隔ms, TJAPlayerPI.app.Timer);
            }

            if (ctRainbow.n現在の値 == ctRainbow.n終了値)
            {
                ctRainbow = new CCounter(0, 80, 1, TJAPlayerPI.app.Timer);
                nRainbowFrame = (nRainbowFrame + 1) % TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.RainbowFrameCount;
            }
        }

        public void Draw(int x, int y, float scale, int opacity)
        {
            float fOpacity = opacity / 255f;
            CTexture? txBase = null;
            CTexture? txRainbow = null;
            CTexture? txEdge = null;
            CTexture? txLine = TJAPlayerPI.app.Tx.Gauge_Line[this.nPlayer];
            CTexture? txFlash = TJAPlayerPI.app.Tx.Gauge_Flash[this.nPlayer];
            CTexture? txFrame = null;
            CTexture? txClearText = TJAPlayerPI.app.Tx.Gauge_ClearText;
            CTexture? txSoulFire = TJAPlayerPI.app.Tx.Gauge_SoulFire;
            CTexture? txSoulText = TJAPlayerPI.app.Tx.Gauge_SoulText;

            bool clearLineFlashable = false;

            if (this.bIsDan)
            {
                txBase = TJAPlayerPI.app.Tx.Gauge_Base_LvDan[this.nPlayer];
                txRainbow = TJAPlayerPI.app.Tx.Gauge_Rainbow_LvDan;
                txEdge = TJAPlayerPI.app.Tx.Gauge_Edge_LvDan;
                txFrame = TJAPlayerPI.app.Tx.Gauge_Frame_LvDan;
                txClearText = null;
            }
            else if (dbClearLine == 80)
            {
                txBase = TJAPlayerPI.app.Tx.Gauge_Base_Lv3[this.nPlayer];
                txRainbow = TJAPlayerPI.app.Tx.Gauge_Rainbow_Lv3;
                txEdge = TJAPlayerPI.app.Tx.Gauge_Edge_Lv3;
                txFrame = TJAPlayerPI.app.Tx.Gauge_Frame_Lv3;
                clearLineFlashable = true;
            }
            else if (dbClearLine == 70)
            {
                txBase = TJAPlayerPI.app.Tx.Gauge_Base_Lv2[this.nPlayer];
                txRainbow = TJAPlayerPI.app.Tx.Gauge_Rainbow_Lv2;
                txEdge = TJAPlayerPI.app.Tx.Gauge_Edge_Lv2;
                txFrame = TJAPlayerPI.app.Tx.Gauge_Frame_Lv2;
                clearLineFlashable = true;
            }
            else if (dbClearLine == 60)
            {
                txBase = TJAPlayerPI.app.Tx.Gauge_Base_Lv1[this.nPlayer];
                txRainbow = TJAPlayerPI.app.Tx.Gauge_Rainbow_Lv1;
                txEdge = TJAPlayerPI.app.Tx.Gauge_Edge_Lv1;
                txFrame = TJAPlayerPI.app.Tx.Gauge_Frame_Lv1;
                clearLineFlashable = true;
            }

            Vector2 vcScale = new Vector2(scale);
            CTexture.EFlipType flipType = this.nPlayer == 0 ? CTexture.EFlipType.None : CTexture.EFlipType.Vertical;

            float step = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.Step * scale;
            int clearBegin = nClearLine - 1;

            if (txEdge is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.EdgeX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.EdgeY[this.nPlayer] * scale;

                txEdge.vcScaling = vcScale;
                txEdge.Opacity = opacity;
                txEdge.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, flipType);
            }
            if (txBase is not null)
            {
                txBase.vcScaling = vcScale;
                txBase.Opacity = opacity;
                txBase.t2D描画(TJAPlayerPI.app.Device, x, y, flipType);
            }

            if (txLine is not null)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.LineSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.LineSize[1];

                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.LineX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.LineY[this.nPlayer] * scale;

                Rectangle rectangle = new Rectangle(0, height * nPlayer, width, height);

                txLine.vcScaling = vcScale;

                float value = ctClearFlash.n現在の値 / 480f;
                value *= 2;
                value = MathF.Min(value, 1.0f);
                float clearFlashOpacity = CConvert.ZigZagWave(value);

                for (int i = 0; i < nLine; i++)
                {
                    if (!bIsDan)
                    {
                        if (i == clearBegin)
                        {
                            rectangle = new Rectangle(width * 2, rectangle.Y, width, height);
                        }
                        else if (i == nClearLine)
                        {
                            rectangle = new Rectangle(width * 3, rectangle.Y, width, height);
                        }
                    }
                    txLine.Opacity = opacity;
                    txLine.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, rectangle);

                    if (clearLineFlashable && i < clearBegin && bIsCleared)
                    {
                        txLine.Opacity = (int)(clearFlashOpacity * 255);
                        txLine.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, new Rectangle(width, rectangle.Y, width, height));
                    }

                    offsetX += step;
                }
            }

            if (txFlash is not null && !bIsMaxed)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.FlashSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.FlashSize[1];

                int shift = 0;
                if (!bIsDan)
                {
                    if (nLine == nClearLine)
                    {
                        shift = this.nPlayer == 0 ? 1 : 2;
                    }
                    else if (nLine > nClearLine)
                    {
                        shift = 3;
                    }
                }

                Rectangle rectangle = new Rectangle(width * shift, 0, width, height);

                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.FlashX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.FlashY[this.nPlayer] * scale;
                offsetX += step * (nLine - 1);

                float flashValue = ctAddFlash.n現在の値 / 255f;
                float flashOpacity = MathF.Cos(flashValue * MathF.PI * 0.5f);
                flashOpacity *= fOpacity;

                txFlash.Opacity = (int)(flashOpacity * 255);
                txFlash.vcScaling = vcScale;
                txFlash.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, rectangle, flipType);
            }

            bool visibleRainbow = bIsMaxed || ctRainbowOut.n現在の値 != ctRainbowOut.n終了値;
            if (txRainbow is not null && visibleRainbow)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.RainbowSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.RainbowSize[1];
                int frameCount = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.RainbowFrameCount;

                int nextFrame = (nRainbowFrame + 1) % frameCount;

                float rainbowOpacity = 0.0f;
                if (bIsMaxed)
                {
                    rainbowOpacity = ctRainbowIn.n現在の値 / 100f;
                }
                else
                {
                    rainbowOpacity = 1.0f - (ctRainbowOut.n現在の値 / 100f);
                }

                txRainbow.vcScaling = vcScale;
                txRainbow.Opacity = (int)(opacity * rainbowOpacity);
                txRainbow.t2D描画(TJAPlayerPI.app.Device, x, y, new Rectangle(0, height * nextFrame, width, height), flipType);

                float nextOpacity = 1.0f - (ctRainbow.n現在の値 / 80f);
                nextOpacity *= rainbowOpacity;
                nextOpacity *= fOpacity;
                txRainbow.Opacity = (int)(nextOpacity * 255);
                txRainbow.t2D描画(TJAPlayerPI.app.Device, x, y, new Rectangle(0, height * nRainbowFrame, width, height), flipType);
            }

            if (txFrame is not null)
            {
                txFrame.Opacity = (int)(80 * fOpacity);
                txFrame.vcScaling = vcScale;
                txFrame.t2D描画(TJAPlayerPI.app.Device, x, y, flipType);
            }

            bool soulFireVisible = bIsMaxed || (ctSoulFireOut.n現在の値 != ctSoulFireOut.n終了値);
            if (txSoulFire is not null && soulFireVisible)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulFireSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulFireSize[1];

                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulFireX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulFireY[this.nPlayer] * scale;
                Rectangle rectangle = new Rectangle(width * ctSoulFire.n現在の値, 0, width, height);

                float soulFireScale = 0.7f;
                if (bIsMaxed)
                {
                    float value = ctSoulFireIn.n現在の値 / 50f;
                    soulFireScale *= value;
                }
                else
                {
                    float value = ctSoulFireOut.n現在の値 / 50f;
                    soulFireScale *= 1.0f - value;
                }

                txSoulFire.Opacity = opacity;
                txSoulFire.vcScaling = vcScale * new Vector2(soulFireScale);
                txSoulFire.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x + offsetX, y + offsetY, rectangle);
            }

            if (txSoulText is not null)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulTextSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulTextSize[1];

                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulTextX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulTextY[this.nPlayer] * scale;
                Rectangle rectangle = new Rectangle(bIsCleared ? width : 0, 0, width, height);

                txSoulText.vcScaling = vcScale;
                txSoulText.Opacity = opacity;
                txSoulText.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, rectangle);

                if (bIsMaxed && ctSoulTextFlash.n現在の値 == 0)
                {
                    float soulFlashOpacity = 0.5f;
                    soulFlashOpacity *= fOpacity;
                    txSoulText.Opacity = (int)(soulFlashOpacity * 255);
                    txSoulText.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, new Rectangle(width * 2, 0, width, height));
                }
            }



            if (txClearText is not null)
            {
                int width = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.ClearTextSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.ClearTextSize[1];

                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.ClearTextX[this.nPlayer] * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.ClearTextY[this.nPlayer] * scale;
                Rectangle rectangle = new Rectangle(bIsCleared ? width : 0, 0, width, height);

                offsetX += step * clearBegin;

                txClearText.vcScaling = vcScale;
                txClearText.Opacity = opacity;
                txClearText.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY, rectangle);
            }
        }

        public class GaugeEventArgs : EventArgs
        {
            public int nPlayer;

            public GaugeEventArgs(int nPlayer)
            {
                this.nPlayer = nPlayer;
            }
        }

        private int nPlayer;
        private int nPreviousLine;
        private int nLine;
        private int nClearLine;
        private int nRainbowFrame;
        private CCounter ctAddFlash = new CCounter(0, 255, 2, TJAPlayerPI.app.Timer) { n現在の値 = 255 };
        private CCounter ctClearFlash = new CCounter(0, 480, 1, TJAPlayerPI.app.Timer);
        private CCounter ctRainbow = new CCounter();
        private CCounter ctRainbowIn = new CCounter();
        private CCounter ctRainbowOut = new CCounter();
        private CCounter ctSoulFire = new CCounter(0, TJAPlayerPI.app.Skin.SkinConfig.Game.Gauge.SoulFireCount - 1, 40, TJAPlayerPI.app.Timer);
        private CCounter ctSoulFireIn = new CCounter();
        private CCounter ctSoulFireOut = new CCounter();
        private CCounter ctSoulTextFlash = new CCounter(0, 1, 40, TJAPlayerPI.app.Timer);

        private void tClearIn()
        {
            ClearIn?.Invoke(this, new GaugeEventArgs(this.nPlayer));
        }

        private void tClearOut()
        {
            ClearOut?.Invoke(this, new GaugeEventArgs(this.nPlayer));
        }

        private void tMaxIn()
        {
            ctRainbowIn = new CCounter(0, 100, 1, TJAPlayerPI.app.Timer);
            ctSoulFireIn = new CCounter(0, 50, 1, TJAPlayerPI.app.Timer);

            MaxIn?.Invoke(this, new GaugeEventArgs(this.nPlayer));
        }

        private void tMaxOut()
        {
            ctRainbowOut = new CCounter(0, 100, 1, TJAPlayerPI.app.Timer);
            ctSoulFireOut = new CCounter(0, 50, 1, TJAPlayerPI.app.Timer);

            MaxOut?.Invoke(this, new GaugeEventArgs(this.nPlayer));
        }
    }
}
