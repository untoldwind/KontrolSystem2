# ksp::game::warp

Collection of functions to control time warp.


## Functions


### cancel

```rust
pub fn cancel ( ) -> Unit
```

Deprecated: use cancel_warp()


### cancel_warp

```rust
pub sync fn cancel_warp ( ) -> Unit
```

Cancel time warp


### current_index

```rust
pub sync fn current_index ( ) -> int
```

Deprecated: Use current_warp_index()


### current_rate

```rust
pub sync fn current_rate ( ) -> float
```

Deprecated: Use current_warp_rate()


### current_warp_index

```rust
pub sync fn current_warp_index ( ) -> int
```

Get the current warp index. Actual factor depends on warp mode.


### current_warp_rate

```rust
pub sync fn current_warp_rate ( ) -> float
```

Get the current warp rate (i.e. actual time multiplier).


### get_warp_rates

```rust
pub sync fn get_warp_rates ( ) -> float[]
```

Get all available warp rates


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


### set_warp_index

```rust
pub sync fn set_warp_index ( index : int ) -> Unit
```

Set the current time warp index.


Parameters

| Name  | Type | Optional | Description |
| ----- | ---- | -------- | ----------- |
| index | int  |          |             |


### warp_to

```rust
pub sync fn warp_to ( ut : float ) -> Unit
```

Synchronized version of `warp_to`. Use with care.


Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| ut   | float |          |             |

