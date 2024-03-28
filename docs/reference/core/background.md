# core::background

Provides means to run functions as asynchronous background task.


## Types


### Task

Represents a background task


#### Fields

| Name         | Type | Read-only | Description                                     |
| ------------ | ---- | --------- | ----------------------------------------------- |
| is_canceled  | bool | R/O       | Check if the task has been canceled             |
| is_completed | bool | R/O       | Check if the task is completed                  |
| is_success   | bool | R/O       | Check if the task is completed and has a value  |
| result       | T    | R/O       | Get the result of the task once completed       |


#### Methods

##### cancel

```rust
task.cancel ( ) -> Unit
```

Cancel/abort the task


##### wait_complete

```rust
task.wait_complete ( ) -> T
```

Asynchronously wait for background task to complete


## Functions


### is_background

```rust
pub sync fn is_background ( ) -> bool
```

Check if current thread is a background thread


### run

```rust
pub sync fn run ( function : sync fn() -> T ) -> core::background::Task<T>
```

Run a function as background task.


Parameters

| Name     | Type           | Optional | Description |
| -------- | -------------- | -------- | ----------- |
| function | sync fn() -> T |          |             |

