# ksp::vessel

Collection of types and functions to get information and control in-game vessels.


## Types


### ActionGroups



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
abort | bool | R/W | 
brakes | bool | R/W | 
custom1 | bool | R/W | 
custom10 | bool | R/W | 
custom2 | bool | R/W | 
custom3 | bool | R/W | 
custom4 | bool | R/W | 
custom5 | bool | R/W | 
custom6 | bool | R/W | 
custom7 | bool | R/W | 
custom8 | bool | R/W | 
custom9 | bool | R/W | 
gear | bool | R/W | 
light | bool | R/W | 
radiator_panels | bool | R/W | 
rcs | bool | R/W | 
sas | bool | R/W | 
solar_panels | bool | R/W | 

### Autopilot



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
enabled | bool | R/W | 
global_lock_direction | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/W | 
global_target_orientation | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/W | 
lock_direction | [ksp::math::Direction](/reference/ksp/math.md#direction) | R/W | 
mode | string | R/W | 
target_orientation | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W | 

### EngineDeltaV



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
engine_module | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine) | R/O | 
start_burn_stage | int | R/O | Number of the stage when engine is supposed to start 

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

Name | Type | Read-only | Description
--- | --- | --- | ---
allow_restart | bool | R/O | 
allow_shutdown | bool | R/O | 
engine_type | string | R/O | 
max_thrust | float | R/O | 
min_thrust | float | R/O | 
name | string | R/O | 
throttle_locked | bool | R/O | 

### Maneuver



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
nodes | [ksp::vessel::ManeuverNode](/reference/ksp/vessel.md#maneuvernode)[] | R/O | 

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

Name | Type | Read-only | Description
--- | --- | --- | ---
burn_duration | float | R/O | 
burn_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W | 
ETA | float | R/W | 
expected_orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | 
global_burn_vector | [ksp::math::GlobalVelocity](/reference/ksp/math.md#globalvelocity) | R/W | 
normal | float | R/W | 
orbit_patch | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | 
prograde | float | R/W | 
radial_out | float | R/W | 
time | float | R/W | 

#### Methods

##### remove

```rust
maneuvernode.remove ( ) -> Unit
```



### ModuleAirIntake



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
enabled | bool | R/O | 
resource_units | float | R/O | 

### ModuleDecoupler



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
ejection_impulse | float | R/W | 
is_decoupled | bool | R/O | 
part_name | string | R/O | 

#### Methods

##### decouple

```rust
moduledecoupler.decouple ( ) -> bool
```



### ModuleDeployable



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
deploy_state | string | R/O | 
extendable | bool | R/O | 
part_name | string | R/O | 
retractable | bool | R/O | 

#### Methods

##### set_extended

```rust
moduledeployable.set_extended ( extend : bool ) -> Unit
```



### ModuleDockingNode



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
docking_state | string | R/O | 
is_deployable_docking_port | bool | R/O | 

### ModuleEngine



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
current_engine_mode | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode) | R/O | 
current_throttle | float | R/O | 
current_thrust | float | R/O | 
engine_modes | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode)[] | R/O | 
has_ignited | bool | R/O | 
is_flameout | bool | R/O | 
is_operational | bool | R/O | 
is_shutdown | bool | R/O | 
is_staged | bool | R/O | 
max_fuel_flow | float | R/O | 
max_thrust_output_atm | float | R/O | 
max_thrust_output_vac | float | R/O | 
min_fuel_flow | float | R/O | 
part_name | string | R/O | 
throttle_min | float | R/O | 

#### Methods

##### change_mode

```rust
moduleengine.change_mode ( name : string ) -> bool
```



### ModuleFairing



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
ejection_force | float | R/W | 
is_jettisoned | bool | R/O | 
part_name | string | R/O | 

#### Methods

##### jettison

```rust
modulefairing.jettison ( ) -> bool
```



### ModuleLaunchClamp



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
is_released | bool | R/O | 
part_name | string | R/O | 

#### Methods

##### release

```rust
modulelaunchclamp.release ( ) -> bool
```



### ModuleParachute



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
armed | bool | R/W | 
chute_safety | string | R/O | 
deploy_altitude | float | R/W | 
deploy_mode | string | R/W | 
deploy_state | string | R/O | 
min_air_pressure | float | R/W | 

#### Methods

##### cut

```rust
moduleparachute.cut ( ) -> bool
```



##### deploy

```rust
moduleparachute.deploy ( ) -> bool
```



##### repack

```rust
moduleparachute.repack ( ) -> bool
```



