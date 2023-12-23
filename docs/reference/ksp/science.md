# ksp::science

Collection of types and functions to get information and manipulate in-game science experiments.


## Types


### Experiment

Represents an in-game science experiment.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
condition_met | bool | R/O | 
crew_required | int | R/O | 
current_experiment_state | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | 
current_running_time | float | R/O | 
current_situation_is_valid | bool | R/O | 
definition | [ksp::science::ExperimentDefinition](/reference/ksp/science.md#experimentdefinition) | R/O | 
experiment_location | Option&lt;[ksp::science::ResearchLocation](/reference/ksp/science.md#researchlocation)> | R/O | 
has_enough_resources | bool | R/O | 
previous_experiment_state | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | 
region_required | bool | R/O | 
time_to_complete | float | R/O | 
valid_locations | [ksp::science::ResearchLocation](/reference/ksp/science.md#researchlocation)[] | R/O | 

#### Methods

##### cancel_experiment

```rust
experiment.cancel_experiment ( ) -> bool
```



##### pause_experiment

```rust
experiment.pause_experiment ( ) -> bool
```



##### run_experiment

```rust
experiment.run_experiment ( ) -> bool
```



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

### ExperimentState

Science experiment state

#### Methods

##### to_string

```rust
experimentstate.to_string ( ) -> string
```

String representation of the number

### ExperimentStateConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
ALREADYSTORED | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment has already stored results
BLOCKED | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment is blocked
INSUFFICIENTCREW | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment requires more available crew members
INSUFFICIENTSTORAGE | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Not enough storage capacity for experiment
INVALIDLOCATION | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Location not valid
LOCATIONCHANGED | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment location changed
NOCONTROL | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment requires control of the vessel
NONE | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Unknown state
OUTOFRESOURCE | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment ran out of resources
PAUSED | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment is paused
READY | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment is ready to run
RUNNING | [ksp::resource::ExperimentState](/reference/ksp/resource.md#experimentstate) | R/O | Experiment is running

#### Methods

##### from_string

```rust
experimentstateconstants.from_string ( value : string ) -> Option<ksp::resource::ExperimentState>
```

Parse from string

### ResearchLocation

Represents a research location of a science experiment.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
body_name | string | R/O | 
requires_region | bool | R/O | 
science_region | string | R/O | 
science_situation | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | 

### ScienceExperimentType

Science experiment type

#### Methods

##### to_string

```rust
scienceexperimenttype.to_string ( ) -> string
```

String representation of the number

### ScienceExperimentTypeConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Both | [ksp::resource::ScienceExperimentType](/reference/ksp/resource.md#scienceexperimenttype) | R/O | Science experiment producing both sample and data
DataType | [ksp::resource::ScienceExperimentType](/reference/ksp/resource.md#scienceexperimenttype) | R/O | Science experiment producing data
SampleType | [ksp::resource::ScienceExperimentType](/reference/ksp/resource.md#scienceexperimenttype) | R/O | Science experiment producing sample

#### Methods

##### from_string

```rust
scienceexperimenttypeconstants.from_string ( value : string ) -> Option<ksp::resource::ScienceExperimentType>
```

Parse from string

### ScienceSituation

Situation of a science experiment

#### Methods

##### to_string

```rust
sciencesituation.to_string ( ) -> string
```

String representation of the number

### ScienceSituationConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Atmosphere | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | Experiment inside an atmosphere
HighOrbit | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | Experiment in high orbit
Landed | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | Experiment while landed
LowOrbit | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | Experiment in low orbit
None | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | No specific situation required
Splashed | [ksp::resource::ScienceSituation](/reference/ksp/resource.md#sciencesituation) | R/O | Experiment while splashed

#### Methods

##### from_string

```rust
sciencesituationconstants.from_string ( value : string ) -> Option<ksp::resource::ScienceSituation>
```

Parse from string

## Constants

Name | Type | Description
--- | --- | ---
ExperimentState | ksp::resource::ExperimentStateConstants | Science experiment state
ScienceExperimentType | ksp::resource::ScienceExperimentTypeConstants | Science experiment type
ScienceSituation | ksp::resource::ScienceSituationConstants | Situation of a science experiment

