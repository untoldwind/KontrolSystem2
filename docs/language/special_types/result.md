# Result

Things will never go as planed and loosely speaking one has to deal with two types of situations: The things you can deal with and the thing you can not. (Mindblowing isn't it?)

The latter are usually stuff that is pretty much out of your control entirely. In programmers terms this usually leads to some sort of system exception that will crash the process (maybe by proxy since the entire system crashes).

The former are more interesting as these kind of situations could be categories into:

* Things one is not aware of and are therefore not dealt with
* Things one is aware of, but are to hard to be dealt with properly
* Things that actually are dealt with properly

Obviously the last category should be maximized. An essential part for this to raise awareness and make it slightly inconvenient to just ignore potential error cases.

For this TO2 has `Result` wrapper type that encapsulates a successful value and an error value.

E.g.

```rust
Result<int, string>           // if successful an integer, on error a string (containing some sort of message)
Result<string, string>        // if successful a string value, on error also a string but containing some sort of message
Result<(int, string), string> // tuple of integer and string ...
```

Note: In almost all cases a `string` is used in the error-case, potentially this should be replaced with something more sophisticated.

This allows function to return a value that might not be available for some specific reason. E.g. [`vessel.maneuver.next_node()`](../../reference/ksp/vessel.md#next_node) might not have a result because there just is no maneuvering node for that vessel, or it is in the past, or some other reason

## Creation

The simplest way to create/instantiate an optional value is to use the builtin `Ok` and `Err` functions:

```rust
let result_a : Result<int, string> = Ok(2345)
let result_b : Result<int, string> = Err("just not there")
```
in which case

* `result_a` is an integer result that has the value `2345` (i.e. successful)
* `result_a` is an integer that has the error message "just not there" (i.e. not successful)

Alternatively an [`Option`](option.md) can converted to the result via the `ok_or` method by providing an error message for the `None` case:

```rust
let maybe_a : Option<int> = Some(2345)
let maybe_b : Option<int> = None()

let result_a = maybe_a.ok_ok("just not there")
let result_b = maybe_b.ok_ok("just not there")
```
creates the same as above.

## Accessing the value

`Result` has the following properties to access the value directly:

* `result_value.success` a bool indicating if the result was successful
* `result_value.value` the actual value, if the result was successful
* `result_value.error` the error value, if the result was not successful
    * Be careful using either of these! At the every least wrap it in an if condition

```rust
if (result_value.success) {
  result_value.value   // use it 
} else {
  result_value.error   // maybe log the error
}
```

A more convenient way is to use the `?` operator.

```rust
let a = result_value?
```

is a shorthand for

```rust
if ( !result_value.success ) {
   return Err(result.error)
}
let a = result_value.value
```

Obviously this only works if the surrounding function also returns a result of some kind, otherwise the `?` operator will produce a compilation error
