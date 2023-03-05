# Functions

Functions are the bread and butter of every programming language, so we are trying to have as little boilerplate as possible.

## Definition

### Basic syntax

```rust
fn the_function_name(param1: type1, param2: type2) -> return_type {
    ... the implementation ...
}
```

as mentioned in the intro, this defines an asynchronous function that is safe to use (i.e. does not block the current thread).

If best performance is required (e.g. for some numerical stuff), it is also possible to define a synchronous function

```rust
sync fn the_function_name(param1: type1, param2: type2) -> return_type {
    ... the implementation ...
}
```
NOTE: Be somewhat careful with potentially long running loops here as it might impact the game.

By default functions can only be used within the same file. If a function should be callable from other scripts it is necessary to add a `pub` prefix:

```rust
pub fn the_function_name(param1: type1, param2: type2) -> return_type {
    ... the implementation ...
}
```
resp.
```rust
pub sync fn the_function_name(param1: type1, param2: type2) -> return_type {
    ... the implementation ...
}
```

### Examples

```rust
use { CONSOLE } from ksp::console

pub fn print_hello() -> Unit {
    CONSOLE.print_line("Hello world")
}
```
is a public function `print_hello` with no parameters and no return value that prints "Hello world" to the console.

```rust
fn add_three(a : int, b : float, c : float) -> float {
    a.to_float() + b + c
}
```
is a local function `add_three` that takes an integer and two floats, adds them all up and returns a float.

NOTE 1: The result of the final expression of a function is implicitly the result of the function (i.e. a `return` is not necessary)

NOTE 2: Adding an integer with a float is not allowed, i.e. the integer has to be explicitly converted to a float first via `.to_float()`. This prevents any ambiguity of implicit type conversion.

## Lamdbas (anonymous functions)

### Basic syntax

```rust
fn(param1: type1, param2: type2) -> expression
```

`expression` can be a simple expression or a block of code surrounded by `{` `}`, whereas the last expression of that block will be the return value.

Note that lambdas are synchronous by default. Currently there is no support for asynchronous lambdas.

### Examples

```rust
const add_three_lambda = fn(a : int, b : float, c : float) -> a.to_float() + b + c
```
the `add_three` function written as a lambda expression.

## Invoking

A function can be invoked in the regular fashon of most programming languages (expect the lisp-like crowd):

```rust
function_name(value1, value2)
```

