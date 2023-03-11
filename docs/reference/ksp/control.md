# ksp::control



## Types


### MovingAverage



#### Fields

Name | Type | Description
--- | --- | ---
mean | float | 
mean_diff | float | 
sample_limit | int | 
value_count | int | 

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



### PIDLoop



#### Fields

Name | Type | Description
--- | --- | ---
change_rate | float | 
d_term | float | 
error | float | 
error_sum | float | 
extra_unwind | bool | 
i_term | float | 
input | float | 
kd | float | 
ki | float | 
kp | float | 
last_sample_time | float | 
max_output | float | 
min_output | float | 
output | float | 
p_term | float | 
setpoint | float | 

#### Methods

##### reset_i

```rust
pidloop.reset_i ( ) -> Unit
```



##### update

```rust
pidloop.update ( sampleTime : float,
                 input : float ) -> float
```



### RCSTranslateManager



#### Fields

Name | Type | Description
--- | --- | ---
translate | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 

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
rcstranslatemanager.set_translate_provider ( newTranslateProvider : fn() -> ksp::math::Vec3 ) -> Unit
```



### SteeringManager



#### Fields

Name | Type | Description
--- | --- | ---
direction | [ksp::math::Direction](/reference/ksp/math.md#direction) | 
max_stopping_time | float | 
pitch_torque_adjust | float | 
pitch_torque_factor | float | 
pitch_ts | float | 
roll_control_angle_range | float | 
roll_torque_adjust | float | 
roll_torque_factor | float | 
roll_ts | float | 
show_angular_vectors | bool | 
show_facing_vectors | bool | 
show_steering_stats | bool | 
yaw_torque_adjust | float | 
yaw_torque_factor | float | 
yaw_ts | float | 

#### Methods

##### release

```rust
steeringmanager.release ( ) -> Unit
```



##### reset_to_default

```rust
steeringmanager.reset_to_default ( ) -> Unit
```



##### resume

```rust
steeringmanager.resume ( ) -> Unit
```



##### set_direction_provider

```rust
steeringmanager.set_direction_provider ( newDirectionProvider : fn() -> ksp::math::Direction ) -> Unit
```



### ThrottleManager



#### Fields

Name | Type | Description
--- | --- | ---
throttle | float | 

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
throttlemanager.set_throttle_provider ( newThrottleProvider : fn(float) -> float ) -> Unit
```



### TorquePI



#### Fields

Name | Type | Description
--- | --- | ---
i | float | 
loop | [ksp::control::PIDLoop](/reference/ksp/control.md#pidloop) | 
tr | float | 
ts | float | 

#### Methods

##### reset_i

```rust
torquepi.reset_i ( ) -> Unit
```



##### update

```rust
torquepi.update ( sampleTime : float,
                  input : float,
                  setpoint : float,
                  momentOfInertia : float,
                  maxOutput : float ) -> float
```



## Functions


### moving_average

```rust
pub sync fn moving_average ( sampleLimit : int ) -> ksp::control::MovingAverage
```

Create a new MovingAverage with given sample limit.


### pid_loop

```rust
pub sync fn pid_loop ( kp : float,
                       ki : float,
                       kd : float,
                       minOutput : float,
                       maxOutput : float ) -> ksp::control::PIDLoop
```

Create a new PIDLoop with given parameters.

