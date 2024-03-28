# ksp::oab

Collection of types and functions to get information about the current object/vessel assembly.


## Types


### ObjectAssembly

Represents an object assembly, i.e. a potential vessel.


#### Fields

| Name       | Type                                                                         | Read-only | Description                                                           |
| ---------- | ---------------------------------------------------------------------------- | --------- | --------------------------------------------------------------------- |
| delta_v    | [ksp::oab::ObjectAssemblyDeltaV](/reference/ksp/oab.md#objectassemblydeltav) | R/O       | Collection of methods to obtain delta-v information of the assembly.  |
| dry_mass   | float                                                                        | R/O       | Total dry mass of assembly.                                           |
| parts      | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)[]   | R/O       | Get a list of all parts of assembly.                                  |
| total_mass | float                                                                        | R/O       | Total mass of assembly.                                               |
| wet_mass   | float                                                                        | R/O       | Total wet mass of assembly.                                           |


### ObjectAssemblyAirIntake



#### Fields

| Name           | Type                                                                               | Read-only | Description            |
| -------------- | ---------------------------------------------------------------------------------- | --------- | ---------------------- |
| enabled        | bool                                                                               | R/O       | Enable/disable module  |
| flow_rate      | float                                                                              | R/O       | Resource flow rate     |
| part           | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)           | R/O       |                        |
| part_name      | string                                                                             | R/O       |                        |
| resource       | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition) | R/O       |                        |
| resource_units | float                                                                              | R/O       |                        |
| toogle_intake  | bool                                                                               | R/W       | Toggle air intake.     |


### ObjectAssemblyBuilder

Represents the current object assembly builder.


#### Fields

