# std::numerics::regula_falsi_solve



## Functions


### regula_falsi_solve

```rust
pub sync fn regula_falsi_solve ( func : sync fn(float) -> float,
                                 start_a : float,
                                 start_b : float,
                                 tolerance : float,
                                 max_iterations : int ) -> float
```



Parameters

| Name           | Type                    | Optional | Description |
| -------------- | ----------------------- | -------- | ----------- |
| func           | sync fn(float) -> float |          |             |
| start_a        | float                   |          |             |
| start_b        | float                   |          |             |
| tolerance      | float                   |          |             |
| max_iterations | int                     |          |             |

