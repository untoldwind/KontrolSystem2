using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Test {
    [KSModule("test::module")]
    public class TestModule {
        [KSFunction]
        public static long lambda_int_callback(long a, long b, Func<long, long, long> call) {
            return call(a, 2 * b) + call(b, a);
        }
    }
}
