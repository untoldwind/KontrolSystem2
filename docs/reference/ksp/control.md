# ksp::control



## Types


### MovingAverage



#### Fields

| Name             | Type  | Read-only | Description |
| ---------------- | ----- | --------- | ----------- |
| last_sample_time | float | R/W       |             |
| mean             | float | R/W       |             |
| mean_diff        | float | R/W       |             |
| sample_limit     | int   | R/W       |             |
| value_count      | int   | R/O       |             |


#### Methods

##### reset

```rust
movingaverage.reset ( ) -> Unit
```



##### update

```rust
movingaverage.update ( sampleTime : float,
                       value : float ) -> float
```



Parameters

| Name       | Type  | Optional | Description |
| ---------- | ----- | -------- | ----------- |
| sampleTime | float |          |             |
| value      | float |          |             |


##### update_delta

```rust
movingaverage.update_delta ( deltaT : float,
                             input : float ) -> float
```



Parameters

| Name   | Type  | Optional | Description |
| ------ | ----- | -------- | ----------- |
| deltaT | float |          |             |
| input  | float |          |             |


### PIDLoop



#### Fields

| Name             | Type  | Read-only | Description |
| ---------------- | ----- | --------- | ----------- |
| change_rate      | float | R/W       |             |
| d_term           | float | R/W       |             |
| error            | float | R/W       |             |
| error_sum        | float | R/W       |             |
| extra_unwind     | bool  | R/W       |             |
| i_term           | float | R/W       |             |
| input            | float | R/W       |             |
| kd               | float | R/W       |             |
| ki               | float | R/W       |             |
| kp               | float | R/W       |             |
| last_sample_time | float | R/W       |             |
| max_output       | float | R/W       |             |
| min_output       | float | R/W       |             |
| output           | float | R/W       |             |
| p_term           | float | R/W       |             |
| setpoint         | float | R/W       |             |


#### Methods

##### reset_i

```rust
pidloop.reset_i ( ) -> Unit
```

Reset the integral part of the PID loop


##### update

```rust
pidloop.update ( sampleTime : float,
                 input : float ) -> float
```



Parameters

| Name       | Type  | Optional | Description |
| ---------- | ----- | -------- | ----------- |
| sampleTime | float |          |             |
| input      | float |          |             |


##### update_delta

```rust
pidloop.update_delta ( deltaT : float,
                       input : float ) -> float
```



Parameters

| Name   | Type  | Optional | Description |
| ------ | ----- | -------- | ----------- |
| deltaT | float |          |             |
| input  | float |          |             |


### RCSTranslateManager



#### Fields

| Name      | Type                                           | Read-only | Description |
| --------- | ---------------------------------------------- | --------- | ----------- |
| translate | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W       |             |


#### Methods

##### release

```rust
rcstranslatemanager.release ( ) -> Unit
```



##### resume

```rust
rcstranslatemanager.resume ( ) -> Unit
```



##### set_translate_provider

```rust
rcstranslatemanager.set_translate_provider ( newTranslateProvider : sync fn(float) -> ksp::math::Vec3 ) -> Unit
```



Parameters

| Name                 | Type                              | Optional | Description |
| -------------------- | --------------------------------- | -------- | ----------- |
| newTranslateProvider | sync fn(float) -> ksp::math::Vec3 |          |             |


### SteeringManager



#### Fields

| Name           | Type                                           | Read-only | Description |
| -------------- | ---------------------------------------------- | --------- | ----------- |
| pitch_yaw_roll | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W       |             |


#### Methods

##### release

```rust
steeringmanager.release ( ) -> Unit
```



##### resume

```rust
steeringmanager.resume ( ) -> Unit
```



##### set_pitch_yaw_roll_provider

```rust
steeringmanager.set_pitch_yaw_roll_provider ( newPitchYawRollProvider : sync fn(float) -> ksp::math::Vec3 ) -> Unit
```



Parameters

| Name                    | Type                              | Optional | Description |
| ----------------------- | --------------------------------- | -------- | ----------- |
| newPitchYawRollProvider | sync fn(float) -> ksp::math::Vec3 |          |             |


### ThrottleManager



#### Fields

| Name     | Type  | Read-only | Description |
| -------- | ----- | --------- | ----------- |
| throttle | float | R/W       |             |


#### Methods

##### release

```rust
throttlemanager.release ( ) -> Unit
```



##### resume

```rust
throttlemanager.resume ( ) -> Unit
```



