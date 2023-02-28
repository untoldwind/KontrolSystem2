namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// Expect the end of input/file to be reached
        /// </summary>
        public static readonly Parser<char> Eof = input => {
            if (input.Available > 0) return Result.Failure<char>(input, "<EOF>");
            return Result.Success(input, '\0');
        };
    }
}
