# ksp::vessel

Collection of types and functions to get information and control in-game vessels.


## Types


### ActionGroups



#### Fields

Name | Type | Description
--- | --- | ---
abort | bool | 
brakes | bool | 
gear | bool | 
light | bool | 
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

### EngineData



#### Fields

Name | Type | Description
--- | --- | ---
current_throttle | float | 
current_thrust | float | 
has_ignited | bool | 
is_flameout | bool | 
is_shutdown | bool | 
is_staged | bool | 
max_fuel_flow | float | 
max_thrust_output_atm | float | 
max_thrust_output_vac | float | 
min_fuel_flow | float | 
throttle_min | float | 

### EngineDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
engine_data | [ksp::vessel::EngineData](/reference/ksp/vessel.md#enginedata) | 
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


### Maneuver



#### Fields

Name | Type | Description
--- | --- | ---
nodes | [ksp::vessel::ManeuverNode[]](/reference/ksp/vessel.md#maneuvernode) | 

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



### Part



#### Fields

Name | Type | Description
--- | --- | ---
engine_data | [Option<ksp::vessel::EngineData>](/reference/Option<ksp/vessel.md#enginedata>) | 
is_engine | bool | 
is_solar_panel | bool | 
part_name | string | 

### StageDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
active_engines | [ksp::vessel::EngineDeltaV[]](/reference/ksp/vessel.md#enginedeltav) | 
burn_time | float | Estimated burn time of the stage. 
dry_mass | float | Dry mass of the stage. 
end_mass | float | End mass of the stage. 
engines | [ksp::vessel::EngineDeltaV[]](/reference/ksp/vessel.md#enginedeltav) | 
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
altitude_scenery | float | 
altitude_sealevel | float | 
altitude_terrain | float | 
angular_momentum | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
atmosphere_density | float | 
autopilot | [ksp::vessel::Autopilot](/reference/ksp/vessel.md#autopilot) | 
available_thrust | float | 
CoM | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
control_status | string | 
delta_v | [ksp::vessel::VesselDeltaV](/reference/ksp/vessel.md#vesseldeltav) | 
east | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
engines | [ksp::vessel::EngineData[]](/reference/ksp/vessel.md#enginedata) | 
facing | [ksp::math::Direction](/reference/ksp/math.md#direction) | 
geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | 
heading | float | 
horizontal_surface_speed | float | 
is_active | bool | 
main_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | 
maneuver | [ksp::vessel::Maneuver](/reference/ksp/vessel.md#maneuver) | 
mass | float | 
name | string | The name of the vessel. 
north | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
offset_ground | float | 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | 
orbital_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
parts | [ksp::vessel::Part[]](/reference/ksp/vessel.md#part) | 
staging | [ksp::vessel::Staging](/reference/ksp/vessel.md#staging) | 
surface_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
target | [Option<ksp::vessel::Targetable>](/reference/Option<ksp/vessel.md#targetable>) | 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | 
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
vessel.manage_rcs_translate ( translateProvider : fn() -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



##### manage_steering

```rust
vessel.manage_steering ( directionProvider : fn() -> ksp::math::Direction ) -> ksp::control::SteeringManager
```



##### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : fn(float) -> float ) -> ksp::control::ThrottleManager
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
vessel.set_steering ( direction : ksp::math::Direction ) -> ksp::control::SteeringManager
```



##### set_throttle

```rust
vessel.set_throttle ( throttle : float ) -> ksp::control::ThrottleManager
```



### VesselDeltaV



#### Fields

Name | Type | Description
--- | --- | ---
stages | [ksp::vessel::StageDeltaV[]](/reference/ksp/vessel.md#stagedeltav) | 

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

