# ksp::debug

Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire.


## Types


### Debug

Collection of debug helper


#### Methods

##### add_billboard

```rust
debug.add_billboard ( positionProvider : sync fn() -> ksp::math::GlobalPosition,
                      textProvider : sync fn() -> string,
                      color : ksp::console::RgbaColor,
                      fontSize : int ) -> ksp::debug::DebugBillboard
```



Parameters

| Name             | Type                                   | Optional | Description |
| ---------------- | -------------------------------------- | -------- | ----------- |
| positionProvider | sync fn() -> ksp::math::GlobalPosition |          |             |
| textProvider     | sync fn() -> string                    |          |             |
| color            | ksp::console::RgbaColor                |          |             |
| fontSize         | int                                    |          |             |


##### add_ground_marker

```rust
debug.add_ground_marker ( geoCoordinates : ksp::orbit::GeoCoordinates,
                          color : ksp::console::RgbaColor,
                          rotation : float ) -> ksp::debug::GroundMarker
```



Parameters

| Name           | Type                       | Optional | Description |
| -------------- | -------------------------- | -------- | ----------- |
| geoCoordinates | ksp::orbit::GeoCoordinates |          |             |
| color          | ksp::console::RgbaColor    |          |             |
| rotation       | float                      |          |             |


##### add_line

```rust
debug.add_line ( startProvider : sync fn() -> ksp::math::GlobalPosition,
                 endProvider : sync fn() -> ksp::math::GlobalPosition,
                 color : ksp::console::RgbaColor,
                 label : string,
                 width : float ) -> ksp::debug::DebugVector
```

Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
The line may have a `label` at its mid-point.



Parameters

| Name          | Type                                   | Optional | Description |
| ------------- | -------------------------------------- | -------- | ----------- |
| startProvider | sync fn() -> ksp::math::GlobalPosition |          |             |
| endProvider   | sync fn() -> ksp::math::GlobalPosition |          |             |
| color         | ksp::console::RgbaColor                |          |             |
| label         | string                                 |          |             |
| width         | float                                  |          |             |


##### add_path

```rust
debug.add_path ( path : ksp::math::GlobalPosition[],
                 color : ksp::console::RgbaColor,
                 width : float ) -> ksp::debug::DebugPath
```



Parameters

| Name  | Type                        | Optional | Description |
| ----- | --------------------------- | -------- | ----------- |
| path  | ksp::math::GlobalPosition[] |          |             |
| color | ksp::console::RgbaColor     |          |             |
| width | float                       |          |             |


##### add_vector

```rust
debug.add_vector ( startProvider : sync fn() -> ksp::math::GlobalPosition,
                   vectorProvider : sync fn() -> ksp::math::GlobalVector,
                   color : ksp::console::RgbaColor,
                   label : string,
                   width : float ) -> ksp::debug::DebugVector
```

Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
The vector may have a `label` at its mid-point.



Parameters

| Name           | Type                                   | Optional | Description |
| -------------- | -------------------------------------- | -------- | ----------- |
| startProvider  | sync fn() -> ksp::math::GlobalPosition |          |             |
| vectorProvider | sync fn() -> ksp::math::GlobalVector   |          |             |
| color          | ksp::console::RgbaColor                |          |             |
| label          | string                                 |          |             |
| width          | float                                  |          |             |


##### clear_markers

```rust
debug.clear_markers ( ) -> Unit
```

Remove all markers from the game-scene.


### DebugBillboard

Represents a ground marker on a given celestial body.


#### Fields

| Name      | Type                                                           | Read-only | Description                                                        |
| --------- | -------------------------------------------------------------- | --------- | ------------------------------------------------------------------ |
| color     | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       | The color of the billboard text                                    |
| font_size | int                                                            | R/W       |                                                                    |
| visible   | bool                                                           | R/W       | Controls if the billboard is currently visible (initially `true`)  |


#### Methods

##### remove

```rust
debugbillboard.remove ( ) -> Unit
```



### DebugPath

Represents a debugging path in the current scene.


#### Fields

