# ksp::game

Collection to game and runtime related functions.


## Types


### Mainframe



#### Fields

| Name                | Type                                                   | Read-only | Description |
| ------------------- | ------------------------------------------------------ | --------- | ----------- |
| available_processes | [ksp::game::Process](/reference/ksp/game.md#process)[] | R/O       |             |


#### Methods

##### find_process

```rust
mainframe.find_process ( name : string ) -> Option<ksp::game::Process>
```



Parameters

| Name | Type   | Optional | Description |
| ---- | ------ | -------- | ----------- |
| name | string |          |             |


### Process



#### Fields

| Name       | Type                                                                   | Read-only | Description |
| ---------- | ---------------------------------------------------------------------- | --------- | ----------- |
| arguments  | [ksp::game::ProcessArgument](/reference/ksp/game.md#processargument)[] | R/O       |             |
| is_running | bool                                                                   | R/O       |             |
| name       | string                                                                 | R/O       |             |


#### Methods

##### start

```rust
process.start ( forVessel : Option<ksp::vessel::Vessel>,
                arguments : string[] ) -> bool
```



Parameters

| Name      | Type                        | Optional | Description |
| --------- | --------------------------- | -------- | ----------- |
| forVessel | Option<ksp::vessel::Vessel> | x        |             |
| arguments | string[]                    | x        |             |


##### stop

```rust
process.stop ( ) -> bool
```



### ProcessArgument



#### Fields

| Name          | Type   | Read-only | Description |
| ------------- | ------ | --------- | ----------- |
| default_value | string | R/O       |             |
| name          | string | R/O       |             |
| type          | string | R/O       |             |


## Constants

| Name      | Type                 | Description              |
| --------- | -------------------- | ------------------------ |
| MAINFRAME | ksp::game::Mainframe | KontrolSystem mainframe  |


## Functions


### current_time

```rust
pub sync fn current_time ( ) -> float
```

Get the current universal time (UT) in seconds from start.


### sleep

```rust
pub fn sleep ( seconds : float ) -> Unit
```

Stop execution of given number of seconds (factions of a seconds are supported as well).


Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| seconds | float |          |             |


### wait_until

```rust
pub fn wait_until ( predicate : sync fn() -> bool ) -> Unit
```

Stop execution until a given condition is met.


Parameters

| Name      | Type              | Optional | Description |
| --------- | ----------------- | -------- | ----------- |
| predicate | sync fn() -> bool |          |             |


### wait_while

```rust
pub fn wait_while ( predicate : sync fn() -> bool ) -> Unit
```

Stop execution as long as a given condition is met.


Parameters

| Name      | Type              | Optional | Description |
| --------- | ----------------- | -------- | ----------- |
| predicate | sync fn() -> bool |          |             |


### yield

```rust
pub fn yield ( ) -> Unit
```

Yield execution to allow Unity to do some other stuff inbetween.

