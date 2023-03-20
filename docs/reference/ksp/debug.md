# ksp::debug

Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire.


## Types


### Debug

Collection of debug helper


#### Methods

##### add_ground_marker

```rust
debug.add_ground_marker ( geoCoordinates : ksp::orbit::GeoCoordinates,
                          color : ksp::console::RgbaColor,
                          rotation : float ) -> ksp::debug::GroundMarker
```



##### add_line

```rust
debug.add_line ( startProvider : fn() -> ksp::math::GlobalPosition,
                 endProvider : fn() -> ksp::math::GlobalPosition,
                 color : ksp::console::RgbaColor,
                 label : string,
                 width : float ) -> ksp::debug::DebugVector
```

Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
The line may have a `label` at its mid-point.



##### add_vector

```rust
debug.add_vector ( startProvider : fn() -> ksp::math::GlobalPosition,
                   vectorProvider : fn() -> ksp::math::GlobalVector,
                   color : ksp::console::RgbaColor,
                   label : string,
                   width : float ) -> ksp::debug::DebugVector
```

Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
The vector may have a `label` at its mid-point.



##### clear_markers

```rust
debug.clear_markers ( ) -> Unit
```

Remove all markers from the game-scene.


### DebugVector

Represents a debugging vector in the current scene.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W | The color of the debugging vector 
end | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/W | The current end position of the debugging vector. 
pointy | bool | R/W | Controls if an arrow should be drawn at the end. 
scale | float | R/W | 
start | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/W | The current starting position of the debugging vector. 
visible | bool | R/W | Controls if the debug-vector is currently visible (initially `true`) 
width | float | R/W | The width of the debugging vector 

#### Methods

##### set_end_provider

```rust
debugvector.set_end_provider ( endProvider : fn() -> ksp::math::GlobalPosition ) -> Unit
```

Change the function providing the end position of the debug vector.


##### set_start_provider

```rust
debugvector.set_start_provider ( startProvider : fn() -> ksp::math::GlobalPosition ) -> Unit
```

Change the function providing the start position of the debug vector.


##### set_vector_provider

```rust
debugvector.set_vector_provider ( vectorProvider : fn() -> ksp::math::GlobalVector ) -> Unit
```

Change the function providing the vector/direction of the debug vector.


### GroundMarker

Represents a ground marker on a given celestial body.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W | The color of the ground marker vector 
geo_coordinates | [ksp::orbit::GeoCoordinates](/reference/ksp/orbit.md#geocoordinates) | R/W | 
rotation | float | R/W | 
visible | bool | R/W | Controls if the ground marker is currently visible (initially `true`) 

#### Methods

##### remove

```rust
groundmarker.remove ( ) -> Unit
```



## Constants

Name | Type | Description
--- | --- | ---
DEBUG | ksp::debug::Debug | Collection of debug helper 

