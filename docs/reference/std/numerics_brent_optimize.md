# std::numerics::brent_optimize



## Functions


### brent_optimize

```rust
pub sync fn brent_optimize ( func : sync fn(float) -> float,
                             start_a : float,
                             start_b : float,
                             tolerance : float,
                             max_iterations : int ) -> Result<(fx : float, x : float)>
```



Parameters

| Name           | Type                    | Optional | Description |
| -------------- | ----------------------- | -------- | ----------- |
| func           | sync fn(float) -> float |          |             |
| start_a        | float                   |          |             |
| start_b        | float                   |          |             |
| tolerance      | float                   |          |             |
| max_iterations | int                     |          |             |

