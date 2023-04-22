using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPDebug;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        // For the most part this is a rip-off from KOS
        // ... maybe outdated now since it is possible to set target orientation on SAS itself
        //[KSClass("SteeringManager")]
        public class SteeringManagerLegacy : IKSPAutopilot {
            private const double Epsilon = 1e-16;
            private const double RenderMultiplier = 50;

            private readonly IKSPContext context;
            private readonly KSPVesselModule.VesselAdapter vessel;
            private Func<Direction> directionProvider;

            private readonly MovingAverage pitchTorqueCalc = new MovingAverage { SampleLimit = 15 };
            private readonly MovingAverage yawTorqueCalc = new MovingAverage { SampleLimit = 15 };
            private readonly MovingAverage rollTorqueCalc = new MovingAverage { SampleLimit = 15 };
            private double sampleTime = 0;

            [KSField] public bool ShowFacingVectors { get; set; }

            [KSField] public bool ShowAngularVectors { get; set; }

            [KSField] public bool ShowSteeringStats { get; set; }

            [KSField] public double PitchTorqueAdjust { get; set; }
            [KSField] public double YawTorqueAdjust { get; set; }
            [KSField] public double RollTorqueAdjust { get; set; }

            [KSField] public double PitchTorqueFactor { get; set; }
            [KSField] public double YawTorqueFactor { get; set; }
            [KSField] public double RollTorqueFactor { get; set; }
            [KSField] public double MaxStoppingTime { get; set; }
            private double rollControlAngleRange;

            [KSField]
            public double RollControlAngleRange {
                get => rollControlAngleRange;
                set => rollControlAngleRange = Math.Max(Epsilon, Math.Min(180, value));
            }

            [KSField] private bool EnableTorqueAdjust { get; set; }

            private readonly TorquePI pitchPI = new TorquePI();
            private readonly TorquePI yawPI = new TorquePI();
            private readonly TorquePI rollPI = new TorquePI();

            private readonly PidLoop pitchRatePI = new PidLoop(1, 0.1, 0, extraUnwind: true);
            private readonly PidLoop yawRatePI = new PidLoop(1, 0.1, 0, extraUnwind: true);
            private readonly PidLoop rollRatePI = new PidLoop(1, 0.1, 0, extraUnwind: true);

            private double accPitch;
            private double accYaw;
            private double accRoll;

            private double phi;
            private double phiPitch;
            private double phiYaw;
            private double phiRoll;

            private double maxPitchOmega;
            private double maxYawOmega;
            private double maxRollOmega;

            private double tgtPitchOmega;
            private double tgtYawOmega;
            private double tgtRollOmega;

            private double tgtPitchTorque;
            private double tgtYawTorque;
            private double tgtRollTorque;

            private Quaternion vesselRotation;
            private Quaternion targetRot;

            private Vector3d centerOfMass;

            private Vector3d vesselForward;
            private Vector3d vesselTop;
            private Vector3d vesselStarboard;

            private Vector3d targetForward;
            private Vector3d targetTop;
            private Vector3d targetStarboard;
            private Vector3d adjustTorque;
            private Vector3d omega = Vector3d.zero; // x: pitch, y: yaw, z: roll
            private Vector3d angularAcceleration = Vector3d.zero;
            private Vector3d momentOfInertia = Vector3d.zero; // x: pitch, z: yaw, y: roll
            private Vector3d measuredTorque = Vector3d.zero;
            private Vector3d controlTorque = Vector3d.zero; // x: pitch, z: yaw, y: roll
            private Vector3d rawTorque = Vector3d.zero; // x: pitch, z: yaw, y: roll

            private int vesselParts;

            private readonly Dictionary<PartComponentModule, ITorqueProvider> torqueProviders =
                new Dictionary<PartComponentModule, ITorqueProvider>();

            private ITransformModel vesselTransform;

            /*
            private KSPDebugModule.VectorRenderer vForward;
            private KSPDebugModule.VectorRenderer vTop;
            private KSPDebugModule.VectorRenderer vStarboard;

            private KSPDebugModule.VectorRenderer vTgtForward;
            private KSPDebugModule.VectorRenderer vTgtTop;
            private KSPDebugModule.VectorRenderer vTgtStarboard;

            private KSPDebugModule.VectorRenderer vWorldX;
            private KSPDebugModule.VectorRenderer vWorldY;
            private KSPDebugModule.VectorRenderer vWorldZ;

            private KSPDebugModule.VectorRenderer vOmegaX;
            private KSPDebugModule.VectorRenderer vOmegaY;
            private KSPDebugModule.VectorRenderer vOmegaZ;

            private KSPDebugModule.VectorRenderer vTgtTorqueX;
            private KSPDebugModule.VectorRenderer vTgtTorqueY;
            private KSPDebugModule.VectorRenderer vTgtTorqueZ;
            */

            public SteeringManagerLegacy(IKSPContext context, KSPVesselModule.VesselAdapter vessel,
                Func<Direction> directionProvider) {
                this.context = context;
                this.vessel = vessel;
                this.directionProvider = directionProvider;
                ShowFacingVectors = false;
                ShowAngularVectors = false;
                ShowSteeringStats = false;
                pitchPI.Ts = 2; // TODO: Make the tweakable
                yawPI.Ts = 2; // TODO: Make the tweakable
                rollPI.Ts = 2; // TODO: Make the tweakable

                ResetToDefault();

                this.context.HookAutopilot(this.vessel.vessel, this);
            }

            [KSField]
            public double YawTs {
                get => yawPI.Ts;
                set => yawPI.Ts = value;
            }

            [KSField]
            public double PitchTs {
                get => pitchPI.Ts;
                set => pitchPI.Ts = value;
            }

            [KSField]
            public double RollTs {
                get => rollPI.Ts;
                set => rollPI.Ts = value;
            }

            [KSField]
            public Direction Direction {
                get => directionProvider();
                set => directionProvider = () => value;
            }

            [KSMethod]
            public void SetDirectionProvider(Func<Direction> newDirectionProvider) =>
                directionProvider = newDirectionProvider;

            [KSMethod]
            public Future<object> Release() {
                context.UnhookAutopilot(vessel.vessel, this);

                return new Future.Success<object>(null);
            }

            [KSMethod]
            public void Resume() => context.HookAutopilot(this.vessel.vessel, this);

            [KSMethod]
            public void ResetToDefault() {
                pitchPI.Ts = 2; // TODO: Make the tweakable
                yawPI.Ts = 2; // TODO: Make the tweakable
                rollPI.Ts = 2; // TODO: Make the tweakable
                // only neet to reset the PI's I value, other values are not accessible to users to modify
                pitchPI.ResetI();
                yawPI.ResetI();
                rollPI.ResetI();

                pitchRatePI.Kp = 1;
                pitchRatePI.Ki = 0.1;
                pitchRatePI.Kd = 0;
                yawRatePI.Kp = 1;
                yawRatePI.Ki = 0.1;
                yawRatePI.Kd = 0;
                rollRatePI.Kp = 1;
                rollRatePI.Ki = 0.1;
                rollRatePI.Kd = 0;

                adjustTorque = Vector3d.zero;

                EnableTorqueAdjust = false;

                MaxStoppingTime = 2;
                RollControlAngleRange = 5;

                PitchTorqueAdjust = 0;
                YawTorqueAdjust = 0;
                RollTorqueAdjust = 0;

                PitchTorqueFactor = 1;
                YawTorqueFactor = 1;
                RollTorqueFactor = 1;
            }

            private void ResetIs() {
                pitchPI.ResetI();
                yawPI.ResetI();
                rollPI.ResetI();
                pitchRatePI.ResetI();
                yawRatePI.ResetI();
                rollRatePI.ResetI();
            }

            public void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
                if (deltaT > 1) ResetIs();
                if (deltaT > 0) {
                    sampleTime += deltaT;
                }
                if (vessel.vessel.Autopilot.Enabled) {
                    UpdateStateVectors(deltaT);
                    UpdateControl(c);
                    UpdateVectorRenders();
                } else {
                    UpdateStateVectors(deltaT);
                    UpdateControlParts();
                    UpdateTorque();
                    UpdatePredictionPI();
                    UpdateControl(c);
                    if (ShowSteeringStats) PrintDebug();
                    UpdateVectorRenders();
                }
            }

            public void UpdateStateVectors(float detlaT) {
                targetRot = directionProvider().Rotation;
                centerOfMass = vessel.CoM;

                vesselTransform = vessel.vessel.transform;
                // Found that the default rotation has top pointing forward, forward pointing down, and right pointing starboard.
                // This fixes that rotation.
                vesselRotation = vesselTransform.localRotation * QuaternionD.Euler(-90, 0, 0);

                vesselForward = vesselRotation * Vector3d.forward;
                vesselTop = vesselRotation * Vector3d.up;
                vesselStarboard = vesselRotation * Vector3d.right;

                targetForward = targetRot * Vector3d.forward;
                targetTop = targetRot * Vector3d.up;
                targetStarboard = targetRot * Vector3d.right;

                Vector3d oldOmega = omega;
                // omega is angular rotation.  need to correct the signs to agree with the facing axis
                omega = Quaternion.Inverse(vesselRotation) *
                        vessel.vessel.AngularVelocity.relativeAngularVelocity.vector;
                omega.x *= -1; //positive values pull the nose to the starboard.
                //omega.y *= -1; // positive values pull the nose up.
                omega.z *= -1; // positive values pull the starboard side up.

                // TODO: Currently adjustments to MOI are only enabled in debug compiles.  Using this feature seems to be buggy, but it has potential
                // to be more resilient against random spikes in angular velocity.
                if (detlaT > 0) {
                    double dt = detlaT;
                    angularAcceleration = (omega - oldOmega) / dt;
                    angularAcceleration =
                        new Vector3d(angularAcceleration.x, angularAcceleration.z, angularAcceleration.y);
                }

                // TODO: If stock vessel.MOI stops being so weird, we might be able to change the following line
                // into this instead.  (See the comment on FindMOI()'s header):
                //      momentOfInertia = shared.Vessel.MOI;
                momentOfInertia = FindMoI();

                adjustTorque = Vector3d.zero;
                measuredTorque = Vector3d.Scale(momentOfInertia, angularAcceleration);

                //                Debug.LogError("Sample time: " + sampletime);
                if (detlaT > 0 && EnableTorqueAdjust) {
                    if (Math.Abs(accPitch) > Epsilon) {
                        adjustTorque.x =
                            Math.Min(
                                Math.Abs(pitchTorqueCalc.Update(sampleTime, measuredTorque.x / accPitch)) - rawTorque.x,
                                0);
                        //adjustTorque.x = Math.Abs(pitchTorqueCalc.Update(measuredTorque.x / accPitch) / rawTorque.x);
                    } else adjustTorque.x = Math.Abs(pitchTorqueCalc.Update(sampleTime, pitchTorqueCalc.Mean));

                    if (Math.Abs(accYaw) > Epsilon) {
                        adjustTorque.z =
                            Math.Min(
                                Math.Abs(yawTorqueCalc.Update(sampleTime, measuredTorque.z / accYaw)) - rawTorque.z, 0);
                        //adjustTorque.z = Math.Abs(yawTorqueCalc.Update(measuredTorque.z / accYaw) / rawTorque.z);
                    } else adjustTorque.z = Math.Abs(yawTorqueCalc.Update(sampleTime, yawTorqueCalc.Mean));

                    if (Math.Abs(accRoll) > Epsilon) {
                        adjustTorque.y =
                            Math.Min(
                                Math.Abs(rollTorqueCalc.Update(sampleTime, measuredTorque.y / accRoll)) - rawTorque.y,
                                0);
                        //adjustTorque.y = Math.Abs(rollTorqueCalc.Update(measuredTorque.y / accRoll) / rawTorque.y);
                    } else adjustTorque.y = Math.Abs(rollTorqueCalc.Update(sampleTime, rollTorqueCalc.Mean));
                }
            }

            public void UpdateControlParts() {
                if (vessel.vessel.SimulationObject.PartOwner.PartCount != vesselParts) {
                    vesselParts = vessel.vessel.SimulationObject.PartOwner.PartCount;
                    torqueProviders.Clear();
                    foreach (PartComponent part in vessel.vessel.SimulationObject.PartOwner.Parts) {
                        foreach (PartComponentModule pm in part.Modules.Values) {
                            ITorqueProvider tp = pm as ITorqueProvider;
                            if (tp != null) {
                                torqueProviders.Add(pm, tp);
                            }
                        }
                    }
                }
            }

            public void UpdateTorque() {
                // controlTorque is the maximum amount of torque applied by setting a control to 1.0.
                controlTorque.Zero();
                rawTorque.Zero();

                Vector3 pos;
                Vector3 neg;
                foreach (var pm in torqueProviders.Keys) {
                    var tp = torqueProviders[pm];
                    tp.GetPotentialTorque(out pos, out neg);
                    // It is possible for the torque returned to be negative.  It's also possible
                    // for the positive and negative actuation to differ.  Below averages the value
                    // for positive and negative actuation in an attempt to compensate for some issues
                    // of differing signs and asymmetric torque.
                    rawTorque.x += (Math.Abs(pos.x) + Math.Abs(neg.x)) / 2;
                    rawTorque.y += (Math.Abs(pos.y) + Math.Abs(neg.y)) / 2;
                    rawTorque.z += (Math.Abs(pos.z) + Math.Abs(neg.z)) / 2;
                }

                rawTorque.x = (rawTorque.x + PitchTorqueAdjust) * PitchTorqueFactor;
                rawTorque.z = (rawTorque.z + YawTorqueAdjust) * YawTorqueFactor;
                rawTorque.y = (rawTorque.y + RollTorqueAdjust) * RollTorqueFactor;

                controlTorque = rawTorque + adjustTorque;
                //controlTorque = Vector3d.Scale(rawTorque, adjustTorque);
                //controlTorque = rawTorque;

                double minTorque = Epsilon;
                if (controlTorque.x < minTorque) controlTorque.x = minTorque;
                if (controlTorque.y < minTorque) controlTorque.y = minTorque;
                if (controlTorque.z < minTorque) controlTorque.z = minTorque;
            }

            /// <summary>
            /// This is a replacement for the stock API Property "vessel.MOI", which seems buggy when used
            /// with "control from here" on parts other than the default control part.
            /// <br/>
            /// Right now the stock Moment of Inertia Property returns values in inconsistent reference frames that
            /// don't make sense when used with "control from here".  (It doesn't merely rotate the reference frame, as one
            /// would expect "control from here" to do.)
            /// </summary>   
            /// TODO: Check this again after each KSP stock release to see if it's been changed or not.
            public Vector3 FindMoI() {
                var tensor = Matrix4x4.zero;
                Matrix4x4 partTensor = Matrix4x4.identity;
                Matrix4x4 inertiaMatrix = Matrix4x4.identity;
                Matrix4x4 productMatrix = Matrix4x4.identity;
                foreach (var part in vessel.vessel.SimulationObject.PartOwner.Parts) {
                    if (part.SimulationObject.Rigidbody != null) {
                        KSPUtil.ToDiagonalMatrix2(part.SimulationObject.Rigidbody.inertiaTensor.vector, ref partTensor);

                        Quaternion rot = QuaternionD.Inverse(vesselRotation) * part.transform.localRotation *
                                         part.SimulationObject.Rigidbody.inertiaTensorRotation.localRotation;
                        Quaternion inv = Quaternion.Inverse(rot);

                        Matrix4x4 rotMatrix = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
                        Matrix4x4 invMatrix = Matrix4x4.TRS(Vector3.zero, inv, Vector3.one);

                        // add the part inertiaTensor to the ship inertiaTensor
                        KSPUtil.Add(ref tensor, rotMatrix * partTensor * invMatrix);

                        Vector3 position = QuaternionD.Inverse(vesselTransform.localRotation) * (part.SimulationObject.Rigidbody.Position.localPosition - centerOfMass);

                        // add the part mass to the ship inertiaTensor
                        KSPUtil.ToDiagonalMatrix2(part.SimulationObject.Rigidbody.mass * position.sqrMagnitude, ref inertiaMatrix);
                        KSPUtil.Add(ref tensor, inertiaMatrix);

                        // add the part distance offset to the ship inertiaTensor
                        OuterProduct2(position, -part.SimulationObject.Rigidbody.mass * position, ref productMatrix);
                        KSPUtil.Add(ref tensor, productMatrix);
                    }
                }

                return KSPUtil.Diag(tensor);
            }

            /// <summary>
            /// Construct the outer product of two 3-vectors as a 4x4 matrix
            /// DOES NOT ZERO ANY THINGS WOT ARE ZERO OR IDENTITY INNIT
            /// </summary>
            public static void OuterProduct2(Vector3 left, Vector3 right, ref Matrix4x4 m) {
                m.m00 = left.x * right.x;
                m.m01 = left.x * right.y;
                m.m02 = left.x * right.z;
                m.m10 = left.y * right.x;
                m.m11 = left.y * right.y;
                m.m12 = left.y * right.z;
                m.m20 = left.z * right.x;
                m.m21 = left.z * right.y;
                m.m22 = left.z * right.z;
            }

            // Update prediction based on PI controls, sets the target angular velocity and the target torque for the vessel
            public void UpdatePredictionPI() {
                // calculate phi and pitch, yaw, roll components of phi (angular error)
                phi = Vector3d.Angle(vesselForward, targetForward) / DirectBindingMath.RadToDeg;
                if (Vector3d.Angle(vesselTop, targetForward) > 90)
                    phi *= -1;
                phiPitch = Vector3d.Angle(vesselForward, Vector3d.Exclude(vesselStarboard, targetForward)) /
                           DirectBindingMath.RadToDeg;
                if (Vector3d.Angle(vesselTop, Vector3d.Exclude(vesselStarboard, targetForward)) > 90)
                    phiPitch *= -1;
                phiYaw = Vector3d.Angle(vesselForward, Vector3d.Exclude(vesselTop, targetForward)) /
                         DirectBindingMath.RadToDeg;
                if (Vector3d.Angle(vesselStarboard, Vector3d.Exclude(vesselTop, targetForward)) > 90)
                    phiYaw *= -1;
                phiRoll = Vector3d.Angle(vesselTop, Vector3d.Exclude(vesselForward, targetTop)) /
                          DirectBindingMath.RadToDeg;
                if (Vector3d.Angle(vesselStarboard, Vector3d.Exclude(vesselForward, targetTop)) > 90)
                    phiRoll *= -1;

                // Calculate the maximum allowable angular velocity and apply the limit, something we can stop in a reasonable amount of time
                maxPitchOmega = controlTorque.x * MaxStoppingTime / momentOfInertia.x;
                maxYawOmega = controlTorque.z * MaxStoppingTime / momentOfInertia.z;
                maxRollOmega = controlTorque.y * MaxStoppingTime / momentOfInertia.y;

                // Because the value of phi is already error, we say the input is -error and the setpoint is 0 so the PID has the correct sign
                tgtPitchOmega = pitchRatePI.Update(sampleTime, -phiPitch, 0, maxPitchOmega);
                tgtYawOmega = yawRatePI.Update(sampleTime, -phiYaw, 0, maxYawOmega);
                if (Math.Abs(phi) > RollControlAngleRange * Math.PI / 180d) {
                    tgtRollOmega = 0;
                    rollRatePI.ResetI();
                } else {
                    tgtRollOmega = rollRatePI.Update(sampleTime, -phiRoll, 0, maxRollOmega);
                }

                // Calculate target torque based on PID
                tgtPitchTorque = pitchPI.Update(sampleTime, omega.x, tgtPitchOmega, momentOfInertia.x, controlTorque.x);
                tgtYawTorque = yawPI.Update(sampleTime, omega.y, tgtYawOmega, momentOfInertia.z, controlTorque.z);
                tgtRollTorque = rollPI.Update(sampleTime, omega.z, tgtRollOmega, momentOfInertia.y, controlTorque.y);
            }

            public void UpdateControl(FlightCtrlState c) {
                if (vessel.vessel.Autopilot.Enabled) {
                    pitchPI.ResetI();
                    yawPI.ResetI();
                    rollPI.ResetI();
                    pitchRatePI.ResetI();
                    yawRatePI.ResetI();
                    rollRatePI.ResetI();
                    Quaternion target = targetRot * Quaternion.Euler(90, 0, 0);
                    vessel.vessel.Autopilot.SAS.LockRotation(target);
                } else {
                    //TODO: include adjustment for static torque (due to engines)
                    double clampAccPitch = Math.Max(Math.Abs(accPitch), 0.005) * 2;
                    accPitch = tgtPitchTorque / controlTorque.x;
                    if (Math.Abs(accPitch) < Epsilon)
                        accPitch = 0;
                    accPitch = Math.Max(Math.Min(accPitch, clampAccPitch), -clampAccPitch);
                    c.pitch = (float)accPitch;
                    double clampAccYaw = Math.Max(Math.Abs(accYaw), 0.005) * 2;
                    accYaw = tgtYawTorque / controlTorque.z;
                    if (Math.Abs(accYaw) < Epsilon)
                        accYaw = 0;
                    accYaw = Math.Max(Math.Min(accYaw, clampAccYaw), -clampAccYaw);
                    c.yaw = (float)accYaw;
                    double clampAccRoll = Math.Max(Math.Abs(accRoll), 0.005) * 2;
                    accRoll = tgtRollTorque / controlTorque.y;
                    if (Math.Abs(accRoll) < Epsilon)
                        accRoll = 0;
                    accRoll = Math.Max(Math.Min(accRoll, clampAccRoll), -clampAccRoll);
                    c.roll = (float)accRoll;
                }
            }

            public void UpdateVectorRenders() {
                /*
                if (ShowFacingVectors) {
                    if (vForward == null) {
                        vForward = InitVectorRenderer(Color.red, 1);
                    }

                    if (vTop == null) {
                        vTop = InitVectorRenderer(Color.red, 1);
                    }

                    if (vStarboard == null) {
                        vStarboard = InitVectorRenderer(Color.red, 1);
                    }

                    vForward.End = vesselForward * RenderMultiplier;
                    vTop.End = vesselTop * RenderMultiplier;
                    vStarboard.End = vesselStarboard * RenderMultiplier;

                    if (vTgtForward == null) {
                        vTgtForward = InitVectorRenderer(Color.blue, 1);
                    }

                    if (vTgtTop == null) {
                        vTgtTop = InitVectorRenderer(Color.blue, 1);
                    }

                    if (vTgtStarboard == null) {
                        vTgtStarboard = InitVectorRenderer(Color.blue, 1);
                    }

                    vTgtForward.End = targetForward * RenderMultiplier * 0.75f;
                    vTgtTop.End = targetTop * RenderMultiplier * 0.75f;
                    vTgtStarboard.End = targetStarboard * RenderMultiplier * 0.75f;

                    if (vWorldX == null) {
                        vWorldX = InitVectorRenderer(Color.white, 1);
                    }

                    if (vWorldY == null) {
                        vWorldY = InitVectorRenderer(Color.white, 1);
                    }

                    if (vWorldZ == null) {
                        vWorldZ = InitVectorRenderer(Color.white, 1);
                    }

                    vWorldX.End = new Vector3d(1, 0, 0) * RenderMultiplier * 2;
                    vWorldY.End = new Vector3d(0, 1, 0) * RenderMultiplier * 2;
                    vWorldZ.End = new Vector3d(0, 0, 1) * RenderMultiplier * 2;

                    if (!vForward.Visible) vForward.Visible = true;
                    if (!vTop.Visible) vTop.Visible = true;
                    if (!vStarboard.Visible) vStarboard.Visible = true;

                    if (!vTgtForward.Visible) vTgtForward.Visible = true;
                    if (!vTgtTop.Visible) vTgtTop.Visible = true;
                    if (!vTgtStarboard.Visible) vTgtStarboard.Visible = true;

                    if (!vWorldX.Visible) vWorldX.Visible = true;
                    if (!vWorldY.Visible) vWorldY.Visible = true;
                    if (!vWorldZ.Visible) vWorldZ.Visible = true;
                } else {
                    if (vForward != null) {
                        if (vForward.Visible) vForward.Visible = false;
                        vForward = null;
                    }

                    if (vTop != null) {
                        if (vTop.Visible) vTop.Visible = false;
                        vTop = null;
                    }

                    if (vStarboard != null) {
                        if (vStarboard.Visible) vStarboard.Visible = false;
                        vStarboard = null;
                    }

                    if (vTgtForward != null) {
                        if (vTgtForward.Visible) vTgtForward.Visible = false;
                        vTgtForward = null;
                    }

                    if (vTgtTop != null) {
                        if (vTgtTop.Visible) vTgtTop.Visible = false;
                        vTgtTop = null;
                    }

                    if (vTgtStarboard != null) {
                        if (vTgtStarboard.Visible) vTgtStarboard.Visible = false;
                        vTgtStarboard = null;
                    }

                    if (vWorldX != null) {
                        if (vWorldX.Visible) vWorldX.Visible = false;
                        vWorldX = null;
                    }

                    if (vWorldY != null) {
                        if (vWorldY.Visible) vWorldY.Visible = false;
                        vWorldY = null;
                    }

                    if (vWorldZ != null) {
                        if (vWorldZ.Visible) vWorldZ.Visible = false;
                        vWorldZ = null;
                    }
                }

                if (ShowAngularVectors && !vessel.vessel.Autopilot.Enabled) {
                    if (vOmegaX == null) {
                        vOmegaX = InitVectorRenderer(Color.cyan, 1);
                    }

                    if (vOmegaY == null) {
                        vOmegaY = InitVectorRenderer(Color.cyan, 1);
                    }

                    if (vOmegaZ == null) {
                        vOmegaZ = InitVectorRenderer(Color.cyan, 1);
                    }

                    vOmegaX.End = vesselTop * omega.x * RenderMultiplier * 100f - vesselForward * RenderMultiplier;
                    vOmegaX.Start = vesselForward * RenderMultiplier;
                    vOmegaY.End = vesselStarboard * omega.y * RenderMultiplier * 100f -
                                  vesselForward * RenderMultiplier;
                    vOmegaY.Start = vesselForward * RenderMultiplier;
                    vOmegaZ.End = vesselStarboard * omega.z * RenderMultiplier * 100f -
                                  vesselForward * RenderMultiplier;
                    vOmegaZ.Start = vesselTop * RenderMultiplier;

                    if (vTgtTorqueX == null) {
                        vTgtTorqueX = InitVectorRenderer(Color.green, 1);
                    }

                    if (vTgtTorqueY == null) {
                        vTgtTorqueY = InitVectorRenderer(Color.green, 1);
                    }

                    if (vTgtTorqueZ == null) {
                        vTgtTorqueZ = InitVectorRenderer(Color.green, 1);
                    }

                    vTgtTorqueX.End = vesselTop * tgtPitchOmega * RenderMultiplier * 100f -
                                      vesselForward * RenderMultiplier;
                    vTgtTorqueX.Start = vesselForward * RenderMultiplier;
                    //vTgtTorqueX.SetLabel("tgtPitchOmega: " + tgtPitchOmega);
                    vTgtTorqueY.End = vesselStarboard * tgtRollOmega * RenderMultiplier * 100f -
                                      vesselForward * RenderMultiplier;
                    vTgtTorqueY.Start = vesselTop * RenderMultiplier;
                    //vTgtTorqueY.SetLabel("tgtRollOmega: " + tgtRollOmega);
                    vTgtTorqueZ.End = vesselStarboard * tgtYawOmega * RenderMultiplier * 100f -
                                      vesselForward * RenderMultiplier;
                    vTgtTorqueZ.Start = vesselForward * RenderMultiplier;
                    //vTgtTorqueZ.SetLabel("tgtYawOmega: " + tgtYawOmega);

                    if (!vOmegaX.Visible) vOmegaX.Visible = true;
                    if (!vOmegaY.Visible) vOmegaY.Visible = true;
                    if (!vOmegaZ.Visible) vOmegaZ.Visible = true;

                    if (!vTgtTorqueX.Visible) vTgtTorqueX.Visible = true;
                    if (!vTgtTorqueY.Visible) vTgtTorqueY.Visible = true;
                    if (!vTgtTorqueZ.Visible) vTgtTorqueZ.Visible = true;
                } else {
                    if (vOmegaX != null) {
                        if (vOmegaX.Visible) vOmegaX.Visible = false;
                        vOmegaX = null;
                    }

                    if (vOmegaY != null) {
                        if (vOmegaY.Visible) vOmegaY.Visible = false;
                        vOmegaY = null;
                    }

                    if (vOmegaZ != null) {
                        if (vOmegaZ.Visible) vOmegaZ.Visible = false;
                        vOmegaZ = null;
                    }

                    if (vTgtTorqueX != null) {
                        if (vTgtTorqueX.Visible) vTgtTorqueX.Visible = false;
                        vTgtTorqueX = null;
                    }

                    if (vTgtTorqueY != null) {
                        if (vTgtTorqueY.Visible) vTgtTorqueY.Visible = false;
                        vTgtTorqueY = null;
                    }

                    if (vTgtTorqueZ != null) {
                        if (vTgtTorqueZ.Visible) vTgtTorqueZ.Visible = false;
                        vTgtTorqueZ = null;
                    }
                }*/
            }

            public void PrintDebug() {
                /*
                shared.Screen.ClearScreen();
                shared.Screen.Print(string.Format("phi: {0}", phi * RadToDeg));
                shared.Screen.Print(string.Format("phiRoll: {0}", phiRoll * RadToDeg));
                shared.Screen.Print("    Pitch Values:");
                shared.Screen.Print(string.Format("phiPitch: {0}", phiPitch * RadToDeg));
                //shared.Screen.Print(string.Format("phiPitch: {0}", deltaRotation.eulerAngles.x));
                shared.Screen.Print(string.Format("I pitch: {0}", momentOfInertia.x));
                shared.Screen.Print(string.Format("torque pitch: {0}", controlTorque.x));
                shared.Screen.Print(string.Format("maxPitchOmega: {0}", maxPitchOmega));
                shared.Screen.Print(string.Format("tgtPitchOmega: {0}", tgtPitchOmega));
                shared.Screen.Print(string.Format("pitchOmega: {0}", omega.x));
                shared.Screen.Print(string.Format("tgtPitchTorque: {0}", tgtPitchTorque));
                shared.Screen.Print(string.Format("accPitch: {0}", accPitch));
                shared.Screen.Print("    Yaw Values:");
                shared.Screen.Print(string.Format("phiYaw: {0}", phiYaw * RadToDeg));
                //shared.Screen.Print(string.Format("phiYaw: {0}", deltaRotation.eulerAngles.y));
                shared.Screen.Print(string.Format("I yaw: {0}", momentOfInertia.z));
                shared.Screen.Print(string.Format("torque yaw: {0}", controlTorque.z));
                shared.Screen.Print(string.Format("maxYawOmega: {0}", maxYawOmega));
                shared.Screen.Print(string.Format("tgtYawOmega: {0}", tgtYawOmega));
                shared.Screen.Print(string.Format("yawOmega: {0}", omega.y));
                shared.Screen.Print(string.Format("tgtYawTorque: {0}", tgtYawTorque));
                shared.Screen.Print(string.Format("accYaw: {0}", accYaw));
                shared.Screen.Print("    Roll Values:");
                shared.Screen.Print(string.Format("phiRoll: {0}", phiRoll * RadToDeg));
                //shared.Screen.Print(string.Format("phiRoll: {0}", deltaRotation.eulerAngles.z));
                shared.Screen.Print(string.Format("I roll: {0}", momentOfInertia.y));
                shared.Screen.Print(string.Format("torque roll: {0}", controlTorque.y));
                shared.Screen.Print(string.Format("maxRollOmega: {0}", maxRollOmega));
                shared.Screen.Print(string.Format("tgtRollOmega: {0}", tgtRollOmega));
                shared.Screen.Print(string.Format("rollOmega: {0}", omega.z));
                shared.Screen.Print(string.Format("tgtRollTorque: {0}", tgtRollTorque));
                shared.Screen.Print(string.Format("accRoll: {0}", accRoll));
                shared.Screen.Print("    Processing Stats:");
                shared.Screen.Print(string.Format("Average Duration: {0}", AverageDuration.Mean));
                shared.Screen.Print(string.Format("Based on count: {0}", AverageDuration.ValueCount));
                */
            }

            /*
            public KSPDebugModule.VectorRenderer InitVectorRenderer(Color c, double width) {
                KSPDebugModule.VectorRenderer renderer = new KSPDebugModule.VectorRenderer(
                    () => Vector3d.zero,
                    () => Vector3d.zero, new KSPConsoleModule.RgbaColor(c.r, c.g, c.b), "", width, true);

                context?.AddMarker(renderer);

                return renderer;
            }
            */
        }
    }
}
