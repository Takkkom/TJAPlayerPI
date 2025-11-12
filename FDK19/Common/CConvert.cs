namespace FDK;

public static class CConvert
{
    // メソッド

    public static double DegreeToRadian(double angle)
    {
        return ((Math.PI * angle) / 180.0);
    }
    public static double RadianToDegree(double angle)
    {
        return (angle * 180.0 / Math.PI);
    }
    public static float DegreeToRadian(float angle)
    {
        return (float)DegreeToRadian((double)angle);
    }
    public static float RadianToDegree(float angle)
    {
        return (float)RadianToDegree((double)angle);
    }

    /// <summary>
    /// 百分率数値を255段階数値に変換するメソッド。透明度用。
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static int nParsentTo255(double num)
    {
        return (int)(255.0 * num);
    }

    public static float InverseLerp(float begin, float end, float value)
    {
        float val = value - begin;
        float length = end - begin;
        return val / length;
    }

    public static float InverseLerpClamp(float begin, float end, float value)
    {
        return InverseLerp(begin, end, Math.Clamp(value, begin, end));
    }

    public static float ZigZagWave(float value)
    {
        value -= (int)value;
        if (value < 0.25f)
        {
            return InverseLerp(0.0f, 0.25f, value);
        }
        else if (value < 0.5f)
        {
            return 1.0f - InverseLerp(0.25f, 0.5f, value);
        }
        else if (value < 0.75f)
        {
            return -InverseLerp(0.5f, 0.75f, value);
        }
        else if (value < 0.75f)
        {
            return -1.0f + InverseLerp(0.75f, 1.0f, value);
        }

        return 0;
    }
}