##### set_throttle_provider

```rust
throttlemanager.set_throttle_provider ( newThrottleProvider : sync fn(float) -> float ) -> Unit
```



Parameters

| Name                | Type                    | Optional | Description |
| ------------------- | ----------------------- | -------- | ----------- |
| newThrottleProvider | sync fn(float) -> float |          |             |


### TorquePI



#### Fields

| Name | Type                                                       | Read-only | Description |
| ---- | ---------------------------------------------------------- | --------- | ----------- |
| I    | float                                                      | R/W       |             |
| loop | [ksp::control::PIDLoop](/reference/ksp/control.md#pidloop) | R/W       |             |
| tr   | float                                                      | R/W       |             |
| ts   | float                                                      | R/W       |             |


#### Methods

##### reset_i

```rust
torquepi.reset_i ( ) -> Unit
```

Reset the integral part of the PID loop


##### update

```rust
torquepi.update ( sampleTime : float,
                  input : float,
                  setpoint : float,
                  momentOfInertia : float,
                  maxOutput : float ) -> float
```



Parameters

| Name            | Type  | Optional | Description |
| --------------- | ----- | -------- | ----------- |
| sampleTime      | float |          |             |
| input           | float |          |             |
| setpoint        | float |          |             |
| momentOfInertia | float |          |             |
| maxOutput       | float |          |             |


##### update_delta

```rust
torquepi.update_delta ( deltaT : float,
                        input : float,
                        setpoint : float,
                        momentOfInertia : float,
                        maxOutput : float ) -> float
```



Parameters

| Name            | Type  | Optional | Description |
| --------------- | ----- | -------- | ----------- |
| deltaT          | float |          |             |
| input           | float |          |             |
| setpoint        | float |          |             |
| momentOfInertia | float |          |             |
| maxOutput       | float |          |             |


### WheelSteeringManager



#### Fields

| Name        | Type  | Read-only | Description |
| ----------- | ----- | --------- | ----------- |
| wheel_steer | float | R/W       |             |


#### Methods

##### release

```rust
wheelsteeringmanager.release ( ) -> Unit
```



##### resume

```rust
wheelsteeringmanager.resume ( ) -> Unit
```



##### set_wheel_steer_provider

```rust
wheelsteeringmanager.set_wheel_steer_provider ( newWheelSteerProvider : sync fn(float) -> float ) -> Unit
```



Parameters

| Name                  | Type                    | Optional | Description |
| --------------------- | ----------------------- | -------- | ----------- |
| newWheelSteerProvider | sync fn(float) -> float |          |             |


### WheelThrottleManager



#### Fields

| Name           | Type  | Read-only | Description |
| -------------- | ----- | --------- | ----------- |
| wheel_throttle | float | R/W       |             |


#### Methods

##### release

```rust
wheelthrottlemanager.release ( ) -> Unit
```



##### resume

```rust
wheelthrottlemanager.resume ( ) -> Unit
```



##### set_wheel_throttle_provider

```rust
wheelthrottlemanager.set_wheel_throttle_provider ( newWheelThrottleProvider : sync fn(float) -> float ) -> Unit
```



Parameters

| Name                     | Type                    | Optional | Description |
| ------------------------ | ----------------------- | -------- | ----------- |
| newWheelThrottleProvider | sync fn(float) -> float |          |             |


## Functions


### moving_average

```rust
pub sync fn moving_average ( sampleLimit : int ) -> ksp::control::MovingAverage
```

Create a new MovingAverage with given sample limit.


Parameters

| Name        | Type | Optional | Description |
| ----------- | ---- | -------- | ----------- |
| sampleLimit | int  |          |             |


### pid_loop

```rust
pub sync fn pid_loop ( kp : float,
                       ki : float,
                       kd : float,
                       minOutput : float,
                       maxOutput : float,
                       extraUnwind : bool ) -> ksp::control::PIDLoop
```

Create a new PIDLoop with given parameters.


Parameters

| Name        | Type  | Optional | Description |
| ----------- | ----- | -------- | ----------- |
| kp          | float |          |             |
| ki          | float |          |             |
| kd          | float |          |             |
| minOutput   | float |          |             |
| maxOutput   | float |          |             |
| extraUnwind | bool  | x        |             |


### torque_pi

```rust
pub sync fn torque_pi ( ts : float ) -> ksp::control::TorquePI
```

Create a new TorquePI with given parameters.


Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| ts   | float |          |             |

