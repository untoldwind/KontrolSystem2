# ksp::science

Collection of types and functions to get information and manipulate in-game science experiments.


## Types


### Experiment

Represents an in-game science experiment.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
crew_required | int | R/O | 
definition | [ksp::science::ExperimentDefinition](/reference/ksp/science.md#experimentdefinition) | R/O | 
time_to_complete | float | R/O | 

### ExperimentDefinition

Represents definition of an in-game science experiment.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
data_value | float | R/O | 
display_name | string | R/O | 
id | string | R/O | 
requires_eva | bool | R/O | 
sample_value | float | R/O | 
transmission_size | float | R/O | 

### FlowDirection

Resource flow direction

#### Methods

##### to_string

```rust
flowdirection.to_string ( ) -> string
```

String representation of the number

### FlowDirectionConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
FLOW_INBOUND | [ksp::resource::FlowDirection](/reference/ksp/resource.md#flowdirection) | R/O | 
FLOW_OUTBOUND | [ksp::resource::FlowDirection](/reference/ksp/resource.md#flowdirection) | R/O | Science experiment producing data

#### Methods

##### from_string

```rust
flowdirectionconstants.from_string ( value : string ) -> Option<ksp::resource::FlowDirection>
```

Parse from string

## Constants

Name | Type | Description
--- | --- | ---
FlowDirection | ksp::resource::FlowDirectionConstants | Resource flow direction

