﻿using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Autopilot")]
        public class AutopilotAdapter {
            private readonly VesselAdapter vesselAdapter;

            public AutopilotAdapter(VesselAdapter vesselAdapter) => this.vesselAdapter = vesselAdapter;

            [KSField]
            public bool Enabled {
                get => vesselAdapter.vessel.AutopilotStatus.IsEnabled;
                set {
                    if (value && !vesselAdapter.vessel.AutopilotStatus.IsEnabled) {
                        vesselAdapter.vessel.SetAutopilotEnableDisable(true);
                    } else if (!value && vesselAdapter.vessel.AutopilotStatus.IsEnabled) {
                        vesselAdapter.vessel.SetAutopilotEnableDisable(false);
                    }
                }
            }

            [KSField]
            public AutopilotMode Mode {
                get => vesselAdapter.vessel.AutopilotStatus.Mode;
                set => vesselAdapter.vessel.SetAutopilotMode(value);
            }

            [KSField]
            public Vector3d TargetOrientation {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? vesselAdapter.vessel.mainBody.coordinateSystem
                        .ToLocalVector(sas.ReferenceFrame, sas.TargetOrientation) : vesselAdapter.Facing.Vector;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.SetTargetOrientation(new Vector(vesselAdapter.vessel.mainBody.coordinateSystem, value), false);
            }

            [KSField]
            public Vector GlobalTargetOrientation {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? new Vector(sas.ReferenceFrame, sas.TargetOrientation) : vesselAdapter.GlobalFacing.Vector;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.SetTargetOrientation(value, false);
            }

            [KSField]
            public Direction LockDirection {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? new Direction(vesselAdapter.vessel.mainBody.coordinateSystem
                        .ToLocalRotation(sas.ReferenceFrame, sas.LockedRotation)) : vesselAdapter.Facing;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.LockRotation(value.Rotation);
            }

            [KSField]
            public RotationWrapper GlobalLockDirection {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? new RotationWrapper(new Rotation(sas.ReferenceFrame, sas.LockedRotation)) : vesselAdapter.GlobalFacing;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.LockRotation(value.Rotation);
            }
        }
    }
}
