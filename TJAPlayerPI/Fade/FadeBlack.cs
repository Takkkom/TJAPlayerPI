using FDK;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Fade
{
    internal class FadeBlack : FadeBase
    {
        public override float DefaultFadeOutInterval => 0.5f;
        public override float DefaultFadeInInterval => 0.5f;

        public override void On活性化()
        {
            if (this.b活性化してる)
                return;




            base.On活性化();
        }

        public override void On非活性化()
        {
            if (this.b活性化してない)
                return;


            base.On非活性化();
        }

        public override int OnUpdate()
        {
            if (this.b活性化してない)
                return 0;


            return base.OnUpdate();
        }

        public override int On進行描画()
        {
            if (this.b活性化してない)
                return 0;

            if (TJAPlayerPI.app.Tx.Tile_Black is not null && State != FadeState.None)
            {
                float opacity = 0.0f;
                switch(State)
                {
                    case FadeState.Wait:
                        opacity = 1.0f;
                        break;
                    case FadeState.FadeOut:
                        opacity = Value;
                        break;
                    case FadeState.FadeIn:
                        opacity = 1.0f - Value;
                        break;
                }

                TJAPlayerPI.app.Tx.Tile_Black.Opacity = (int)(opacity * 255);

                for (int i = 0; i <= (TJAPlayerPI.app.LogicalSize.Width / TJAPlayerPI.app.Tx.Tile_Black.szTextureSize.Width); i++)     // #23510 2010.10.31 yyagi: change "clientSize.Width" to "640" to fix FIFO drawing size
                {
                    for (int j = 0; j <= (TJAPlayerPI.app.LogicalSize.Height / TJAPlayerPI.app.Tx.Tile_Black.szTextureSize.Height); j++)    // #23510 2010.10.31 yyagi: change "clientSize.Height" to "480" to fix FIFO drawing size
                    {
                        TJAPlayerPI.app.Tx.Tile_Black.t2D描画(TJAPlayerPI.app.Device, i * TJAPlayerPI.app.Tx.Tile_Black.szTextureSize.Width, j * TJAPlayerPI.app.Tx.Tile_Black.szTextureSize.Height);
                    }
                }
            }

            return base.On進行描画();
        }

        public override void StartFadeOut(float interval, Action? finished = null)
        {
            base.StartFadeOut(interval, finished);
        }

        public override void StartFadeIn(float interval, Action? finished = null)
        {
            base.StartFadeIn(interval, finished);
        }
    }
}
