# ksp::addons

Provides access to optional addons.


## Types


### FlightPlan



#### Fields

| Name    | Type   | Read-only | Description |
| ------- | ------ | --------- | ----------- |
| version | string | R/O       |             |


#### Methods

##### circularize

```rust
flightplan.circularize ( burnUt : float,
                         burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### course_correction

```rust
flightplan.course_correction ( burnUt : float,
                               burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### ellipticize

```rust
flightplan.ellipticize ( burnUt : float,
                         newAp : float,
                         newPe : float,
                         burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newAp            | float |          |             |
| newPe            | float |          |             |
| burnOffsetFactor | float | x        |             |


##### hohmann_transfer

```rust
flightplan.hohmann_transfer ( burnUt : float,
                              burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### intercept_tgt

```rust
flightplan.intercept_tgt ( burnUt : float,
                           tgtUt : float,
                           burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| tgtUt            | float |          |             |
| burnOffsetFactor | float | x        |             |


##### match_planes

```rust
flightplan.match_planes ( burnUt : float,
                          burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### match_velocity

```rust
flightplan.match_velocity ( burnUt : float,
                            burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### moon_return

```rust
flightplan.moon_return ( burnUt : float,
                         burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### planetary_xfer

```rust
flightplan.planetary_xfer ( burnUt : float,
                            burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_inclination

```rust
flightplan.set_inclination ( burnUt : float,
                             inclination : float,
                             burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| inclination      | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_new_ap

```rust
flightplan.set_new_ap ( burnUt : float,
                        newAp : float,
                        burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newAp            | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_new_lan

```rust
flightplan.set_new_lan ( burnUt : float,
                         newLanValue : float,
                         burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newLanValue      | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_new_pe

```rust
flightplan.set_new_pe ( burnUt : float,
                        newPe : float,
                        burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newPe            | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_new_sma

```rust
flightplan.set_new_sma ( burnUt : float,
                         newSma : float,
                         burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newSma           | float |          |             |
| burnOffsetFactor | float | x        |             |


##### set_node_longitude

```rust
flightplan.set_node_longitude ( burnUt : float,
                                newNodeLongValue : float,
                                burnOffsetFactor : float ) -> bool
```



Parameters

| Name             | Type  | Optional | Description |
| ---------------- | ----- | -------- | ----------- |
| burnUt           | float |          |             |
| newNodeLongValue | float |          |             |
| burnOffsetFactor | float | x        |             |


## Functions


### flight_plan

```rust
pub sync fn flight_plan ( ) -> Option<ksp::addons::FlightPlan>
```

Access the "Flight Plan" API (https://github.com/schlosrat/FlightPlan)
Will be undefined if FlightPlan is not installed.

