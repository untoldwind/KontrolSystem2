---
title: "ksp::vessel"
---

Collection of types and functions to get information and control in-game vessels.


# Types


## ActionGroups



### Fields

Name | Type | Description
--- | --- | ---
abort | bool | 
brakes | bool | 
gear | bool | 
light | bool | 
rcs | bool | 
sas | bool | 
solar_panels | bool | 

## Autopilot



### Fields

Name | Type | Description
--- | --- | ---
enabled | bool | 
mode | string | 
target_orientation | ksp::math::Vec3 | 

## Maneuver



### Fields

Name | Type | Description
--- | --- | ---
nodes | ksp::vessel::ManeuverNode[] | 

### Methods

#### add

```rust
maneuver.add ( ut : float,
               radialOut : float,
               normal : float,
               prograde : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



#### add_burn_vector

```rust
maneuver.add_burn_vector ( ut : float,
                           burnVector : ksp::math::Vec3 ) -> Result<ksp::vessel::ManeuverNode, string>
```



#### next_node

```rust
maneuver.next_node ( ) -> Result<ksp::vessel::ManeuverNode, string>
```



## ManeuverNode



### Fields

Name | Type | Description
--- | --- | ---
burn_vector | ksp::math::Vec3 | 
ETA | float | 
normal | float | 
prograde | float | 
radial_out | float | 
time | float | 

### Methods

#### remove

```rust
maneuvernode.remove ( ) -> Unit
```



## Targetable



### Fields

Name | Type | Description
--- | --- | ---
name | string | 
orbit | ksp::orbit::Orbit | 

## Vessel

Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.


### Fields

Name | Type | Description
--- | --- | ---
actions | ksp::vessel::ActionGroups | 
autopilot | ksp::vessel::Autopilot | 
CoM | ksp::math::Vec3 | 
control_status | string | 
maneuver | ksp::vessel::Maneuver | 
mass | float | 
name | string | The name of the vessel. 
orbit | ksp::orbit::Orbit | 
orbital_velocity | ksp::math::Vec3 | 
surface_velocity | ksp::math::Vec3 | 
target | Option<ksp::vessel::Targetable> | 

### Methods

#### manage_rcs_translate

```rust
vessel.manage_rcs_translate ( translateProvider : fn() -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



#### manage_steering

```rust
vessel.manage_steering ( directionProvider : fn() -> ksp::math::Direction ) -> ksp::control::SteeringManager
```



#### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : fn() -> float ) -> ksp::control::ThrottleManager
```



#### release_control

```rust
vessel.release_control ( ) -> Unit
```



#### set_rcs_translate

```rust
vessel.set_rcs_translate ( translate : ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



#### set_steering

```rust
vessel.set_steering ( direction : ksp::math::Direction ) -> ksp::control::SteeringManager
```



#### set_throttle

```rust
vessel.set_throttle ( throttle : float ) -> ksp::control::ThrottleManager
```


