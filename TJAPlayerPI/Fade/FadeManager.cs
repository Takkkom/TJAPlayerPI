using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Fade
{
    internal class FadeManager : CActivity
    {
        public static readonly FadeBlack FadeBlack = new FadeBlack();
        public static readonly FadeSongLoadingV1 FadeSongLoadingV1 = new FadeSongLoadingV1();
        public static readonly FadeSongLoadingV2 FadeSongLoadingV2 = new FadeSongLoadingV2();
        public static readonly FadeResult FadeResult = new FadeResult();

        public static FadeSongLoadingBase GetSongLoading()
        {
            if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
            {
                return FadeSongLoadingV2;
            }
            else
            {
                return FadeSongLoadingV1;
            }
        }

        public static FadeBase GetFadeResult()
        {
            if (TJAPlayerPI.app.ConfigToml.EnableSkinV2)
            {
                return FadeBlack;
            }
            else
            {
                return FadeResult;
            }
        }

        public FadeState FadeState => currentFade?.State ?? FadeState.None;

        public FadeManager()
        {
            listChildren.Add(FadeBlack);
            listChildren.Add(FadeSongLoadingV1);
            listChildren.Add(FadeSongLoadingV2);
            listChildren.Add(FadeResult);
        }

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

        public int OnUpdate()
        {
            if (this.b活性化してない)
                return 0;

            if (currentFade is null)
                return 0;

            currentFade.OnUpdate();

            previousState = FadeState;

            return 0;
        }

        public override int On進行描画()
        {
            if (this.b活性化してない)
                return 0;

            if (currentFade is null)
                return 0;

            currentFade.On進行描画();

            return 0;
        }

        public void FadeOut(FadeBase fade, float? interval = null, Action? finished = null)
        {
            currentFade = fade;
            finishedAction = finished;

            currentFade?.StartFadeOut(interval ?? currentFade?.DefaultFadeOutInterval ?? 1, finished);
        }

        public void FadeIn(FadeBase? fade = null, float? interval = null, Action? finished = null)
        {
            if (fade is not null)
            {
                currentFade = fade;
            }
            finishedAction = finished;

            currentFade?.StartFadeIn(interval ?? currentFade?.DefaultFadeInInterval ?? 1, finished);
        }

        private FadeBase? currentFade;
        private Action? finishedAction;
        private FadeState previousState;
    }
}