### ModuleSolarPanel



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
blocking_body | Option&lt;[ksp::orbit::Body](/reference/ksp/orbit.md#body)> | R/O | 
energy_flow | float | R/O | 
part_name | string | R/O | 

### Part



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
air_intake | Option&lt;[ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)> | R/O | 
decoupler | Option&lt;[ksp::vessel::ModuleDecoupler](/reference/ksp/vessel.md#moduledecoupler)> | R/O | 
deployable | Option&lt;[ksp::vessel::ModuleDeployable](/reference/ksp/vessel.md#moduledeployable)> | R/O | 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | R/O | 
engine_module | Option&lt;[ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)> | R/O | 
fairing | Option&lt;[ksp::vessel::ModuleFairing](/reference/ksp/vessel.md#modulefairing)> | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | 
global_rotation | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
is_decoupler | bool | R/O | 
is_deployable | bool | R/O | 
is_engine | bool | R/O | 
is_fairing | bool | R/O | 
is_launch_clamp | bool | R/O | 
is_parachute | bool | R/O | 
is_solar_panel | bool | R/O | 
launch_clamp | Option&lt;[ksp::vessel::ModuleLaunchClamp](/reference/ksp/vessel.md#modulelaunchclamp)> | R/O | 
parachute | Option&lt;[ksp::vessel::ModuleParachute](/reference/ksp/vessel.md#moduleparachute)> | R/O | 
part_name | string | R/O | 
resources | [ksp::vessel::ResourceContainer](/reference/ksp/vessel.md#resourcecontainer) | R/O | 
solar_panel | Option&lt;[ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)> | R/O | 

### ResourceContainer



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
list | [ksp::vessel::ResourceData](/reference/ksp/vessel.md#resourcedata)[] | R/O | 

#### Methods

##### dump_all

```rust
resourcecontainer.dump_all ( ) -> Unit
```



### ResourceData



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
capacity_units | float | R/O | 
stored_units | float | R/O | 

### StageDeltaV



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
active_engines | [ksp::vessel::EngineDeltaV](/reference/ksp/vessel.md#enginedeltav)[] | R/O | 
burn_time | float | R/O | Estimated burn time of the stage. 
dry_mass | float | R/O | Dry mass of the stage. 
end_mass | float | R/O | End mass of the stage. 
engines | [ksp::vessel::EngineDeltaV](/reference/ksp/vessel.md#enginedeltav)[] | R/O | 
fuel_mass | float | R/O | Mass of the fuel in the stage. 
stage | int | R/O | The stage number. 
start_mass | float | R/O | Start mass of the stage. 

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

Name | Type | Read-only | Description
--- | --- | --- | ---
count | int | R/O | 
current | int | R/O | 
ready | bool | R/O | 
total_count | int | R/O | 

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

Name | Type | Read-only | Description
--- | --- | --- | ---
name | string | R/O | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | 

### Vessel

Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
actions | [ksp::vessel::ActionGroups](/reference/ksp/vessel.md#actiongroups) | R/O | 
air_intakes | [ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)[] | R/O | 
altitude_scenery | float | R/O | 
altitude_sealevel | float | R/O | 
altitude_terrain | float | R/O | 
angular_momentum | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
atmosphere_density | float | R/O | 
autopilot | [ksp::vessel::Autopilot](/reference/ksp/vessel.md#autopilot) | R/O | 
available_thrust | float | R/O | 
body_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
celestial_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
CoM | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
control_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
control_status | string | R/O | 
delta_v | [ksp::vessel::VesselDeltaV](/reference/ksp/vessel.md#vesseldeltav) | R/O | 
docking_nodes | [ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)[] | R/O | 
dynamic_pressure_kpa | float | R/O | 
east | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
engines | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)[] | R/O | 
facing | [ksp::math::Direction](/reference/ksp/math.md#direction) | R/O | 
geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | R/O | 
global_angular_momentum | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | 
global_angular_velocity | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | 
global_center_of_mass | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | 
global_east | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_facing | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
global_north | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | 
global_up | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_velocity | [ksp::math::GlobalVelocity](/reference/ksp/math.md#globalvelocity) | R/O | 
heading | float | R/O | 
horizontal_surface_speed | float | R/O | 
is_active | bool | R/O | 
mach_number | float | R/O | 
main_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | 
maneuver | [ksp::vessel::Maneuver](/reference/ksp/vessel.md#maneuver) | R/O | 
mass | float | R/O | 
name | string | R/O | The name of the vessel. 
north | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
offset_ground | float | R/O | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | 
orbital_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
parts | [ksp::vessel::Part](/reference/ksp/vessel.md#part)[] | R/O | 
pitch_horizon_relative | float | R/O | 
pitch_yaw_roll | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
roll_horizon_relative | float | R/O | 
situation | string | R/O | 
solar_panels | [ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)[] | R/O | 
sound_speed | float | R/O | 
staging | [ksp::vessel::Staging](/reference/ksp/vessel.md#staging) | R/O | 
static_pressure_kpa | float | R/O | 
surface_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
target | Option&lt;[ksp::vessel::Targetable](/reference/ksp/vessel.md#targetable)> | R/W | 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
vertical_speed | float | R/O | 
vertical_surface_speed | float | R/O | 

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

Name | Type | Read-only | Description
--- | --- | --- | ---
stages | [ksp::vessel::StageDeltaV](/reference/ksp/vessel.md#stagedeltav)[] | R/O | 

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