| Name    | Type                                                                 | Read-only | Description                                                         |
| ------- | -------------------------------------------------------------------- | --------- | ------------------------------------------------------------------- |
| color   | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor)       | R/W       | The color of the debugging path                                     |
| path    | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition)[] | R/W       |                                                                     |
| visible | bool                                                                 | R/W       | Controls if the debug path is currently visible (initially `true`)  |
| width   | float                                                                | R/W       | The width of the debugging path                                     |


#### Methods

##### remove

```rust
debugpath.remove ( ) -> Unit
```



### DebugVector

Represents a debugging vector in the current scene.


#### Fields

| Name    | Type                                                               | Read-only | Description                                                           |
| ------- | ------------------------------------------------------------------ | --------- | --------------------------------------------------------------------- |
| color   | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor)     | R/W       | The color of the debugging vector                                     |
| end     | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/W       | The current end position of the debugging vector.                     |
| pointy  | bool                                                               | R/W       | Controls if an arrow should be drawn at the end.                      |
| scale   | float                                                              | R/W       |                                                                       |
| start   | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/W       | The current starting position of the debugging vector.                |
| visible | bool                                                               | R/W       | Controls if the debug-vector is currently visible (initially `true`)  |
| width   | float                                                              | R/W       | The width of the debugging vector                                     |


#### Methods

##### remove

```rust
debugvector.remove ( ) -> Unit
```



##### set_end_provider

```rust
debugvector.set_end_provider ( endProvider : sync fn() -> ksp::math::GlobalPosition ) -> Unit
```

Change the function providing the end position of the debug vector.


Parameters

| Name        | Type                                   | Optional | Description |
| ----------- | -------------------------------------- | -------- | ----------- |
| endProvider | sync fn() -> ksp::math::GlobalPosition |          |             |


##### set_start_provider

```rust
debugvector.set_start_provider ( startProvider : sync fn() -> ksp::math::GlobalPosition ) -> Unit
```

Change the function providing the start position of the debug vector.


Parameters

| Name          | Type                                   | Optional | Description |
| ------------- | -------------------------------------- | -------- | ----------- |
| startProvider | sync fn() -> ksp::math::GlobalPosition |          |             |


##### set_vector_provider

```rust
debugvector.set_vector_provider ( vectorProvider : sync fn() -> ksp::math::GlobalVector ) -> Unit
```

Change the function providing the vector/direction of the debug vector.


Parameters

| Name           | Type                                 | Optional | Description |
| -------------- | ------------------------------------ | -------- | ----------- |
| vectorProvider | sync fn() -> ksp::math::GlobalVector |          |             |


### GroundMarker

Represents a ground marker on a given celestial body.


#### Fields

| Name            | Type                                                                 | Read-only | Description                                                            |
| --------------- | -------------------------------------------------------------------- | --------- | ---------------------------------------------------------------------- |
| color           | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor)       | R/W       | The color of the ground marker vector                                  |
| geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | R/W       |                                                                        |
| rotation        | float                                                                | R/W       |                                                                        |
| visible         | bool                                                                 | R/W       | Controls if the ground marker is currently visible (initially `true`)  |


#### Methods

##### remove

```rust
groundmarker.remove ( ) -> Unit
```



### LogFile

Represents a log file.


#### Methods

##### log

```rust
logfile.log ( message : string ) -> Unit
```

Write a log message to the file.


Parameters

| Name    | Type   | Optional | Description |
| ------- | ------ | -------- | ----------- |
| message | string |          |             |


##### truncate

```rust
logfile.truncate ( ) -> Unit
```

Truncate/clear the log file.


## Constants

| Name     | Type                | Description                    |
| -------- | ------------------- | ------------------------------ |
| DEBUG    | ksp::debug::Debug   | Collection of debug helper     |
| MAIN_LOG | ksp::debug::LogFile | Main script specific log file  |


## Functions


### open_log_file

```rust
pub sync fn open_log_file ( name : string ) -> ksp::debug::LogFile
```



Parameters

| Name | Type   | Optional | Description |
| ---- | ------ | -------- | ----------- |
| name | string |          |             |

