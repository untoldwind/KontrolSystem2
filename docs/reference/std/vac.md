# std::vac



## Functions


### estimate_burn_time

```rust
pub sync fn estimate_burn_time ( vessel : ksp::vessel::Vessel,
                                 delta_v : float,
                                 stage_delay : float,
                                 throttle_limit : float ) -> (burn_time : float, half_burn_time : float)
```



Parameters

| Name           | Type                | Optional | Description |
| -------------- | ------------------- | -------- | ----------- |
| vessel         | ksp::vessel::Vessel |          |             |
| delta_v        | float               |          |             |
| stage_delay    | float               |          |             |
| throttle_limit | float               |          |             |


### exec_next_node

```rust
pub fn exec_next_node ( vessel : ksp::vessel::Vessel ) -> Result<Unit>
```

Execute the next planed maneuver node.

Will result in an error if there are no planed maneuver nodes.

Parameters

| Name   | Type                | Optional | Description |
| ------ | ------------------- | -------- | ----------- |
| vessel | ksp::vessel::Vessel |          |             |

