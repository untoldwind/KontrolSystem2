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
mode | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/W | 
target_orientation | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W | 

### AutopilotMode

Vessel autopilot (SAS) mode

#### Methods

##### to_string

```rust
autopilotmode.to_string ( ) -> string
```

String representation of the number

### DeltaVSituation

Vessel situation for delta-v calculation

#### Methods

##### to_string

```rust
deltavsituation.to_string ( ) -> string
```

String representation of the number

### EngineDeltaV



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
engine_module | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine) | R/O | 
start_burn_stage | int | R/O | Number of the stage when engine is supposed to start 

#### Methods

##### get_ISP

```rust
enginedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the engine in a given `situation`


##### get_thrust

```rust
enginedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the engine in a given `situation`


##### get_thrust_vector

```rust
enginedeltav.get_thrust_vector ( situation : ksp::vessel::DeltaVSituation ) -> ksp::math::Vec3
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

### FlightCtrlState

Current state of the (pilots) flight controls.

#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
breaks | bool | R/W | Brakes
gear_down | bool | R/W | Gear down
gear_up | bool | R/W | Gear up
kill_rot | bool | R/W | Kill rotation
main_throttle | float | R/W | Setting for the main throttle (0 - 1)
pitch | float | R/W | Setting for pitch rotation (-1 - 1)
pitch_trim | float | R/W | Current trim value for pitch
roll | float | R/W | Setting for roll rotation (-1 - 1)
roll_trim | float | R/W | Current trim value for roll
stage | bool | R/W | Stage
wheel_steer | float | R/W | Setting for wheel steering (-1 - 1, applied to Rovers)
wheel_steer_trim | float | R/W | Current trim value for wheel steering
wheel_throttle | float | R/W | Setting for wheel throttle (0 - 1, applied to Rovers)
wheel_throttle_trim | float | R/W | Current trim value for wheel throttle
x | float | R/W | Setting for x-translation (-1 - 1)
y | float | R/W | Setting for y-translation (-1 - 1)
yaw | float | R/W | Setting for yaw rotation (-1 - 1)
yaw_trim | float | R/W | Current trim value for yaw
z | float | R/W | Setting for z-translation (-1 - 1)

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
chute_safety | [ksp::vessel::ParachuteSafeStates](/reference/ksp/vessel.md#parachutesafestates) | R/O | 
deploy_altitude | float | R/W | 
deploy_mode | [ksp::vessel::ParachuteDeployMode](/reference/ksp/vessel.md#parachutedeploymode) | R/W | 
deploy_state | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 
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

### ParachuteDeployMode

Parachute deploy mode

#### Methods

##### to_string

```rust
parachutedeploymode.to_string ( ) -> string
```

String representation of the number

### ParachuteDeployState

Parachute deploy state

#### Methods

##### to_string

```rust
parachutedeploystate.to_string ( ) -> string
```

String representation of the number

### ParachuteSafeStates

Parachute deploy safe states

#### Methods

##### to_string

```rust
parachutesafestates.to_string ( ) -> string
```

String representation of the number

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
stagedeltav.get_deltav ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated delta-v of the stage in a given `situation`


##### get_ISP

```rust
stagedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the stage in a given `situation`


##### get_thrust

