# Conditionals

## If

If conditions can be written as

```rust
if ( condition ) {
    ... expressions
}
```
or inline

```rust
if ( condition ) expression
```

Example

```rust
if(fb > fa) {
    let temp = ax
    ax = bx
    bx = temp
    temp = fa
    fa = fb
    fb = temp
}
```

## If/Else

If/else can be written as

```rust
if ( condition ) {
    ... expressions
} else {
    ... expressions
}
```
or inline

```rust
if ( condition ) expression else expression
```

This can then be combined into an if/elseif/else construct

```rust
if ( condition1 ) {
    ... expressions
} else if ( condition2 ) {
    ... expressions
} else {
    ... expressions
}
```

## Ternary

TO2 does not have a ternary expression, instead `if` and `if/else` are themselves an expression and have a result. I.e. it is possible to write

```rust
let a = if (b < 0) 12 else 34
```
where `a` will be the integer `12` if `b < 0` is true or otherwise the integer `34`.

If the else part is omitted the result becomes an [`Option`](./special_types.md#option).

```rust
let maybe_a = if (b < 0) 12
```
here `maybe_a` will be an `Option<int>` with the value `Some(12)` if `b < 0` is true  or `None` otherwise.
