# core::error

Error reporting and error handling.


## Types


### Error

Error information of a failed Result.


#### Fields

| Name        | Type                                                             | Read-only | Description |
| ----------- | ---------------------------------------------------------------- | --------- | ----------- |
| message     | string                                                           | R/O       |             |
| stack_trace | [core::error::StackEntry](/reference/core/error.md#stackentry)[] | R/O       |             |


#### Methods

##### to_string

```rust
error.to_string ( ) -> string
```



### StackEntry

Stacktrace entry.


#### Fields

| Name        | Type     | Read-only | Description |
| ----------- | -------- | --------- | ----------- |
| arguments   | string[] | R/O       |             |
| line        | int      | R/O       |             |
| name        | string   | R/O       |             |
| source_name | string   | R/O       |             |


#### Methods

##### to_string

```rust
stackentry.to_string ( ) -> string
```



## Functions


### current_stack

```rust
pub sync fn current_stack ( ) -> core::error::StackEntry[]
```

Get current stack.

