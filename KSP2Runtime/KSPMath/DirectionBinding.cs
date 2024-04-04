﻿using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public static class DirectionBinding {
    public static readonly BoundType DirectionType = Direct.BindType("ksp::math", "Direction",
        "Represents the rotation from an initial coordinate system when looking down the z-axis and \"up\" being the y-axis",
        typeof(Direction),
        new OperatorCollection {
            {
                Operator.Neg,
                new StaticMethodOperatorEmitter(() => BuiltinType.Unit, LazyDirectionType,
                    typeof(Direction).GetMethod("op_UnaryNegation", [typeof(Direction)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type,
                    typeof(Direction).GetMethod("op_Multiply", [typeof(Vector3d), typeof(Direction)]))
            }
        },
        new OperatorCollection {
            {
                Operator.Add,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Addition", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.AddAssign,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Addition", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.Sub,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Subtraction", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.SubAssign,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Subtraction", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Multiply", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.MulAssign,
                new StaticMethodOperatorEmitter(LazyDirectionType, LazyDirectionType,
                    typeof(Direction).GetMethod("op_Multiply", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type,
                    typeof(Direction).GetMethod("op_Multiply", [typeof(Direction), typeof(Vector3d)]))
            }, {
                Operator.Eq,
                new StaticMethodOperatorEmitter(LazyDirectionType, () => BuiltinType.Bool,
                    typeof(Direction).GetMethod("op_Equality", [typeof(Direction), typeof(Direction)]))
            }, {
                Operator.NotEq,
                new StaticMethodOperatorEmitter(LazyDirectionType, () => BuiltinType.Bool,
                    typeof(Direction).GetMethod("op_Equality", [typeof(Direction), typeof(Direction)]),
                    null, OpCodes.Ldc_I4_0, OpCodes.Ceq)
            }
        },
        new Dictionary<string, IMethodInvokeFactory> {
            {
                "to_string",
                new BoundMethodInvokeFactory("Convert the direction to string", true, () => BuiltinType.String,
                    () => [], false, typeof(Direction),
                    typeof(Direction).GetMethod("ToString"))
            }, {
                "to_global",
                new BoundMethodInvokeFactory("Associate this direction with a coordinate system",
                    true,
                    () => RotationBinding.RotationType,
                    () => [new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")],
                    false, typeof(DirectionBinding), typeof(DirectionBinding).GetMethod("ToGlobal"))
            }
        },
        new Dictionary<string, IFieldAccessFactory> {
            {
                "euler",
                new BoundPropertyLikeFieldAccessFactory("Euler angles in degree of the rotation",
                    () => Vector3Binding.Vector3Type, typeof(Direction),
                    typeof(Direction).GetProperty("Euler"))
            }, {
                "vector",
                new BoundPropertyLikeFieldAccessFactory(
                    "Fore vector of the rotation (i.e. looking/facing direction", () => Vector3Binding.Vector3Type,
                    typeof(Direction), typeof(Direction).GetProperty("Vector"))
            }, {
                "up_vector",
                new BoundPropertyLikeFieldAccessFactory("Up vector of the rotation",
                    () => Vector3Binding.Vector3Type, typeof(Direction),
                    typeof(Direction).GetProperty("UpVector"))
            }, {
                "right_vector",
                new BoundPropertyLikeFieldAccessFactory("Right vector of the rotation",
                    () => Vector3Binding.Vector3Type, typeof(Direction),
                    typeof(Direction).GetProperty("RightVector"))
            }, {
                "pitch",
                new BoundPropertyLikeFieldAccessFactory("Pitch in degree", () => BuiltinType.Float,
                    typeof(Direction), typeof(Direction).GetProperty("Pitch"))
            }, {
                "yaw",
                new BoundPropertyLikeFieldAccessFactory("Yaw in degree", () => BuiltinType.Float, typeof(Direction),
                    typeof(Direction).GetProperty("Yaw"))
            }, {
                "roll",
                new BoundPropertyLikeFieldAccessFactory("Roll in degree", () => BuiltinType.Float,
                    typeof(Direction), typeof(Direction).GetProperty("Roll"))
            }, {
                "axis",
                new BoundPropertyLikeFieldAccessFactory("The rotation axis", () => Vector3Binding.Vector3Type,
                    typeof(Direction), typeof(Direction).GetProperty("Axis"))
            }, {
                "angle",
                new BoundPropertyLikeFieldAccessFactory("The rotation angle around the axis in degrees", () => BuiltinType.Float,
                    typeof(Direction), typeof(Direction).GetProperty("Angle"))
            }, {
                "inverse",
                new BoundPropertyLikeFieldAccessFactory("Inverse direction", LazyDirectionType,
                    typeof(DirectionBinding),
                    typeof(DirectionBinding).GetMethod("Inverse"), null)
            }
        });

    private static BoundType LazyDirectionType() => DirectionType;

    public static Direction LookDirUp(Vector3d lookDirection, Vector3d upDirection) {
        return Direction.LookRotation(lookDirection, upDirection);
    }

    public static Direction Euler(double x, double y, double z) {
        return new Direction(new Vector3d(x, y, z), true);
    }

    public static Direction AngleAxis(double angle, Vector3d axis) {
        return Direction.AngleAxis(angle, axis);
    }

    public static Direction FromVectorToVector(Vector3d v1, Vector3d v2) {
        return Direction.FromVectorToVector(v1, v2);
    }

    public static Direction Inverse(Direction direction) {
        return new Direction(QuaternionD.Inverse(direction.Rotation));
    }

    public static RotationWrapper ToGlobal(Direction direction, ITransformFrame frame) {
        return new RotationWrapper(new Rotation(frame, direction.Rotation));
    }
}
