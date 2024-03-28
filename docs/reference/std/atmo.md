# std::atmo



## Functions


### atmo_launch

```rust
pub fn atmo_launch ( vessel : ksp::vessel::Vessel,
                     target_apoapsis : float,
                     heading : float,
                     low_turn : float,
                     high_turn : float ) -> Result<Unit>
```

Automatically launch a rocket from an atmosphere to a circular orbit.

Parameters

| Name            | Type                | Optional | Description |
| --------------- | ------------------- | -------- | ----------- |
| vessel          | ksp::vessel::Vessel |          |             |
| target_apoapsis | float               |          |             |
| heading         | float               |          |             |
| low_turn        | float               | x        |             |
| high_turn       | float               | x        |             |


### atmo_launch_ascent

```rust
pub fn atmo_launch_ascent ( vessel : ksp::vessel::Vessel,
                            target_apoapsis : float,
                            heading : float,
                            low_turn : float,
                            high_turn : float ) -> Unit
```



Parameters

| Name            | Type                | Optional | Description |
| --------------- | ------------------- | -------- | ----------- |
| vessel          | ksp::vessel::Vessel |          |             |
| target_apoapsis | float               |          |             |
| heading         | float               |          |             |
| low_turn        | float               | x        |             |
| high_turn       | float               | x        |             |

