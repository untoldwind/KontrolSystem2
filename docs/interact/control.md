# Controlling the vessel

## SAS

The simplest way to control the orientation of the vessel is by using the SAS system.

SAS can be enabled/disable via

```
vessel.autopilot.enabled = true/false
```
or
```
vessel.actions.sas = true/false
```
Note: The SAS action group is somewhat wonky, so under the hood both ways do exactly the same thing.

The SAS system can be then set to different modes:
```
vessel.autopilot.mode = MODE_....
```
with
* `MODE_STABILITYASSIST`: Default mode that does not any steering, just kills rotation
* `MODE_PROGRADE`, `MODE_RETROGRADE`, `MODE_NORMAL`, `MODE_ANTINORMAL`, `MODE_RADIALIN`, `MODE_RADIALOUT`: Steer to orbital directions
* `MODE_TARGET`: Steer towards target (if vessel has a target)
* `MODE_ANTITARGET`: Steer away from taret (if vessel has a target)
* `MODE_MANEUVER`: Steer in direction of next maneuvering node (if there is one)
* `MODE_NAVIGATION`, `MODE_AUTOPILOT`: Not sure what the difference is between those two, but you can provide a target orientation.

So
```
vessel.autopilot.enabled = true
vessel.autopilot.mode = MODE_AUTOPILOT
vessel.autopilot.target_orientation = direction
```
tells the SAS to steer the vessel into `direction`.

## Use input override methods

If direct control is required is is possible to just override the controls of a vessel by using the following methods:
* [`override_input_pitch`](../reference/ksp/vessel.md#overrideinputpitch)
* [`override_input_yaw`](../reference/ksp/vessel.md#overrideinputyaw)
* [`override_input_roll`](../reference/ksp/vessel.md#overrideinputroll)
* [`override_input_translate_x`](../reference/ksp/vessel.md#overrideinputtranslatex)
* [`override_input_translate_y`](../reference/ksp/vessel.md#overrideinputtranslatey)
* [`override_input_translate_z`](../reference/ksp/vessel.md#overrideinputtranslatez)

## Control manager

All control managers in the [`ksp::control`](../reference/ksp/control.md) module follow the same principle:

```
let manager = vessel.manage_the_thing_to_control(value_provider)  // Script takes over the control (overriding the pilot)

// ... wait/sleep for whatever condition

manager.release()  // Script releases control, i.e. give control back to the pilot


// Optionally wait from something else

manager.resume()  // Take back control again
```

The `value_provider` is a function that will call on very game update, it gets a `deltaT: float` of the time since the last update and return the steering value.

### ThrottleManager

```
let manager = vessel.manage_throttle(fn(deltaT) -> throttle_value) 
```
manages the throttle of a rocket or airplane. The value is between `0.0` (no throttle) and `1.0` (full throttle).

```
let manager = vessel.set_throttle(throttle_value) 
```
is just a shorthand to set the throttle to a constant value. This can be changed at any time via `manager.throttle = new_value`.

### SteeringManager

```
let manager = vessel.manage_steering(fn(deltaT) -> vec3(pitch_value, yaw_value, roll_value)) 
```
manages the pitch, yaw and roll of the flight stick. All values are between `-1.0` and `1.0` (full rudder in either direction)

```
let manager = vessel.set_sterring(vec3(pitch_value, yaw_value, roll_value)) 
```
is just a shorthand to set pitch, yaw, roll to a constant value. This can be changed at any time via `manager.pitch_yaw_roll = new_value`.

### RCSTranslateManager

```
let manager = vessel.manage_rcs_translate(fn(deltaT) -> vec3(x, y, z)) 
```
manages the RCS controls. All values are between `-1.0` and `1.0` (full thrust in either direction)

```
let manager = vessel.set_rcs_translate(vec3(x, y, z)) 
```
is just a shorthand RCS thrust to a constant value. This can be changed at any time via `manager.translate = new_value`.

Note: This will do nothing is RCS is disabled.

RCS can be enabled/disabled via

```
vessel.actions.rcs = true/false
```

### WheelSteeringManager / WheelThrottleManager

... the corresponding control managers for rovers. Completely untested so far.
