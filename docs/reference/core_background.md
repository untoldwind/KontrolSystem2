---
title: "core::background"
---

Provides means to run functions as asynchronous background task.


# Types


## Task

Represents a background task


### Fields

Name | Type | Description
--- | --- | ---
is_canceled | bool | Check if the task has been canceled 
is_completed | bool | Check if the task is completed 
is_success | bool | Check if the task is completed and has a value 
result | T | Get the result of the task once completed 

### Methods

#### cancel

```rust
task.cancel ( ) -> Unit
```

Cancel/abort the task


# Functions


## is_background

```rust
pub sync fn is_background ( ) -> bool
```

Check if current thread is a background thread


## run

```rust
pub sync fn run ( function : fn() -> T ) -> core::background::Task<T>
```

Run a function as background task.

