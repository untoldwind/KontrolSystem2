# std::maneuvers



## Functions


### bi_impulsive_transfer

```rust
pub sync fn bi_impulsive_transfer ( start : ksp::orbit::Orbit,
                                    target : ksp::orbit::Orbit,
                                    min_UT : float,
                                    max_UT : float ) -> Result<(TT : float, UT : float, delta_v : ksp::math::Vec3)>
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |
| min_UT | float             |          |             |
| max_UT | float             | x        |             |


### bi_impulsive_transfer_body

```rust
pub sync fn bi_impulsive_transfer_body ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Body,
                                         min_UT : float,
                                         target_periapsis : float ) -> Result<(TT : float, UT : float, delta_v : ksp::math::Vec3)>
```



Parameters

| Name             | Type              | Optional | Description |
| ---------------- | ----------------- | -------- | ----------- |
| start            | ksp::orbit::Orbit |          |             |
| target           | ksp::orbit::Body  |          |             |
| min_UT           | float             |          |             |
| target_periapsis | float             |          |             |


### bi_impulsive_transfer_near

```rust
pub sync fn bi_impulsive_transfer_near ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float,
                                         TT : float ) -> Result<(TT : float, UT : float, delta_v : ksp::math::Vec3)>
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |
| UT     | float             |          |             |
| TT     | float             |          |             |


### change_apoapsis

```rust
pub sync fn change_apoapsis ( orbit : ksp::orbit::Orbit,
                              UT : float,
                              apoapsis_radius : float ) -> Result<ksp::math::Vec3>
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| orbit           | ksp::orbit::Orbit |          |             |
| UT              | float             |          |             |
| apoapsis_radius | float             |          |             |


### change_periapsis

```rust
pub sync fn change_periapsis ( orbit : ksp::orbit::Orbit,
                               UT : float,
                               periapsis_radius : float ) -> Result<ksp::math::Vec3>
```



Parameters

| Name             | Type              | Optional | Description |
| ---------------- | ----------------- | -------- | ----------- |
| orbit            | ksp::orbit::Orbit |          |             |
| UT               | float             |          |             |
| periapsis_radius | float             |          |             |


### cheapest_course_correction

```rust
pub sync fn cheapest_course_correction ( orbit : ksp::orbit::Orbit,
                                         min_UT : float,
                                         target : ksp::orbit::Orbit ) -> (UT : float, delta_v : ksp::math::Vec3)
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| orbit  | ksp::orbit::Orbit |          |             |
| min_UT | float             |          |             |
| target | ksp::orbit::Orbit |          |             |


### circularize_orbit

```rust
pub sync fn circularize_orbit ( orbit : ksp::orbit::Orbit ) -> Result<(UT : float, delta_v : ksp::math::Vec3)>
```

Calculate the required delta-v and time to change the given `orbit`
to a (mostly) circular orbit at the next apoapsis (if `orbit` is elliplic)
or periapsis (if `orbit` is hyperbolic).

Parameters

| Name  | Type              | Optional | Description |
| ----- | ----------------- | -------- | ----------- |
| orbit | ksp::orbit::Orbit |          |             |


### circularize_orbit_at

```rust
pub sync fn circularize_orbit_at ( orbit : ksp::orbit::Orbit,
                                   UT : float ) -> ksp::math::Vec3
```



Parameters

| Name  | Type              | Optional | Description |
| ----- | ----------------- | -------- | ----------- |
| orbit | ksp::orbit::Orbit |          |             |
| UT    | float             |          |             |


### circularize_orbit_pe

```rust
pub sync fn circularize_orbit_pe ( orbit : ksp::orbit::Orbit ) -> Result<(UT : float, delta_v : ksp::math::Vec3)>
```



Parameters

| Name  | Type              | Optional | Description |
| ----- | ----------------- | -------- | ----------- |
| orbit | ksp::orbit::Orbit |          |             |


### course_correction_body

```rust
pub sync fn course_correction_body ( start : ksp::orbit::Orbit,
                                     target : ksp::orbit::Body,
                                     UT : float,
                                     target_periapsis : float ) -> ksp::math::Vec3
```



Parameters

| Name             | Type              | Optional | Description |
| ---------------- | ----------------- | -------- | ----------- |
| start            | ksp::orbit::Orbit |          |             |
| target           | ksp::orbit::Body  |          |             |
| UT               | float             |          |             |
| target_periapsis | float             |          |             |


### ellipticize

```rust
pub sync fn ellipticize ( orbit : ksp::orbit::Orbit,
                          UT : float,
                          periapsis : float,
                          apoapsis : float ) -> ksp::math::Vec3
```



Parameters

| Name      | Type              | Optional | Description |
| --------- | ----------------- | -------- | ----------- |
| orbit     | ksp::orbit::Orbit |          |             |
| UT        | float             |          |             |
| periapsis | float             |          |             |
| apoapsis  | float             |          |             |


### ideal_ejection

```rust
pub sync fn ideal_ejection ( body : ksp::orbit::Body,
                             UT : float,
                             radius : float,
                             normal : ksp::math::Vec3,
                             exit_velocity : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```



Parameters

| Name          | Type             | Optional | Description |
| ------------- | ---------------- | -------- | ----------- |
| body          | ksp::orbit::Body |          |             |
| UT            | float            |          |             |
| radius        | float            |          |             |
| normal        | ksp::math::Vec3  |          |             |
| exit_velocity | ksp::math::Vec3  |          |             |


### intercept_at

```rust
pub sync fn intercept_at ( start : ksp::orbit::Orbit,
                           start_UT : float,
                           target : ksp::orbit::Orbit,
                           target_UT : float,
                           offset_distance : float ) -> (start_velocity : ksp::math::Vec3, target_velocity : ksp::math::Vec3)
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| start           | ksp::orbit::Orbit |          |             |
| start_UT        | float             |          |             |
| target          | ksp::orbit::Orbit |          |             |
| target_UT       | float             |          |             |
| offset_distance | float             | x        |             |


### match_apoapsis

```rust
pub sync fn match_apoapsis ( start : ksp::orbit::Orbit,
                             target : ksp::orbit::Orbit ) -> Result<(UT : float, delta_v : ksp::math::Vec3)>
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |


### match_inclination

```rust
pub sync fn match_inclination ( start : ksp::orbit::Orbit,
                                target : ksp::orbit::Orbit ) -> (UT : float, delta_v : ksp::math::Vec3)
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |


### match_periapsis

```rust
pub sync fn match_periapsis ( start : ksp::orbit::Orbit,
                              target : ksp::orbit::Orbit ) -> (UT : float, delta_v : ksp::math::Vec3)
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |


### match_velocities

```rust
pub sync fn match_velocities ( start : ksp::orbit::Orbit,
                               target : ksp::orbit::Orbit ) -> (UT : float, delta_v : ksp::math::Vec3)
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |


### next_closest_approach_time

```rust
pub sync fn next_closest_approach_time ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float ) -> float
```



Parameters

| Name   | Type              | Optional | Description |
| ------ | ----------------- | -------- | ----------- |
| start  | ksp::orbit::Orbit |          |             |
| target | ksp::orbit::Orbit |          |             |
| UT     | float             |          |             |

