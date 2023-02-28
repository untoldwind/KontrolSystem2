using System;
using System.Linq;

namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// TryParse a single character matching 'predicate'
        /// </summary>
        public static Parser<char> Char(Predicate<char> predicate, string expected) => input => {
            if (input.Available < 1) return Result.Failure<char>(input, expected);
            char current = input.Current;
            if (!predicate(current)) return Result.Failure<char>(input, expected);
            return Result.Success(input.Advance(1), current);
        };

        /// <summary>
        /// Parse a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        public static Parser<char> CharExcept(Predicate<char> predicate, string expected) =>
            Char(c => !predicate(c), "any character except " + expected);

        /// <summary>
        /// Zero or more characters matching 'predicate'
        /// </summary>
        public static Parser<string> Chars0(Predicate<char> predicate) => input => {
            int count = input.FindNext(ch => !predicate(ch));
            if (count < 0) return Result.Success(input.Advance(input.Available), input.Take(input.Available));
            return Result.Success(input.Advance(count), input.Take(count));
        };

        /// <summary>
        /// One or more characters matching 'predicate'
        /// </summary>
        public static Parser<string> Chars1(Predicate<char> predicate, string expected) => input => {
            if (input.Available < 1) return Result.Failure<string>(input, expected);
            int count = input.FindNext(ch => !predicate(ch));
            if (count < 0) return Result.Success(input.Advance(input.Available), input.Take(input.Available));
            if (count == 0) return Result.Failure<string>(input, expected);
            return Result.Success(input.Advance(count), input.Take(count));
        };

        /// <summary>
        /// Parse a single character c.
        /// </summary>
        public static Parser<char> Char(char c) => Char(ch => ch == c, $"'{c}'");

        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        public static Parser<char> OneOf(string c) => Char(c.Contains, String.Join("|", c.ToArray()));

        /// <summary>
        /// One or more characters not in list
        /// </summary>
        public static Parser<string> CharsExcept0(string c) => Chars0(ch => !c.Contains(ch));

        public static Parser<char> CharExcept(string c) => CharExcept(c.Contains, c);

        /// <summary>
        /// Parse a whitespace.
        /// </summary>
        public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "<whitespace>");

        /// <summary>
        /// Parse zero or more whitespace.
        /// </summary>
        public static readonly Parser<string> WhiteSpaces0 = Chars0(char.IsWhiteSpace);

        /// <summary>
        /// Parse zero or more SpaceSeparators (including tabs)
        /// </summary>
        public static readonly Parser<string> Spacing0 = Chars0(ch =>
            ch == '\t' || System.Char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.SpaceSeparator);

        /// <summary>
        /// Parse one or more whitespace.
        /// </summary>
        public static readonly Parser<string> WhiteSpaces1 = Chars1(char.IsWhiteSpace, "<whitespace>");

        /// <summary>
        /// Parse one or more SpaceSeparators (including tabs)
        /// </summary>
        public static readonly Parser<string> Spacing1 =
            Chars1(
                ch => ch == '\t' || System.Char.GetUnicodeCategory(ch) ==
                    System.Globalization.UnicodeCategory.SpaceSeparator, "<space");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly Parser<char> Letter = Char(char.IsLetter, "<letter>");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly Parser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "<letter or digit>");

        /// <summary>
        /// Parse zero or more digits.
        /// </summary>
        public static readonly Parser<string> Digits0 = Chars0(char.IsDigit);

        /// <summary>
        /// Parse one or more digits.
        /// </summary>
        public static readonly Parser<string> Digits1 = Chars1(char.IsDigit, "<digit>");

        /// <summary>
        /// Parse an exact tag (i.e. sequence of chars).
        /// </summary>
        public static Parser<string> Tag(string tag) => input => {
            if (input.Available < tag.Length) return Result.Failure<string>(input, $"'{tag}'");
            string content = input.Take(tag.Length);
            if (content != tag) return Result.Failure<string>(input, $"'{tag}'");
            return Result.Success(input.Advance(tag.Length), content);
        };

        public static readonly Parser<string> LineEnd = Opt(Char('\r'))
            .Then(r => Char('\n').Map(n => r.Map(char.ToString).GetOrElse("") + n.ToString()));

        /// <summary>
        /// Non-consuming parser that succeeds if input is at a line end or end of input.
        /// </summary>
        public static readonly Parser<bool> PeekLineEnd = input => {
            if (input.Available == 0) return Result.Success(input, false);
            if (input.Available >= 1 && input.Current == '\n') return Result.Success(input, true);
            if (input.Take(2) == "\r\n") return Result.Success(input, true);
            return Result.Failure<bool>(input, "<end of line>");
        };
    }
}
