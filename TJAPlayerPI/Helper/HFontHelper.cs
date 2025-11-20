using FDK;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Helper
{
    internal static class HFontHelper
    {
        public enum FontType
        {
            Main,
            Lyric
        }
        public static CCachedFontRenderer tCreateFont(int size, CFontRenderer.FontStyle fontStyle = CFontRenderer.FontStyle.Regular, FontType fontType = FontType.Main)
        {
            string fontName;
            string skinFontName;
            switch (fontType)
            {
                case FontType.Lyric:
                    fontName = TJAPlayerPI.app.ConfigToml.General.FontName;
                    skinFontName = TJAPlayerPI.app.Skin.SkinConfig.Font.LyricFontName;
                    break;
                case FontType.Main:
                default:
                    fontName = TJAPlayerPI.app.ConfigToml.General.FontName;
                    skinFontName = TJAPlayerPI.app.Skin.SkinConfig.Font.MainFontName;
                    break;
            }
            skinFontName = CSkin.Path(skinFontName);

            if (CFontRenderer.FontExists(skinFontName))
            {
                return new CCachedFontRenderer(skinFontName, size, fontStyle);
            }
            else if (CFontRenderer.FontExists(fontName))
            {
                return new CCachedFontRenderer(fontName, size, fontStyle);
            }
            else
            {
                return new CCachedFontRenderer(CFontRenderer.DefaultFontName, size, fontStyle);
            }
        }

        public static CTexture? tCreateFontTexture(CFontRenderer renderer, string str, Color fontColor)
        {
            using SKBitmap bitmap = renderer.DrawText(str, fontColor);
            return TJAPlayerPI.app.tCreateTexture(bitmap);
        }

        public static CTexture? tCreateFontTexture(CFontRenderer renderer, string str, Color fontColor, Color edgeColor, int edgeRatio)
        {
            using SKBitmap bitmap = renderer.DrawText(str, fontColor, edgeColor, edgeRatio);
            return TJAPlayerPI.app.tCreateTexture(bitmap);
        }

        public static CTexture? tCreateFontTexture(CFontRenderer renderer, string str, Color fontColor, Color gradationTopColor, Color gradataionBottomColor, int edgeRatio)
        {
            using SKBitmap bitmap = renderer.DrawText(str, fontColor, gradationTopColor, gradataionBottomColor, edgeRatio);
            return TJAPlayerPI.app.tCreateTexture(bitmap);
        }

        public static CTexture? tCreateFontTexture(CFontRenderer renderer, string str, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradataionBottomColor, int edgeRatio)
        {
            using SKBitmap bitmap = renderer.DrawText(str, fontColor, edgeColor, gradationTopColor, gradataionBottomColor, edgeRatio);
            return TJAPlayerPI.app.tCreateTexture(bitmap);
        }

        /*
        public static CTexture?[] tCreateFontTextureArray(CFontRenderer renderer, string str, Color fontColor, Color edgeColor, int edgeRatio)
        {
            CTexture?[] textures = new CTexture[str.Length]; 
            for (int i = 0; i < textures.Length; i++)
            {
                char ch = str[i];
                textures[i] = tCreateFontTexture(renderer, ch.ToString(), fontColor, edgeColor, edgeRatio);
            }
            return textures;
        }

        public static void tDrawTextureArray(CTexture?[] textures, float x, float y, Vector2 scale, int padding)
        {
            float fPadding = padding * scale.X;
            x -= fPadding * ((textures.Length - 1) * 0.5f);

            for (int i = 0; i < textures.Length; i++)
            {
                CTexture? texture = textures[i];

                if (texture is not null)
                {
                    texture.vcScaling = scale;
                    texture.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y);

                    x += fPadding;
                }
            }
        }
        */
    }
}
