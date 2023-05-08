using System;
using System.Reflection;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPAddons {
    public partial class KSPAddonsModule {
        [KSClass("FlightPlan")]
        public class FlightPlanAdapter {
            public const string ModGuid = "com.github.schlosrat.flight_plan";

            private readonly object instance;
            private readonly Version version;
            private readonly MethodInfo circularize;
            private readonly MethodInfo setNewPe;
            private readonly MethodInfo setNewAp;
            private readonly MethodInfo ellipticize;
            private readonly MethodInfo setInclination;
            private readonly MethodInfo setNewLAN;
            private readonly MethodInfo setNodeLongitude;
            private readonly MethodInfo setNewSMA;
            private readonly MethodInfo matchPlanes;
            private readonly MethodInfo planetaryXfer;
            private readonly MethodInfo interceptTgt;
            private readonly MethodInfo hohmannTransfer;
            private readonly MethodInfo courseCorrection;
            private readonly MethodInfo moonReturn;
            private readonly MethodInfo matchVelocity;

            internal FlightPlanAdapter(object instance, Version version) {
                this.instance = instance;
                this.version = version;

                var type = this.instance.GetType();

                if (version >= new Version("0.8.1")) {
                    circularize = type.GetMethod("Circularize");
                    setNewPe = type.GetMethod("SetNewPe");
                    setNewAp = type.GetMethod("SetNewAp");
                    ellipticize = type.GetMethod("Ellipticize");
                    setInclination = type.GetMethod("SetInclination");
                    setNewLAN = type.GetMethod("SetNewLAN");
                    setNodeLongitude = type.GetMethod("SetNodeLongitude");
                    setNewSMA = type.GetMethod("SetNewSMA");
                    matchPlanes = type.GetMethod("MatchPlanes");
                    hohmannTransfer = type.GetMethod("HohmannTransfer");
                    interceptTgt = type.GetMethod("InterceptTgt");
                    courseCorrection = type.GetMethod("CourseCorrection");
                    moonReturn = type.GetMethod("MoonReturn");
                    matchVelocity = type.GetMethod("MatchVelocity");
                    planetaryXfer = type.GetMethod("PlanetaryXfer");
                }
            }

            [KSField]
            public string Version {
                get => version.ToString();
            }

            [KSMethod]
            public bool Circularize(double burnUt, double burnOffsetFactor = -0.5) {
                if (circularize != null)
                    return (bool)circularize.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetNewPe(double burnUt, double newPe, double burnOffsetFactor = -0.5) {
                if (setNewPe != null)
                    return (bool)setNewPe.Invoke(instance, new object[] { burnUt, newPe, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetNewAp(double burnUt, double newAp, double burnOffsetFactor = -0.5) {
                if (setNewAp != null)
                    return (bool)setNewAp.Invoke(instance, new object[] { burnUt, newAp, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool Ellipticize(double burnUt, double newAp, double newPe, double burnOffsetFactor = -0.5) {
                if (ellipticize != null)
                    return (bool)ellipticize.Invoke(instance, new object[] { burnUt, newAp, newPe, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetInclination(double burnUt, double inclination, double burnOffsetFactor = -0.5) {
                if (setInclination != null)
                    return (bool)setInclination.Invoke(instance, new object[] { burnUt, inclination, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetNewLan(double burnUt, double newLanValue, double burnOffsetFactor = -0.5) {
                if (setNewLAN != null)
                    return (bool)setNewLAN.Invoke(instance, new object[] { burnUt, newLanValue, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetNodeLongitude(double burnUt, double newNodeLongValue, double burnOffsetFactor = -0.5) {
                if (setNodeLongitude != null)
                    return (bool)setNodeLongitude.Invoke(instance, new object[] { burnUt, newNodeLongValue, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool SetNewSma(double burnUt, double newSma, double burnOffsetFactor = -0.5) {
                if (setNewSMA != null)
                    return (bool)setNewSMA.Invoke(instance, new object[] { burnUt, newSma, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool MatchPlanes(double burnUt, double burnOffsetFactor = -0.5) {
                if (matchPlanes != null)
                    return (bool)matchPlanes.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool HohmannTransfer(double burnUt, double burnOffsetFactor = -0.5) {
                if (hohmannTransfer != null)
                    return (bool)hohmannTransfer.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool InterceptTgt(double burnUt, double tgtUt, double burnOffsetFactor = -0.5) {
                if (interceptTgt != null)
                    return (bool)interceptTgt.Invoke(instance, new object[] { burnUt, tgtUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool CourseCorrection(double burnUt, double burnOffsetFactor = -0.5) {
                if (courseCorrection != null)
                    return (bool)interceptTgt.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool MoonReturn(double burnUt, double burnOffsetFactor = -0.5) {
                if (moonReturn != null)
                    return (bool)moonReturn.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool MatchVelocity(double burnUt, double burnOffsetFactor = -0.5) {
                if (matchVelocity != null)
                    return (bool)matchVelocity.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }

            [KSMethod]
            public bool PlanetaryXfer(double burnUt, double burnOffsetFactor = -0.5) {
                if (planetaryXfer != null)
                    return (bool)planetaryXfer.Invoke(instance, new object[] { burnUt, burnOffsetFactor });
                return false;
            }
        }
    }
}
