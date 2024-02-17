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
science | bool | R/W | 
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

### AutopilotModeConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
AntiTarget | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the vector pointing away from its target (if a target is set).
Antinormal | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the anti-normal vector of its orbit.
Autopilot | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the `vessel.autopilot.target_orientation` vector. (probably no difference to AutopilotMode.Navigation)
Maneuver | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the burn vector of the next maneuver node (if a maneuver node exists).
Navigation | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the `vessel.autopilot.target_orientation` vector.
Normal | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the normal vector of its orbit.
Prograde | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the prograde vector of its orbit.
RadialIn | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the radial-in vector of its orbit.
RadialOut | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the radial-out vector of its orbit.
Retrograde | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the retrograde vector of its orbit.
StabilityAssist | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Stability assist mode. The autopilot tries to stop the rotation of the vessel. 
Target | [ksp::vessel::AutopilotMode](/reference/ksp/vessel.md#autopilotmode) | R/O | Align the vessel to the vector pointing to its target (if a target is set).

#### Methods

##### from_string

```rust
autopilotmodeconstants.from_string ( value : string ) -> Option<ksp::vessel::AutopilotMode>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### CommandControlState

Current state of a command module

#### Methods

##### to_string

```rust
commandcontrolstate.to_string ( ) -> string
```

String representation of the number

### CommandControlStateConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Disabled | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module disabled.
FullyFunctional | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module is functional.
Hibernating | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module is hibernating.
NoCommNetConnection | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module has no comm net connection.
None | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | 
NotEnoughCrew | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module has not enough crew.
NotEnoughResources | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | Command module has not resource crew.

#### Methods

##### from_string

```rust
commandcontrolstateconstants.from_string ( value : string ) -> Option<ksp::vessel::CommandControlState>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### DeltaVSituation

Vessel situation for delta-v calculation

#### Methods

##### to_string

```rust
deltavsituation.to_string ( ) -> string
```

String representation of the number

### DeltaVSituationConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Altitude | [ksp::vessel::DeltaVSituation](/reference/ksp/vessel.md#deltavsituation) | R/O | Calculate delta-v at the current altitude of the vessel.
SeaLevel | [ksp::vessel::DeltaVSituation](/reference/ksp/vessel.md#deltavsituation) | R/O | Calculate delta-v at sea level of the current main body.
Vaccum | [ksp::vessel::DeltaVSituation](/reference/ksp/vessel.md#deltavsituation) | R/O | Calculate delta-v in vaccum.

#### Methods

##### from_string

```rust
deltavsituationconstants.from_string ( value : string ) -> Option<ksp::vessel::DeltaVSituation>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### DockingState

Current state of a docking node

#### Methods

##### to_string

```rust
dockingstate.to_string ( ) -> string
```

String representation of the number

### DockingStateConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Acquire_Dockee | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
Acquire_Docker | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
Disengaged | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
Docked | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
None | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
Ready | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 

#### Methods

##### from_string

```rust
dockingstateconstants.from_string ( value : string ) -> Option<ksp::vessel::DockingState>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

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


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_thrust

```rust
enginedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the engine in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_thrust_vector

```rust
enginedeltav.get_thrust_vector ( situation : ksp::vessel::DeltaVSituation ) -> ksp::math::Vec3
```

Estimated thrust vector of the engine in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

### EngineMode



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
allow_restart | bool | R/O | 
allow_shutdown | bool | R/O | 
engine_type | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
max_thrust | float | R/O | 
min_thrust | float | R/O | 
name | string | R/O | 
throttle_locked | bool | R/O | 

### EngineType

Engine types

#### Methods

##### to_string

```rust
enginetype.to_string ( ) -> string
```

String representation of the number

### EngineTypeConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Antimatter | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
Electric | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
Generic | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Generic engine type (not specified)
Helium3 | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
MetallicHydrogen | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
Methalox | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Methan-oxigene rocket engine
MonoProp | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Mono-propellant engine
Nuclear | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Nuclear engine
NuclearSaltwater | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
Piston | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
ScramJet | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | 
SolidBooster | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Engine is a solid fuel booster
Turbine | [ksp::vessel::EngineType](/reference/ksp/vessel.md#enginetype) | R/O | Turbine engine

#### Methods

##### from_string

```rust
enginetypeconstants.from_string ( value : string ) -> Option<ksp::vessel::EngineType>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

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
trajectory | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit)[] | R/O | Get the planed trajectory of the vessel if all maneuvers are successfully executed.The list of orbit patch will always start after the first maneuvering node.I.e. if not maneuvers are planed this list will be empty. 

#### Methods

##### add

```rust
maneuver.add ( ut : float,
               radialOut : float,
               normal : float,
               prograde : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
radialOut | float |  | 
normal | float |  | 
prograde | float |  | 

##### add_burn_vector

```rust
maneuver.add_burn_vector ( ut : float,
                           burnVector : ksp::math::Vec3 ) -> Result<ksp::vessel::ManeuverNode, string>
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
burnVector | ksp::math::Vec3 |  | 

##### next_node

```rust
maneuver.next_node ( ) -> Result<ksp::vessel::ManeuverNode, string>
```



##### remove_all

```rust
maneuver.remove_all ( ) -> Unit
```

Remove all maneuver nodes


### ManeuverNode



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
ETA | float | R/W | 
burn_duration | float | R/O | 
burn_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/W | 
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

### ModuleCommand



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
activation_stage | int | R/O | 
air_intake | Option&lt;[ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)> | R/O | 
command_module | Option&lt;[ksp::vessel::ModuleCommand](/reference/ksp/vessel.md#modulecommand)> | R/O | 
control_state | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O | 
control_surface | Option&lt;[ksp::vessel::ModuleControlSurface](/reference/ksp/vessel.md#modulecontrolsurface)> | R/O | 
decouple_stage | int | R/O | 
decoupler | Option&lt;[ksp::vessel::ModuleDecoupler](/reference/ksp/vessel.md#moduledecoupler)> | R/O | 
deployable | Option&lt;[ksp::vessel::ModuleDeployable](/reference/ksp/vessel.md#moduledeployable)> | R/O | 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | R/O | 
dry_mass | float | R/O | Dry mass of the part 
engine_module | Option&lt;[ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)> | R/O | 
fairing | Option&lt;[ksp::vessel::ModuleFairing](/reference/ksp/vessel.md#modulefairing)> | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Get coordinate independent position of the part. 
global_rotation | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
green_mass | float | R/O | Green mass (Kerbals) of the part 
is_decoupler | bool | R/O | 
is_deployable | bool | R/O | 
is_engine | bool | R/O | 
is_fairing | bool | R/O | 
is_launch_clamp | bool | R/O | 
is_parachute | bool | R/O | 
is_science_experiment | bool | R/O | 
is_solar_panel | bool | R/O | 
launch_clamp | Option&lt;[ksp::vessel::ModuleLaunchClamp](/reference/ksp/vessel.md#modulelaunchclamp)> | R/O | 
max_temperature | float | R/O | Maximum temperature of the part 
parachute | Option&lt;[ksp::vessel::ModuleParachute](/reference/ksp/vessel.md#moduleparachute)> | R/O | 
part_name | string | R/O | 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get position of the part in celestial frame of the main body. 
resource_mass | float | R/O | Resource mass of the part 
resource_thermal_mass | float | R/O | 
resources | [ksp::resource::ResourceContainer](/reference/ksp/resource.md#resourcecontainer) | R/O | 
science_experiment | Option&lt;[ksp::vessel::ModuleScienceExperiment](/reference/ksp/vessel.md#modulescienceexperiment)> | R/O | 
solar_panel | Option&lt;[ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)> | R/O | 
splashed | bool | R/O | Indicate if the part has splashed 
temperature | float | R/O | Temperature of the part 
thermal_mass | float | R/O | 
total_mass | float | R/O | Total mass of the part 
vessel | [ksp::vessel::Vessel](/reference/ksp/vessel.md#vessel) | R/O | 

#### Methods

##### control_from_here

```rust
modulecommand.control_from_here ( ) -> Unit
```



### ModuleControlSurface



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
angle_of_attack | float | R/O | 
authority_limiter | float | R/W | 
drag | float | R/O | 
enable_pitch | bool | R/W | 
enable_roll | bool | R/W | 
enable_yaw | bool | R/W | 
invert_control | bool | R/W | 
lift | float | R/O | 
part_name | string | R/O | 

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



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
extend | bool |  | 

### ModuleDockingNode



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
activation_stage | int | R/O | 
air_intake | Option&lt;[ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)> | R/O | 
command_module | Option&lt;[ksp::vessel::ModuleCommand](/reference/ksp/vessel.md#modulecommand)> | R/O | 
control_surface | Option&lt;[ksp::vessel::ModuleControlSurface](/reference/ksp/vessel.md#modulecontrolsurface)> | R/O | 
decouple_stage | int | R/O | 
decoupler | Option&lt;[ksp::vessel::ModuleDecoupler](/reference/ksp/vessel.md#moduledecoupler)> | R/O | 
deployable | Option&lt;[ksp::vessel::ModuleDeployable](/reference/ksp/vessel.md#moduledeployable)> | R/O | 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | R/O | 
docking_state | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate) | R/O | 
dry_mass | float | R/O | Dry mass of the part 
engine_module | Option&lt;[ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)> | R/O | 
fairing | Option&lt;[ksp::vessel::ModuleFairing](/reference/ksp/vessel.md#modulefairing)> | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Get coordinate independent position of the part. 
global_rotation | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
green_mass | float | R/O | Green mass (Kerbals) of the part 
is_decoupler | bool | R/O | 
is_deployable | bool | R/O | 
is_deployable_docking_port | bool | R/O | 
is_engine | bool | R/O | 
is_fairing | bool | R/O | 
is_launch_clamp | bool | R/O | 
is_parachute | bool | R/O | 
is_science_experiment | bool | R/O | 
is_solar_panel | bool | R/O | 
launch_clamp | Option&lt;[ksp::vessel::ModuleLaunchClamp](/reference/ksp/vessel.md#modulelaunchclamp)> | R/O | 
max_temperature | float | R/O | Maximum temperature of the part 
node_types | string[] | R/O | 
parachute | Option&lt;[ksp::vessel::ModuleParachute](/reference/ksp/vessel.md#moduleparachute)> | R/O | 
part_name | string | R/O | 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get position of the part in celestial frame of the main body. 
resource_mass | float | R/O | Resource mass of the part 
resource_thermal_mass | float | R/O | 
resources | [ksp::resource::ResourceContainer](/reference/ksp/resource.md#resourcecontainer) | R/O | 
science_experiment | Option&lt;[ksp::vessel::ModuleScienceExperiment](/reference/ksp/vessel.md#modulescienceexperiment)> | R/O | 
solar_panel | Option&lt;[ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)> | R/O | 
splashed | bool | R/O | Indicate if the part has splashed 
temperature | float | R/O | Temperature of the part 
thermal_mass | float | R/O | 
total_mass | float | R/O | Total mass of the part 
vessel | [ksp::vessel::Vessel](/reference/ksp/vessel.md#vessel) | R/O | 

#### Methods

##### control_from_here

```rust
moduledockingnode.control_from_here ( ) -> Unit
```



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
is_propellant_starved | bool | R/O | 
is_shutdown | bool | R/O | 
is_staged | bool | R/O | 
max_fuel_flow | float | R/O | 
max_thrust_output_atm | float | R/O | 
max_thrust_output_vac | float | R/O | 
min_fuel_flow | float | R/O | 
part_name | string | R/O | 
real_isp | float | R/O | 
throttle_min | float | R/O | 

#### Methods

##### change_mode

```rust
moduleengine.change_mode ( name : string ) -> bool
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
name | string |  | 

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



### ModuleScienceExperiment



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
experiments | [ksp::science::Experiment](/reference/ksp/science.md#experiment)[] | R/O | 
is_deployed | bool | R/O | 
part_name | string | R/O | 

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

### ParachuteDeployModeConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
IMMEDIATE | [ksp::vessel::ParachuteDeployMode](/reference/ksp/vessel.md#parachutedeploymode) | R/O | 
RISKY | [ksp::vessel::ParachuteDeployMode](/reference/ksp/vessel.md#parachutedeploymode) | R/O | 
SAFE | [ksp::vessel::ParachuteDeployMode](/reference/ksp/vessel.md#parachutedeploymode) | R/O | 

#### Methods

##### from_string

```rust
parachutedeploymodeconstants.from_string ( value : string ) -> Option<ksp::vessel::ParachuteDeployMode>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### ParachuteDeployState

Parachute deploy state

#### Methods

##### to_string

```rust
parachutedeploystate.to_string ( ) -> string
```

String representation of the number

### ParachuteDeployStateConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
ARMED | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 
CUT | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 
DEPLOYED | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 
SEMIDEPLOYED | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 
STOWED | [ksp::vessel::ParachuteDeployState](/reference/ksp/vessel.md#parachutedeploystate) | R/O | 

#### Methods

##### from_string

```rust
parachutedeploystateconstants.from_string ( value : string ) -> Option<ksp::vessel::ParachuteDeployState>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### ParachuteSafeStates

Parachute safe states

#### Methods

##### to_string

```rust
parachutesafestates.to_string ( ) -> string
```

String representation of the number

### ParachuteSafeStatesConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
NONE | [ksp::vessel::ParachuteSafeStates](/reference/ksp/vessel.md#parachutesafestates) | R/O | 
RISKY | [ksp::vessel::ParachuteSafeStates](/reference/ksp/vessel.md#parachutesafestates) | R/O | 
SAFE | [ksp::vessel::ParachuteSafeStates](/reference/ksp/vessel.md#parachutesafestates) | R/O | 
UNSAFE | [ksp::vessel::ParachuteSafeStates](/reference/ksp/vessel.md#parachutesafestates) | R/O | 

#### Methods

##### from_string

```rust
parachutesafestatesconstants.from_string ( value : string ) -> Option<ksp::vessel::ParachuteSafeStates>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

### Part



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
activation_stage | int | R/O | 
air_intake | Option&lt;[ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)> | R/O | 
command_module | Option&lt;[ksp::vessel::ModuleCommand](/reference/ksp/vessel.md#modulecommand)> | R/O | 
control_surface | Option&lt;[ksp::vessel::ModuleControlSurface](/reference/ksp/vessel.md#modulecontrolsurface)> | R/O | 
decouple_stage | int | R/O | 
decoupler | Option&lt;[ksp::vessel::ModuleDecoupler](/reference/ksp/vessel.md#moduledecoupler)> | R/O | 
deployable | Option&lt;[ksp::vessel::ModuleDeployable](/reference/ksp/vessel.md#moduledeployable)> | R/O | 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | R/O | 
dry_mass | float | R/O | Dry mass of the part 
engine_module | Option&lt;[ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)> | R/O | 
fairing | Option&lt;[ksp::vessel::ModuleFairing](/reference/ksp/vessel.md#modulefairing)> | R/O | 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Get coordinate independent position of the part. 
global_rotation | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | 
green_mass | float | R/O | Green mass (Kerbals) of the part 
is_decoupler | bool | R/O | 
is_deployable | bool | R/O | 
is_engine | bool | R/O | 
is_fairing | bool | R/O | 
is_launch_clamp | bool | R/O | 
is_parachute | bool | R/O | 
is_science_experiment | bool | R/O | 
is_solar_panel | bool | R/O | 
launch_clamp | Option&lt;[ksp::vessel::ModuleLaunchClamp](/reference/ksp/vessel.md#modulelaunchclamp)> | R/O | 
max_temperature | float | R/O | Maximum temperature of the part 
parachute | Option&lt;[ksp::vessel::ModuleParachute](/reference/ksp/vessel.md#moduleparachute)> | R/O | 
part_name | string | R/O | 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get position of the part in celestial frame of the main body. 
resource_mass | float | R/O | Resource mass of the part 
resource_thermal_mass | float | R/O | 
resources | [ksp::resource::ResourceContainer](/reference/ksp/resource.md#resourcecontainer) | R/O | 
science_experiment | Option&lt;[ksp::vessel::ModuleScienceExperiment](/reference/ksp/vessel.md#modulescienceexperiment)> | R/O | 
solar_panel | Option&lt;[ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)> | R/O | 
splashed | bool | R/O | Indicate if the part has splashed 
temperature | float | R/O | Temperature of the part 
thermal_mass | float | R/O | 
total_mass | float | R/O | Total mass of the part 
vessel | [ksp::vessel::Vessel](/reference/ksp/vessel.md#vessel) | R/O | 

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

##### get_ISP

```rust
stagedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_TWR

```rust
stagedeltav.get_TWR ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated TWR of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_deltav

```rust
stagedeltav.get_deltav ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated delta-v of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_thrust

```rust
stagedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

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



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
stage | int |  | 

### Targetable



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
body | Option&lt;[ksp::orbit::Body](/reference/ksp/orbit.md#body)> | R/O | Get the targeted celestial body, if target is a body. 
docking_node | Option&lt;[ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)> | R/O | Get the targeted docking node, if target is a docking node. 
name | string | R/O | Name of the vessel target. 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | Orbit of the vessel target. 
vessel | Option&lt;[ksp::vessel::Vessel](/reference/ksp/vessel.md#vessel)> | R/O | Get the targeted vessel, if target is a vessel. 

### Vessel

Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
CoM | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Position of the center of mass of the vessel. 
actions | [ksp::vessel::ActionGroups](/reference/ksp/vessel.md#actiongroups) | R/O | Collection of methods to trigger action groups. 
air_intakes | [ksp::vessel::ModuleAirIntake](/reference/ksp/vessel.md#moduleairintake)[] | R/O | Get a list of all air intake parts of the vessel. 
altitude_scenery | float | R/O | 
altitude_sealevel | float | R/O | 
altitude_terrain | float | R/O | 
angular_momentum | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the angular momentum of the vessel in the celestial frame of its main body. 
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the coordinate angular velocity in the celestial frame of its main body. 
atmosphere_density | float | R/O | 
autopilot | [ksp::vessel::Autopilot](/reference/ksp/vessel.md#autopilot) | R/O | Collection of methods to interact with the SAS system of the vessel. 
available_thrust | float | R/O | Returns the maximum thrust of all engines in the current situation of the vessel. 
body_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The body/rotating reference frame of the vessel. 
celestial_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The celestial/non-rotating reference frame of the vessel. 
command_modules | [ksp::vessel::ModuleCommand](/reference/ksp/vessel.md#modulecommand)[] | R/O | Get a list of all command module parts of the vessel. 
control_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | Reference frame for the current control position. 
control_status | [ksp::vessel::VesselControlState](/reference/ksp/vessel.md#vesselcontrolstate) | R/O | Current control status of the vessel. 
control_surfaces | [ksp::vessel::ModuleControlSurface](/reference/ksp/vessel.md#modulecontrolsurface)[] | R/O | Get a list of all control service parts of the vessel. 
delta_v | [ksp::vessel::VesselDeltaV](/reference/ksp/vessel.md#vesseldeltav) | R/O | Collection of methods to obtain delta-v information of the vessel. 
docking_nodes | [ksp::vessel::ModuleDockingNode](/reference/ksp/vessel.md#moduledockingnode)[] | R/O | Get a list of all docking node parts of the vessel. 
dynamic_pressure_kpa | float | R/O | 
east | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the horizon east vector in the celestial frame of the main body. 
engines | [ksp::vessel::ModuleEngine](/reference/ksp/vessel.md#moduleengine)[] | R/O | Get a list of all engine parts of the vessel. 
facing | [ksp::math::Direction](/reference/ksp/math.md#direction) | R/O | Get the current facing direction of the vessel in the celestial frame of its main body. 
geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | R/O | Get the current geo-coordinate of the vessel. 
global_angular_momentum | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | Get the coordinate system independent angular momentum of the vessel. 
global_angular_velocity | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | Get the coordinate system independent angular velocity of the vessel. 
global_center_of_mass | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Coordinate independent position of the center of mass. 
global_east | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Get the coordinate system independent horizon north vector. 
global_facing | [ksp::math::GlobalDirection](/reference/ksp/math.md#globaldirection) | R/O | Get the coordinate system independent facing direction of the vessel. 
global_moment_of_inertia | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Get the coordinate system independent moment of inertial of the vessel 
global_north | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Get the coordinate system independent horizon east vector. 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | Coordinate independent position of the vessel. 
global_up | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Get the coordinate system independent horizon up vector. 
global_velocity | [ksp::math::GlobalVelocity](/reference/ksp/math.md#globalvelocity) | R/O | Get the coordinate independent velocity of the vessel. 
heading | float | R/O | 
horizon_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | Reference frame for the horizon at the current position of the vessel. 
horizontal_surface_speed | float | R/O | 
id | string | R/O | Unique vessel id 
is_active | bool | R/O | Check if the vessel is currently active. 
is_controllable | bool | R/O | Check if the vessel is controllable. 
is_flying | bool | R/O | Check if the vessel is flyging. 
mach_number | float | R/O | 
main_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | The main body of the current SOI the vessel is in. 
maneuver | [ksp::vessel::Maneuver](/reference/ksp/vessel.md#maneuver) | R/O | Collection of methods to interact with the maneuver plan of the vessel. 
mass | float | R/O | Total mass of the vessel. 
name | string | R/O | The name of the vessel. 
north | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the horizon north vector in the celestial frame of the main body. 
offset_ground | float | R/O | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | Current orbit or orbit patch of the vessel. 
orbital_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Orbital velocity of the vessel relative to the main body. This is equivalent of expressing the `global_velocity` in the celestial frame of the main body. 
parts | [ksp::vessel::Part](/reference/ksp/vessel.md#part)[] | R/O | Get a list of all vessel parts. 
pitch_yaw_roll | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Returns the pitch, yaw/heading and roll of the vessel relative to the horizon. 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Coordinate position of the vessel in the celestial frame of the main body. 
research_location | Option&lt;[ksp::science::ResearchLocation](/reference/ksp/science.md#researchlocation)> | R/O | Get the current research location of the vessel. 
science_storage | Option&lt;[ksp::science::ScienceStorage](/reference/ksp/science.md#sciencestorage)> | R/O | Access the science storage/research inventory of the vessel. 
situation | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Get the current situation of the vessel. 
solar_panels | [ksp::vessel::ModuleSolarPanel](/reference/ksp/vessel.md#modulesolarpanel)[] | R/O | Get a list of all solar panel parts of the vessel. 
sound_speed | float | R/O | 
staging | [ksp::vessel::Staging](/reference/ksp/vessel.md#staging) | R/O | Collection of methods to obtain information about stages and trigger staging. 
static_pressure_kpa | float | R/O | 
surface_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Surface velocity of the vessel relative to the main body. This is equivalent of expressing the `global_velocity` in the body frame of the main body. 
target | Option&lt;[ksp::vessel::Targetable](/reference/ksp/vessel.md#targetable)> | R/W | Get the currently selected target of the vessel, if there is one. 
total_torque | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the available torque of relative to its control frame. 
trajectory | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit)[] | R/O | Get the entire trajectory of the vessel containing all orbit patches. 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the horizon up vector in the celestial frame of the main body. 
vertical_speed | float | R/O | Get the vertical speed of the vessel. 
vertical_surface_speed | float | R/O | 

#### Methods

##### global_heading_direction

```rust
vessel.global_heading_direction ( degreesFromNorth : float,
                                  pitchAboveHorizon : float,
                                  roll : float ) -> ksp::math::GlobalDirection
```

Calculate a coordinate system independent direction based on
heading, pitch an roll relative to the horizon.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
degreesFromNorth | float |  | 
pitchAboveHorizon | float |  | 
roll | float |  | 

##### heading_direction

```rust
vessel.heading_direction ( degreesFromNorth : float,
                           pitchAboveHorizon : float,
                           roll : float ) -> ksp::math::Direction
```

Calculate a direction in the celestial frame of the main body based on
heading, pitch an roll relative to the horizon.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
degreesFromNorth | float |  | 
pitchAboveHorizon | float |  | 
roll | float |  | 

##### make_active

```rust
vessel.make_active ( ) -> bool
```

Make this vessel the active vessel.


##### manage_rcs_translate

```rust
vessel.manage_rcs_translate ( translateProvider : sync fn(float) -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
translateProvider | sync fn(float) -> ksp::math::Vec3 |  | 

##### manage_steering

```rust
vessel.manage_steering ( pitchYawRollProvider : sync fn(float) -> ksp::math::Vec3 ) -> ksp::control::SteeringManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
pitchYawRollProvider | sync fn(float) -> ksp::math::Vec3 |  | 

##### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : sync fn(float) -> float ) -> ksp::control::ThrottleManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
throttleProvider | sync fn(float) -> float |  | 

##### manage_wheel_steering

```rust
vessel.manage_wheel_steering ( wheelSteeringProvider : sync fn(float) -> float ) -> ksp::control::WheelSteeringManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
wheelSteeringProvider | sync fn(float) -> float |  | 

##### manage_wheel_throttle

```rust
vessel.manage_wheel_throttle ( wheelThrottleProvider : sync fn(float) -> float ) -> ksp::control::WheelThrottleManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
wheelThrottleProvider | sync fn(float) -> float |  | 

##### override_input_pitch

```rust
vessel.override_input_pitch ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### override_input_roll

```rust
vessel.override_input_roll ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### override_input_translate_x

```rust
vessel.override_input_translate_x ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### override_input_translate_y

```rust
vessel.override_input_translate_y ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### override_input_translate_z

```rust
vessel.override_input_translate_z ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### override_input_yaw

```rust
vessel.override_input_yaw ( value : float ) -> Unit
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | float |  | 

##### release_control

```rust
vessel.release_control ( ) -> Unit
```



##### set_rcs_translate

```rust
vessel.set_rcs_translate ( translate : ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
translate | ksp::math::Vec3 |  | 

##### set_steering

```rust
vessel.set_steering ( pitchYawRoll : ksp::math::Vec3 ) -> ksp::control::SteeringManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
pitchYawRoll | ksp::math::Vec3 |  | 

##### set_throttle

```rust
vessel.set_throttle ( throttle : float ) -> ksp::control::ThrottleManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
throttle | float |  | 

##### set_wheel_steering

```rust
vessel.set_wheel_steering ( wheelSteering : float ) -> ksp::control::WheelSteeringManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
wheelSteering | float |  | 

##### set_wheel_throttle

```rust
vessel.set_wheel_throttle ( wheelThrottle : float ) -> ksp::control::WheelThrottleManager
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
wheelThrottle | float |  | 

### VesselControlState

Vessel control state

#### Methods

##### to_string

```rust
vesselcontrolstate.to_string ( ) -> string
```

String representation of the number

### VesselControlStateConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
FullControl | [ksp::vessel::VesselControlState](/reference/ksp/vessel.md#vesselcontrolstate) | R/O | 
FullControlHibernation | [ksp::vessel::VesselControlState](/reference/ksp/vessel.md#vesselcontrolstate) | R/O | 
NoCommNet | [ksp::vessel::VesselControlState](/reference/ksp/vessel.md#vesselcontrolstate) | R/O | 
NoControl | [ksp::vessel::VesselControlState](/reference/ksp/vessel.md#vesselcontrolstate) | R/O | 

#### Methods

##### from_string

```rust
vesselcontrolstateconstants.from_string ( value : string ) -> Option<ksp::vessel::VesselControlState>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

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


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
stage | int |  | 

### VesselSituation

Vessel situation

#### Methods

##### to_string

```rust
vesselsituation.to_string ( ) -> string
```

String representation of the number

### VesselSituationConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Escaping | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel is on an escape trajectory.
Flying | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel is flying.
Landed | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel has landed.
Orbiting | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel is orbiting its main body.
PreLaunch | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel is in pre-launch situation.
Splashed | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel has splashed in water.
SubOrbital | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel is on a sub-orbital trajectory.
Unknown | [ksp::vessel::VesselSituation](/reference/ksp/vessel.md#vesselsituation) | R/O | Vessel situation is unknown.

#### Methods

##### from_string

```rust
vesselsituationconstants.from_string ( value : string ) -> Option<ksp::vessel::VesselSituation>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

## Constants

Name | Type | Description
--- | --- | ---
AutopilotMode | ksp::vessel::AutopilotModeConstants | Vessel autopilot (SAS) mode
CommandControlState | ksp::vessel::CommandControlStateConstants | Current state of a command module
DeltaVSituation | ksp::vessel::DeltaVSituationConstants | Vessel situation for delta-v calculation
DockingState | ksp::vessel::DockingStateConstants | Current state of a docking node
EngineType | ksp::vessel::EngineTypeConstants | Engine types
ParachuteDeployMode | ksp::vessel::ParachuteDeployModeConstants | Parachute deploy mode
ParachuteDeployState | ksp::vessel::ParachuteDeployStateConstants | Parachute deploy state
ParachuteSafeStates | ksp::vessel::ParachuteSafeStatesConstants | Parachute safe states
VesselControlState | ksp::vessel::VesselControlStateConstants | Vessel control state
VesselSituation | ksp::vessel::VesselSituationConstants | Vessel situation


## Functions


### active_vessel

```rust
pub sync fn active_vessel ( ) -> Result<ksp::vessel::Vessel, string>
```

Try to get the currently active vessel. Will result in an error if there is none.


### get_all_owned_vessels

```rust
pub sync fn get_all_owned_vessels ( ) -> ksp::vessel::Vessel[]
```

Get all vessels owned by the current player.


### get_vessels_in_range

```rust
pub sync fn get_vessels_in_range ( ) -> ksp::vessel::Vessel[]
```

Get all vessels in range of the current view.

