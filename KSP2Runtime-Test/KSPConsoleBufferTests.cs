using System.Collections.Generic;
using Xunit;
using KontrolSystem.KSP.Runtime.KSPConsole;

namespace KontrolSystem.KSP.Runtime.Test {
    public class KSPConsoleBufferTests {
        [Fact]
        public void TestPrint() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(0, console.CursorCol);
            Assert.Equal(0, console.CursorRow);

            console.Print("Line1");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(5, console.CursorCol);
            Assert.Equal(0, console.CursorRow);

            console.Print("Line1a");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(11, console.CursorCol);
            Assert.Equal(0, console.CursorRow);

            console.Print("Line1b\nLine2\nLine3\nLine4\nLine5");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(5, console.CursorCol);
            Assert.Equal(4, console.CursorRow);

            console.Print("Line5a");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(11, console.CursorCol);
            Assert.Equal(4, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            Assert.Equal("Line1Line1aLine1b", visibleLines[0].ToString().TrimEnd('\0'));
            Assert.Equal("Line2", visibleLines[1].ToString().TrimEnd('\0'));
            Assert.Equal("Line3", visibleLines[2].ToString().TrimEnd('\0'));
            Assert.Equal("Line4", visibleLines[3].ToString().TrimEnd('\0'));
            Assert.Equal("Line5Line5a", visibleLines[4].ToString().TrimEnd('\0'));
        }

        [Fact]
        public void TestPrintLineWithScroll() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            for (int i = 0; i < 60; i++) console.PrintLine($"Line{i}");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(0, console.CursorCol);
            Assert.Equal(24, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            for (int i = 0; i < 24; i++) {
                Assert.Equal($"Line{i + 36}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.Equal(i + 36, visibleLines[i].lineNumber);
            }

            Assert.Equal("", visibleLines[24].ToString().TrimEnd('\0'));
        }

        [Fact]
        public void TestClear() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            for (int i = 0; i < 60; i++) console.PrintLine($"Line{i}");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(0, console.CursorCol);
            Assert.Equal(24, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            for (int i = 0; i < 24; i++) {
                Assert.Equal($"Line{i + 36}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.Equal(i + 36, visibleLines[i].lineNumber);
            }

            Assert.Equal("", visibleLines[24].ToString().TrimEnd('\0'));

            console.Clear();

            for (int i = 0; i < 10; i++) console.PrintLine($"Line{i}");

            Assert.Equal(25, console.VisibleLines.Count);
            Assert.Equal(0, console.CursorCol);
            Assert.Equal(10, console.CursorRow);

            visibleLines = console.VisibleLines;

            for (int i = 0; i < 10; i++) {
                Assert.Equal($"Line{i}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.Equal(i, visibleLines[i].lineNumber);
            }

            Assert.Equal("", visibleLines[10].ToString().TrimEnd('\0'));
        }
    }
}
