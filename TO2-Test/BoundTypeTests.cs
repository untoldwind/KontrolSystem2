using Xunit;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    public class NonGeneric {
    }

    public class SimpleGeneric<T> {
    }

    public class BoundTypeTests {
        [Fact]
        public void TestNonGeneric() {
            BoundType type = new BoundType("module", "NonGeneric", "", typeof(NonGeneric),
                BuiltinType.NoOperators,
                BuiltinType.NoOperators,
                Enumerable.Empty<(string name, IMethodInvokeFactory invoker)>(),
                Enumerable.Empty<(string name, IFieldAccessFactory access)>()
            );
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal("NonGeneric", type.LocalName);
            Assert.Equal("module::NonGeneric", type.Name);
            Assert.True(type.IsValid(moduleContext));
            Assert.Empty(type.GenericParameters);

            Assert.Equal(typeof(NonGeneric), type.GeneratedType(moduleContext));
        }

        [Fact]
        public void TestSimpleGeneric() {
            BoundType type = new BoundType("module", "SimpleGeneric", "", typeof(SimpleGeneric<>),
                BuiltinType.NoOperators,
                BuiltinType.NoOperators,
                Enumerable.Empty<(string name, IMethodInvokeFactory invoker)>(),
                Enumerable.Empty<(string name, IFieldAccessFactory access)>()
            );
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal("SimpleGeneric", type.LocalName);
            Assert.Equal("module::SimpleGeneric<T>", type.Name);
            Assert.Equal(new[] { "T" }, type.GenericParameters);
            Assert.False(type.IsValid(moduleContext));
            Assert.Equal(typeof(SimpleGeneric<>), type.GeneratedType(moduleContext));

            RealizedType filledType = type.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                {"T", BuiltinType.Int}
            });

            Assert.Equal("SimpleGeneric", filledType.LocalName);
            Assert.Equal("module::SimpleGeneric<int>", filledType.Name);
            Assert.True(filledType.IsValid(moduleContext));
            Assert.Empty(filledType.GenericParameters);
            Assert.Equal(typeof(SimpleGeneric<long>), filledType.GeneratedType(moduleContext));

            RealizedType aliased = type.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                {"T", new GenericParameter("U")}
            });

            Assert.Equal("SimpleGeneric", aliased.LocalName);
            Assert.Equal("module::SimpleGeneric<U>", aliased.Name);
            Assert.False(aliased.IsValid(moduleContext));
            Assert.Equal(new[] { "U" }, aliased.GenericParameters);
            Assert.Equal(typeof(SimpleGeneric<>), aliased.GeneratedType(moduleContext));

            RealizedType filledType2 = aliased.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                {"U", BuiltinType.String}
            });

            Assert.Equal("SimpleGeneric", filledType2.LocalName);
            Assert.Equal("module::SimpleGeneric<string>", filledType2.Name);
            Assert.True(filledType2.IsValid(moduleContext));
            Assert.Empty(filledType2.GenericParameters);
            Assert.Equal(typeof(SimpleGeneric<string>), filledType2.GeneratedType(moduleContext));

            Assert.Equal(new Dictionary<string, RealizedType> {
                {"T", BuiltinType.Int}
            }, type.InferGenericArgument(moduleContext, filledType).ToDictionary(t => t.name, t => t.type));
            Assert.Equal(new Dictionary<string, RealizedType> {
                {"U", BuiltinType.Int}
            }, aliased.InferGenericArgument(moduleContext, filledType).ToDictionary(t => t.name, t => t.type));
            Assert.Equal(new Dictionary<string, RealizedType> {
                {"T", BuiltinType.String}
            }, type.InferGenericArgument(moduleContext, filledType2).ToDictionary(t => t.name, t => t.type));
            Assert.Equal(new Dictionary<string, RealizedType> {
                {"U", BuiltinType.String}
            }, aliased.InferGenericArgument(moduleContext, filledType2).ToDictionary(t => t.name, t => t.type));
        }
    }
}
