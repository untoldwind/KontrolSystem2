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


### is_physics_time_warp

```rust
pub sync fn is_physics_time_warp ( ) -> bool
```

Check if time warp is still in physics mode


### is_warping

```rust
pub sync fn is_warping ( ) -> bool
```

Check if time warp is currently active


### max_warp_index

```rust
pub sync fn max_warp_index ( ) -> int
```

Get current maximum allowed time warp index.


### set_time_wrap_index

```rust
pub sync fn set_time_wrap_index ( index : int ) -> bool
```

Set the current time warp index.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
index | int |  | 

### warp_to

```rust
pub fn warp_to ( ut : float ) -> Unit
```

Warp forward to a specific universal time.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
