using FDK;
using Lua;
using Lua.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Scripting
{
    internal class CLuaScript : IDisposable
    {
        public LuaState? laLuaState { get; private set; }
        private CLuaContentManager contentManager;

        public CLuaScript(string fileName)
        {
            contentManager = new CLuaContentManager()
            {
                strWorkDirectory = Path.GetDirectoryName(fileName) ?? ""
            };

            laLuaState = LuaState.Create();
            laLuaState.OpenStandardLibraries();
            /*
            LuaTable content = new LuaTable();
            content["LoadTexture"] = new LuaFunction((context, buffer, ct) =>
            {
                var texPath = context.GetArgument<string>(0);

                CTexture? texture = TJAPlayerPI.app.tCreateTexture(Path.Combine(strWorkDirectory, texPath));
                contentManager.Add(texture);

                CLuaTexture luaTexture = new CLuaTexture()
                {
                    Texture = texture
                };

                buffer.Span[0] = luaTexture;
                return new ValueTask<int>(0);
            });
            */

#pragma warning disable CS0029
            laLuaState.Environment["Content"] = contentManager;
            laLuaState.Environment["MathEx"] = new CLuaMathEx();
#pragma warning restore CS0029

            laLuaState.DoFileAsync(fileName).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            laLuaState = null;
            contentManager.Dispose();
        }

        public LuaFunction? GetFunction(string key)
        {
            if (laLuaState is null) return null;

            laLuaState.Environment.TryGetValue(key, out LuaValue luaValue);

            if (luaValue.TryRead<LuaFunction>(out LuaFunction luaFunction))
            {
                return luaFunction;
            }
            return null;
        }
    }
}
