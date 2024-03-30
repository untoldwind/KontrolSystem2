using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPUI.Builtin;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

[KSModule("ksp::ui")]
public partial class KSPUIModule {
    [KSFunction]
    public static Vector2d ScreenSize() {
        return new Vector2d(Screen.width, Screen.height);
    }

    [KSConstant("CONSOLE_WINDOW", Description = "Main console window")]
    public static readonly ConsoleWindowAdapter MainConsole = new();

    [KSFunction]
    public static Window OpenWindow(string title, double x, double y, double width, double height) {
        var uiWindow = UIWindows.Instance!.gameObject.AddComponent<UGUIResizableWindow>();
        uiWindow.Initialize(title, new Rect((float)x, (float)y, (float)width, (float)height));
        return new Window(uiWindow);
    }

    [KSFunction]
    public static Window OpenCenteredWindow(string title, double width, double height) {
        var uiWindow = UIWindows.Instance!.gameObject.AddComponent<UGUIResizableWindow>();
        uiWindow.Initialize(title,
            new Rect(0.5f * (Screen.width - (float)width), 0.5f * (Screen.height + (float)height), (float)width,
                (float)height));
        return new Window(uiWindow);
    }

    [KSFunction]
    public static GradientWrapper Gradient(KSPConsoleModule.RgbaColor start, KSPConsoleModule.RgbaColor end) => new(start, end);

    public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
        return BindingGenerator.RegisterEnumTypeMappings("ksp::ui",
            new[] {
                ("Align",
                    "Alignment of the element in off direction (horizontal for vertical container and vice versa)",
                    typeof(UGUILayout.Align), new (Enum value, string description)[] {
                        (UGUILayout.Align.Start, "Align the element to start of container (left/top)."),
                        (UGUILayout.Align.Center, "Align the element to the center of the container."),
                        (UGUILayout.Align.End, "Align the element to end of container (right/bottom)."),
                        (UGUILayout.Align.Stretch, "Stretch the element to full size of container")
                    })
            });
    }
}
