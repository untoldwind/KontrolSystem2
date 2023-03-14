# Range

In many cases is convenient to have an interval of integers. TO2 has two convenient shorthands for this:

* `min..max` a range of integers from `min` to `max` exclusive
    * E.g. `3..6` would be the numbers `[3, 4, 5]`
* `min...max` a range of integers from `min` to `max` inclusive
    * E.g. `3...6` would be the numbers `[3, 4, 5, 6]`

Most commonly this is used in [for loops](../loops.md#for):

```rust
let k = 0

for(i in 0..10) {
    k = k + 1
}
```

## Properties

A range has a `length` property. Therefor

```
(3..6).length
```

will be `3`

## Methods

A range has a `map(converter)` function that can be used to convert to into an array, whereas `converter` is a generic function that converts an `int` into something else:

```rust
(0..4).map(fn(i) -> i * 2)
```
will be the array `[0, 2, 4, 6]`.

```rust
(0..4).map(fn(i) -> i.to_string())
```
will be the array `["0", "1", "2", "3"]`.
