using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ErrorType : RealizedType {
    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; } = new() {
        {
            "to_string", new BoundMethodInvokeFactory("Get string representation of the range", true,
                () => BuiltinType.String,
                () => [],
                false, typeof(Error), typeof(Error).GetMethod("ToString"))
        }
    };
    
    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; } = new() {
        {
            "message",
            new BoundPropertyLikeFieldAccessFactory("The length of the range", () => BuiltinType.Int,
                typeof(Error), typeof(Error).GetProperty("Message"))
        }
    };

    public override string Name => "Error";

    public override bool IsValid(ModuleContext context) {
        return true;
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return this;
    }

    public override Type GeneratedType(ModuleContext context) {
        return typeof(Error);
    }
}
