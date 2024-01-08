# Coordinate system independent vectors

The following new types have been introduces to `ksp::math`
* `GlobalPosition` a position in space
* `GlobalVector` a vector in space (i.e. a direction with a magnitude)
* `GlobalDirection` a direction/orientation (like the orientation of the vessel)
* `GlobalVelocity` a velocity
* `GlobalAngularVelocity` an angular velocity

All of these are coordinate system independent, i.e. to get the actual `x`, `y`, `z` values one has to provide a frame of reference/coordinate system.
The `Body`, `Vessel` and `Orbit` types now have corresponding `global_...` methods to obtain these value.

The following frames of reference are available:
* `body.celestial_frame` the celestial (non-rotating) frame of a planet or moon
* `body.body_frame` the rotating frame of a planet or moon
* `vessel.celestial_frame` the celestial (non-rotating) frame of a vessel
* `vessel.body_frame` the rotating frame of a vessel
* `vessel.control_frame` the rotating frame of a vessel from the current control point (e.g this changes when controlling a vessel from a docking port)

As this sound pretty abstract here are some examples:

```rust
const p : GlobalPosition = vessel.global_position

const p_body : Vec3 = p.to_local(vessel.main_body.celestial_frame)

const p_vessel : Vec3 = p.to_local(vessel.celestial_frame)
```
in this case
* `p` represents the current position of the vessel
* `p_body` is the coordinates of the vessel in the coordinate system of the main body in the current SOI
* `p_vessel` is the coordinates of the vessel in the coordinate system of the vessel itself, which happens to be `[0, 0, 0]`

To print a `GlobalPosition` to the console, it is also necessary to provide a coordinate system

```rust
const frame = vessel.main_body.celestial_frame  // we will do our calculations in this frame of reference

CONSOLE.print_line(p.to_string(frame))
CONSOLE.print_line(p.to_fixed(frame, 3))
```

A `GlobalVector` is very similar to a `GlobalPosition`, just that it represents something that goes from A to B, which is represented in the calculations:

```rust
// Just to positions of something
const p1 : GlobalPosition = ...
const p2 : GlobalPosition = ...

p2 - p1     // this a GlobalVector from p1 to p2
p1 - p2     // this a GlobalVector from p2 to p1
p1 + p2     // ERROR: This is not allowed

// A vector
const v1 : GlobalVector = ...

p1 + v1         // New position when moving from p1 in direction v1
p2 + 2 * v2     // New position when moving from p2 twice the distrance in direction v1
```

Things become more tricky when dealing with velocities, because it now depends if the frame of reference itself is in motion or not.
In general: The `celestial_frame` are considered to be non-rotation (fixed to celestial stars) while the `body_frame` is taking the rotation of the current body into account.

```
const v = vessel.global_velocity

const v_celestial = v.to_local(vessel.main_body.celestial_frame)

const v_body = v.to_local(vessel.main_body.body_frame) 
```
in this case
* `v` represents the current velocity of the vessel
* `v_celestial` is the velocity vector in the celestial frame of the main body, this is also known as the `orbital_velocity`
* `v_body` is the velocity vector in the rotating frame of the main body, this is also known as the `surface_velocity`

## Supported operations

### GlobalPosition

* Two `GlobalPosition` can be subtracted from each other resulting in a `GlobalVector`
  ```rust
  let p1 : GlobalPosition = ...
  let p2 : GlobalPosition = ...
  let v : GlobalVector = p2 - p1 // Global vector pointing from p1 to p2
  ```
* A `GlobalVector` can be added to a `GlobalPosition` resulting in a new `GlobalPosition`
  ```rust
  // p1, p2, v from above
  let p3 : GlobalPosition = p1 + v // This will be the same as p2
  ```
* `GlobalPosition` has a `distance` helper to calculate the distance between two points in space:
  ```rust
  let p1 : GlobalPosition = ...
  let p2 : GlobalPosition = ...
  let d : float = p1.distance(p2)
  let d2 : float = p1.distance_sqr(p2) // Squared distance, i.e. d2 = d * d
  ```
* `GlobalPosition` has a `lerp_to` helper to get midpoints between two positions
  ```rust
  let p1 : GlobalPosition = ...
  let p2 : GlobalPosition = ...
  let p3 : GlobalPosition = p1.lerp_to(p2, 0.5)  // Midpoint between p1, p2
  let v : GlobalVector = p2 - p1
  let p4 : GlobalPosition = p1 + 0.5 * v         // Same as p3 
  ```

### GlobalVector

`GlobalVector` can be used almost the same as a regular vector.

* A `GlobalVector` can be multiplied with a `float` resulting in a new `GlobalVector`
  ```rust
  let v : GlobalVector = ...
  let f : float = ...
  let v1 : GlobalVector = f * v
  let v2 : GlobalVector = v * f
  ```
* Two `GlobalVector` can be added to and subtracted from each other resulting in a new `GlobalVector`
  ```rust
  let v1 : GlobalVector = ...
  let v2 : GlobalVector = ...
  let v3 : GlobalVector = v1 + v2
  let v4 : GlobalVector = v1 + v2
  ```
* `GlobalVector` supports the dot-product:
  ```rust
  let v1 : GlobalVector = ...
  let v2 : GlobalVector = ...
  let f1 : float = v1 * v2
  let f2 : float = v1.dot(v2)  // Same as above
  ```
* `GlobalVector` supports the cross-product:
  ```rust
  let v1 : GlobalVector = ...
  let v2 : GlobalVector = ...
  let v3 : GlobalVector = v1.cross(v2)
  ```