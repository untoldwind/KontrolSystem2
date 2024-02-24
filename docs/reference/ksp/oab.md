# ksp::oab

Collection of types and functions to get information about the current object/vessel assembly.


## Types


### ObjectAssembly

Represents an object assembly, i.e. a potential vessel.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
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

### ObjectAssemblyPart

Represents are part in an object assembly.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
activation_stage | int | R/O | 
decouple_stage | int | R/O | 
dry_mass | float | R/O | Dry mass of the part 
fuel_cross_feed | bool | R/O | 
green_mass | float | R/O | Green mass (Kerbals) of the part 
part_name | string | R/O | 
relative_position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
total_mass | float | R/O | Total mass of the part 
wet_mass | float | R/O | 

## Functions


### active_object_assembly_builder

```rust
pub sync fn active_object_assembly_builder ( ) -> Result<ksp::oab::ObjectAssemblyBuilder, string>
```

Try to get the currently active vessel. Will result in an error if there is none.

