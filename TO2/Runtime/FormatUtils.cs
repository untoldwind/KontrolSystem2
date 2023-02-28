using System;
using System.Linq;
using System.Globalization;

namespace KontrolSystem.TO2.Runtime {
    public static class FormatUtils {
        public static string BoolToString(bool b) => b ? "true" : "false";

        public static string IntToString(long i) => i.ToString(CultureInfo.InvariantCulture);

        public static string FloatToString(double d) => d.ToString(CultureInfo.InvariantCulture);

        public static string FloatToFixed(double d, long decimals) =>
            d.ToString(decimals <= 0 ? "F0" : "F" + decimals, CultureInfo.InvariantCulture);

        public static string StringRepeat(string s, long count) =>
            count <= 0 ? "" : String.Concat(Enumerable.Repeat(s, (int)count));

        public static string StringPadLeft(string s, long length) =>
            s.Length >= length ? s : new String(' ', (int)length - s.Length) + s;

        public static string StringPadRight(string s, long length) =>
            s.Length >= length ? s : s + new String(' ', (int)length - s.Length);
    }
}
