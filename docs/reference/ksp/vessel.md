# ksp::vessel

Collection of types and functions to get information and control in-game vessels.


## Types


### ActionGroups



#### Fields

Name | Type | Description
--- | --- | ---
abort | bool | 
brakes | bool | 
custom1 | bool | 
custom10 | bool | 
custom2 | bool | 
custom3 | bool | 
custom4 | bool | 
custom5 | bool | 
custom6 | bool | 
custom7 | bool | 
custom8 | bool | 
custom9 | bool | 
gear | bool | 
light | bool | 
radiator_panels | bool | 
rcs | bool | 
sas | bool | 
solar_panels | bool | 

### Autopilot



#### Fields

Name | Type | Description
--- | --- | ---
enabled | bool | 
lock_direction | [ksp::math::Direction](/reference/ksp/math.md#direction) | 
mode | string | 
target_orientation | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 

### EngineDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
engine_module | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine) | 
start_burn_stage | int | Number of the stage when engine is supposed to start 

#### Methods

##### get_ISP

```rust
enginedeltav.get_ISP ( situation : string ) -> float
```

Estimated ISP of the engine in a given `situation`


##### get_thrust

```rust
enginedeltav.get_thrust ( situation : string ) -> float
```

Estimated thrust of the engine in a given `situation`


##### get_thrust_vector

```rust
enginedeltav.get_thrust_vector ( situation : string ) -> ksp::math::Vec3
```

Estimated thrust vector of the engine in a given `situation`


### EngineMode



#### Fields

Name | Type | Description
--- | --- | ---
allow_restart | bool | 
allow_shutdown | bool | 
engine_type | string | 
max_thrust | float | 
min_thrust | float | 
name | string | 
throttle_locked | bool | 

### Maneuver



#### Fields