```rust
stagedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the stage in a given `situation`


##### get_TWR

```rust
stagedeltav.get_TWR ( situation : ksp::vessel::DeltaVSituation ) -> float
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
body_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The body/rotating reference frame of the vessel. 
celestial_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The celestial/non-rotating reference frame of the vessel. 
CoM | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Position of the center of mass of the vessel. 
control_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | Reference frame for the current control position. 
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
global_center_of_mass | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Coordinate independent position of the center of mass. 
global_east | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_facing | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
global_north | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Coordinate independent position of the vessel. 
global_up | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_velocity | [ksp::math::GlobalVelocity](/reference/ksp/math.md#globalvelocity) | R/O | Get the coordinate independent velocity of the vessel. 
heading | float | R/O | 
horizon_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | Reference frame for the horizon at the current position of the vessel. 
horizontal_surface_speed | float | R/O | 
is_active | bool | R/O | Check if the vessel is currently active. 
mach_number | float | R/O | 
main_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | The main body of the current SOI the vessel is in. 
maneuver | [ksp::vessel::Maneuver](/reference/ksp/vessel.md#maneuver) | R/O |  
mass | float | R/O | Total mass of the vessel. 
name | string | R/O | The name of the vessel. 
north | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
offset_ground | float | R/O | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | Current orbit or orbit patch of the vessel. 
orbital_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Orbital velocity of the vessel relative to the main body. 
parts | [ksp::vessel::Part](/reference/ksp/vessel.md#part)[] | R/O | 
pitch_yaw_roll | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
situation | string | R/O | 
solar_panels | [ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)[] | R/O | 
sound_speed | float | R/O | 
staging | [ksp::vessel::Staging](/reference/ksp/vessel.md#staging) | R/O | 
static_pressure_kpa | float | R/O | 
surface_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Surface velocity of the vessel relative to the main body. 
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
vessel.manage_rcs_translate ( translateProvider : sync fn(float) -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



##### manage_steering

```rust
vessel.manage_steering ( pitchYawRollProvider : sync fn(float) -> ksp::math::Vec3 ) -> ksp::control::SteeringManager
```



##### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : sync fn(float) -> float ) -> ksp::control::ThrottleManager
```



##### manage_wheel_steering

```rust
vessel.manage_wheel_steering ( wheelSteeringProvider : sync fn(float) -> float ) -> ksp::control::WheelSteeringManager
```



##### manage_wheel_throttle

```rust
vessel.manage_wheel_throttle ( wheelThrottleProvider : sync fn(float) -> float ) -> ksp::control::WheelThrottleManager
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
MODE_ANTI_TARGET | ksp::vessel::AutopilotMode | 
MODE_ANTINORMAL | ksp::vessel::AutopilotMode | 
MODE_AUTOPILOT | ksp::vessel::AutopilotMode | 
MODE_MANEUVER | ksp::vessel::AutopilotMode | 
MODE_NAVIGATION | ksp::vessel::AutopilotMode | 
MODE_NORMAL | ksp::vessel::AutopilotMode | 
MODE_PROGRADE | ksp::vessel::AutopilotMode | 
MODE_RADIAL_IN | ksp::vessel::AutopilotMode | 
MODE_RADIAL_OUT | ksp::vessel::AutopilotMode | 
MODE_RETROGRADE | ksp::vessel::AutopilotMode | 
MODE_STABILITY_ASSIST | ksp::vessel::AutopilotMode | 
MODE_TARGET | ksp::vessel::AutopilotMode | 
PARACHUTE_MODE_IMMEDIATE | ksp::vessel::ParachuteDeployMode | 
PARACHUTE_MODE_RISKY | ksp::vessel::ParachuteDeployMode | 
PARACHUTE_MODE_SAFE | ksp::vessel::ParachuteDeployMode | 
PARACHUTE_SAFETY_NONE | ksp::vessel::ParachuteSafeStates | 
PARACHUTE_SAFETY_RISKY | ksp::vessel::ParachuteSafeStates | 
PARACHUTE_SAFETY_SAFE | ksp::vessel::ParachuteSafeStates | 
PARACHUTE_SAFETY_UNSAFE | ksp::vessel::ParachuteSafeStates | 
PARACHUTE_STATE_ARMED | ksp::vessel::ParachuteDeployState | 
PARACHUTE_STATE_CUT | ksp::vessel::ParachuteDeployState | 
PARACHUTE_STATE_DEPLOYED | ksp::vessel::ParachuteDeployState | 
PARACHUTE_STATE_SEMIDEPLOYED | ksp::vessel::ParachuteDeployState | 
PARACHUTE_STATE_STOWED | ksp::vessel::ParachuteDeployState | 
SITUATION_ALTITUDE | ksp::vessel::DeltaVSituation | 
SITUATION_SEA_LEVEL | ksp::vessel::DeltaVSituation | 
SITUATION_VACCUM | ksp::vessel::DeltaVSituation | 


## Functions


### active_vessel

```rust
pub sync fn active_vessel ( ) -> Result<ksp::vessel::Vessel, string>
```

Try to get the currently active vessel. Will result in an error if there is none.

