﻿using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ConstDeclaration : Node, IModuleItem {
        public readonly bool isPublic;
        public readonly string name;
        public readonly string description;
        public readonly TO2Type type;
        public readonly Expression expression;

        public ConstDeclaration(bool isPublic, string name, string description, TO2Type type, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.isPublic = isPublic;
            this.name = name;
            this.description = description;
            this.type = type;
            this.expression = expression;
            this.expression.TypeHint = context => this.type.UnderlyingType(context.ModuleContext);
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public override REPLValueFuture Eval(REPLContext context) {
            var expressionFuture = this.expression.Eval(context);

            if (context.FindVariable(name) != null) {
                throw new REPLException(this, $"Variable '{name}' already declared in this scope");
            }

            if (!type.IsAssignableFrom(context.replModuleContext, expressionFuture.Type)) {
                throw new REPLException(this,
                    $"Variable '{name}' is of type {expressionFuture.Type} but is initialized with {expressionFuture.Type}");
            }

            var variable = context.DeclaredVariable(name, true, type.UnderlyingType(context.replModuleContext));
            var assign = variable.declaredType.AssignFrom(context.replModuleContext, expressionFuture.Type);

            return expressionFuture.Then(BuiltinType.Unit, value => {
                var converted = assign.EvalConvert(this, value);

                variable.value = converted;

                return converted;
            });
        }
    }
}