Name | Type | Description
--- | --- | ---
nodes | [ksp::vessel::ManeuverNode](/reference/ksp/vessel.md#maneuvernode)[] | 

#### Methods

##### add

```rust
maneuver.add ( ut : float,
               radialOut : float,
               normal : float,
               prograde : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



##### add_burn_vector

```rust
maneuver.add_burn_vector ( ut : float,
                           burnVector : ksp::math::Vec3 ) -> Result<ksp::vessel::ManeuverNode, string>
```



##### next_node

```rust
maneuver.next_node ( ) -> Result<ksp::vessel::ManeuverNode, string>
```



### ManeuverNode



#### Fields

Name | Type | Description
--- | --- | ---
burn_duration | float | 
burn_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
ETA | float | 
expected_orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | 
normal | float | 
prograde | float | 
radial_out | float | 
time | float | 

#### Methods

##### remove

```rust
maneuvernode.remove ( ) -> Unit
```



### ModuleAirIntake



#### Fields

Name | Type | Description
--- | --- | ---
enabled | bool | 
resource_units | float | 

### ModuleDecoupler



#### Fields

Name | Type | Description
--- | --- | ---
ejection_impulse | float | 
is_decoupled | bool | 
part_name | string | 

#### Methods

##### decouple

```rust
moduledecoupler.decouple ( ) -> bool
```



### ModuleDeployable



#### Fields

Name | Type | Description
--- | --- | ---
deploy_state | string | 
extendable | bool | 
part_name | string | 
retractable | bool | 

#### Methods

##### set_extended

```rust
moduledeployable.set_extended ( extend : bool ) -> Unit
```



### ModuleDockingNode



#### Fields

Name | Type | Description
--- | --- | ---
docking_state | string | 
is_deployable_docking_port | bool | 

### ModuleEngine



#### Fields

Name | Type | Description
--- | --- | ---
current_engine_mode | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode) | 
current_throttle | float | 
current_thrust | float | 
engine_modes | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode)[] | 
has_ignited | bool | 
is_flameout | bool | 
is_operational | bool | 
is_shutdown | bool | 
is_staged | bool | 
max_fuel_flow | float | 
max_thrust_output_atm | float | 
max_thrust_output_vac | float | 
min_fuel_flow | float | 
part_name | string | 
throttle_min | float | 

#### Methods

##### change_mode

```rust
moduleengine.change_mode ( name : string ) -> bool
```



### ModuleFairing



#### Fields

Name | Type | Description
--- | --- | ---
ejection_force | float | 
is_deployed | bool | 
part_name | string | 

#### Methods

##### perform_jettison

```rust
modulefairing.perform_jettison ( ) -> bool
```



### ModuleLaunchClamp



#### Fields

Name | Type | Description
--- | --- | ---
is_released | bool | 
part_name | string | 

#### Methods

##### release

```rust
modulelaunchclamp.release ( ) -> bool
```



### ModuleSolarPanel



#### Fields

Name | Type | Description
--- | --- | ---
blocking_body | Option&lt;[ksp::orbit::Body](/reference/ksp/orbit.md#body)> | 
energy_flow | float | 
part_name | string | 

### Part



#### Fields

Name | Type | Description
--- | --- | ---
air_intake | Option&lt;[ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)> | 
decoupler | Option&lt;[ksp::vessel::ModuleDecoupler](/reference/ksp/vessel.md#moduledecoupler)> | 
deployable | Option&lt;[ksp::vessel::ModuleDeployable](/reference/ksp/vessel.md#moduledeployable)> | 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | 
engine_module | Option&lt;[ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)> | 
fairing | Option&lt;[ksp::vessel::ModuleFairing](/reference/ksp/vessel.md#modulefairing)> | 
is_decoupler | bool | 
is_deployable | bool | 
is_engine | bool | 
is_fairing | bool | 
is_launch_clamp | bool | 
is_solar_panel | bool | 
launch_clamp | Option&lt;[ksp::vessel::ModuleLaunchClamp](/reference/ksp/vessel.md#modulelaunchclamp)> | 
part_name | string | 
resources | [ksp::vessel::ResourceContainer](/reference/ksp/vessel.md#resourcecontainer) | 
solar_panel | Option&lt;[ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)> | 

### ResourceContainer



#### Fields

Name | Type | Description
--- | --- | ---
list | [ksp::vessel::ResourceData](/reference/ksp/vessel.md#resourcedata)[] | 

#### Methods

##### dump_all

```rust
resourcecontainer.dump_all ( ) -> Unit
```



### ResourceData



#### Fields

Name | Type | Description
--- | --- | ---
capacity_units | float | 
stored_units | float | 

### StageDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
active_engines | [ksp::vessel::EngineDeltaV](/reference/ksp/vessel.md#enginedeltav)[] | 
burn_time | float | Estimated burn time of the stage. 
dry_mass | float | Dry mass of the stage. 
end_mass | float | End mass of the stage. 
engines | [ksp::vessel::EngineDeltaV](/reference/ksp/vessel.md#enginedeltav)[] | 
fuel_mass | float | Mass of the fuel in the stage. 
stage | int | The stage number. 
start_mass | float | Start mass of the stage. 

#### Methods

##### get_deltav

```rust
stagedeltav.get_deltav ( situation : string ) -> float
```

Estimated delta-v of the stage in a given `situation`


##### get_ISP

```rust
stagedeltav.get_ISP ( situation : string ) -> float
```

Estimated ISP of the stage in a given `situation`


##### get_thrust

```rust
stagedeltav.get_thrust ( situation : string ) -> float
```

Estimated thrust of the stage in a given `situation`


##### get_TWR

```rust
stagedeltav.get_TWR ( situation : string ) -> float
```

Estimated TWR of the stage in a given `situation`


### Staging



#### Fields

Name | Type | Description
--- | --- | ---
count | int | 
current | int | 
ready | bool | 
total_count | int | 

#### Methods

##### next

```rust
staging.next ( ) -> bool
```



##### parts_in_stage

```rust
staging.parts_in_stage ( stage : int ) -> ksp::vessel::Part[]
```



### Targetable



#### Fields

Name | Type | Description
--- | --- | ---
name | string | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | 

### Vessel

Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.


#### Fields

Name | Type | Description
--- | --- | ---
actions | [ksp::vessel::ActionGroups](/reference/ksp/vessel.md#actiongroups) | 
air_intakes | [ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)[] | 
altitude_scenery | float | 
altitude_sealevel | float | 
altitude_terrain | float | 
angular_momentum | [ksp::math::Vector](/reference/ksp/math.md#vector) | 
angular_velocity | [ksp::math::Vector](/reference/ksp/math.md#vector) | 
atmosphere_density | float | 
autopilot | [ksp::vessel::Autopilot](/reference/ksp/vessel.md#autopilot) | 
available_thrust | float | 
CoM | [ksp::math::Position](/reference/ksp/math.md#position) | 
control_status | string | 
delta_v | [ksp::vessel::VesselDeltaV](/reference/ksp/vessel.md#vesseldeltav) | 
docking_nodes | [ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)[] | 
east | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
engines | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)[] | 
facing | [ksp::math::Direction](/reference/ksp/math.md#direction) | 
geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | 
heading | float | 
horizontal_surface_speed | float | 
is_active | bool | 
main_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | 
maneuver | [ksp::vessel::Maneuver](/reference/ksp/vessel.md#maneuver) | 
mass | float | 
name | string | The name of the vessel. 
north | [ksp::math::Vector](/reference/ksp/math.md#vector) | 
offset_ground | float | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | 
orbital_velocity | [ksp::math::Vector](/reference/ksp/math.md#vector) | 
parts | [ksp::vessel::Part](/reference/ksp/vessel.md#part)[] | 
pitch_horizon_relative | float | 
pitch_yaw_roll | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
roll_horizon_relative | float | 
position | [ksp::math::Position](/reference/ksp/math.md#position) | 
reference_frame | [ksp::math::CoordinateSystem](/reference/ksp/math.md#coordinatesystem) | 
situation | string | 
solar_panels | [ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)[] | 
staging | [ksp::vessel::Staging](/reference/ksp/vessel.md#staging) | 
surface_velocity | [ksp::math::Vector](/reference/ksp/math.md#vector) | 
target | Option&lt;[ksp::vessel::Targetable](/reference/ksp/vessel.md#targetable)> | 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
vertical_speed | float | 
vertical_surface_speed | float | 

#### Methods

##### heading_direction

```rust
vessel.heading_direction ( degreesFromNorth : float,
                           pitchAboveHorizon : float,
                           roll : float ) -> ksp::math::Direction
```



##### manage_rcs_translate

```rust
vessel.manage_rcs_translate ( translateProvider : fn(float) -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



##### manage_steering

```rust
vessel.manage_steering ( pitchYawRollProvider : fn(float) -> ksp::math::Vec3 ) -> ksp::control::SteeringManager
```



##### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : fn(float) -> float ) -> ksp::control::ThrottleManager
```



##### manage_wheel_steering

```rust
vessel.manage_wheel_steering ( wheelSteeringProvider : fn(float) -> float ) -> ksp::control::WheelSteeringManager
```



##### manage_wheel_throttle

```rust
vessel.manage_wheel_throttle ( wheelThrottleProvider : fn(float) -> float ) -> ksp::control::WheelThrottleManager
```



##### override_input_pitch

```rust
vessel.override_input_pitch ( value : float ) -> Unit
```



##### override_input_roll

```rust
vessel.override_input_roll ( value : float ) -> Unit
```



##### override_input_translate_x

```rust
vessel.override_input_translate_x ( value : float ) -> Unit
```



##### override_input_translate_y

```rust
vessel.override_input_translate_y ( value : float ) -> Unit
```



##### override_input_translate_z

```rust
vessel.override_input_translate_z ( value : float ) -> Unit
```



##### override_input_yaw

```rust
vessel.override_input_yaw ( value : float ) -> Unit
```



##### release_control

```rust
vessel.release_control ( ) -> Unit
```



##### set_rcs_translate

```rust
vessel.set_rcs_translate ( translate : ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



##### set_steering

```rust
vessel.set_steering ( pitchYawRoll : ksp::math::Vec3 ) -> ksp::control::SteeringManager
```



##### set_throttle

```rust
vessel.set_throttle ( throttle : float ) -> ksp::control::ThrottleManager
```



##### set_wheel_steering

```rust
vessel.set_wheel_steering ( wheelSteering : float ) -> ksp::control::WheelSteeringManager
```



##### set_wheel_throttle

```rust
vessel.set_wheel_throttle ( wheelThrottle : float ) -> ksp::control::WheelThrottleManager
```



### VesselDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
stages | [ksp::vessel::StageDeltaV](/reference/ksp/vessel.md#stagedeltav)[] | 

#### Methods

##### stage

```rust
vesseldeltav.stage ( stage : int ) -> Option<ksp::vessel::StageDeltaV>
```

Get delta-v information for a specific `stage` of the vessel, if existent.


## Constants

Name | Type | Description
--- | --- | ---
MODE_ANTINORMAL | string | 
MODE_ANTITARGET | string | 
MODE_AUTOPILOT | string | 
MODE_MANEUVER | string | 
MODE_NAVIGATION | string | 
MODE_NORMAL | string | 
MODE_PROGRADE | string | 
MODE_RADIALIN | string | 
MODE_RADIALOUT | string | 
MODE_RETROGRADE | string | 
MODE_STABILITYASSIST | string | 
MODE_TARGET | string | 
SITUATION_ALTITUDE | string | Used for delta-v calculation at the current altitude. 
SITUATION_SEALEVEL | string | Used for delta-v calculation at sea level of the current body. 
SITUATION_VACUUM | string | Used for delta-v calculation in vacuum. 


## Functions


### active_vessel

```rust
pub sync fn active_vessel ( ) -> Result<ksp::vessel::Vessel, string>
```

Try to get the currently active vessel. Will result in an error if there is none.

