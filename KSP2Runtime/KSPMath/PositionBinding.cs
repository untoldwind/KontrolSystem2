using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class PositionBinding {
        public static readonly BoundType PositionType = Direct.BindType("ksp::math", "Position",
            "A position in space. This is a 3-dimenstional vector in a specific coordindate system", typeof(Position),
            new OperatorCollection { },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => PositionType,
                        typeof(Position).GetMethod("op_Addition", new[] {typeof(Position), typeof(Vector)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => PositionType,
                        typeof(Position).GetMethod("op_Addition", new[] {typeof(Position), typeof(Vector)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => PositionType, () => VectorBinding.VectorType,
                        typeof(Position).GetMethod("op_Subtraction", new[] {typeof(Position), typeof(Position)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => PositionType, () => VectorBinding.VectorType,
                        typeof(Position).GetMethod("op_Subtraction", new[] {typeof(Position), typeof(Position)}))
                },
            }, 
            new Dictionary<string, IMethodInvokeFactory> {},
            new Dictionary<string, IFieldAccessFactory> {
                { "local", new BoundPropertyLikeFieldAccessFactory("coordinates in coordindate system", () => Vector3Binding.Vector3Type, typeof(Position), typeof(Position).GetProperty("localPosition") )},
                { "coordinate_system", new BoundPropertyLikeFieldAccessFactory("coordindate system", () => CoordindateSystemBinding.CoordindateSystemType, typeof(Position), typeof(Position).GetProperty("coordinateSystem") )}
            });

    }
}
