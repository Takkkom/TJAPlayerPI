using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI.Fade
{
    internal abstract class FadeSongLoadingBase : FadeBase
    {
        public abstract int TitleFontSize { get; }
        public abstract int SubTitleFontSize { get; }

        private string _title = "";
        public string Title
        {
            get => _title;
            set
            {
                if (value != _title)
                {
                    CreateTitle(value);
                }
                _title = value;
            }
        }
        private string _subTitle = "";
        public string SubTitle
        {
            get => _subTitle;
            set
            {
                if (value != _subTitle)
                {
                    CreateSubTitle(value);
                }
                _subTitle = value;
            }
        }

        public override void On活性化()
        {
            if (this.b活性化してる)
                return;

            pfTitle = HFontHelper.tCreateFont(TitleFontSize);
            pfSubTitle = HFontHelper.tCreateFont(SubTitleFontSize);

            CreateTitle(Title);
            CreateSubTitle(SubTitle);

            base.On活性化();
        }

        public override void On非活性化()
        {
            if (this.b活性化してない)
                return;

            TJAPlayerPI.t安全にDisposeする(ref pfTitle);
            TJAPlayerPI.t安全にDisposeする(ref pfSubTitle);
            TJAPlayerPI.t安全にDisposeする(ref txTitle);
            TJAPlayerPI.t安全にDisposeする(ref txSubTitle);

            base.On非活性化();
        }

        public override int OnUpdate()
        {
            if (this.b活性化してない)
                return 0;


            return base.OnUpdate();
        }


        private CCachedFontRenderer? pfTitle;
        private CCachedFontRenderer? pfSubTitle;
        protected CTexture? txTitle;
        protected CTexture? txSubTitle;

        private void CreateTitle(string title)
        {
            TJAPlayerPI.t安全にDisposeする(ref txTitle);

            if (pfTitle is null)
            {
                return;
            }
            txTitle = HFontHelper.tCreateFontTexture(pfTitle, title, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._TitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio);

            if (txTitle is not null)
            {
                txTitle.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txTitle, 710);
            }
        }

        private void CreateSubTitle(string title)
        {
            TJAPlayerPI.t安全にDisposeする(ref txSubTitle);

            if (pfSubTitle is null)
            {
                return;
            }
            txSubTitle = HFontHelper.tCreateFontTexture(pfSubTitle, title, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.SongLoading._SubTitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio);

            if (txSubTitle is not null)
            {
                txSubTitle.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref txSubTitle, 710);
            }
        }
    }
}
