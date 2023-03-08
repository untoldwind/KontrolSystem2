# Option

Some times values are optional, e.g. an expression like `array_of_string.find(fn(s) -> s == "hallo")` may or may not have a result.

To handle these there is an `Option<type>` wrapper.

E.g.
```rust
Option<int>            // an optional integer
Option<string>         // an optional string
Option<(int, string)>  // an optional tuple of integer and string
```

## Creation

The simplest way to create/instantiate an optional value is to use the builtin `Some` and `None` functions:

```rust
let maybe_a : Option<int> = Some(2345)
let maybe_b : Option<int> = None()
```
in which case

* `maybe_a` is an optional integer that has the value `2345` (i.e. is defined)
* `maybe_b` is an optional integer that has no value (i.e. is not defined)

Alternative an optional value can be create as the result of an [if-expression](conditionals.md#ternary)

```
let maybe_a = if (b < 0) 2345
```
in which case

* if `b < 0` is true `maybe_a` will be an optional integer with value `2345`
* if `b < 0` is not true `maybe_a` will be an optional integer with no value

## Accessing the value

`Option` has the following properties to access the value directly:

* `optional_value.defined` a bool indicating if there is a value or not
* `optional_value.value` the actual value, if there is one
  * Be careful using this! Accessing the value of an option with `defined == false` will most likely lead to unexpected behaviour. At the very least wrap this in an if condition like
```rust
if (optional_value.defined) {
  optional.value  // use it in some way
}
```

A better way to access an optional value is to use the `|` operator:

```rust
let maybe_a : Option<int> = Some(2345)
let maybe_b : Option<int> = None()

maybe_a | 1234    // will be integer 2345 since maybe_a is defined
maybe_b | 1234    // will be integer 1234 since maybe_b is not defined
```
