# std::numerics::runge_kutta



## Functions


### rk23

```rust
pub sync fn rk23 ( accel : sync fn(float, ksp::math::Vec3, ksp::math::Vec3) -> ksp::math::Vec3,
                   end_condition : sync fn(float, ksp::math::Vec3, ksp::math::Vec3) -> bool,
                   start_t : float,
                   start_position : ksp::math::Vec3,
                   start_velocity : ksp::math::Vec3,
                   min_dt : float,
                   max_dt : float ) -> (position : ksp::math::Vec3, t : float, velocity : ksp::math::Vec3)[]
```



Parameters

| Name           | Type                                                                | Optional | Description |
| -------------- | ------------------------------------------------------------------- | -------- | ----------- |
| accel          | sync fn(float, ksp::math::Vec3, ksp::math::Vec3) -> ksp::math::Vec3 |          |             |
| end_condition  | sync fn(float, ksp::math::Vec3, ksp::math::Vec3) -> bool            |          |             |
| start_t        | float                                                               |          |             |
| start_position | ksp::math::Vec3                                                     |          |             |
| start_velocity | ksp::math::Vec3                                                     |          |             |
| min_dt         | float                                                               |          |             |
| max_dt         | float                                                               |          |             |

