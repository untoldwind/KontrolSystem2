# std::maneuvers



## Functions


### bi_impulsive_transfer

```rust
pub sync fn bi_impulsive_transfer ( start : ksp::orbit::Orbit,
                                    target : ksp::orbit::Orbit,
                                    min_UT : float,
                                    max_UT : float ) -> Result<(delta_v : ksp::math::Vec3, TT : float, UT : float), string>
```



### bi_impulsive_transfer_body

```rust
pub sync fn bi_impulsive_transfer_body ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Body,
                                         min_UT : float,
                                         target_periapsis : float ) -> Result<(delta_v : ksp::math::Vec3, TT : float, UT : float), string>
```



### bi_impulsive_transfer_near

```rust
pub sync fn bi_impulsive_transfer_near ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float,
                                         TT : float ) -> Result<(delta_v : ksp::math::Vec3, TT : float, UT : float), string>
```



### change_apoapsis

```rust
pub sync fn change_apoapsis ( orbit : ksp::orbit::Orbit,
                              UT : float,
                              apoapsis_radius : float ) -> Result<ksp::math::Vec3, string>
```



### change_periapsis

```rust
pub sync fn change_periapsis ( orbit : ksp::orbit::Orbit,
                               UT : float,
                               periapsis_radius : float ) -> Result<ksp::math::Vec3, string>
```



### cheapest_course_correction

```rust
pub sync fn cheapest_course_correction ( orbit : ksp::orbit::Orbit,
                                         min_UT : float,
                                         target : ksp::orbit::Orbit ) -> (delta_v : ksp::math::Vec3, UT : float)
```



### circularize_orbit

```rust
pub sync fn circularize_orbit ( orbit : ksp::orbit::Orbit ) -> Result<(delta_v : ksp::math::Vec3, UT : float), string>
```

Calculate the required delta-v and time to change the given `orbit`
to a (mostly) circular orbit at the next apoapsis (if `orbit` is elliplic)
or periapsis (if `orbit` is hyperbolic).

### circularize_orbit_at

```rust
pub sync fn circularize_orbit_at ( orbit : ksp::orbit::Orbit,
                                   UT : float ) -> ksp::math::Vec3
```



### circularize_orbit_pe

```rust
pub sync fn circularize_orbit_pe ( orbit : ksp::orbit::Orbit ) -> Result<(delta_v : ksp::math::Vec3, UT : float), string>
```



### course_correction_body

```rust
pub sync fn course_correction_body ( start : ksp::orbit::Orbit,
                                     target : ksp::orbit::Body,
                                     UT : float,
                                     target_periapsis : float ) -> ksp::math::Vec3
```



### ellipticize

```rust
pub sync fn ellipticize ( orbit : ksp::orbit::Orbit,
                          UT : float,
                          periapsis : float,
                          apoapsis : float ) -> ksp::math::Vec3
```



### ideal_ejection

```rust
pub sync fn ideal_ejection ( body : ksp::orbit::Body,
                             UT : float,
                             radius : float,
                             normal : ksp::math::Vec3,
                             exit_velocity : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```



### intercept_at

```rust
pub sync fn intercept_at ( start : ksp::orbit::Orbit,
                           start_UT : float,
                           target : ksp::orbit::Orbit,
                           target_UT : float,
                           offset_distance : float ) -> (start_velocity : ksp::math::Vec3, target_velocity : ksp::math::Vec3)
```



### match_apoapsis

```rust
pub sync fn match_apoapsis ( start : ksp::orbit::Orbit,
                             target : ksp::orbit::Orbit ) -> Result<(delta_v : ksp::math::Vec3, UT : float), string>
```



### match_inclination

```rust
pub sync fn match_inclination ( start : ksp::orbit::Orbit,
                                target : ksp::orbit::Orbit ) -> (delta_v : ksp::math::Vec3, UT : float)
```



### match_periapsis

```rust
pub sync fn match_periapsis ( start : ksp::orbit::Orbit,
                              target : ksp::orbit::Orbit ) -> (delta_v : ksp::math::Vec3, UT : float)
```



### match_velocities

```rust
pub sync fn match_velocities ( start : ksp::orbit::Orbit,
                               target : ksp::orbit::Orbit ) -> (delta_v : ksp::math::Vec3, UT : float)
```



### next_closest_approach_time

```rust
pub sync fn next_closest_approach_time ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float ) -> float
```


