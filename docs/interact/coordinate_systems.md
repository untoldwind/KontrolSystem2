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
