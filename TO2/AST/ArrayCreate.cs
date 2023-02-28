using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class ArrayCreate : Expression {
        private TO2Type ElementType { get; }
        private List<Expression> Elements { get; }
        private TypeHint typeHint;

        public ArrayCreate(TO2Type elementType, List<Expression> elements, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            ElementType = elementType;
            Elements = elements;
        }

        public override IVariableContainer VariableContainer {
            set {
                foreach (Expression element in Elements) element.VariableContainer = value;
            }
        }

        public override TypeHint TypeHint {
            set {
                typeHint = value;
                foreach (Expression element in Elements)
                    if (ElementType != null)
                        element.TypeHint = context => ElementType.UnderlyingType(context.ModuleContext);
                    else
                        element.TypeHint = context =>
                            (typeHint?.Invoke(context) as ArrayType)?.ElementType.UnderlyingType(context.ModuleContext);
            }
        }

        public override TO2Type ResultType(IBlockContext context) {
            if (ElementType != null) return new ArrayType(ElementType);
            foreach (Expression element in Elements) {
                TO2Type valueType = element.ResultType(context);
                if (valueType != BuiltinType.Unit) return new ArrayType(valueType);
            }

            ArrayType arrayHint = typeHint?.Invoke(context) as ArrayType;

            return arrayHint ?? BuiltinType.Unit;
        }

        public override void Prepare(IBlockContext context) {
            foreach (Expression element in Elements) element.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            ArrayType arrayType = ResultType(context) as ArrayType;

            if (arrayType == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    "Unable to infer type of array. Please add some type hint",
                    Start,
                    End
                ));
                return;
            }

            RealizedType elementType = arrayType.ElementType.UnderlyingType(context.ModuleContext);

            context.IL.Emit(OpCodes.Ldc_I4, Elements.Count);
            context.IL.Emit(OpCodes.Newarr, elementType.GeneratedType(context.ModuleContext));

            foreach (var element in Elements) {
                TO2Type valueType = element.ResultType(context);
                if (!elementType.IsAssignableFrom(context.ModuleContext, valueType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        "Element {i} is of type {valueType}, expected {elementType}",
                        element.Start,
                        element.End
                    ));
                }
            }

            if (context.HasErrors) return;

            for (var i = 0; i < Elements.Count; i++) {
                context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(OpCodes.Ldc_I4, i);
                Elements[i].EmitCode(context, false);
                if (elementType == BuiltinType.Bool) context.IL.Emit(OpCodes.Stelem_I4);
                else if (elementType == BuiltinType.Int) context.IL.Emit(OpCodes.Stelem_I8);
                else if (elementType == BuiltinType.Float) context.IL.Emit(OpCodes.Stelem_R8);
                else context.IL.Emit(OpCodes.Stelem, elementType.GeneratedType(context.ModuleContext));
            }
        }
    }
}
