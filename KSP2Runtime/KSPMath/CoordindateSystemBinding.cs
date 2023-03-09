using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Api;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class CoordindateSystemBinding {
        public static readonly BoundType CoordindateSystemType = Direct.BindType("ksp::math", "CoordinateSystem",
            "Representation of a coordinate system", typeof(ICoordinateSystem),
            new OperatorCollection { },
            new OperatorCollection { },
            new Dictionary<string, IMethodInvokeFactory> {},
            new Dictionary<string, IFieldAccessFactory> {
                { "forward", new BoundPropertyLikeFieldAccessFactory("forward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("forward") )},
                { "back", new BoundPropertyLikeFieldAccessFactory("backward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("back") )},
                { "up", new BoundPropertyLikeFieldAccessFactory("up vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("up") )},
                { "down", new BoundPropertyLikeFieldAccessFactory("down vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("down") )},
                { "right", new BoundPropertyLikeFieldAccessFactory("right vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("right") )},
                { "left", new BoundPropertyLikeFieldAccessFactory("left vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("left") )},
            }
        );
    }
}
