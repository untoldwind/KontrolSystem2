using System.Collections.Generic;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2 {
    public readonly struct StructuralError {
        public enum ErrorType {
            ArgumentMismatch,
            CoreGeneration,
            DuplicateConstantName,
            DuplicateFunctionName,
            DuplicateTypeName,
            DuplicateVariableName,
            IncompatibleTypes,
            InvalidScope,
            InvalidImport,
            InvalidOperator,
            InvalidType,
            NoIndexAccess,
            NoSuchUnapply,
            NoSuchField,
            NoSuchFunction,
            NoSuchMethod,
            NoSuchModule,
            NoSuchVariable
        };

        public readonly ErrorType errorType;
        public readonly string message;
        public readonly Position start;
        public readonly Position end;

        public StructuralError(ErrorType errorType, string message, Position start, Position end) {
            this.errorType = errorType;
            this.message = message;
            this.start = start;
            this.end = end;
        }

        public override string ToString() => $"{start}: ERROR {errorType}: {message}";
    }

    public class CompilationErrorException : System.Exception {
        public readonly List<StructuralError> errors;

        public CompilationErrorException(List<StructuralError> errors) : base($"{errors.Count} structural errors") {
            this.errors = errors;
        }
    }
}
