using FDK;
using Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Scripting
{
    [LuaObject]
    public partial class CLuaTexture
    {
        internal CTexture? Texture;


        [LuaMember("width")]
        public int Width
        {
            get => Texture?.szTextureSize.Width ?? 0;
            set { }
        }

        [LuaMember("height")]
        public int Height
        {
            get => Texture?.szTextureSize.Height ?? 0;
            set { }
        }

        [LuaMember("opacity")]
        public float Opacity
        {
            get => (Texture?.Opacity ?? 0) / 255.0f;
            set
            {
                if (Texture is CTexture texture)
                {
                    texture.Opacity = (int)(value * 255);
                }
            }
        }

        private CLuaVector2 scaling = new CLuaVector2() { X = 1, Y = 1 };

        [LuaMember("getScaling")]
        public LuaValue GetScaling()
        {
            return scaling;
        }

        [LuaMember("draw")]
        public int Draw(float x, float y)
        {
            if (Texture is CTexture texture)
            {
                texture.vcScaling = new Vector2(scaling.X, scaling.Y);
                texture.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y);
            }

            return 0;
        }

        [LuaMember("drawRect")]
        public int DrawRect(float x, float y, float rectX, float rectY, float rectWidth, float rectHeight)
        {
            if (Texture is CTexture texture)
            {
                texture.vcScaling = new Vector2(scaling.X, scaling.Y);
                texture.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Center, x, y, new Rectangle((int)rectX, (int)rectY, (int)rectWidth, (int)rectHeight));
            }

            return 0;
        }
    }
}
