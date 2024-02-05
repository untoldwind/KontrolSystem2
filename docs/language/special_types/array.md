# Arrays

An array type is defined by just adding adding a `[]` to an existing type:

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

An array can be initialized with a `[ element_list ]` notation:

```rust
let a : string[] = ["zero", "one", "two", "three"]
```
initializes the variable `a` with an array of string with the elements "zero", "one", "two", "three".

Elements of an array can be accessed with the usual index notation:

```rest
a[2]
```
will be the string "two".

## Properties

All arrays have a `length` property giving the the number of elements.

E.g.
```rust
["zero", "one", "two", "three"].length
```
will be `4`.

## Methods

### reverse

The `.reverse()` method just reverts the array.

E.g.
```
["zero", "one", "two", "three"].revert()
```
will be `["three", "two", "one", "zero"]`.

### map

The `map(converter)` function can be used to convert an array to another array, whereas `converter` is a generic function that converts each element into something else:

```rust
[0, 1, 2, 3].map(fn(i) -> i.to_string())
```
will be the array `["0", "1", "2", "3"]`.

```rust
["zero", "one", "two", "three"].map(fn(s) -> "the " + s)
```
will be `["the zero", "the one", "the two", "the three"]`

### map_with_index

`map_with_index` is similar to `map`, with the extension that `convert` also gets the current index.

```rust
["zero", "one", "two", "three"].map(fn(s, idx) -> s + " = " + idx.to_string())
```
will be `["zero = 0", "one = 1", "two = 2", "three = 3"]`

### find

`find(condition)` finds the fist element in the array satisfying a condition, whereas `condition` is a function taking an element and returning a bool. The result of `find` is an [Option](option.md).

```rust
["zero", "one", "two", "three"].find(fn(s) -> s == "one")
```
will be `Some("one")` (i.e. an `Option<string>` which is defined and has value "one")

```rust
["zero", "one", "two", "three"].find(fn(s) -> s == "something")
```
will be `None`.

### exists

`exists(condition)` just checks of one of the elements satisfies the given condition. It is a shorthand for `find(condition).defined`

### filter

`filter(condition)` creates a new array containing all the elements satisfying a given condition.

```rust
["zero", "one", "two", "three"].filter(fn(s) -> s != "one")
```
will be `["zero", "two", "three"]`.

