# Loops

## While

Loops of the "while condition is true" kind can be written in the regular way:

```rust
while ( condition ) {
    ... expressions
}
```

Example:
```rust
let a : int = 0
let b : int = 0

while(a < 10) {
    a = a + 1
    b = b + a
}
```

## For

TO2 supports "for in" loops:

```rust
for(variable in collection) {
    ... expressions
}
```

If a simple "from 0 to n" loop is required one can use a [range](special_types/range.md) can be use as "collection":

```rust
let sum = 0

for(i in 0..10) {
    sum += i + 1
}
```

If the collection contains [tuples](types.md#tuples) or [records](types.md#records) a deconstruction can be used as well:

```rust
use { CONSOLE } from ksp::console

let arr = [(f: 1.0, s: "First"), (f: 2.2, s: "Second")]
let sum = 0.0

for((s, f) in arr) {
    CONSOLE.print_line(s)
    sum += f
}
```