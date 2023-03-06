# ksp::game

Collection to game and runtime related functions.


## Functions


### current_time

```rust
pub sync fn current_time ( ) -> float
```

Get the current universal time (UT) in seconds from start.


### sleep

```rust
pub fn sleep ( seconds : float ) -> Unit
```

Stop execution of given number of seconds (factions of a seconds are supported as well).


### wait_until

```rust
pub fn wait_until ( predicate : fn() -> bool ) -> Unit
```

Stop execution until a given condition is met.


### wait_while

```rust
pub fn wait_while ( predicate : fn() -> bool ) -> Unit
```

Stop execution as long as a given condition is met.


### yield

```rust
pub fn yield ( ) -> Unit
```

Yield execution to allow Unity to do some other stuff inbetween.

