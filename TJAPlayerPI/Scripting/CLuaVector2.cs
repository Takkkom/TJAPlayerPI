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
    public partial class CLuaVector2
    {
        private Vector2 vector2;

        [LuaMember("x")]
        public float X
        {
            get => vector2.X;
            set => vector2.X = value;
        }

        [LuaMember("y")]
        public float Y
        {
            get => vector2.X;
            set => vector2.X = value;
        }

        [LuaMember("normalize")]
        public CLuaVector2 Normalize()
        {
            Vector2 normalized = Vector2.Normalize(vector2);
            return new CLuaVector2()
            {
                X = normalized.X,
                Y = normalized.Y
            };
        }
    }
}
