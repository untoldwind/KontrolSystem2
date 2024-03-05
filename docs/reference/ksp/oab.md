# ksp::oab

Collection of types and functions to get information about the current object/vessel assembly.


## Types


### ObjectAssembly

Represents an object assembly, i.e. a potential vessel.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
delta_v | [ksp::oab::ObjectAssemblyDeltaV](/reference/ksp/oab.md#objectassemblydeltav) | R/O | 
dry_mass | float | R/O | 
parts | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)[] | R/O | 
total_mass | float | R/O | 
wet_mass | float | R/O | 

### ObjectAssemblyBuilder

Represents the current object assembly builder/


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
assemblies | [ksp::oab::ObjectAssembly](/reference/ksp/oab.md#objectassembly)[] | R/O | Get all object assemblies (i.e. all parts that are not fully connected) 
main_assembly | Option&lt;[ksp::oab::ObjectAssembly](/reference/ksp/oab.md#objectassembly)> | R/O | Get the current main assembly if there is one. 

### ObjectAssemblyDeltaV

Delta V information of an object assembly


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
stages | [ksp::oab::ObjectAssemblyStageDeltaV](/reference/ksp/oab.md#objectassemblystagedeltav)[] | R/O | 

#### Methods

##### stage

```rust
objectassemblydeltav.stage ( stage : int ) -> Option<ksp::oab::ObjectAssemblyStageDeltaV>
```

Get delta-v information for a specific `stage` of the object assembly, if existent.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
stage | int |  | 

### ObjectAssemblyEngine



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
auto_switch_mode | bool | R/W | 
current_engine_mode | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode) | R/O | Get the current engine mode 
current_propellant | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition) | R/O | Get the propellant of the current engine mode 
current_throttle | float | R/O | 
current_thrust | float | R/O | 
engine_modes | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode)[] | R/O | Get all engine modes 
has_ignited | bool | R/O | Check if engine has ignited 
independent_throttle | float | R/W | 
independent_throttle_enabled | bool | R/W | 
is_flameout | bool | R/O | Check if engine had a flame-out 
is_operational | bool | R/O | Check if engine is operational 
is_propellant_starved | bool | R/O | 
is_shutdown | bool | R/O | Check if engine is shutdown 
is_staged | bool | R/O | 
max_fuel_flow | float | R/O | 
max_thrust_output_atm | float | R/O | 
max_thrust_output_vac | float | R/O | 
min_fuel_flow | float | R/O | 
propellants | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition)[] | R/O | Get the propellants of the different engine modes 
real_isp | float | R/O | 
throttle_min | float | R/O | 
thrust_direction | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Direction of thrust 
thrust_limiter | float | R/W | 

### ObjectAssemblyEngineDeltaV



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
start_burn_stage | int | R/O | Number of the stage when engine is supposed to start 

#### Methods

##### get_ISP

```rust
objectassemblyenginedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the engine in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_thrust

```rust
objectassemblyenginedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the engine in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

### ObjectAssemblyPart

Represents are part in an object assembly.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
activation_stage | int | R/O | 
decouple_stage | int | R/O | 
dry_mass | float | R/O | Dry mass of the part 
engine | Option&lt;[ksp::oab::ObjectAssemblyEngine](/reference/ksp/oab.md#objectassemblyengine)> | R/O | 
fuel_cross_feed | bool | R/O | 
green_mass | float | R/O | Green mass (Kerbals) of the part 
is_engine | bool | R/O | 
is_solar_panel | bool | R/O | 
part_name | string | R/O | 
relative_position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
solar_panel | Option&lt;[ksp::oab::ObjectAssemblySolarPanel](/reference/ksp/oab.md#objectassemblysolarpanel)> | R/O | 
total_mass | float | R/O | Total mass of the part 
wet_mass | float | R/O | 

### ObjectAssemblySolarPanel



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
base_flow_rate | float | R/O | Base flow rate 
efficiency_multiplier | float | R/O | 
part | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O | 
part_name | string | R/O | 

### ObjectAssemblyStageDeltaV



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
active_engines | [ksp::oab::ObjectAssemblyEngineDeltaV](/reference/ksp/oab.md#objectassemblyenginedeltav)[] | R/O | 
burn_time | float | R/O | Estimated burn time of the stage. 
dry_mass | float | R/O | Dry mass of the stage. 
end_mass | float | R/O | End mass of the stage. 
engines | [ksp::oab::ObjectAssemblyEngineDeltaV](/reference/ksp/oab.md#objectassemblyenginedeltav)[] | R/O | 
fuel_mass | float | R/O | Mass of the fuel in the stage. 
stage | int | R/O | The stage number. 
start_mass | float | R/O | Start mass of the stage. 

#### Methods

##### get_ISP

```rust
objectassemblystagedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_TWR

```rust
objectassemblystagedeltav.get_TWR ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated TWR of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_deltav

```rust
objectassemblystagedeltav.get_deltav ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated delta-v of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

##### get_thrust

```rust
objectassemblystagedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the stage in a given `situation`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
situation | ksp::vessel::DeltaVSituation |  | 

## Functions


### active_object_assembly_builder

```rust
pub sync fn active_object_assembly_builder ( ) -> Result<ksp::oab::ObjectAssemblyBuilder, string>
```

Try to get the currently active vessel. Will result in an error if there is none.

