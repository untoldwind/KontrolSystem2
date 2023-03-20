
# Types

## Builtin

* `bool`
  * A boolean value
  * booleans have the following helper methods:
    * `.to_string()` converts the boolean to a `string`
* `int`
  * An integer value
  * Internally a 64-bit value is used, i.e. in C# this would be a `long`
  * integers have the following helper methods:
    * `.to_float` converts the integer to a `float`
    * `.to_string()` converts the integer to a `string`
* `float`
  * A floating point value
  * Internally a 64-bit value is used, i.e in C# this would be a `double`
  * floats have the following helper methods:
    * `.to_int` converts the float to an `int` (be truncating it)
    * `.to_string()` converts the float to a `string` (with all the decimals)
    * `.to_fixed(decimals)` converts the float to a `string` with number of decimals limited to `decimals`
* `string`
  * A string value
  * strings have the following helper methods:
    * `.length` will give the length of the string
    * `.contains("substring")` checks if the string contains "substring"
    * `.starts_with("prefix")` checks if the string starts with "prefix"
    * `.ends_with("suffix")` checks if the string ends with "suffix"
    * `.pad_left(number)` will left pad the string with spaces (does nothing if `number < length`)
    * `.pad_right(number` will right pad the string with spaces (does nothing if `number < length`)
    * `.to_lower()` will convert the string to lowercase
    * `.to_upper()` will convert the string to uppercase
* `Unit`
  * Something that is not relevant for processing.
  * Usually used to mark a function with no result, i.e. in C# this would be something like a `void`

## Arrays

An array type is defined by just adding add `[]`:

```rust
element_type[]
```

E.g.

```rust
string[]
```
is an array of strings.

```rust
int[][]
```
is an array of arrays of integers.

More details can be found [here](special_types/array.md).

## Tuples

A tuple is an ordered list of types.

```rust
(first_type, second_type)

// or

(first_type, second_type, third_type, fourth_type)
```

E.g.
```rust
(int, string)
```
is a pair (2-tuple) of an integer and a string

```rust
(bool, float, string)
```
is a triplet (3-tuple) of a boolean, a float and a string 

These are handy if a function has multiple result values.

E.g.
```rust
use { floor } from core::math

fn floor_remain(a : float) -> (int, float) = (floor(a).to_int, a - floor(a))
```
is a function that returns an integer and a float.

In assignments tuples can be deconstructed:

```rust
let (b, c) = floor_remain(12.34)
```
will define an integer variable `b` with value `12` and a float variable `c` with value `0.34`

## Records

Records are similar to tuples just with the addition that every element gets a unique name.

```rust
( name1: first_type, name2: second_type, name3: third_type )
```

E.g.

```rust
( ra: int, rb: string, rc: float )
```
is a record with three elements, where the first is an integer named `ra`, second is a string named `rb` and third is float named `rc`.

This way the example above may provide more details about the return value:

```rust
use { floor } from core::math

fn floor_remain(a : float) -> (base: int, remainder: float) = (base: floor(a).to_int, remainder: a - floor(a))
```

Like tuples records can be deconstructed as well. By default the names have to match exactly.

```rust
let (base, remainder) = floor_remain(12.34)
```
will define an integer variable `base` with value `12` and a float variable `remainder` with value `0.34`

If different variable names are desired this can be mapped by using an `@` operator.

```rust
let (b @ base, c @ remainder) = floor_remain(12.34)
```
will define an integer variable `b` with value `12` and a float variable `c` with value `0.34`


## Type aliases

A type alias is just a convenient shorthand for a potentially more complex types.

```rust
type alias_name = type definition
```

e.g.

```rust
type MyParameters = ( ra: int, rb: string, rc: float )
```
created an alias `MyParameters` for the record type defined above.

Like functions type aliases can be made available to other scripts with the `pub` keyword:

```rust
pub type MyParameters = ( ra: int, rb: string, rc: float )
```
