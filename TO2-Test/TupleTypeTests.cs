using Xunit;
using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    public class TupleTypeTests {
        [Fact]
        public void MakeShortTuple() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal(typeof(ValueTuple<string>),
                new TupleType(new List<TO2Type> { BuiltinType.String }).GeneratedType(moduleContext));
            Assert.Equal(typeof(ValueTuple<long, string>),
                new TupleType(new List<TO2Type> { BuiltinType.Int, BuiltinType.String }).GeneratedType(moduleContext));
            Assert.Equal(typeof(ValueTuple<long, string, double>),
                new TupleType(new List<TO2Type> { BuiltinType.Int, BuiltinType.String, BuiltinType.Float }).GeneratedType(
                    moduleContext));
            Assert.Equal(typeof(ValueTuple<long, string, double, bool>),
                new TupleType(new List<TO2Type>
                        {BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool})
                    .GeneratedType(moduleContext));
            Assert.Equal(typeof(ValueTuple<double, long, string, double, bool>),
                new TupleType(new List<TO2Type>
                        {BuiltinType.Float, BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool})
                    .GeneratedType(moduleContext));
            Assert.Equal(typeof(ValueTuple<double, long, string, double, bool, string, long>),
                new TupleType(new List<TO2Type> {
                    BuiltinType.Float, BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.String, BuiltinType.Int
                }).GeneratedType(moduleContext));
        }

        [Fact]
        public void MakeLongTuple() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal(typeof(ValueTuple<double, long, string, double, bool, string, long, ValueTuple<string>>),
                new TupleType(new List<TO2Type> {
                    BuiltinType.Float, BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.String, BuiltinType.Int, BuiltinType.String
                }).GeneratedType(moduleContext));
            Assert.Equal(
                typeof(ValueTuple<double, long, string, double, bool, string, long,
                    ValueTuple<string, bool, long, double, bool, long, double>>),
                new TupleType(new List<TO2Type> {
                    BuiltinType.Float, BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.String, BuiltinType.Int,
                    BuiltinType.String, BuiltinType.Bool, BuiltinType.Int, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.Int, BuiltinType.Float
                }).GeneratedType(moduleContext));
            Assert.Equal(
                typeof(ValueTuple<double, long, string, double, bool, string, long, ValueTuple<string, bool, long,
                    double, bool, long, double, ValueTuple<string, bool>>>),
                new TupleType(new List<TO2Type> {
                    BuiltinType.Float, BuiltinType.Int, BuiltinType.String, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.String, BuiltinType.Int,
                    BuiltinType.String, BuiltinType.Bool, BuiltinType.Int, BuiltinType.Float, BuiltinType.Bool,
                    BuiltinType.Int, BuiltinType.Float,
                    BuiltinType.String, BuiltinType.Bool
                }).GeneratedType(moduleContext));
        }
    }
}
