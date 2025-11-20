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
    public partial class CLuaMathEx
    {
        [LuaMember("Lerp")]
        public double Lerp(double begin, double end, double value) => double.Lerp(begin, end, value);

        [LuaMember("InvLerp")]
        public double InvLerp(double begin, double end, double value) => CConvert.InverseLerp(begin, end, value);
    }
}
