# ksp::game::warp

Collection of functions to control time warp.


## Functions


### cancel

```rust
pub sync fn cancel ( ) -> Unit
```

Cancel time warp


### current_index

```rust
pub sync fn current_index ( ) -> int
```

Get the current warp index. Actual factor depends on warp mode.


### current_rate

```rust
pub sync fn current_rate ( ) -> float
```

Get the current warp rate (i.e. actual time multiplier).


### warp_to

```rust
pub sync fn warp_to ( ut : float ) -> Unit
```

Warp forward to a specific universal time.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
