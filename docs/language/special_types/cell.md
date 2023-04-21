# Cell

Variables in a function are local by nature.
But sometimes it is necessary (or just convenient) to have a value that is shared across multiple function or threads.

For this TO2 has the `Cell` wrapper type.

E.g.

```rust
Cell<int>              // a (memory) cell containing an integer
Cell<string>         // a (memory) cell containing a string
Cell<(int, string)>  // a (memory) cell containing a tuple of integer and string
```

## Creation

A cell can be created by using the builtin `Cell` functions (not to be confused the the type itself)

```rest
let cell_a = Cell(1234)
let cell_b = Cell("Hallo")
```
in which case

* `cell_a` is a cell containing the integer value `1234`
* `cell_b` is a cell containing the string "Hallo"

## Accessing the value

The current value of a cell can be access via the `value` property:

```rust
cell_a.value     // will be the interger 1234
cell_b.value     // will be the string "Hallo"
```

The other way round the value of the cell can be overwritten by using the same property

```rust
cell_a.value = 2345    // now cell_a will have the value 2345
cell_b.value = "World" // now cell_b will have the value "World"
```

Since cells call potentially be modified in multiple thread, calculating with cell values can be tricky.

E.g.

```rust
cell_a.value = cell_a.value + 1
```

has the ever so slight chance that `cell_a` is modified by a different thread just between `cell_a.value` has been read but the addition has not yet occured, which might lead to unpredictable behaviour.

To mitigate this the `Cell` type has an `update` method taking a function that converts the current value to the new value.

I.e.

```rust
cell.update( fn(current) -> current + 1 )
```
is the correct way to implement a thread-safe counter.
