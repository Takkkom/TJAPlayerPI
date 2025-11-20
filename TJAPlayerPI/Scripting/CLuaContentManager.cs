using FDK;
using FFmpeg.AutoGen;
using Lua;
using Lua.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Scripting
{
    [LuaObject]
    public partial class CLuaContentManager
    {
        internal string strWorkDirectory = "";
        private List<object?> listItems = new List<object?>();

        public int Add(object? item)
        {
            listItems.Add(item);
            return listItems.Count - 1;
        }

        /*
        private T? Get<T>(int index)
        {
            object? obj = listItems[index];
            if (obj is T t)
            {
                return t;
            }
            return default;
        }
        */

        internal void Dispose()
        {
            listItems.ForEach(x =>
            {
                if (x is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });
            listItems.Clear();
        }

        private LuaValue ToLuaValue(JsonElement jsonElement)
        {
            switch(jsonElement.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return new LuaValue(jsonElement.GetBoolean());
                case JsonValueKind.String:
                    return new LuaValue(jsonElement.GetString() ?? "");
                case JsonValueKind.Number:
                    return new LuaValue(jsonElement.GetDouble());
                case JsonValueKind.Array:
                    {
                        LuaTable luaTable = new LuaTable();

                        int index = 1;
                        foreach (var item in jsonElement.EnumerateArray())
                        {
                            luaTable[index] = ToLuaValue(item);
                            index++;
                        }

                        return luaTable;
                    }
                case JsonValueKind.Object:
                    {
                        LuaTable luaTable = new LuaTable();

                        foreach (var item in jsonElement.EnumerateObject())
                        {
                            luaTable[item.Name] = ToLuaValue(item.Value);
                        }

                        return luaTable;
                    }
                default:
                    return LuaValue.Nil;
            }
        }

        [LuaMember("GetPath")]
        public string GetPath(string path) => Path.Combine(strWorkDirectory, path).Replace(Path.DirectorySeparatorChar, '/');

        [LuaMember("LoadTexture")]
        public LuaValue LoadTexture(string path)
        {
            CTexture? texture = TJAPlayerPI.app.tCreateTexture(GetPath(path));
            Add(texture);

            LuaValue luaTexture = new CLuaTexture()
            {
                Texture = texture
            };

            return luaTexture;
        }

        [LuaMember("LoadJson")]
        public LuaTable LoadJson(string path)
        {
            string text = CJudgeTextEncoding.ReadTextFile(GetPath(path)) ?? "";
            JsonElement table = JsonSerializer.Deserialize<JsonElement>(text);

            if (ToLuaValue(table) is LuaValue luaValue && luaValue.TryRead(out LuaTable luaTable))
            {
                return luaTable;
            }
            else
            {
                return new LuaTable();
            }
        }
    }
}
