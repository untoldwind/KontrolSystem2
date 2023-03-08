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
