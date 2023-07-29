# Operators

List of all supported operators ordered by precedence.

Precedence affects the order how things are evaluated. The most basic example is `a * b + c * d` in which case you want the parser to interpret it like `(a * b) + (c * d)` and not like `a * (b + (c * d))` (or some other variation).

## Prefix: `-`, `!`

* `-` prefix (e.g. `-a`) is a negation, which can be done on `int`, `float`, `ksp::math::Vec2`, `ksp::math::Vec3`
* `!` prefix (e.g. `!condition`) is a logical not, which can be done on `bool`

## Power of (version >= 0.4.3)

* `**` (e.g. `a ** b`) raises `a` to the power of `b`
  * `int ** int` result `int`
  * `float ** float` result `float`

## Multiplication / division / modulo: `*`, `/`, `%`

* `*` (e.g. `a * b`) multiplies two values, which is allowed for
  * `int * int` result `int`
  * `float * float` result `float`
  * `float * ksp::math::Vec2` result `ksp::math::Vec2`
  * `ksp::math::Vec2 * float` result `ksp::math::Vec2`
  * `ksp::math::Vec2 * ksp::math::Vec2` result `float` (vector dot product)
  * `float * ksp::math::Vec3` result `ksp::math::Vec3`
  * `ksp::math::Vec3 * float` result `ksp::math::Vec3`
  * `ksp::math::Vec3 * ksp::math::Vec3` result `float` (vector dot product)
  * Note: You can not multiply `int` with `float`, one of them as to be converted via `.to_float` or `.to_int`
* `/` (e.g. `a / b`) divides first value by the second value, which is allowed for
  * `int / int` result `int` (Note: result will be truncated `1 / 2` is `0`)
  * `float / float` result `float`
  * `ksp::math::Vec2 / float` result `ksp::math::Vec2`
  * `ksp::math::Vec3 / float` result `ksp::math::Vec3`
  * Note: You can not device `int` with `float` or vice versa, one of them as to be converted via `.to_float` or `.to_int`
* `%` (e.g. `a % b`) gets the remainder of the division of the first value by the second value, which can on, which is allowed for
  * `int % int` result `int`
  * `float % float` result `float`

## Addition / subtraction: `+`, `-`

* `+` (e.g. `a + b`) adds two values, which is allowed for
  * `int + int` result `int`
  * `float + float` result `float`
  * `string + string` result `string` (concatenate two strings)
  * `ksp::math::Vec2 + ksp::math::Vec2` result `ksp::math::Vec2`
  * `ksp::math::Vec3 + ksp::math::Vec3` result `ksp::math::Vec3`
  * Note: You can not add `int` with `float`, one of them as to be converted via `.to_float` or `.to_int`
* `-` (e.g. `a - b`) subtracts the second value from the first, which is allowed for
  * `int - int` result `int`
  * `float - float` result `float`
  * `ksp::math::Vec2 - ksp::math::Vec2` result `ksp::math::Vec2`
  * `ksp::math::Vec3 - ksp::math::Vec3` result `ksp::math::Vec3`
  * Note: You can not subtract `int` with `float`, one of them as to be converted via `.to_float` or `.to_int`

## Bit-And / Bit-Or / Bit-Xor: `&`, `|`, `^`

* `int` values can be combined with bit operations
  * `int | int` result `int`, bit-wise or
  * `int & int` result `int`, bit-wise and
  * `int ^ int` result `int`, bit-wise xor
* An optional value can be combined with `|` to provide a default value, see [Option](special_types/option.md) for details
* Two records can be merged with a `&`, see [Record](types.md#records)

## Create range: `..`, `...`

Ranges can be created from two `int` value.

* `a .. b` creates a range from `a` to `b` exclusively (i.e. `>= a` and `< b`)
* `a ... b` creates a range from `a` to `b` inclusively (i.e. `>= a` and `<= b`)

## Comparison: `==`, `!=`, `<`, `<=`, `>`, `>=`

* The result of a comparison is always a `bool`
* `==` and `!=` check the equality resp. not-equality of two values. The following types can be compared
  * `bool` with `bool`
  * `int` with `int`
  * `float` with `float`
  * `string` with `string`
  * `ksp::math::Vec2` with `ksp::math::Vec2`
  * `ksp::math::Vec3` with `ksp::math::Vec3`
* `<`, `<=`, `>`, `>=` compare the order of two values, i.e. less than, less of equal, greater than, greater or equal. The following types can be compared
  * `int` with `int`
  * `float` with `float`
  * `string` with `string` (standard string ordering)

## Boolean and / Boolean or: `&&`, `||`

* `&&` combines two `bool` with an and
* `||` combines two `bool` with an or

Note that this operator short-circuits if the result is already determined by the first boolean.
E.g. if you have a boolean value and a function returning a boolean:
```
fn some_func() -> bool = { ... }

let a : bool
```
then `a && some_func()` will not invoke `some_func` if `a` is `false`. Correspondingly `a || some_func()` will not invoke `some_func` if `a` is `true.