using System;
using KontrolSystem.TO2.Runtime;
using Xunit;

namespace KontrolSystem.KSP.Runtime.Test {
    public class CellTests {
        [Fact]
        public void TestUpdate() {
            var cell = new Cell<string>("initial");

            Assert.Equal("initial", cell.Value);

            cell.Value = "direct";

            Assert.Equal("direct", cell.Value);

            cell.Update(old => old + "1234");

            Assert.Equal("direct1234", cell.Value);
        }

        [Fact]
        public void TestObserver() {
            var cell = new Cell<string>("initial");
            var observer1 = new Obverser();

            var sub1 = cell.Subscribe(observer1);

            cell.Value = "Value1";

            Assert.Equal("Value1", observer1.lastCall);

            var observer2 = new Obverser();

            var sub2 = cell.Subscribe(observer2);

            cell.Value = "Value2";

            Assert.Equal("Value2", observer1.lastCall);
            Assert.Equal("Value2", observer2.lastCall);

            sub1.Dispose();

            cell.Value = "Value3";

            Assert.Equal("Value2", observer1.lastCall);
            Assert.Equal("Value3", observer2.lastCall);

            sub2.Dispose();

            cell.Value = "Value4";

            Assert.Equal("Value2", observer1.lastCall);
            Assert.Equal("Value3", observer2.lastCall);
        }

        class Obverser : IObserver<string> {
            internal string lastCall;

            public void OnNext(string value) {
                lastCall = value;
            }

            public void OnError(Exception error) {
            }

            public void OnCompleted() {
            }
        }
    }
}