| Name          | Type                                                                        | Read-only | Description                                                              |
| ------------- | --------------------------------------------------------------------------- | --------- | ------------------------------------------------------------------------ |
| assemblies    | [ksp::oab::ObjectAssembly](/reference/ksp/oab.md#objectassembly)[]          | R/O       | Get all object assemblies (i.e. all parts that are not fully connected)  |
| main_assembly | Option&lt;[ksp::oab::ObjectAssembly](/reference/ksp/oab.md#objectassembly)> | R/O       | Get the current main assembly if there is one.                           |


### ObjectAssemblyCommand



#### Fields

| Name                   | Type                                                                             | Read-only | Description |
| ---------------------- | -------------------------------------------------------------------------------- | --------- | ----------- |
| control_state          | [ksp::vessel::CommandControlState](/reference/ksp/vessel.md#commandcontrolstate) | R/O       |             |
| has_hibernation        | bool                                                                             | R/O       |             |
| hibernation_multiplier | float                                                                            | R/O       |             |
| is_hibernating         | bool                                                                             | R/O       |             |
| part                   | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)         | R/O       |             |
| part_name              | string                                                                           | R/O       |             |
| required_resources     | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting)[]   | R/O       |             |


### ObjectAssemblyControlSurface



#### Fields

| Name              | Type                                                                     | Read-only | Description |
| ----------------- | ------------------------------------------------------------------------ | --------- | ----------- |
| angle_of_attack   | float                                                                    | R/O       |             |
| authority_limiter | float                                                                    | R/W       |             |
| drag              | float                                                                    | R/O       |             |
| enable_pitch      | bool                                                                     | R/W       |             |
| enable_roll       | bool                                                                     | R/W       |             |
| enable_yaw        | bool                                                                     | R/W       |             |
| invert_control    | bool                                                                     | R/W       |             |
| lift              | float                                                                    | R/O       |             |
| lift_drag_ratio   | float                                                                    | R/O       |             |
| part              | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O       |             |
| part_name         | string                                                                   | R/O       |             |


### ObjectAssemblyDecoupler



#### Fields

| Name             | Type                                                                     | Read-only | Description |
| ---------------- | ------------------------------------------------------------------------ | --------- | ----------- |
| ejection_impulse | float                                                                    | R/W       |             |
| is_decoupled     | bool                                                                     | R/O       |             |
| part             | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O       |             |
| part_name        | string                                                                   | R/O       |             |


### ObjectAssemblyDeltaV

Delta V information of an object assembly


#### Fields

| Name   | Type                                                                                     | Read-only | Description |
| ------ | ---------------------------------------------------------------------------------------- | --------- | ----------- |
| stages | [ksp::oab::ObjectAssemblyStageDeltaV](/reference/ksp/oab.md#objectassemblystagedeltav)[] | R/O       |             |


#### Methods

##### stage

```rust
objectassemblydeltav.stage ( stage : int ) -> Option<ksp::oab::ObjectAssemblyStageDeltaV>
```

Get delta-v information for a specific `stage` of the object assembly, if existent.


Parameters

| Name  | Type | Optional | Description |
| ----- | ---- | -------- | ----------- |
| stage | int  |          |             |


### ObjectAssemblyDeployable



#### Fields

| Name         | Type                                                                                 | Read-only | Description |
| ------------ | ------------------------------------------------------------------------------------ | --------- | ----------- |
| deploy_limit | float                                                                                | R/W       |             |
| deploy_state | [ksp::vessel::DeployableDeployState](/reference/ksp/vessel.md#deployabledeploystate) | R/O       |             |
| extendable   | bool                                                                                 | R/O       |             |
| extended     | bool                                                                                 | R/W       |             |
| part         | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)             | R/O       |             |
| part_name    | string                                                                               | R/O       |             |
| retractable  | bool                                                                                 | R/O       |             |


#### Methods

##### set_extended

```rust
objectassemblydeployable.set_extended ( extend : bool ) -> Unit
```



Parameters

| Name   | Type | Optional | Description |
| ------ | ---- | -------- | ----------- |
| extend | bool |          |             |


### ObjectAssemblyDockingNode



#### Fields

| Name                       | Type                                                                     | Read-only | Description |
| -------------------------- | ------------------------------------------------------------------------ | --------- | ----------- |
| docking_state              | [ksp::vessel::DockingState](/reference/ksp/vessel.md#dockingstate)       | R/O       |             |
| is_deployable_docking_port | bool                                                                     | R/O       |             |
| node_types                 | string[]                                                                 | R/O       |             |
| part                       | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O       |             |
| part_name                  | string                                                                   | R/O       |             |


### ObjectAssemblyDrag



#### Fields

| Name           | Type                                                                     | Read-only | Description |
| -------------- | ------------------------------------------------------------------------ | --------- | ----------- |
| exposed_area   | float                                                                    | R/O       |             |
| part           | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O       |             |
| part_name      | string                                                                   | R/O       |             |
| reference_area | float                                                                    | R/O       |             |
| total_area     | float                                                                    | R/O       |             |


### ObjectAssemblyEngine



#### Fields

| Name                         | Type                                                                                 | Read-only | Description                                        |
| ---------------------------- | ------------------------------------------------------------------------------------ | --------- | -------------------------------------------------- |
| auto_switch_mode             | bool                                                                                 | R/W       |                                                    |
| current_engine_mode          | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode)                       | R/O       | Get the current engine mode                        |
| current_propellant           | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition)   | R/O       | Get the propellant of the current engine mode      |
| current_throttle             | float                                                                                | R/O       |                                                    |
| current_thrust               | float                                                                                | R/O       |                                                    |
| engine_modes                 | [ksp::vessel::EngineMode](/reference/ksp/vessel.md#enginemode)[]                     | R/O       | Get all engine modes                               |
| has_ignited                  | bool                                                                                 | R/O       | Check if engine has ignited                        |
| independent_throttle         | float                                                                                | R/W       | Current independent throttle between 0.0 - 1.0     |
| independent_throttle_enabled | bool                                                                                 | R/W       | Toggle independent throttle                        |
| is_flameout                  | bool                                                                                 | R/O       | Check if engine had a flame-out                    |
| is_operational               | bool                                                                                 | R/O       | Check if engine is operational                     |
| is_propellant_starved        | bool                                                                                 | R/O       |                                                    |
| is_shutdown                  | bool                                                                                 | R/O       | Check if engine is shutdown                        |
| is_staged                    | bool                                                                                 | R/O       |                                                    |
| max_fuel_flow                | float                                                                                | R/O       |                                                    |
| max_thrust_output_atm        | float                                                                                | R/O       |                                                    |
| max_thrust_output_vac        | float                                                                                | R/O       |                                                    |
| min_fuel_flow                | float                                                                                | R/O       |                                                    |
| part                         | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)             | R/O       |                                                    |
| part_name                    | string                                                                               | R/O       |                                                    |
| propellants                  | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition)[] | R/O       | Get the propellants of the different engine modes  |
| real_isp                     | float                                                                                | R/O       |                                                    |
| throttle_min                 | float                                                                                | R/O       |                                                    |
| thrust_direction             | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                       | R/O       | Direction of thrust                                |
| thrust_limiter               | float                                                                                | R/W       | Current thrust limit value between 0.0 - 1.0       |


