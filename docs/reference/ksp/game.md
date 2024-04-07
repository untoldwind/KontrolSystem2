# ksp::game

Collection to game and runtime related functions.


## Types


### EventProcessStarted

Process started event will be published to message bus when a process is started

#### Fields

| Name      | Type     | Read-only | Description             |
| --------- | -------- | --------- | ----------------------- |
| arguments | string[] | R/O       | Process start arguments |
| name      | string   | R/O       | Process name            |


### EventProcessStopped

Process stop event will be published to message bus when a process is stopped

#### Fields

| Name  | Type              | Read-only | Description                                   |
| ----- | ----------------- | --------- | --------------------------------------------- |
| error | Option&lt;string> | R/O       | Error message in case of abnormal termination |
| name  | string            | R/O       | Process name                                  |


### Importance

Importance of a notification

#### Methods

##### to_string

```rust
importance.to_string ( ) -> string
```

String representation of the number

### ImportanceConstants



#### Fields

| Name   | Type                                                       | Read-only | Description |
| ------ | ---------------------------------------------------------- | --------- | ----------- |
| High   | [ksp::game::Importance](/reference/ksp/game.md#importance) | R/O       | High        |
| Low    | [ksp::game::Importance](/reference/ksp/game.md#importance) | R/O       | Low         |
| Medium | [ksp::game::Importance](/reference/ksp/game.md#importance) | R/O       | Medium      |
| None   | [ksp::game::Importance](/reference/ksp/game.md#importance) | R/O       |             |


#### Methods

##### from_string

```rust
importanceconstants.from_string ( value : string ) -> Option<ksp::game::Importance>
```

Parse from string

Parameters

| Name  | Type   | Optional | Description          |
| ----- | ------ | -------- | -------------------- |
| value | string |          | Enum value to lookup |


### Mainframe



#### Fields

| Name                | Type                                                   | Read-only | Description                          |
| ------------------- | ------------------------------------------------------ | --------- | ------------------------------------ |
| available_processes | [ksp::game::Process](/reference/ksp/game.md#process)[] | R/O       |                                      |
| version             | string                                                 | R/O       | Version number of the KontrolSystem  |


#### Methods

##### find_process

```rust
mainframe.find_process ( name : string ) -> Option<ksp::game::Process>
```



Parameters

| Name | Type   | Optional | Description |
| ---- | ------ | -------- | ----------- |
| name | string |          |             |


### MessageBus

Shared message bus


#### Methods

##### publish

```rust
messagebus.publish ( message : T ) -> Unit
```

Publish a message to anyone interested (or the void)


Parameters

| Name    | Type | Optional | Description |
| ------- | ---- | -------- | ----------- |
| message | T    |          |             |


##### subscribe

```rust
messagebus.subscribe ( ) -> ksp::game::Subscription<T>
```

Create a subscription to a specific message type


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


### Subscription

Central message bus

#### Fields

| Name         | Type | Read-only | Description                                |
| ------------ | ---- | --------- | ------------------------------------------ |
| has_messages | bool | R/O       | Check if subscription has pending messages |


#### Methods

##### peek

```rust
subscription.peek ( ) -> Option<T>
```

Peek for next message without consuming it

##### recv

```rust
subscription.recv ( ) -> Option<T>
```

Receive next message

##### unsubscribe

```rust
subscription.unsubscribe ( ) -> Unit
```

Unsubscribe from the message bus. No further messages will be received

## Constants

| Name        | Type                           | Description                  |
| ----------- | ------------------------------ | ---------------------------- |
| Importance  | ksp::game::ImportanceConstants | Importance of a notification |
| MAINFRAME   | ksp::game::Mainframe           | KontrolSystem mainframe      |
| MESSAGE_BUS | ksp::game::MessageBus          | Shared message bus           |


## Functions


### current_time

```rust
pub sync fn current_time ( ) -> float
```

Get the current universal time (UT) in seconds from start.


### notification_alert

```rust
pub sync fn notification_alert ( title : string,
                                 message : string,
                                 importance : ksp::game::Importance,
                                 duration : float ) -> Unit
```

Show an alert notification


Parameters

| Name       | Type                  | Optional | Description |
| ---------- | --------------------- | -------- | ----------- |
| title      | string                |          |             |
| message    | string                |          |             |
| importance | ksp::game::Importance | x        |             |
| duration   | float                 | x        |             |


### notification_passive

```rust
pub sync fn notification_passive ( message : string,
                                   duration : float ) -> Unit
```

Show a passive notification


Parameters

| Name     | Type   | Optional | Description |
| -------- | ------ | -------- | ----------- |
| message  | string |          |             |
| duration | float  | x        |             |


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

