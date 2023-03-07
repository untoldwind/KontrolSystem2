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