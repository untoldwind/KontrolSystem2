using System;
using System.Collections;
using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Test;

[KSModule("test::module")]
public class TestModule {
    [KSFunction]
    public static long lambda_int_callback(long a, long b, Func<long, long, long> call) {
        return call(a, 2 * b) + call(b, a);
    }

    [KSFunction]
    public static TestEnumerable CreateTestEnumerable() => new();
    

    [KSClass("TestEnumerable")]
    public class TestEnumerable : IEnumerable<string> {
        private readonly List<string> data = ["first", "second", "third"];
        
        public IEnumerator<string> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
