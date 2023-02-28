using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public static class BuiltinFunctions {
        private static readonly IKontrolFunction Some = new CompiledKontrolFunction("Some",
            "Wrap a value as defined optional", false,
            new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T")) },
            new OptionType(new GenericParameter("T")), typeof(Option).GetMethod("Some"));

        private static readonly IKontrolFunction None = new CompiledKontrolFunction("None",
            "Create an undefined optional", false, new List<RealizedParameter>(),
            new OptionType(new GenericParameter("T")), typeof(Option).GetMethod("None"));

        private static readonly IKontrolFunction Ok = new CompiledKontrolFunction("Ok",
            "Wrap a value as successful result", false,
            new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T")) },
            new ResultType(new GenericParameter("T"), new GenericParameter("E")), typeof(Result).GetMethod("Ok"));

        private static readonly IKontrolFunction Err = new CompiledKontrolFunction("Err",
            "Wrap an error message as failed result", false,
            new List<RealizedParameter> { new RealizedParameter("error", new GenericParameter("E")) },
            new ResultType(new GenericParameter("T"), new GenericParameter("E")), typeof(Result).GetMethod("Err"));

        private static readonly IKontrolFunction Cell = new CompiledKontrolFunction("Cell", "Wrap a value as cell",
            false, new List<RealizedParameter> { new RealizedParameter("value", new GenericParameter("T")) },
            BuiltinType.Cell, typeof(Cell).GetMethod("Create"));

        private static readonly IKontrolFunction ArrayBuilder = new CompiledKontrolFunction("ArrayBuilder",
            "Create a new ArrayBuilder", false,
            new List<RealizedParameter> { new RealizedParameter("capacity", BuiltinType.Int, new IntDefaultValue(32)) },
            BuiltinType.ArrayBuilder, typeof(ArrayBuilder).GetMethod("Create"));

        public static readonly Dictionary<string, IKontrolFunction> ByName = new Dictionary<string, IKontrolFunction> {
            {"Some", Some},
            {"None", None},
            {"Ok", Ok},
            {"Err", Err},
            {"Cell", Cell},
            {"ArrayBuilder", ArrayBuilder},
        };
    }
}
