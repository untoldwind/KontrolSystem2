﻿using KontrolSystem.KSP.Runtime.KSPConsole;
using Xunit;

namespace KontrolSystem.KSP.Runtime.Test;

public class KSPConsoleBufferTests {
    [Fact]
    public void TestPrint() {
        var console = new KSPConsoleBuffer(25, 40);

        Assert.Empty(console.VisibleLines(25));
        Assert.Equal(0, console.CursorCol);
        Assert.Equal(0, console.CursorRow);

        console.Print("Line1");

        Assert.Single(console.VisibleLines(25));
        Assert.Equal(5, console.CursorCol);
        Assert.Equal(0, console.CursorRow);

        console.Print("Line1a");

        Assert.Single(console.VisibleLines(25));
        Assert.Equal(11, console.CursorCol);
        Assert.Equal(0, console.CursorRow);

        console.Print("Line1b\nLine2\nLine3\nLine4\nLine5");

        Assert.Equal(5, console.VisibleLines(25).Count);
        Assert.Equal(5, console.CursorCol);
        Assert.Equal(4, console.CursorRow);

        console.Print("Line5a");

        Assert.Equal(5, console.VisibleLines(25).Count);
        Assert.Equal(11, console.CursorCol);
        Assert.Equal(4, console.CursorRow);

        var visibleLines = console.VisibleLines(25);

        Assert.Equal("Line1Line1aLine1b", visibleLines[0].ToString().TrimEnd('\0'));
        Assert.Equal("Line2", visibleLines[1].ToString().TrimEnd('\0'));
        Assert.Equal("Line3", visibleLines[2].ToString().TrimEnd('\0'));
        Assert.Equal("Line4", visibleLines[3].ToString().TrimEnd('\0'));
        Assert.Equal("Line5Line5a", visibleLines[4].ToString().TrimEnd('\0'));
    }

    [Fact]
    public void TestPrintLineWithScroll() {
        var console = new KSPConsoleBuffer(25, 40);

        for (var i = 0; i < 60; i++) console.PrintLine($"Line{i}");

        Assert.Equal(25, console.VisibleLines(25).Count);
        Assert.Equal(0, console.CursorCol);
        Assert.Equal(24, console.CursorRow);

        var visibleLines = console.VisibleLines(25);

        for (var i = 0; i < 25; i++) {
            Assert.Equal($"Line{i + 35}", visibleLines[i].ToString().TrimEnd('\0'));
            Assert.Equal(i + 35, visibleLines[i].lineNumber);
        }
    }

    [Fact]
    public void TestClear() {
        var console = new KSPConsoleBuffer(25, 40);

        for (var i = 0; i < 60; i++) console.PrintLine($"Line{i}");

        Assert.Equal(25, console.VisibleLines(25).Count);
        Assert.Equal(0, console.CursorCol);
        Assert.Equal(24, console.CursorRow);

        var visibleLines = console.VisibleLines(25);

        for (var i = 0; i < 25; i++) {
            Assert.Equal($"Line{i + 35}", visibleLines[i].ToString().TrimEnd('\0'));
            Assert.Equal(i + 35, visibleLines[i].lineNumber);
        }

        console.Clear();

        for (var i = 0; i < 10; i++) console.PrintLine($"Line{i}");

        Assert.Equal(10, console.VisibleLines(25).Count);
        Assert.Equal(0, console.CursorCol);
        Assert.Equal(10, console.CursorRow);

        visibleLines = console.VisibleLines(25);

        for (var i = 0; i < 10; i++) {
            Assert.Equal($"Line{i}", visibleLines[i].ToString().TrimEnd('\0'));
            Assert.Equal(i, visibleLines[i].lineNumber);
        }
    }

    [Fact]
    public void TestPrintAt() {
        var console = new KSPConsoleBuffer(25, 40);

        console.MoveCursor(0, 2);
        console.Print("Test Line 1");
        console.MoveCursor(2, 0);
        console.Print("Test Line 2");

        var visibleLines = console.VisibleLines(25);
        Assert.Equal(3, visibleLines.Count);
        Assert.Equal(11, console.CursorCol);
        Assert.Equal(2, console.CursorRow);


        Assert.Equal("  Test Line 1", visibleLines[0].ToString().TrimEnd('\0'));
        Assert.Equal("", visibleLines[1].ToString().TrimEnd('\0'));
        Assert.Equal("Test Line 2", visibleLines[2].ToString().TrimEnd('\0'));
    }
}
