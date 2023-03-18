# std::land::landing_simulation



## Types


### BodyParameters



#### Fields

Name | Type | Description
--- | --- | ---
aerobraked_radius | float | 
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
decel_radius | float | 
epoch | float | 
grav_parameter | float | 
landing_radius | float | 
lat0_lon0_at_start | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
lat90_at_start | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
lot0_lon90_at_start | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
rotation_period | float | 
speed_policy | [fn(ksp::math::Vec3, ksp::math::Vec3) -> float](/reference/fn(ksp/math_Vec3, ksp_math.md#vec3) -> float) | 

#### Methods

##### find_freefall_end_time

```rust
bodyparameters.find_freefall_end_time ( orbit : ksp::orbit::Orbit,
                                        ut : float ) -> float
```



##### freefall_ended

```rust
bodyparameters.freefall_ended ( orbit : ksp::orbit::Orbit,
                                ut : float ) -> bool
```



##### grav_accel

```rust
bodyparameters.grav_accel ( pos : ksp::math::Vec3 ) -> ksp::math::Vec3
```



##### reset

```rust
bodyparameters.reset ( body : ksp::orbit::Body ) -> Unit
```



##### surface_position

```rust
bodyparameters.surface_position ( pos : ksp::math::Vec3,
                                  UT : float ) -> (latitude : float, longitude : float)
```



##### surface_velocity

```rust
bodyparameters.surface_velocity ( pos : ksp::math::Vec3,
                                  vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



##### total_accel

```rust
bodyparameters.total_accel ( pos : ksp::math::Vec3,
                             vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



### ReentrySimulation



#### Fields

Name | Type | Description
--- | --- | ---
body | [std::land::landing_simulation::BodyParameters](/reference/std/land_landing_simulation.md#bodyparameters) | 
deltav_expended | float | 
dt | float | 
max_thrust_accel | float | 
min_dt | float | 
start_dt | float | 
steps | int | 
t | float | 
v | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
x | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 

#### Methods

##### bs34_step

```rust
reentrysimulation.bs34_step ( ) -> Unit
```



##### escaping

```rust
reentrysimulation.escaping ( ) -> bool
```



##### landed

```rust
reentrysimulation.landed ( ) -> bool
```



##### limit_speed

```rust
reentrysimulation.limit_speed ( ) -> Unit
```



##### reset

```rust
reentrysimulation.reset ( vessel : ksp::vessel::Vessel,
                          start_ut : float ) -> std::land::landing_simulation::ReentrySimulation
```



##### run

```rust
reentrysimulation.run ( ) -> (brake_time : float, end_latitude : float, end_longitude : float, end_time : float, path : ksp::math::Vec3[])
```



### ReentryTrajectory



#### Fields

Name | Type | Description
--- | --- | ---
brake_time | float | 
end_latitude | float | 
end_longitude | float | 
end_time | float | 
path | [ksp::math::Vec3](/reference/ksp/math.md#vec3)[] | 

## Functions


### init_simulation

```rust
pub sync fn init_simulation ( vessel : ksp::vessel::Vessel,
                              start_ut : float,
                              start_dt : float,
                              min_dt : float,
                              max_thrust_accel : float,
                              landing_altitude_asl : float,
                              speed_policy : fn(ksp::math::Vec3, ksp::math::Vec3) -> float ) -> std::land::landing_simulation::ReentrySimulation
```


