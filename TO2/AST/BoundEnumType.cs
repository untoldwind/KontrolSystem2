using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class BoundEnumType : RealizedType {
        internal readonly string modulePrefix;
        public readonly string localName;
        private readonly string description;
        public readonly Type enumType;
        public readonly OperatorCollection allowedSuffixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

        public BoundEnumType(string modulePrefix, string localName, string description, Type enumType) {
            this.modulePrefix = modulePrefix;
            this.localName = localName;
            this.description = description;
            this.enumType = enumType;

            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory>() {
                { "to_string", new BoundMethodInvokeFactory("String representation of the number",
                    true, () => BuiltinType.String, () => new List<RealizedParameter>(), false,
                    enumType, enumType.GetMethod("ToString", Type.EmptyTypes )) }
            };
            allowedSuffixOperators = new OperatorCollection() {
                {
                    Operator.Eq,
                    new DirectOperatorEmitter(() => this, () => BuiltinType.Bool, REPLAny.ObjEq, OpCodes.Ceq)
                }, {
                    Operator.NotEq,
                    new DirectOperatorEmitter(() => this, () => BuiltinType.Bool, REPLAny.ObjNeq, OpCodes.Ceq,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }
            };
        }

        public override string Name {
            get {
                StringBuilder builder = new StringBuilder();

                if (modulePrefix != null) {
                    builder.Append(modulePrefix);
                    builder.Append("::");
                }

                builder.Append(localName);

                return builder.ToString();
            }
        }

        public override string Description => description;

        public override string LocalName => localName;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => enumType;
    }

    public class BoundEnumConstType : RealizedType {
        private readonly BoundEnumType boundEnumType;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public BoundEnumConstType(BoundEnumType boundEnumType) {
            this.boundEnumType = boundEnumType;

            DeclaredFields = new Dictionary<string, IFieldAccessFactory>();

            var names = Enum.GetNames(boundEnumType.enumType);
            var values = Enum.GetValues(boundEnumType.enumType);

            for (int i = 0; i < names.Length; i++) {
                int value = (int)Convert.ChangeType(values.GetValue(i), typeof(int));

                DeclaredFields.Add((string)names.GetValue(i), new EnumConstantFieldAccessFactory(boundEnumType, value));
            }
        }

        public override string Name {
            get {
                StringBuilder builder = new StringBuilder();

                if (boundEnumType.modulePrefix != null) {
                    builder.Append(boundEnumType.modulePrefix);
                    builder.Append("::");
                }

                builder.Append(boundEnumType.localName);
                builder.Append("Constants");

                return builder.ToString();
            }
        }

        public override string Description => "";

        public override string LocalName => boundEnumType.localName + "Constants";

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => typeof(object);
    }

    internal class EnumConstantFieldAccessFactory : IFieldAccessFactory {
        private readonly BoundEnumType boundEnumType;
        private readonly int value;

        internal EnumConstantFieldAccessFactory(BoundEnumType boundEnumType, int value) {
            this.boundEnumType = boundEnumType;
            this.value = value;
        }

        public TO2Type DeclaredType => boundEnumType;

        public string Description => "";

        public bool CanStore => false;

        public IFieldAccessEmitter Create(ModuleContext context) => new EnumConstantFieldAccessEmitter(boundEnumType, value);

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class EnumConstantFieldAccessEmitter : IFieldAccessEmitter {
        private readonly BoundEnumType boundEnumType;
        private readonly int value;

        internal EnumConstantFieldAccessEmitter(BoundEnumType boundEnumType, int value) {
            this.boundEnumType = boundEnumType;
            this.value = value;
        }

        public RealizedType FieldType => boundEnumType;

        public bool RequiresPtr => false;

        public bool CanStore => false;

        public void EmitLoad(IBlockContext context) {
            context.IL.Emit(OpCodes.Ldc_I4, value);
        }

        public void EmitPtr(IBlockContext context) {
        }

        public void EmitStore(IBlockContext context) {
        }

        public IREPLValue EvalGet(Node node, IREPLValue target) =>
            new REPLAny(boundEnumType, Enum.ToObject(boundEnumType.enumType, value));

        public IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value) =>
            throw new REPLException(node, "Field assign not supported");
    }
}
