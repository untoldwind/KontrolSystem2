# Literal values

## Boolean

To create a boolean value there are just the usual two keywords:
* `true`
* `false`

## Integer

Integers can be created via:
* Decimals, e.g. `1234`
* Hexadecimals with a `0x` prefix, e.g `0x12abcd`
* Octal numbers with a `0o` prefix, e.g `0o1234`
* Binary numbers with a `0b` prefix, e.g. `0b010111`

To improve readability of large numbers an `_` can be added between any digit:
* e.g. one million: `1_000_000`
* Some readable hex: `0x1111_ffff`
* Some binary: `0b1111_0000_0101_1010`

## Float

Floating point numbers can be created via:
* Decimals, e.g. `12.34`
* Exponential/scientific notation, e.g. `1.234e-5`

## Strings

String values can be created using double quotes, e.g.
```
"Hello world"
```
Inside a string the following escape sequences are allowed:
* `\\` to insert a backslash
* `\"` to insert a double-quote (without terminating the string itself)
* `\n` a line feed
* `\r` a carriage return
* `\t` a tabulator

## String interpolation

In addition to static strings, c-sharp like string interpolation (i.e. strings with placeholders) are supported by starting with a `$"`, e.g.
```
const a = 1234
const b = "Hello"

$"{b} world {a}"
```
will produce the string `Hello world 1234`

Like in c-sharp, the placeholders can formatted via the `{<interpolationExpression>[,<alignment>][:<formatString>]}` pattern, e.g.
```
const c = 12.34

$"a fixed point {c,10:N4}"
```
will produce the string `a fixed point    12.3400`

Refer to https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated for a detailed description of the various formatting options.