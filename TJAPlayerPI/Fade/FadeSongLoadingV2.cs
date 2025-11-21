using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Fade
{
    internal class FadeSongLoadingV2 : FadeSongLoadingBase
    {
        public override int TitleFontSize => TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleFontSize;
        public override int SubTitleFontSize => TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleFontSize;

        public override float DefaultFadeOutInterval => 1.0f;
        public override float DefaultFadeInInterval => 1.0f;

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

            float x = TJAPlayerPI.app.LogicalSize.Width * 0.5f;
            float titleOpacity = 0.0f;
            switch (State)
            {
                case FadeState.Wait:
                    x *= 0;
                    titleOpacity = 1.0f;
                    break;
                case FadeState.FadeOut:
                    {
                        float fadeoutValue = CConvert.InverseLerpClamp(0.0f, 0.5f, Value);
                        x *= 1.0f - fadeoutValue;

                        titleOpacity = CConvert.InverseLerpClamp(0.5f, 1.0f, Value);
                    }
                    break;
                case FadeState.FadeIn:
                    {
                        float fadeoutValue = CConvert.InverseLerpClamp(0.5f, 1.0f, Value);
                        x *= fadeoutValue;

                        titleOpacity = 1.0f - CConvert.InverseLerpClamp(0.5f, 1.0f, Value);
                    }
                    break;
            }

            int num = (int)Math.Min(100, x);

            if (TJAPlayerPI.app.Tx.SongLoading_v2_BG is CTexture bg)
            {
                int width = bg.szTextureSize.Width / 2;
                bg.t2D幕用描画(TJAPlayerPI.app.Device, -x, 0, new Rectangle(0, 0, width, bg.szTextureSize.Height), true, num);
                bg.t2D幕用描画(TJAPlayerPI.app.Device, width + x, 0, new Rectangle(width, 0, width, bg.szTextureSize.Height), false, num);
            }

            if (TJAPlayerPI.app.Tx.SongLoading_v2_Plate is CTexture plate)
            {
                plate.Opacity = (int)(titleOpacity * 255);
                plate.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2PlateY);
            }
            if (txTitle is CTexture title)
            {
                int subtitleOffset = string.IsNullOrEmpty(SubTitle) ? 15 : 0;
                title.Opacity = (int)(titleOpacity * 255);
                title?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleX + subtitleOffset, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2TitleY);
            }
            if (txSubTitle is CTexture subTitle)
            {
                subTitle.Opacity = (int)(titleOpacity * 255);
                subTitle.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.v2SubTitleY);
            }

            return base.On進行描画();
        }
    }
}