#### Methods

##### calc_max_thrust_output_atm

```rust
objectassemblyengine.calc_max_thrust_output_atm ( atmPressurekPa : float,
                                                  atmTemp : float,
                                                  atmDensity : float,
                                                  machNumber : float ) -> float
```

Calculate maximum thrust in atmosphere given atmospheric parameters


Parameters

| Name           | Type  | Optional | Description |
| -------------- | ----- | -------- | ----------- |
| atmPressurekPa | float | x        |             |
| atmTemp        | float | x        |             |
| atmDensity     | float | x        |             |
| machNumber     | float | x        |             |


### ObjectAssemblyEngineDeltaV



#### Fields

| Name             | Type                                                                         | Read-only | Description                                           |
| ---------------- | ---------------------------------------------------------------------------- | --------- | ----------------------------------------------------- |
| engine           | [ksp::oab::ObjectAssemblyEngine](/reference/ksp/oab.md#objectassemblyengine) | R/O       |                                                       |
| part             | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)     | R/O       |                                                       |
| start_burn_stage | int                                                                          | R/O       | Number of the stage when engine is supposed to start  |


#### Methods

##### get_ISP

```rust
objectassemblyenginedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the engine in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


##### get_thrust

```rust
objectassemblyenginedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the engine in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


### ObjectAssemblyExperiment



#### Fields

| Name                      | Type                                                                                 | Read-only | Description                            |
| ------------------------- | ------------------------------------------------------------------------------------ | --------- | -------------------------------------- |
| crew_required             | int                                                                                  | R/O       |                                        |
| definition                | [ksp::science::ExperimentDefinition](/reference/ksp/science.md#experimentdefinition) | R/O       | Get the definition of the experiment.  |
| experiment_id             | string                                                                               | R/O       |                                        |
| experiment_uses_resources | bool                                                                                 | R/O       |                                        |
| resources_cost            | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting)[]       | R/O       |                                        |
| time_to_complete          | float                                                                                | R/O       |                                        |


### ObjectAssemblyGenerator



#### Fields

| Name             | Type                                                                         | Read-only | Description |
| ---------------- | ---------------------------------------------------------------------------- | --------- | ----------- |
| enabled          | bool                                                                         | R/W       |             |
| generator_output | float                                                                        | R/O       |             |
| is_always_active | bool                                                                         | R/O       |             |
| part             | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)     | R/O       |             |
| part_name        | string                                                                       | R/O       |             |
| resource_setting | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting) | R/O       |             |


### ObjectAssemblyLiftingSurface



#### Fields

| Name            | Type                                                                     | Read-only | Description |
| --------------- | ------------------------------------------------------------------------ | --------- | ----------- |
| angle_of_attack | float                                                                    | R/O       |             |
| drag_scalar     | float                                                                    | R/O       |             |
| lift_drag_ratio | float                                                                    | R/O       |             |
| lift_scalar     | float                                                                    | R/O       |             |
| part            | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart) | R/O       |             |
| part_name       | string                                                                   | R/O       |             |


### ObjectAssemblyLight



