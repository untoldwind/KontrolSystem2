using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    /// <summary>
    /// Base class of all type in to2.
    /// </summary>
    public abstract class TO2Type {
        /// <summary>
        /// Full name of the type.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Type description of generated documentation.
        /// </summary>
        public virtual string Description => "";

        /// <summary>
        /// Local part of the type name (i.e. without the module prefix)
        /// </summary>
        public virtual string LocalName => Name;

        /// <summary>
        /// Check if the type is valid.
        /// </summary>
        public virtual bool IsValid(ModuleContext context) => true;

        /// <summary>
        /// Get the underlying type in case this type is an alias or reference to another type.
        /// </summary>
        public abstract RealizedType UnderlyingType(ModuleContext context);

        /// <summary>
        /// Get the C# compatible type the will be used in the IL code.
        /// </summary>
        public abstract Type GeneratedType(ModuleContext context);

        /// <summary>
        /// Get collection of operators that may be used as prefix to this type (i.e. where this type is on the right side)
        /// </summary>
        public virtual IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuiltinType.NoOperators;

        /// <summary>
        /// Get collection of operators that may be used as postfix to this type (i.e. where this type is on the left side)
        /// </summary>
        public virtual IOperatorCollection AllowedSuffixOperators(ModuleContext context) => BuiltinType.NoOperators;

        /// <summary>
        /// Find a method of this type by name.
        /// Will return null if there is no such method.
        /// </summary>
        public abstract IMethodInvokeFactory FindMethod(ModuleContext context, string methodName);

        /// <summary>
        /// Find a field of this type by name.
        /// Will return null if there is no such field.
        /// </summary>
        public abstract IFieldAccessFactory FindField(ModuleContext context, string fieldName);

        /// <summary>
        /// Check if index access is allowed for this type.
        /// </summary>
        public virtual IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        /// <summary>
        /// Get a list of all pattern this type can be unapplied to.
        /// </summary>
        public virtual IUnapplyEmitter
            AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) => null;

        /// <summary>
        /// Check if this type can be used as source in a for .. in expression.
        /// </summary>
        public virtual IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => null;

        /// <summary>
        /// Check if a variable of this type can be assigned from an other type.
        /// </summary>
        public virtual bool IsAssignableFrom(ModuleContext context, TO2Type otherType) =>
            GeneratedType(context).IsAssignableFrom(otherType.GeneratedType(context));

        /// <summary>
        /// Get the rule how to assign/convert an other type to this type.
        /// </summary>
        public virtual IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) =>
            DefaultAssignEmitter.Instance;

        public virtual IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
            RealizedType concreteType) => Enumerable.Empty<(string name, RealizedType type)>();

        public override string ToString() => Name;
    }

    /// <summary>
    /// Marker class for all types that are no aliases or references (i.e. can actually be realized)
    /// </summary>
    public abstract class RealizedType : TO2Type {
        public virtual Dictionary<string, IMethodInvokeFactory> DeclaredMethods => BuiltinType.NoMethods;

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) =>
            DeclaredMethods.Get(methodName);

        public virtual Dictionary<string, IFieldAccessFactory> DeclaredFields => BuiltinType.NoFields;

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) =>
            DeclaredFields.Get(fieldName);

        public virtual string[] GenericParameters => new string[0];

        public virtual RealizedType
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;

        public virtual void EmitInitialize(IBlockContext context, IBlockVariable variable) {
        }
    }
}
