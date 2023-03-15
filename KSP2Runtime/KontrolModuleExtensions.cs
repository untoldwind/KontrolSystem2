using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using static KSP.Api.UIDataPropertyStrings.View;

namespace KontrolSystem.KSP.Runtime {
    public delegate IAnyFuture Entrypoint(VesselComponent vessel, object[] args = null);

    public class EntrypointArgumentDescriptor {
        public string Name { get; }
        public string Type { get; }
        public object DefaultValue { get; }

        public EntrypointArgumentDescriptor(string name, string type, object defaultValue) {
            this.Name = name;
            this.Type = type;
            this.DefaultValue = defaultValue;
        }
    }

    public static class KontrolModuleExtensions {
        private const string MainKsc = "main_ksc";
        private const string MainEditor = "main_editor";
        private const string MainTracking = "main_tracking";
        private const string MainFlight = "main_flight";

        private static Entrypoint GetEntrypoint(IKontrolModule module, string name, IKSPContext context) {
            try {
                IKontrolFunction function = module.FindFunction(name);
                if (function == null || !function.IsAsync) return null;

                if (function.Parameters.Count == 0) {
                    return new Entrypoint((_, _arg) => (IAnyFuture)function.Invoke(context));
                }

                if (function.Parameters.Count == 1 && function.Parameters[0].type.Name == "ksp::vessel::Vessel") {
                    return (vessel, _) =>
                        (IAnyFuture)function.Invoke(context, new KSPVesselModule.VesselAdapter(context, vessel));
                }

                if (function.Parameters.Count > 1 && function.Parameters[0].type.Name == "ksp::vessel::Vessel") {
                    return (vessel, args) => {
                        var vesselAdapter = new KSPVesselModule.VesselAdapter(context, vessel);

                        if (args == null) {
                            context.Logger.Error("Calling without extra args");
                            return (IAnyFuture)function.Invoke(context, vesselAdapter);
                        } else {
                            context.Logger.Error($"Calling with {args.Length} extra args");
                            return (IAnyFuture)function.Invoke(context, args.Prepend(vesselAdapter).ToArray());
                        }
                    };
                }

                context.Logger.Error($"GetEntrypoint {name} failed: Invalid parameters {function.Parameters}");
                return null;
            } catch (Exception e) {
                context.Logger.Error($"GetEntrypoint {name} failed: {e}");
                return null;
            }
        }

        public static EntrypointArgumentDescriptor[] GetFlightEntrypointArgumentDescriptors(this IKontrolModule module, ITO2Logger logger = null) => GetEntrypointParameterDescriptors(module, MainFlight, logger);
        public static EntrypointArgumentDescriptor[] GetKSCEntrypointArgumentDescriptors(this IKontrolModule module, ITO2Logger logger = null) => GetEntrypointParameterDescriptors(module, MainKsc, logger);
        public static EntrypointArgumentDescriptor[] GetTrackingEntrypointArgumentDescriptors(this IKontrolModule module, ITO2Logger logger = null) => GetEntrypointParameterDescriptors(module, MainTracking, logger);
        public static EntrypointArgumentDescriptor[] GetEditorEntrypointArgumentDescriptors(this IKontrolModule module, ITO2Logger logger = null) => GetEntrypointParameterDescriptors(module, MainEditor, logger);

        private static EntrypointArgumentDescriptor[] GetEntrypointParameterDescriptors(IKontrolModule module, string name, ITO2Logger logger = null) {
            try {
                IKontrolFunction function = module.FindFunction(name);
                if (function == null || function.Parameters.Count <= 1) {
                    throw new Exception($"Function {name} does not exist or does not have any parameters");
                }

                return function.Parameters.Skip(1).Select(param => {
                    if (param.type.Name == "int") {
                        var defaultValue = param.defaultValue as IntDefaultValue;
                        if (defaultValue == null) {
                            throw new Exception($"Parameter {param.name} does not have a default value");
                        }
                        return new EntrypointArgumentDescriptor(param.name, param.type.Name, defaultValue.Value);
                    } else if (param.type.Name == "float") {
                        var defaultValue = param.defaultValue as FloatDefaultValue;
                        if (defaultValue == null) {
                            throw new Exception($"Parameter {param.name} does not have a default value");
                        }
                        return new EntrypointArgumentDescriptor(param.name, param.type.Name, defaultValue.Value);
                    } else if (param.type.Name == "bool") {
                        var defaultValue = param.defaultValue as BoolDefaultValue;
                        if (defaultValue == null) {
                            throw new Exception($"Parameter {param.name} does not have a default value");
                        }
                        return new EntrypointArgumentDescriptor(param.name, param.type.Name, defaultValue.Value);
                    } else {
                        throw new Exception($"Parameter {param.name} unsupported type {param.type.Name}");
                    }
                }).ToArray();
            } catch (Exception e) {
                logger?.Error($"GetEntrypointParameterDescriptors failed: {e}");
            }

            return Array.Empty<EntrypointArgumentDescriptor>();
        }

        private static bool HasEntrypoint(IKontrolModule module, string name, bool allowVessel) {
            IKontrolFunction function = module.FindFunction(name);
            if (function == null || !function.IsAsync) {
                return false;
            }
            if (function.Parameters.Count == 0) {
                return true;
            }
            if (allowVessel && function.Parameters.Count > 0 &&
                       function.Parameters[0].type.Name == "ksp::vessel::Vessel") {
                return true;
            }
            return false;
        }

        public static bool HasKSCEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainKsc, false);

        public static Entrypoint GetKSCEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainKsc, context);

        public static bool HasEditorEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainEditor, false);

        public static Entrypoint GetEditorEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainEditor, context);

        public static bool HasTrackingEntrypoint(this IKontrolModule module) =>
            HasEntrypoint(module, MainTracking, false);

        public static Entrypoint GetTrackingEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainTracking, context);

        public static bool HasFlightEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainFlight, true);

        public static Entrypoint GetFlightEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainFlight, context);

        public static bool IsBootFlightEntrypointFor(this IKontrolModule module, VesselComponent vessel) =>
            vessel != null && module.Name.ToLowerInvariant() == "boot::vessels::" + vessel.Name.ToLowerInvariant().Replace(' ', '_') &&
            HasEntrypoint(module, MainFlight, true);
    }
}
