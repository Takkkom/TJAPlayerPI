using FDK;

namespace TJAPlayerPI.Common
{
    internal class CNamePlate : CActivity
    {
        public CNamePlate()
        {

        }

        public override void On活性化()
        {
            if (this.b活性化してる)
                return;

            pfNameFont = CFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameSize);
            pfTitleFont = CFontHelper.tCreateFont(TJAPlayerPI.app.Skin.SkinConfig.NamePlate.TitleSize);

            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                tUpdatePlayerName(nPlayer);
                tUpdateTitle(nPlayer);
            }

            base.On活性化();
        }
        public override void On非活性化()
        {
            if (this.b活性化してない)
                return;

            TJAPlayerPI.t安全にDisposeする(ref pfNameFont);
            TJAPlayerPI.t安全にDisposeする(ref pfTitleFont);

            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                TJAPlayerPI.t安全にDisposeする(ref txPlayerName[nPlayer]);
                TJAPlayerPI.t安全にDisposeする(ref txTitle[nPlayer]);
            }

            base.On非活性化();
        }

        public override int On進行描画()
        {
            if (this.b活性化してない)
                return 0;



            return base.On進行描画();
        }

        public void On進行描画(int x, int y, int player, float scale = 1.0f, int opacity = 255)
        {
            bool validDan = false;
            Vector2 vcScaling = new Vector2(scale);

            CTexture? txShadow = TJAPlayerPI.app.Tx.NamePlate_Shadow;
            CTexture? txBase = TJAPlayerPI.app.Tx.NamePlate_Base;
            CTexture? txDanBase = TJAPlayerPI.app.Tx.NamePlate_DanBase;
            CTexture? txPlayerNumber = null;
            CTexture? txPlayerName = this.txPlayerName[player];
            CTexture? txTitleBase = null;
            CTexture? txTitle = this.txTitle[player];

            txPlayerNumber = TJAPlayerPI.app.Tx.NamePlate_PlayerNumber[player];
            txTitleBase = TJAPlayerPI.app.Tx.NamePlate_TitleBase_Player[player];

            if (txTitleBase is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.TitleBaseX * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.TitleBaseY * scale;
                txTitleBase.vcScaling = vcScaling;
                txTitleBase.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY);
            }
            if (txShadow is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.ShadowX * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.ShadowY * scale;
                txShadow.vcScaling = vcScaling;
                txShadow.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY);
            }
            if (txBase is not null)
            {
                txBase.vcScaling = vcScaling;
                txBase.t2D描画(TJAPlayerPI.app.Device, x, y);
            }
            //CFontHelper.tDrawTextureArray(txPlayerName, x + (151 * scale), y + (45 * scale), vcScaling, 24);
            if (txPlayerName is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameX * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameY * scale;
                if (validDan)
                {
                    offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameWithDanX * scale;
                    offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameWithDanY * scale;
                }
                else
                {
                    offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameX * scale;
                    offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.NameY * scale;
                }
                txPlayerName.vcScaling = vcScaling;
                txPlayerName.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x + offsetX, y + offsetY);
            }
            if (txTitle is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.TitleX * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.TitleY * scale;
                txTitle.vcScaling = vcScaling;
                txTitle.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x + offsetX, y + offsetY);
            }
            if (validDan)
            {
                if (txDanBase is not null)
                {
                    float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.DanBaseX * scale;
                    float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.DanBaseY * scale;
                    txDanBase.vcScaling = vcScaling;
                    txDanBase.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY);
                }
            }
            if (txPlayerNumber is not null)
            {
                float offsetX = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.PlayerNumberX * scale;
                float offsetY = TJAPlayerPI.app.Skin.SkinConfig.NamePlate.PlayerNumberY * scale;
                txPlayerNumber.vcScaling = vcScaling;
                txPlayerNumber.t2D描画(TJAPlayerPI.app.Device, x + offsetX, y + offsetY);
            }
        }

        public void tUpdatePlayerName(int nPlayer)
        {
            TJAPlayerPI.t安全にDisposeする(ref txPlayerName[nPlayer]);
            if (pfNameFont is not null)
            {
                //padding 24
                txPlayerName[nPlayer] = CFontHelper.tCreateFontTexture(pfNameFont, TJAPlayerPI.app.SaveManager.SaveDatas[nPlayer].Name, Color.White, Color.Black, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio);
            }
        }

        public void tUpdateTitle(int nPlayer)
        {
            TJAPlayerPI.t安全にDisposeする(ref txTitle[nPlayer]);
            if (pfTitleFont is not null)
            {
                txTitle[nPlayer] = CFontHelper.tCreateFontTexture(pfTitleFont, "", Color.Black);
            }
        }

        private CCachedFontRenderer? pfNameFont;
        private CCachedFontRenderer? pfTitleFont;
        private CTexture?[] txPlayerName = new CTexture[2];
        private CTexture?[] txTitle = new CTexture[2];
    }
}
