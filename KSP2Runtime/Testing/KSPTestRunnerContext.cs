using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Testing;

public class KSPTestRunnerContext : TestRunnerContext, IKSPContext {
    private Dictionary<string, KSPOrbitModule.IBody> Bodies => new KSPOrbitModule.IBody[] {
        MockBody.Kerbol,
        MockBody.Eve,
        MockBody.Gilly,
        MockBody.Kerbin,
        MockBody.Mun,
        MockBody.Minmus,
        MockBody.Duna,
        MockBody.Ike,
        MockBody.Jool,
        MockBody.Tylo,
        MockBody.Vall,
        MockBody.Pol
    }.ToDictionary(body => body.Name);

    public GameInstance Game => throw new NotSupportedException("Game is no available in test-mode");

    public KSPGameMode GameMode => KSPGameMode.Unknown;

    public KSPConsoleBuffer ConsoleBuffer { get; } = new(50, 80);

    public TimeSeriesCollection TimeSeriesCollection { get; } = new();

    public double UniversalTime => 0;

    public VesselComponent? ActiveVessel => null;

    public KSPOrbitModule.IBody? FindBody(string name) {
        return Bodies.Get(name);
    }

    public object? NextYield { get; set; }

    public Action? OnNextYieldOnce { get; set; }

    public void AddMarker(IMarker marker) {
    }

    public void RemoveMarker(IMarker marker) {
    }

    public void ClearMarkers() {
    }

    public void AddResourceTransfer(KSPResourceModule.ResourceTransfer resourceTransfer) {
    }

    public void AddWindow(KSPUIModule.Window window) {
    }

    public bool TryFindAutopilot<T>(VesselComponent vessel, [MaybeNullWhen(false)] out T autopilot) where T : IKSPAutopilot {
        autopilot = default;
        return false;
    }

    public void HookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
    }

    public void UnhookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
    }

    public void UnhookAllAutopilots(VesselComponent vessel) {
    }

    public OptionalAddons OptionalAddons => new();

    public Font? ConsoleFont(int fontSize) {
        return null;
    }
}