#### Fields

| Name                     | Type                                                                         | Read-only | Description |
| ------------------------ | ---------------------------------------------------------------------------- | --------- | ----------- |
| blink_enabled            | bool                                                                         | R/W       |             |
| blink_rate               | float                                                                        | R/W       |             |
| has_resources_to_operate | bool                                                                         | R/O       |             |
| light_color              | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                               | R/W       |             |
| light_enabled            | bool                                                                         | R/W       |             |
| part                     | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)     | R/O       |             |
| part_name                | string                                                                       | R/O       |             |
| pitch                    | float                                                                        | R/W       |             |
| required_resource        | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting) | R/O       |             |
| rotation                 | float                                                                        | R/W       |             |


### ObjectAssemblyPart

Represents are part in an object assembly.


#### Fields

| Name                    | Type                                                                                                          | Read-only | Description                       |
| ----------------------- | ------------------------------------------------------------------------------------------------------------- | --------- | --------------------------------- |
| activation_stage        | int                                                                                                           | R/O       |                                   |
| air_intake              | Option&lt;[ksp::oab::ObjectAssemblyAirIntake](/reference/ksp/oab.md#objectassemblyairintake)>                 | R/O       |                                   |
| command_module          | Option&lt;[ksp::oab::ObjectAssemblyCommand](/reference/ksp/oab.md#objectassemblycommand)>                     | R/O       |                                   |
| control_surface         | Option&lt;[ksp::oab::ObjectAssemblyControlSurface](/reference/ksp/oab.md#objectassemblycontrolsurface)>       | R/O       |                                   |
| decouple_stage          | int                                                                                                           | R/O       |                                   |
| decoupler               | Option&lt;[ksp::oab::ObjectAssemblyDecoupler](/reference/ksp/oab.md#objectassemblydecoupler)>                 | R/O       |                                   |
| deployable              | Option&lt;[ksp::oab::ObjectAssemblyDeployable](/reference/ksp/oab.md#objectassemblydeployable)>               | R/O       |                                   |
| docking_node            | Option&lt;[ksp::oab::ObjectAssemblyDockingNode](/reference/ksp/oab.md#objectassemblydockingnode)>             | R/O       |                                   |
| drag                    | Option&lt;[ksp::oab::ObjectAssemblyDrag](/reference/ksp/oab.md#objectassemblydrag)>                           | R/O       |                                   |
| dry_mass                | float                                                                                                         | R/O       | Dry mass of the part              |
| engine                  | Option&lt;[ksp::oab::ObjectAssemblyEngine](/reference/ksp/oab.md#objectassemblyengine)>                       | R/O       |                                   |
| fuel_cross_feed         | bool                                                                                                          | R/O       |                                   |
| generator               | Option&lt;[ksp::oab::ObjectAssemblyGenerator](/reference/ksp/oab.md#objectassemblygenerator)>                 | R/O       |                                   |
| green_mass              | float                                                                                                         | R/O       | Green mass (Kerbals) of the part  |
| is_decoupler            | bool                                                                                                          | R/O       |                                   |
| is_deployable           | bool                                                                                                          | R/O       |                                   |
| is_drag                 | bool                                                                                                          | R/O       |                                   |
| is_engine               | bool                                                                                                          | R/O       |                                   |
| is_generator            | bool                                                                                                          | R/O       |                                   |
| is_lifting_surface      | bool                                                                                                          | R/O       |                                   |
| is_light                | bool                                                                                                          | R/O       |                                   |
| is_rcs                  | bool                                                                                                          | R/O       |                                   |
| is_reaction_wheel       | bool                                                                                                          | R/O       |                                   |
| is_s_science_experiment | bool                                                                                                          | R/O       |                                   |
| is_solar_panel          | bool                                                                                                          | R/O       |                                   |
| is_transmitter          | bool                                                                                                          | R/O       |                                   |
| lifting_surface         | Option&lt;[ksp::oab::ObjectAssemblyLiftingSurface](/reference/ksp/oab.md#objectassemblyliftingsurface)>       | R/O       |                                   |
| light                   | Option&lt;[ksp::oab::ObjectAssemblyLight](/reference/ksp/oab.md#objectassemblylight)>                         | R/O       |                                   |
| part_category           | [ksp::vessel::PartCategory](/reference/ksp/vessel.md#partcategory)                                            | R/O       |                                   |
| part_description        | string                                                                                                        | R/O       |                                   |
| part_name               | string                                                                                                        | R/O       |                                   |
| part_title              | string                                                                                                        | R/O       |                                   |
| rcs                     | Option&lt;[ksp::oab::ObjectAssemblyRCS](/reference/ksp/oab.md#objectassemblyrcs)>                             | R/O       |                                   |
| reaction_wheel          | Option&lt;[ksp::oab::ObjectAssemblyReactionWheel](/reference/ksp/oab.md#objectassemblyreactionwheel)>         | R/O       |                                   |
| relative_position       | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                                                | R/O       |                                   |
| resources               | [ksp::oab::ObjectAssemblyResource](/reference/ksp/oab.md#objectassemblyresource)[]                            | R/O       |                                   |
| science_experiment      | Option&lt;[ksp::oab::ObjectAssemblyScienceExperiment](/reference/ksp/oab.md#objectassemblyscienceexperiment)> | R/O       |                                   |
| solar_panel             | Option&lt;[ksp::oab::ObjectAssemblySolarPanel](/reference/ksp/oab.md#objectassemblysolarpanel)>               | R/O       |                                   |
| total_mass              | float                                                                                                         | R/O       | Total mass of the part            |
| transmitter             | Option&lt;[ksp::oab::ObjectAssemblyTransmitter](/reference/ksp/oab.md#objectassemblytransmitter)>             | R/O       |                                   |
| wet_mass                | float                                                                                                         | R/O       |                                   |


### ObjectAssemblyRCS



#### Fields

| Name           | Type                                                                               | Read-only | Description |
| -------------- | ---------------------------------------------------------------------------------- | --------- | ----------- |
| enable_pitch   | bool                                                                               | R/W       |             |
| enable_roll    | bool                                                                               | R/W       |             |
| enable_x       | bool                                                                               | R/W       |             |
| enable_y       | bool                                                                               | R/W       |             |
| enable_yaw     | bool                                                                               | R/W       |             |
| enable_z       | bool                                                                               | R/W       |             |
| enabled        | bool                                                                               | R/W       |             |
| part           | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)           | R/O       |             |
| part_name      | string                                                                             | R/O       |             |
| propellant     | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition) | R/O       |             |
| thrust_limiter | float                                                                              | R/W       |             |


### ObjectAssemblyReactionWheel



#### Fields

| Name                     | Type                                                                           | Read-only | Description |
| ------------------------ | ------------------------------------------------------------------------------ | --------- | ----------- |
| has_resources_to_operate | bool                                                                           | R/O       |             |
| part                     | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)       | R/O       |             |
| part_name                | string                                                                         | R/O       |             |
| potential_torque         | [ksp::math::Vec3](/reference/ksp/math.md#vec3)                                 | R/O       |             |
| required_resources       | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting)[] | R/O       |             |
| toggle_torque            | bool                                                                           | R/W       |             |
| wheel_actuator_mode      | [ksp::vessel::ActuatorMode](/reference/ksp/vessel.md#actuatormode)             | R/W       |             |
| wheel_authority          | float                                                                          | R/W       |             |
| wheel_state              | [ksp::vessel::ReactionWheelState](/reference/ksp/vessel.md#reactionwheelstate) | R/O       |             |


### ObjectAssemblyResource



#### Fields

| Name           | Type                                                                               | Read-only | Description |
| -------------- | ---------------------------------------------------------------------------------- | --------- | ----------- |
| capacity_units | float                                                                              | R/O       |             |
| resource       | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition) | R/O       |             |
| stored_units   | float                                                                              | R/O       |             |
| total_mass     | float                                                                              | R/O       |             |


### ObjectAssemblyScienceExperiment



#### Fields

| Name        | Type                                                                                   | Read-only | Description |
| ----------- | -------------------------------------------------------------------------------------- | --------- | ----------- |
| experiments | [ksp::oab::ObjectAssemblyExperiment](/reference/ksp/oab.md#objectassemblyexperiment)[] | R/O       |             |
| is_deployed | bool                                                                                   | R/O       |             |
| part        | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)               | R/O       |             |
| part_name   | string                                                                                 | R/O       |             |


### ObjectAssemblySolarPanel



#### Fields

| Name                  | Type                                                                         | Read-only | Description     |
| --------------------- | ---------------------------------------------------------------------------- | --------- | --------------- |
| base_flow_rate        | float                                                                        | R/O       | Base flow rate  |
| efficiency_multiplier | float                                                                        | R/O       |                 |
| part                  | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)     | R/O       |                 |
| part_name             | string                                                                       | R/O       |                 |
| resource_setting      | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting) | R/O       |                 |


### ObjectAssemblyStageDeltaV



#### Fields

| Name           | Type                                                                                       | Read-only | Description                        |
| -------------- | ------------------------------------------------------------------------------------------ | --------- | ---------------------------------- |
| active_engines | [ksp::oab::ObjectAssemblyEngineDeltaV](/reference/ksp/oab.md#objectassemblyenginedeltav)[] | R/O       |                                    |
| burn_time      | float                                                                                      | R/O       | Estimated burn time of the stage.  |
| dry_mass       | float                                                                                      | R/O       | Dry mass of the stage.             |
| end_mass       | float                                                                                      | R/O       | End mass of the stage.             |
| engines        | [ksp::oab::ObjectAssemblyEngineDeltaV](/reference/ksp/oab.md#objectassemblyenginedeltav)[] | R/O       |                                    |
| fuel_mass      | float                                                                                      | R/O       | Mass of the fuel in the stage.     |
| parts          | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)[]                 | R/O       |                                    |
| stage          | int                                                                                        | R/O       | The stage number.                  |
| start_mass     | float                                                                                      | R/O       | Start mass of the stage.           |


#### Methods

##### get_ISP

```rust
objectassemblystagedeltav.get_ISP ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated ISP of the stage in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


##### get_TWR

```rust
objectassemblystagedeltav.get_TWR ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated TWR of the stage in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


##### get_deltav

```rust
objectassemblystagedeltav.get_deltav ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated delta-v of the stage in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


##### get_thrust

```rust
objectassemblystagedeltav.get_thrust ( situation : ksp::vessel::DeltaVSituation ) -> float
```

Estimated thrust of the stage in a given `situation`


Parameters

| Name      | Type                         | Optional | Description |
| --------- | ---------------------------- | -------- | ----------- |
| situation | ksp::vessel::DeltaVSituation |          |             |


### ObjectAssemblyTransmitter



#### Fields

| Name                          | Type                                                                           | Read-only | Description |
| ----------------------------- | ------------------------------------------------------------------------------ | --------- | ----------- |
| active_transmission_completed | float                                                                          | R/O       |             |
| active_transmission_size      | float                                                                          | R/O       |             |
| communication_range           | float                                                                          | R/O       |             |
| data_packet_size              | float                                                                          | R/O       |             |
| data_transmission_interval    | float                                                                          | R/O       |             |
| has_resources_to_operate      | bool                                                                           | R/O       |             |
| is_transmitting               | bool                                                                           | R/O       |             |
| part                          | [ksp::oab::ObjectAssemblyPart](/reference/ksp/oab.md#objectassemblypart)       | R/O       |             |
| part_name                     | string                                                                         | R/O       |             |
| required_resources            | [ksp::resource::ResourceSetting](/reference/ksp/resource.md#resourcesetting)[] | R/O       |             |


## Functions


### active_object_assembly_builder

```rust
pub sync fn active_object_assembly_builder ( ) -> Result<ksp::oab::ObjectAssemblyBuilder>
```

Try to get the currently active vessel. Will result in an error if there is none.

