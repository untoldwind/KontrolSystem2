using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public class FlightCtrlStateBinding {
    public static readonly RecordStructType FlightCtrlStateType = new("ksp::vessel",
        "FlightCtrlState",
        "Current state of the (pilots) flight controls.", typeof(FlightCtrlState),
        new[] {
            new RecordStructField("main_throttle", "Setting for the main throttle (0 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("mainThrottle")),
            new RecordStructField("x", "Setting for x-translation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("X")),
            new RecordStructField("y", "Setting for y-translation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("Y")),
            new RecordStructField("z", "Setting for z-translation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("Z")),
            new RecordStructField("pitch", "Setting for pitch rotation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("pitch")),
            new RecordStructField("yaw", "Setting for yaw rotation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("yaw")),
            new RecordStructField("roll", "Setting for roll rotation (-1 - 1)", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("roll")),
            new RecordStructField("pitch_trim", "Current trim value for pitch", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("pitchTrim")),
            new RecordStructField("yaw_trim", "Current trim value for yaw", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("yawTrim")),
            new RecordStructField("roll_trim", "Current trim value for roll", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("rollTrim")),
            new RecordStructField("wheel_throttle", "Setting for wheel throttle (0 - 1, applied to Rovers)",
                BuiltinType.Float, typeof(FlightCtrlState).GetField("wheelThrottle")),
            new RecordStructField("wheel_steer", "Setting for wheel steering (-1 - 1, applied to Rovers)",
                BuiltinType.Float, typeof(FlightCtrlState).GetField("wheelSteer")),
            new RecordStructField("wheel_throttle_trim", "Current trim value for wheel throttle", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("wheelThrottleTrim")),
            new RecordStructField("wheel_steer_trim", "Current trim value for wheel steering", BuiltinType.Float,
                typeof(FlightCtrlState).GetField("wheelSteerTrim")),
            new RecordStructField("kill_rot", "Kill rotation", BuiltinType.Bool,
                typeof(FlightCtrlState).GetField("killRot")),
            new RecordStructField("gear_up", "Gear up", BuiltinType.Bool,
                typeof(FlightCtrlState).GetField("gearUp")),
            new RecordStructField("gear_down", "Gear down", BuiltinType.Bool,
                typeof(FlightCtrlState).GetField("gearDown")),
            new RecordStructField("breaks", "Brakes", BuiltinType.Bool,
                typeof(FlightCtrlState).GetField("brakes")),
            new RecordStructField("stage", "Stage", BuiltinType.Bool,
                typeof(FlightCtrlState).GetField("stage"))
        },
        BuiltinType.NoOperators,
        BuiltinType.NoOperators,
        new Dictionary<string, IMethodInvokeFactory>(),
        new Dictionary<string, IFieldAccessFactory>());
}
