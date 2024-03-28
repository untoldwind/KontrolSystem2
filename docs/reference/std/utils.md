# std::utils

Collection of helper functions not fitting anywhere else

## Functions


### angle_to_180

```rust
pub sync fn angle_to_180 ( angle : float ) -> float
```



Parameters

| Name  | Type  | Optional | Description |
| ----- | ----- | -------- | ----------- |
| angle | float |          |             |


### angle_to_360

```rust
pub sync fn angle_to_360 ( angle : float ) -> float
```



Parameters

| Name  | Type  | Optional | Description |
| ----- | ----- | -------- | ----------- |
| angle | float |          |             |


### global_ship_is_facing

```rust
pub sync fn global_ship_is_facing ( vessel : ksp::vessel::Vessel,
                                    desired_facing : ksp::math::GlobalVector,
                                    max_deviation_degrees : float,
                                    max_angular_velocity : float ) -> bool
```



Parameters

| Name                  | Type                    | Optional | Description |
| --------------------- | ----------------------- | -------- | ----------- |
| vessel                | ksp::vessel::Vessel     |          |             |
| desired_facing        | ksp::math::GlobalVector |          |             |
| max_deviation_degrees | float                   |          |             |
| max_angular_velocity  | float                   |          |             |


### remove_all_nodes

```rust
pub sync fn remove_all_nodes ( vessel : ksp::vessel::Vessel ) -> Unit
```



Parameters

| Name   | Type                | Optional | Description |
| ------ | ------------------- | -------- | ----------- |
| vessel | ksp::vessel::Vessel |          |             |


### ship_is_facing

```rust
pub sync fn ship_is_facing ( vessel : ksp::vessel::Vessel,
                             desired_facing : ksp::math::Vec3,
                             max_deviation_degrees : float,
                             max_angular_velocity : float ) -> bool
```

Determine if `vessel` is facing a given direction.

* `max_deviation_degrees` sets a limit how many degrees the angle may differ
* `max_angular_velocity` sets a limit how much the `vessel` may still be turning

Parameters

| Name                  | Type                | Optional | Description |
| --------------------- | ------------------- | -------- | ----------- |
| vessel                | ksp::vessel::Vessel |          |             |
| desired_facing        | ksp::math::Vec3     |          |             |
| max_deviation_degrees | float               |          |             |
| max_angular_velocity  | float               |          |             |

