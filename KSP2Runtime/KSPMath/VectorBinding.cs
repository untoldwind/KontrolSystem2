using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class VectorBinding {
        public static readonly BoundType VectorType = Direct.BindType("ksp::math", "Vector",
            "This is a 3-dimenstional vector in a specific coordindate system", typeof(Vector),
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => VectorType,
                        typeof(Vector).GetMethod("negate", new[] {typeof(Vector)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Addition", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Addition", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Subtraction", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Subtraction", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => VectorType,
                        typeof(Vector).GetMethod("op_Multiply", new[] {typeof(Vector), typeof(double)}))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => VectorType,
                        typeof(Vector).GetMethod("op_Multiply", new[] {typeof(Vector), typeof(double)}))
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {},
            new Dictionary<string, IFieldAccessFactory> {
                { "local", new BoundPropertyLikeFieldAccessFactory("coordinates in coordindate system", () => Vector3Binding.Vector3Type, typeof(Vector), typeof(Vector).GetProperty("vector") )},
                { "coordinate_system", new BoundPropertyLikeFieldAccessFactory("coordindate system", () => CoordindateSystemBinding.CoordindateSystemType, typeof(Vector), typeof(Vector).GetProperty("coordinateSystem") )}
            });
    }
}
