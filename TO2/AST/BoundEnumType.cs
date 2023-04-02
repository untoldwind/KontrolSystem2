﻿using System;
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
                    enumType, enumType.GetMethod("ToString", Type.EmptyTypes ), null, true) }
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

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

        public BoundEnumConstType(BoundEnumType boundEnumType) {
            this.boundEnumType = boundEnumType;

            var declaredFields = new Dictionary<string, IFieldAccessFactory>();

            var names = Enum.GetNames(boundEnumType.enumType);
            var values = Enum.GetValues(boundEnumType.enumType);

            for (int i = 0; i < names.Length; i++) {
                int value = (int)Convert.ChangeType(values.GetValue(i), typeof(int));

                declaredFields.Add((string)names.GetValue(i), new EnumConstantFieldAccessFactory(boundEnumType, value));
            }

            DeclaredFields = declaredFields;

            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory>() {
                { "from_string", new EnumFromStringMethodFactory(boundEnumType) }
            };
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

        public static Option<T> FromString<T>(string value) where T : struct {
            if (Enum.TryParse<T>(value, true, out var e)) {
                return Option.Some(e);
            }

            return Option.None<T>();
        }
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
            var tmp = context.MakeTempVariable(boundEnumType);
            context.IL.Emit(OpCodes.Ldc_I4, value);
            tmp.EmitStore(context);
            tmp.EmitLoadPtr(context);
        }

        public void EmitStore(IBlockContext context) {
        }

        public IREPLValue EvalGet(Node node, IREPLValue target) =>
            new REPLAny(boundEnumType, Enum.ToObject(boundEnumType.enumType, value));

        public IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value) =>
            throw new REPLException(node, "Field assign not supported");
    }

    internal class EnumFromStringMethodFactory : IMethodInvokeFactory {
        private readonly BoundEnumType boundEnumType;

        public EnumFromStringMethodFactory(BoundEnumType boundEnumType) {
            this.boundEnumType = boundEnumType;
        }

        public bool IsConst => true;

        public TypeHint ReturnHint => context => new OptionType(boundEnumType);

        public TypeHint ArgumentHint(int argumentIdx) => context => BuiltinType.String;

        public string Description => "Parse from string";

        public TO2Type DeclaredReturn => new OptionType(boundEnumType);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter>() {
            new FunctionParameter("value", BuiltinType.String)
        };

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) =>
            new EnumFromStringMethodEmitter(boundEnumType);

        public IMethodInvokeFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    internal class EnumFromStringMethodEmitter : IMethodInvokeEmitter {
        private readonly BoundEnumType boundEnumType;

        public EnumFromStringMethodEmitter(BoundEnumType boundEnumType) {
            this.boundEnumType = boundEnumType;
        }

        public RealizedType ResultType => new OptionType(boundEnumType);

        public List<RealizedParameter> Parameters => new List<RealizedParameter>() {
            new RealizedParameter("value", BuiltinType.String)
        };

        public bool RequiresPtr => false;

        public bool IsAsync => false;

        public void EmitCode(IBlockContext context) {
            context.IL.EmitCall(OpCodes.Call, typeof(BoundEnumConstType).GetMethod("FromString").MakeGenericMethod(boundEnumType.enumType), 1);
        }

        public REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments) {
            if (targetWithArguments.Length != 1) {
                throw new REPLException(node, $"from_string requires one argument");
            }
            if (targetWithArguments[0].Value is REPLString value) {
                try {
                    return REPLValueFuture.Success(
                        new REPLAny(new OptionType(boundEnumType),
                            Option.Some<object>(Enum.Parse(boundEnumType.enumType, value.stringValue, true))));
                } catch (Exception e) {
                    return REPLValueFuture.Success(
                        new REPLAny(new OptionType(boundEnumType), Option.None<object>()));
                }
            }

            throw new REPLException(node, $"from_string requires string argument");
        }
    }
}
