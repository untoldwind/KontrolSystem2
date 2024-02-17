using System.Globalization;
using System.Linq;

namespace KontrolSystem.TO2.Runtime;

public static class FormatUtils {
    public static string BoolToString(bool b) {
        return b ? "true" : "false";
    }

    public static string IntToString(long i) {
        return i.ToString(CultureInfo.InvariantCulture);
    }

    public static string FloatToString(double d) {
        return d.ToString(CultureInfo.InvariantCulture);
    }

    public static string FloatToFixed(double d, long decimals) {
        return d.ToString(decimals <= 0 ? "F0" : "F" + decimals, CultureInfo.InvariantCulture);
    }

    public static string StringRepeat(string s, long count) {
        return count <= 0 ? "" : string.Concat(Enumerable.Repeat(s, (int)count));
    }

    public static string StringPadLeft(string s, long length) {
        return s.Length >= length ? s : new string(' ', (int)length - s.Length) + s;
    }

    public static string StringPadRight(string s, long length) {
        return s.Length >= length ? s : s + new string(' ', (int)length - s.Length);
    }
}
