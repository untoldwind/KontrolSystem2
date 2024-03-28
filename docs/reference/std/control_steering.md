# std::control::steering



## Types


### SteeringControl

Helper to use `SteeringController` with a `SteeringManager` of a vessel.
Use the `control_steering` function to set it up correctly.

#### Fields

| Name       | Type                                                                                                | Read-only | Description |
| ---------- | --------------------------------------------------------------------------------------------------- | --------- | ----------- |
| controller | [std::control::steering::SteeringController](/reference/std/control_steering.md#steeringcontroller) | R/W       |             |
| manager    | [ksp::control::SteeringManager](/reference/ksp/control.md#steeringmanager)                          | R/W       |             |


#### Methods

##### release

```rust
steeringcontrol.release ( ) -> Unit
```



##### resume

```rust
steeringcontrol.resume ( ) -> Unit
```



##### set_direction

```rust
steeringcontrol.set_direction ( dir : ksp::math::Direction ) -> Unit
```



Parameters

| Name | Type                 | Optional | Description |
| ---- | -------------------- | -------- | ----------- |
| dir  | ksp::math::Direction |          |             |


##### set_global_direction

```rust
steeringcontrol.set_global_direction ( dir : ksp::math::GlobalDirection ) -> Unit
```



Parameters

| Name | Type                       | Optional | Description |
| ---- | -------------------------- | -------- | ----------- |
| dir  | ksp::math::GlobalDirection |          |             |


##### set_heading

```rust
steeringcontrol.set_heading ( degrees_from_north : float,
                              pitch_above_horizon : float,
                              roll : float ) -> Unit
```



Parameters

| Name                | Type  | Optional | Description |
| ------------------- | ----- | -------- | ----------- |
| degrees_from_north  | float |          |             |
| pitch_above_horizon | float |          |             |
| roll                | float |          |             |


### SteeringController



#### Fields

| Name                     | Type                                                                   | Read-only | Description |
| ------------------------ | ---------------------------------------------------------------------- | --------- | ----------- |
| acc_pitch                | float                                                                  | R/W       |             |
| acc_roll                 | float                                                                  | R/W       |             |
| acc_yaw                  | float                                                                  | R/W       |             |
| adjust_torque            | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| angular_acceleration     | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| center_of_mass           | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| control_torque           | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| current_rot              | [ksp::math::Direction](/reference/ksp/math.md#direction)               | R/W       |             |
| enable_torque_adjust     | bool                                                                   | R/W       |             |
| max_pitch_omega          | float                                                                  | R/W       |             |
| max_roll_omega           | float                                                                  | R/W       |             |
| max_stopping_time        | float                                                                  | R/W       |             |
| max_yaw_omega            | float                                                                  | R/W       |             |
| measured_torque          | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| moment_of_inertia        | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| omega                    | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| phi                      | float                                                                  | R/W       |             |
| phi_pitch                | float                                                                  | R/W       |             |
| phi_roll                 | float                                                                  | R/W       |             |
| phi_yaw                  | float                                                                  | R/W       |             |
| pitch_pi                 | [ksp::control::TorquePI](/reference/ksp/control.md#torquepi)           | R/W       |             |
| pitch_rate_pi            | [ksp::control::PIDLoop](/reference/ksp/control.md#pidloop)             | R/W       |             |
| pitch_torque_adjust      | float                                                                  | R/W       |             |
| pitch_torque_calc        | [ksp::control::MovingAverage](/reference/ksp/control.md#movingaverage) | R/W       |             |
| pitch_torque_factor      | float                                                                  | R/W       |             |
| raw_torque               | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                         | R/W       |             |
| roll_control_angle_range | float                                                                  | R/W       |             |
| roll_pi                  | [ksp::control::TorquePI](/reference/ksp/control.md#torquepi)           | R/W       |             |
| roll_rate_pi             | [ksp::control::PIDLoop](/reference/ksp/control.md#pidloop)             | R/W       |             |
| roll_torque_adjust       | float                                                                  | R/W       |             |
| roll_torque_calc         | [ksp::control::MovingAverage](/reference/ksp/control.md#movingaverage) | R/W       |             |
| roll_torque_factor       | float                                                                  | R/W       |             |
| target_direction         | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection)   | R/W       |             |
| target_rot               | [ksp::math::Direction](/reference/ksp/math.md#direction)               | R/W       |             |
| tgt_pitch_omega          | float                                                                  | R/W       |             |
| tgt_pitch_torque         | float                                                                  | R/W       |             |
| tgt_roll_omega           | float                                                                  | R/W       |             |
| tgt_roll_torque          | float                                                                  | R/W       |             |
| tgt_yaw_omega            | float                                                                  | R/W       |             |
| tgt_yaw_torque           | float                                                                  | R/W       |             |
| vessel                   | [ksp::vessel::Vessel](/reference/ksp/vessel.md#vessel)                 | R/W       |             |
| yaw_pi                   | [ksp::control::TorquePI](/reference/ksp/control.md#torquepi)           | R/W       |             |
| yaw_rate_pi              | [ksp::control::PIDLoop](/reference/ksp/control.md#pidloop)             | R/W       |             |
| yaw_torque_adjust        | float                                                                  | R/W       |             |
| yaw_torque_calc          | [ksp::control::MovingAverage](/reference/ksp/control.md#movingaverage) | R/W       |             |
| yaw_torque_factor        | float                                                                  | R/W       |             |


#### Methods

##### print_debug

```rust
steeringcontroller.print_debug ( ) -> Unit
```



##### reset_i

```rust
steeringcontroller.reset_i ( ) -> Unit
```



##### update

```rust
steeringcontroller.update ( delta_t : float ) -> ksp::math::Vec3
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| delta_t | float |          |             |


##### update_control

```rust
steeringcontroller.update_control ( ) -> ksp::math::Vec3
```



##### update_prediction_pi

```rust
steeringcontroller.update_prediction_pi ( delta_t : float ) -> Unit
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| delta_t | float |          |             |


##### update_state_vectors

```rust
steeringcontroller.update_state_vectors ( delta_t : float ) -> Unit
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| delta_t | float |          |             |


##### update_torque

```rust
steeringcontroller.update_torque ( ) -> Unit
```



## Functions


### SteeringControl

```rust
pub sync fn SteeringControl ( manager : ksp::control::SteeringManager,
                              controller : std::control::steering::SteeringController ) -> std::control::steering::SteeringControl
```

Helper to use `SteeringController` with a `SteeringManager` of a vessel.
Use the `control_steering` function to set it up correctly.

Parameters

| Name       | Type                                       | Optional | Description |
| ---------- | ------------------------------------------ | -------- | ----------- |
| manager    | ksp::control::SteeringManager              |          |             |
| controller | std::control::steering::SteeringController |          |             |


### SteeringController

```rust
pub sync fn SteeringController ( vessel : ksp::vessel::Vessel,
                                 target_direction : ksp::math::GlobalDirection ) -> std::control::steering::SteeringController
```



Parameters

| Name             | Type                       | Optional | Description |
| ---------------- | -------------------------- | -------- | ----------- |
| vessel           | ksp::vessel::Vessel        |          |             |
| target_direction | ksp::math::GlobalDirection |          |             |


### control_steering

```rust
pub fn control_steering ( vessel : ksp::vessel::Vessel ) -> std::control::steering::SteeringControl
```

Control the steering of a vessel.
Example usage:
```rust
use { control_steering } from std::control::steering
...
const control = control_steering(vessel)
// vessel is now controlled and will keep its current rotation

control.set_heading(30, 10, 5) // change the desired rotation
sleep(5) // Give the controller time to steer the vessel
...
control.release() // release control of the vessel
```

Parameters

| Name   | Type                | Optional | Description |
| ------ | ------------------- | -------- | ----------- |
| vessel | ksp::vessel::Vessel |          |             |

