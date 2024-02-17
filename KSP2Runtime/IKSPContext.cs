using System;
using System.Diagnostics.CodeAnalysis;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime;

public interface IMarker {
    bool Visible { get; set; }
    void OnUpdate();

    void OnRender();
}

public interface IKSPAutopilot {
    void UpdateAutopilot(ref FlightCtrlState st, float deltaTime);
}

public interface IKSPContext : IContext {
    GameInstance Game { get; }

    KSPGameMode GameMode { get; }

    VesselComponent? ActiveVessel { get; }

    KSPConsoleBuffer ConsoleBuffer { get; }

    TimeSeriesCollection TimeSeriesCollection { get; }

    double UniversalTime { get; }

    object? NextYield { get; set; }

    Action? OnNextYieldOnce { get; set; }

    OptionalAddons OptionalAddons { get; }

    KSPOrbitModule.IBody? FindBody(string name);

    void AddMarker(IMarker marker);

    void RemoveMarker(IMarker marker);

    void ClearMarkers();

    void AddResourceTransfer(KSPResourceModule.ResourceTransfer resourceTransfer);

    void AddWindow(KSPUIModule.Window window);

    bool TryFindAutopilot<T>(VesselComponent vessel, [MaybeNullWhen(false)] out T autopilot) where T : IKSPAutopilot;

    void HookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot);

    void UnhookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot);

    void UnhookAllAutopilots(VesselComponent vessel);
}

public class OptionalAddons {
    public (object instance, Version version) FlightPlan { get; set; }
}

public class KSPContext {
    public static IKSPContext CurrentContext {
        get {
            var context = ContextHolder.CurrentContext.Value as IKSPContext;
            if (context == null) throw new ArgumentException("No current IKSPContext");
            return context;
        }
    }
}
