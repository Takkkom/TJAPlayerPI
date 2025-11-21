using FDK;
using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI.Fade
{
    internal class FadeSongLoadingV1 : FadeSongLoadingBase
    {
        public override int TitleFontSize => TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleFontSize;
        public override int SubTitleFontSize => TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleFontSize;

        public override float DefaultFadeOutInterval => 2.0f;
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

            if (State != FadeState.None)
            {
                int bgHeight = TJAPlayerPI.app.LogicalSize.Height + TJAPlayerPI.app.Skin.SkinConfig.SongLoading.BackgroundExtension;

                Vector2 mobLeftPos = new Vector2(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobLeftX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobLeftY);
                Vector2 mobRightPos = new Vector2(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobRightX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobRightY);
                Vector2 mobCenterPos = new Vector2(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobCenterX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobCenterY);

                float bg_y = 0.0f;
                bool mobSideVisible = false;

                Vector2 titleBaseScale = Vector2.One;
                float titleBaseOpacity = 0.6f;
                float titleOpacity = 0.0f;

                switch (State)
                {
                    case FadeState.FadeOut:
                        {
                            float bgInValue = CConvert.InverseLerpClamp(0.0f, 0.2f, Value);
                            bgInValue = 1.0f - MathF.Cos(bgInValue * MathF.PI * 0.5f);
                            bg_y = bgHeight * (1.0f - bgInValue);

                            //mob center
                            float mobCenterIn1Value = CConvert.InverseLerpClamp(0.1f, 0.25f, Value);
                            mobCenterIn1Value = 1.0f - MathF.Cos(mobCenterIn1Value * MathF.PI * 0.5f);
                            mobCenterPos.Y += float.Lerp(TJAPlayerPI.app.LogicalSize.Height, -TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobCenterExY, mobCenterIn1Value);

                            float mobCenterIn2Value = CConvert.InverseLerpClamp(0.25f, 0.4f, Value);
                            mobCenterIn2Value = MathF.Sin(mobCenterIn2Value * MathF.PI * 0.5f);
                            mobCenterPos.Y += float.Lerp(0, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobCenterExY, mobCenterIn2Value);
                            //------------

                            //mob side
                            mobSideVisible = Value >= 0.2f;

                            //in1
                            float mobSideIn1Value = CConvert.InverseLerpClamp(0.2f, 0.35f, Value);
                            mobSideIn1Value = MathF.Sin(mobSideIn1Value * MathF.PI * 0.5f);
                            Vector2 sideOffset = new Vector2(TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobSideMovingX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobSideMovingY - TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobSideMovingEx) * (1.0f - mobSideIn1Value);

                            float mobSideIn2Value = CConvert.InverseLerpClamp(0.35f, 0.45f, Value);
                            sideOffset += new Vector2(0, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobSideMovingEx) * mobSideIn2Value;

                            mobLeftPos += sideOffset;
                            mobRightPos += new Vector2(-sideOffset.X, sideOffset.Y);
                            //--------------

                            //title base
                            float titleBaseInValue = CConvert.InverseLerpClamp(0.45f, 0.6f, Value);
                            titleBaseOpacity *= titleBaseInValue;
                            titleBaseScale.X = float.Lerp(1.0f, 2.1f, titleBaseInValue);
                            //--------------

                            //title
                            float titleInValue = CConvert.InverseLerpClamp(0.6f, 0.8f, Value);
                            titleOpacity = titleInValue;
                            //--------------
                        }
                        break;
                    case FadeState.FadeIn:
                        {
                            float inBGValue = CConvert.InverseLerpClamp(0.5f, 1.0f, Value);
                            inBGValue = 1.0f - MathF.Cos(inBGValue * MathF.PI * 0.5f);
                            bg_y = bgHeight * -inBGValue;

                            //title base
                            float titleBaseValue = CConvert.InverseLerpClamp(0.0f, 0.4f, Value);
                            titleBaseOpacity *= 1.0f - titleBaseValue;
                            titleBaseScale.X = 2.1f;
                            titleBaseScale.Y = 1 - titleBaseValue;
                            //--------------

                            //title
                            float titleValue = CConvert.InverseLerpClamp(0.0f, 0.2f, Value);
                            titleOpacity = 1.0f - titleValue;
                            //--------------

                            //mob center
                            float mobCenterOut1Value = CConvert.InverseLerpClamp(0.0f, 0.25f, Value);
                            mobCenterOut1Value = MathF.Sin(mobCenterOut1Value * MathF.PI * 0.5f);
                            mobCenterPos.Y += float.Lerp(0, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobCenterExY, mobCenterOut1Value);

                            float mobCenterOut2Value = CConvert.InverseLerpClamp(0.2f, 1.0f, Value);
                            mobCenterOut2Value = 1.0f - MathF.Cos(mobCenterOut2Value * MathF.PI * 0.5f);
                            mobCenterPos.Y += mobCenterOut2Value * -TJAPlayerPI.app.LogicalSize.Height * 2;
                            //------------

                            //mob side
                            mobSideVisible = true;

                            //in1
                            float mobSideOut1Value = CConvert.InverseLerpClamp(0.0f, 0.4f, Value);
                            float offsetY = mobSideOut1Value * TJAPlayerPI.app.Skin.SkinConfig.SongLoading.MobSideMovingEx;

                            float mobSideOut2Value = CConvert.InverseLerpClamp(0.4f, 1.0f, Value);
                            mobSideOut2Value = 1.0f - MathF.Cos(mobSideOut2Value * MathF.PI * 0.5f);
                            offsetY += mobSideOut2Value * -TJAPlayerPI.app.LogicalSize.Height * 2;

                            mobLeftPos.Y += offsetY;
                            mobRightPos.Y += offsetY;
                            //--------------
                        }
                        break;
                }

                TJAPlayerPI.app.Tx.SongLoading_Upper?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.DownLeft, 0, bg_y);
                TJAPlayerPI.app.Tx.SongLoading_Lower?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.UpLeft, 0, bg_y + TJAPlayerPI.app.LogicalSize.Height);
                if (TJAPlayerPI.app.Tx.SongLoading_Center is CTexture tx_center)
                {
                    tx_center.t2D描画(TJAPlayerPI.app.Device, 0, bg_y, new Rectangle(0, 0, TJAPlayerPI.app.LogicalSize.Width, TJAPlayerPI.app.LogicalSize.Height));
                }

                if (mobSideVisible)
                {
                    TJAPlayerPI.app.Tx.SongLoading_Mob_Left?.t2D描画(TJAPlayerPI.app.Device, mobLeftPos.X, mobLeftPos.Y);
                    TJAPlayerPI.app.Tx.SongLoading_Mob_Right?.t2D描画(TJAPlayerPI.app.Device, mobRightPos.X, mobRightPos.Y);
                }
                TJAPlayerPI.app.Tx.SongLoading_Mob_Center?.t2D描画(TJAPlayerPI.app.Device, mobCenterPos.X, mobCenterPos.Y);

                if (TJAPlayerPI.app.Tx.SongLoading_Title_Base is CTexture titleBase)
                {
                    titleBase.vcScaling = new Vector2(1.8f) * titleBaseScale;
                    titleBase.Opacity = (int)(titleBaseOpacity * 255);
                    titleBase.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.PlateY);
                }
                if (txTitle is CTexture title)
                {
                    title.vcScaling = new Vector2(1);
                    title.Opacity = (int)(titleOpacity * 255);

                    int subTitleOffset = string.IsNullOrEmpty(SubTitle) ? 15 : 0;
                    title.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.TitleY + subTitleOffset);
                }
                if (txSubTitle is CTexture subTitle)
                {
                    subTitle.vcScaling = new Vector2(1);
                    subTitle.Opacity = (int)(titleOpacity * 255);
                    subTitle.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleX, TJAPlayerPI.app.Skin.SkinConfig.SongLoading.SubTitleY);
                }
            }

            return base.On進行描画();
        }
    }
}
