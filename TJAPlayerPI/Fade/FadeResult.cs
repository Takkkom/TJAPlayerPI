using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Fade
{
    internal class FadeResult : FadeBase
    {
        public override float DefaultFadeOutInterval => 1.0f;
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

            if (State == FadeState.None)
                return 0;

            switch (State)
            {
                case FadeState.FadeOut:
                    {
                        float y = 360;
                        y *= Value;
                        TJAPlayerPI.app.Tx.Result_FadeIn?.t2D描画(TJAPlayerPI.app.Device, 0, -360 + y, new Rectangle(0, 0, 1280, 380));
                        TJAPlayerPI.app.Tx.Result_FadeIn?.t2D描画(TJAPlayerPI.app.Device, 0, 720 - y, new Rectangle(0, 380, 1280, 360));
                    }
                    break;
                case FadeState.Wait:
                case FadeState.FadeIn:
                    if (TJAPlayerPI.app.Tx.Result_Background is CTexture background)
                    {
                        float opacity = 1.0f - CConvert.InverseLerpClamp(0.0f, 0.5f, Value);

                        background.Opacity = (int)(opacity * 255);
                        background.t2D描画(TJAPlayerPI.app.Device, 0, 0);
                        background.Opacity = 255;
                    }
                    break;
            }

            return base.On進行描画();
        }

        public override void StartFadeOut(float interval, Action? finished = null)
        {
            TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND成績発表].t再生する();
            base.StartFadeOut(interval, finished);
        }

        public override void StartFadeIn(float interval, Action? finished = null)
        {
            base.StartFadeIn(interval, finished);
        }
    }
}
