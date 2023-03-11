using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using KontrolSystem.TO2.AST;
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
            public static string Join(string separator, string[] items) => String.Join(separator, items);

            [KSFunction(
                Description = "Format items using C# format strings (https://learn.microsoft.com/en-us/dotnet/api/system.string.format). Items can be either a single value, an array or a tuple."
            )]
            public static string Format<T>(string format, T items) {
                try {
                    if (items is Array a) {
                        return String.Format(CultureInfo.CurrentCulture, format, a.Cast<object>().ToArray());
                    }

                    if (items is ITuple t) {
                        object[] values = new object[t.Length];
                        for (int i = 0; i < t.Length; i++) {
                            values[i] = t[i];
                        }

                        return String.Format(CultureInfo.CurrentCulture, format, values);
                    }

                    return String.Format(CultureInfo.CurrentCulture, format, items);
                } catch (FormatException e) {
                    return $"{format}: {e.Message}";
                }
            }
        }
    }
}
