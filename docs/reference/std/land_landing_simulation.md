# std::land::landing_simulation



## Types


### BodyParameters



#### Fields

| Name                | Type                                                                                                               | Read-only | Description |
| ------------------- | ------------------------------------------------------------------------------------------------------------------ | --------- | ----------- |
| aerobraked_radius   | float                                                                                                              | R/W       |             |
| angular_velocity    | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                     | R/W       |             |
| celestial_frame     | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe)                                                 | R/W       |             |
| decel_radius        | float                                                                                                              | R/W       |             |
| epoch               | float                                                                                                              | R/W       |             |
| grav_parameter      | float                                                                                                              | R/W       |             |
| landing_radius      | float                                                                                                              | R/W       |             |
| lat0_lon0_at_start  | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                     | R/W       |             |
| lat90_at_start      | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                     | R/W       |             |
| lot0_lon90_at_start | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                     | R/W       |             |
| position            | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                     | R/W       |             |
| rotation_period     | float                                                                                                              | R/W       |             |
| speed_policy        | [sync fn(ksp::math::Vec3, ksp::math::Vec3) -> float](/reference/sync fn(ksp/math_Vec3, ksp_math.md#vec3) -> float) | R/W       |             |


#### Methods

##### find_freefall_end_time

```rust
bodyparameters.find_freefall_end_time ( orbit : ksp::orbit::Orbit,
                                        ut : float ) -> float
```



Parameters

| Name  | Type              | Optional | Description |
| ----- | ----------------- | -------- | ----------- |
| orbit | ksp::orbit::Orbit |          |             |
| ut    | float             |          |             |


##### freefall_ended

```rust
bodyparameters.freefall_ended ( orbit : ksp::orbit::Orbit,
                                ut : float ) -> bool
```



Parameters

| Name  | Type              | Optional | Description |
| ----- | ----------------- | -------- | ----------- |
| orbit | ksp::orbit::Orbit |          |             |
| ut    | float             |          |             |


##### grav_accel

```rust
bodyparameters.grav_accel ( pos : ksp::math::Vec3 ) -> ksp::math::Vec3
```



Parameters

| Name | Type            | Optional | Description |
| ---- | --------------- | -------- | ----------- |
| pos  | ksp::math::Vec3 |          |             |


##### reset

```rust
bodyparameters.reset ( body : ksp::orbit::Body ) -> Unit
```



Parameters

| Name | Type             | Optional | Description |
| ---- | ---------------- | -------- | ----------- |
| body | ksp::orbit::Body |          |             |


##### surface_position

```rust
bodyparameters.surface_position ( pos : ksp::math::Vec3,
                                  UT : float ) -> (latitude : float, longitude : float)
```



Parameters

| Name | Type            | Optional | Description |
| ---- | --------------- | -------- | ----------- |
| pos  | ksp::math::Vec3 |          |             |
| UT   | float           |          |             |


##### surface_velocity

```rust
bodyparameters.surface_velocity ( pos : ksp::math::Vec3,
                                  vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



Parameters

| Name | Type            | Optional | Description |
| ---- | --------------- | -------- | ----------- |
| pos  | ksp::math::Vec3 |          |             |
| vel  | ksp::math::Vec3 |          |             |


##### total_accel

```rust
bodyparameters.total_accel ( pos : ksp::math::Vec3,
                             vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



Parameters

| Name | Type            | Optional | Description |
| ---- | --------------- | -------- | ----------- |
| pos  | ksp::math::Vec3 |          |             |
| vel  | ksp::math::Vec3 |          |             |


### ReentrySimulation



#### Fields

| Name             | Type                                                                                                      | Read-only | Description |
| ---------------- | --------------------------------------------------------------------------------------------------------- | --------- | ----------- |
| body             | [std::land::landing_simulation::BodyParameters](/reference/std/land_landing_simulation.md#bodyparameters) | R/W       |             |
| deltav_expended  | float                                                                                                     | R/W       |             |
| dt               | float                                                                                                     | R/W       |             |
| max_thrust_accel | float                                                                                                     | R/W       |             |
| min_dt           | float                                                                                                     | R/W       |             |
| start_dt         | float                                                                                                     | R/W       |             |
| steps            | int                                                                                                       | R/W       |             |
| t                | float                                                                                                     | R/W       |             |
| v                | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                            | R/W       |             |
| x                | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                            | R/W       |             |


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



Parameters

| Name     | Type                | Optional | Description |
| -------- | ------------------- | -------- | ----------- |
| vessel   | ksp::vessel::Vessel |          |             |
| start_ut | float               |          |             |


##### run

```rust
reentrysimulation.run ( ) -> (brake_time : float, end_latitude : float, end_longitude : float, end_time : float, path : ksp::math::GlobalPosition[])
```



### ReentryTrajectory



#### Fields

| Name          | Type                                                                 | Read-only | Description |
| ------------- | -------------------------------------------------------------------- | --------- | ----------- |
| brake_time    | float                                                                | R/W       |             |
| end_latitude  | float                                                                | R/W       |             |
| end_longitude | float                                                                | R/W       |             |
| end_time      | float                                                                | R/W       |             |
| path          | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition)[] | R/W       |             |


## Functions


### BodyParameters

```rust
pub sync fn BodyParameters ( body : ksp::orbit::Body,
                             decel_end_altitude_asl : float,
                             landing_altitude_asl : float,
                             speed_policy : sync fn(ksp::math::Vec3, ksp::math::Vec3) -> float ) -> std::land::landing_simulation::BodyParameters
```



Parameters

| Name                   | Type                                               | Optional | Description |
| ---------------------- | -------------------------------------------------- | -------- | ----------- |
| body                   | ksp::orbit::Body                                   |          |             |
| decel_end_altitude_asl | float                                              |          |             |
| landing_altitude_asl   | float                                              |          |             |
| speed_policy           | sync fn(ksp::math::Vec3, ksp::math::Vec3) -> float |          |             |


### ReentrySimulation

```rust
pub sync fn ReentrySimulation ( body : std::land::landing_simulation::BodyParameters,
                                start_dt : float,
                                min_dt : float,
                                max_thrust_accel : float ) -> std::land::landing_simulation::ReentrySimulation
```



Parameters

| Name             | Type                                          | Optional | Description |
| ---------------- | --------------------------------------------- | -------- | ----------- |
| body             | std::land::landing_simulation::BodyParameters |          |             |
| start_dt         | float                                         |          |             |
| min_dt           | float                                         |          |             |
| max_thrust_accel | float                                         |          |             |


### init_simulation

```rust
pub sync fn init_simulation ( vessel : ksp::vessel::Vessel,
                              start_ut : float,
                              start_dt : float,
                              min_dt : float,
                              max_thrust_accel : float,
                              landing_altitude_asl : float,
                              speed_policy : sync fn(ksp::math::Vec3, ksp::math::Vec3) -> float ) -> std::land::landing_simulation::ReentrySimulation
```



Parameters

| Name                 | Type                                               | Optional | Description |
| -------------------- | -------------------------------------------------- | -------- | ----------- |
| vessel               | ksp::vessel::Vessel                                |          |             |
| start_ut             | float                                              |          |             |
| start_dt             | float                                              |          |             |
| min_dt               | float                                              |          |             |
| max_thrust_accel     | float                                              |          |             |
| landing_altitude_asl | float                                              |          |             |
| speed_policy         | sync fn(ksp::math::Vec3, ksp::math::Vec3) -> float |          |             |

