using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime;

public delegate IAnyFuture Entrypoint(VesselComponent vessel, object[]? args = null);

public class EntrypointArgumentDescriptor {
    public EntrypointArgumentDescriptor(string name, RealizedType type, object defaultValue) {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public RealizedType Type { get; }
    public object DefaultValue { get; }
}

public static class KontrolModuleExtensions {
    private const string MainKsc = "main_ksc";
    private const string MainEditor = "main_editor";
    private const string MainTracking = "main_tracking";
    private const string MainFlight = "main_flight";

    private static Entrypoint? GetEntrypoint(IKontrolModule module, string name, IKSPContext context) {
        try {
            var function = module.FindFunction(name);
            if (function == null || !function.IsAsync) return null;

            if (function.Parameters.Count == 0) return (_, _arg) => (IAnyFuture)function.Invoke(context);

            if (function.Parameters.Count == 1 && function.Parameters[0].type.Name == "ksp::vessel::Vessel")
                return (vessel, _) =>
                    (IAnyFuture)function.Invoke(context, new KSPVesselModule.VesselAdapter(context, vessel));

            if (function.Parameters.Count > 1 && function.Parameters[0].type.Name == "ksp::vessel::Vessel")
                return (vessel, args) => {
                    var vesselAdapter = new KSPVesselModule.VesselAdapter(context, vessel);

                    if (args == null) {
                        context.Logger.Error("Calling without extra args");
                        return (IAnyFuture)function.Invoke(context, vesselAdapter);
                    }

                    context.Logger.Error($"Calling with {args.Length} extra args");
                    return (IAnyFuture)function.Invoke(context, args.Prepend(vesselAdapter).ToArray());
                };

            context.Logger.Error($"GetEntrypoint {name} failed: Invalid parameters {function.Parameters}");
            return null;
        } catch (Exception e) {
            context.Logger.Error($"GetEntrypoint {name} failed: {e}");
            return null;
        }
    }

    public static EntrypointArgumentDescriptor[] GetEntrypointParameterDescriptors(this IKontrolModule module,
        KSPGameMode gameMode, ITO2Logger? logger = null) {
        try {
            var name = GetEntrypointFunctionName(gameMode);
            var function = name != null ? module.FindFunction(name) : null;
            if (function == null || function.Parameters.Count <= 1)
                throw new Exception($"Function {name} does not exist or does not have any parameters");

            return function.Parameters.Skip(1).Select(param => {
                if (param.type == BuiltinType.Int) {
                    var defaultValue = param.defaultValue as IntDefaultValue;
                    return new EntrypointArgumentDescriptor(param.name, param.type, defaultValue?.Value ?? 0);
                }

                if (param.type == BuiltinType.Float) {
                    var defaultValue = param.defaultValue as FloatDefaultValue;
                    return new EntrypointArgumentDescriptor(param.name, param.type, defaultValue?.Value ?? 0.0);
                }

                if (param.type == BuiltinType.Bool) {
                    var defaultValue = param.defaultValue as BoolDefaultValue;
                    return new EntrypointArgumentDescriptor(param.name, param.type, defaultValue?.Value ?? false);
                }

                if (param.type == BuiltinType.String) {
                    var defaultValue = param.defaultValue as StringDefaultValue;
                    return new EntrypointArgumentDescriptor(param.name, param.type,
                        defaultValue?.Value ?? string.Empty);
                }

                throw new Exception($"Parameter {param.name} unsupported type {param.type.Name}");
            }).ToArray();
        } catch (Exception e) {
            logger?.Error($"GetEntrypointParameterDescriptors failed: {e}");
        }

        return Array.Empty<EntrypointArgumentDescriptor>();
    }

    public static int GetEntrypointArgumentCount(this IKontrolModule module, KSPGameMode gameMode) {
        var name = GetEntrypointFunctionName(gameMode);
        var function = name != null ? module.FindFunction(name) : null;
        return function?.Parameters.Count ?? 0;
    }

    private static string? GetEntrypointFunctionName(KSPGameMode gameMode) {
        return gameMode switch {
            KSPGameMode.VAB => MainEditor,
            KSPGameMode.Tracking => MainTracking,
            KSPGameMode.KSC => MainKsc,
            KSPGameMode.Flight => MainFlight,
            _ => null
        };
    }

    private static bool HasEntrypoint(IKontrolModule module, string name, bool allowVessel) {
        var function = module.FindFunction(name);
        if (function == null || !function.IsAsync) return false;
        if (function.Parameters.Count == 0) return true;
        if (allowVessel && function.Parameters.Count > 0 &&
            function.Parameters[0].type.Name == "ksp::vessel::Vessel")
            return true;
        return false;
    }

    public static bool HasKSCEntrypoint(this IKontrolModule module) {
        return HasEntrypoint(module, MainKsc, false);
    }

    public static Entrypoint? GetKSCEntrypoint(this IKontrolModule module, IKSPContext context) {
        return GetEntrypoint(module, MainKsc, context);
    }

    public static bool HasEditorEntrypoint(this IKontrolModule module) {
        return HasEntrypoint(module, MainEditor, false);
    }

    public static Entrypoint? GetEditorEntrypoint(this IKontrolModule module, IKSPContext context) {
        return GetEntrypoint(module, MainEditor, context);
    }

    public static bool HasTrackingEntrypoint(this IKontrolModule module) {
        return HasEntrypoint(module, MainTracking, false);
    }

    public static Entrypoint? GetTrackingEntrypoint(this IKontrolModule module, IKSPContext context) {
        return GetEntrypoint(module, MainTracking, context);
    }

    public static bool HasFlightEntrypoint(this IKontrolModule module) {
        return HasEntrypoint(module, MainFlight, true);
    }

    public static Entrypoint? GetFlightEntrypoint(this IKontrolModule module, IKSPContext context) {
        return GetEntrypoint(module, MainFlight, context);
    }

    public static bool IsBootFlightEntrypointFor(this IKontrolModule module, VesselComponent? vessel) {
        return vessel != null && module.Name.ToLowerInvariant() ==
               "boot::vessels::" + vessel.Name.ToLowerInvariant().Replace(' ', '_') &&
               HasEntrypoint(module, MainFlight, true);
    }
}
