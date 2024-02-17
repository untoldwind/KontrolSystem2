using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    namespace KontrolSystem.TO2.Runtime {
        [KSModule("core::str",
            Description =
                "Provided basic string manipulation and formatting."
        )]
        public class CoreStr {
            [KSFunction(
                Description = "Join an array of string with a separator."
            )]
            public static string Join(string separator, string[] items) {
                return string.Join(separator, items);
            }

            [KSFunction(
                Description =
                    "Format items using C# format strings (https://learn.microsoft.com/en-us/dotnet/api/system.string.format). Items can be either a single value, an array or a tuple."
            )]
            public static string Format<T>(string format, T items) {
                try {
                    if (items is Array a)
                        return string.Format(CultureInfo.InvariantCulture, format, a.Cast<object>().ToArray());

                    if (items is ITuple t) {
                        var values = new object[t.Length];
                        for (var i = 0; i < t.Length; i++) values[i] = t[i];

                        return string.Format(CultureInfo.InvariantCulture, format, values);
                    }

                    return string.Format(CultureInfo.InvariantCulture, format, items);
                } catch (FormatException e) {
                    return $"{format}: {e.Message}";
                }
            }
        }
    }
}
