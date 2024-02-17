# ksp::orbit



## Types


### Body

Represents an in-game celestial body.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
SOI_radius | float | R/O | Radius of the sphere of influence of the body 
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Angular velocity vector of the body 
atmosphere_depth | float | R/O | Depth/height of the atmosphere if present. 
body_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The body/rotating reference frame of the body. 
celestial_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | The celestial/non-rotating reference frame of the body. 
global_angular_velocity | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | Angular velocity vector of the body (coordinate system independent) 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | The current position of the body (coordinate system independent) 
global_right | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Right vector of the body (coordinate system independent) 
global_up | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | Up vector of the body (coordinate system independent) 
grav_parameter | float | R/O | Standard gravitation parameter of the body. 
has_atmosphere | bool | R/O | `true` if the celestial body has an atmosphere to deal with. 
name | string | R/O | Name of the celestial body. 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | The orbit of the celestial body itself (around the parent body) 
orbiting_bodies | [ksp::orbit::Body](/reference/ksp/orbit.md#body)[] | R/O | Get all celestial bodies in orbit around this body (aka child bodies). 
parent_body | Option&lt;[ksp::orbit::Body](/reference/ksp/orbit.md#body)> | R/O | Get the celestial body this celestial body orbits if it exists (aka the parent body). 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | The current position of the body 
radius | float | R/O | Radius of the body at sea level 
right | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Right vector of the body in its celestial frame 
rotation_period | float | R/O | Rotation period of the planet. 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Up vector of the body in its celestial frame 

#### Methods

##### create_orbit

```rust
body.create_orbit ( position : ksp::math::Vec3,
                    velocity : ksp::math::Vec3,
                    ut : float ) -> ksp::orbit::Orbit
```

Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
position | ksp::math::Vec3 |  | 
velocity | ksp::math::Vec3 |  | 
ut | float |  | 

##### geo_coordinates

```rust
body.geo_coordinates ( latitude : float,
                       longitude : float ) -> ksp::orbit::GeoCoordinates
```

Get `GeoCoordinates` struct representing a `latitude` and `longitude` of the body


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 

##### global_create_orbit

```rust
body.global_create_orbit ( velocity : ksp::math::GlobalVelocity,
                           ut : float ) -> ksp::orbit::Orbit
```

Create a new orbit around this body starting at a given a coordinate independent `velocity` at universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
velocity | ksp::math::GlobalVelocity |  | 
ut | float |  | 

##### global_surface_normal

```rust
body.global_surface_normal ( latitude : float,
                             longitude : float ) -> ksp::math::GlobalVector
```

Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate, coordinate system independent)


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 

##### global_surface_position

```rust
body.global_surface_position ( latitude : float,
                               longitude : float,
                               altitude : float ) -> ksp::math::GlobalPosition
```

Position of a `latitude` and `longitude` at an altitude relative to sea-level (coordinate system independent)


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 
altitude | float |  | 

##### surface_normal

```rust
body.surface_normal ( latitude : float,
                      longitude : float ) -> ksp::math::Vec3
```

Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate)


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 

##### surface_position

```rust
body.surface_position ( latitude : float,
                        longitude : float,
                        altitude : float ) -> ksp::math::Vec3
```

Position of a `latitude` and `longitude` at an altitude relative to sea-level in the celestial frame of the body


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 
altitude | float |  | 

##### terrain_height

```rust
body.terrain_height ( latitude : float,
                      longitude : float ) -> float
```

Height of the terrain relative to sea-level a a `latitude` and `longitude`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
latitude | float |  | 
longitude | float |  | 

### GeoCoordinates



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | 
global_surface_normal | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
latitude | float | R/W | 
longitude | float | R/W | 
surface_normal | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
terrain_height | float | R/O | 

#### Methods

##### altitude_position

```rust
geocoordinates.altitude_position ( altitude : float ) -> ksp::math::Vec3
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
altitude | float |  | 

##### global_altitude_position

```rust
geocoordinates.global_altitude_position ( altitude : float ) -> ksp::math::GlobalPosition
```



Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
altitude | float |  | 

### Orbit

Represents an in-game orbit.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
LAN | float | R/O | Longitude of ascending node of the orbit in degree 
apoapsis | Option&lt;float> | R/O | Apoapsis of the orbit above sealevel of the `reference_body`. Is not defined for a hyperbolic orbit 
apoapsis_radius | Option&lt;float> | R/O | Radius of apoapsis of the orbit (i.e. from the center of the `reference_body'). Is not defined for a hyperbolic orbit 
argument_of_periapsis | float | R/O | Argument of periapsis of the orbit. 
eccentricity | float | R/O | Eccentricity of the orbit. 
end_transition | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Get transition type at the end of the orbit patch 
end_ut | float | R/O | Universal time of the start of the orbit, in case it is an orbit-patch 
epoch | float | R/O | Orbit epoch. 
inclination | float | R/O | Inclination of the orbit in degree. 
mean_anomaly_at_epoch | float | R/O | Mean anomaly of the orbit at `epoch` 
mean_motion | float | R/O | Mean motion of the orbit. 
next_patch | Option&lt;[ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit)> | R/O | Get the next orbit patch of the trajectory (if available) 
orbit_normal | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Normal vector perpendicular to orbital plane. 
periapsis | float | R/O | Periapsis of the orbit above sealevel of the `reference_body` 
periapsis_radius | float | R/O | Radius of periapsis of the orbit (i.e. from the center of the `reference_body') 
period | float | R/O | Orbital period. 
previous_patch | Option&lt;[ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit)> | R/O | Get the previous orbit patch of the trajectory (if available) 
reference_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | The celestial body the orbit is referenced on. 
reference_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | Internal reference frame of the orbit. This might be useful to compare numbers. Note: All relative vectors are in the celestial frame of the `reference_body` which might be different! 
relative_ascending_node | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the relative position of the ascending node. 
relative_eccentricity_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Get the relative eccentricity vector. 
semi_major_axis | float | R/O | Semi major axis of the orbit. 
start_transition | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Get transition type at the beginning of the orbit patch 
start_ut | float | R/O | Universal time of the start of the orbit, in case it is an orbit-patch 

#### Methods

##### ascending_node_true_anomaly

```rust
orbit.ascending_node_true_anomaly ( b : ksp::orbit::Orbit ) -> float
```

Gives the true anomaly (in a's orbit) at which a crosses its ascending node with b's orbit.
The returned value is always between 0 and 360.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
b | ksp::orbit::Orbit |  | 

##### descending_node_true_anomaly

```rust
orbit.descending_node_true_anomaly ( b : ksp::orbit::Orbit ) -> float
```

Gives the true anomaly (in a's orbit) at which a crosses its descending node with b's orbit.
The returned value is always between 0 and 360.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
b | ksp::orbit::Orbit |  | 

##### get_eccentric_anomaly_at_true_anomaly

```rust
orbit.get_eccentric_anomaly_at_true_anomaly ( trueAnomaly : float ) -> float
```

Converts a true anomaly into an eccentric anomaly.
For elliptical orbits this returns a value between 0 and 2pi.
For hyperbolic orbits the returned value can be any number.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
trueAnomaly | float |  | 

##### get_mean_anomaly_at_eccentric_anomaly

```rust
orbit.get_mean_anomaly_at_eccentric_anomaly ( ecc : float ) -> float
```

Converts an eccentric anomaly into a mean anomaly.
For an elliptical orbit, the returned value is between 0 and 2pi.
For a hyperbolic orbit, the returned value is any number.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ecc | float |  | 

##### global_position

```rust
orbit.global_position ( ut : float ) -> ksp::math::GlobalPosition
```

Get the coordinate independent position at a given universal time `ut`.
Note: This takes the motion of the parent body into account.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### global_position_for_true_anomaly

```rust
orbit.global_position_for_true_anomaly ( trueAnomaly : float ) -> ksp::math::GlobalPosition
```

Get the coordinate independent position for a given `trueAnomaly`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
trueAnomaly | float |  | 

##### global_relative_position

```rust
orbit.global_relative_position ( ut : float ) -> ksp::math::GlobalVector
```

Shorthand for `orbit.global_position(ut) - orbit.reference_body.orbit.global_position(ut)`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### global_velocity

```rust
orbit.global_velocity ( ut : float ) -> ksp::math::GlobalVelocity
```

Get the coordinate independent velocity at a given universal time `ut`.
Note: This takes the motion of the parent body into account.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### horizontal

```rust
orbit.horizontal ( ut : float ) -> ksp::math::Vec3
```

Relative horizontal vector at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### mean_anomaly_at_ut

```rust
orbit.mean_anomaly_at_ut ( ut : float ) -> float
```

The mean anomaly of the orbit.
For elliptical orbits, the value return is always between 0 and 2pi.
For hyperbolic orbits, the value can be any number.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### next_apoapsis_time

```rust
orbit.next_apoapsis_time ( ut : Option<float> ) -> Option<float>
```

Returns the next time at which the orbiting object will be at apoapsis after a given universal time `ut`.
If `ut` is omitted the current time will be used.
For elliptical orbits, this will be between `ut` and `ut` + Period.
For hyperbolic orbits, this is undefined.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | Option<float> | x | 

##### next_periapsis_time

```rust
orbit.next_periapsis_time ( ut : Option<float> ) -> float
```

The next time at which the orbiting object will be at periapsis after a given universal time `ut`.
If `ut` is omitted the current time will be used.
For elliptical orbits, this will be between `ut` and `ut` + Period.
For hyperbolic orbits, this can be any time, including a time in the past, if the periapsis is in the past.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | Option<float> | x | 

##### next_time_of_radius

```rust
orbit.next_time_of_radius ( ut : float,
                            radius : float ) -> Option<float>
```

Finds the next time at which the orbiting object will achieve a given `radius` from center of the body
after a given universal time `ut`.
This will be undefined if the specified `radius` is impossible for this orbit, otherwise:
For elliptical orbits this will be a time between `ut` and `ut` + period.
For hyperbolic orbits this can be any time. If the given radius will be achieved
in the future then the next time at which that radius will be achieved will be returned.
If the given radius was only achieved in the past, then there are no guarantees
about which of the two times in the past will be returned.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
radius | float |  | 

##### normal_plus

```rust
orbit.normal_plus ( ut : float ) -> ksp::math::Vec3
```

The relative normal-plus vector at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### orbital_velocity

```rust
orbit.orbital_velocity ( ut : float ) -> ksp::math::Vec3
```

Get the relative orbital velocity at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### perturbed_orbit

```rust
orbit.perturbed_orbit ( ut : float,
                        dV : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```

Returns a new Orbit object that represents the result of applying a given relative `deltaV` to o at `ut`.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 
dV | ksp::math::Vec3 |  | 

##### prograde

```rust
orbit.prograde ( ut : float ) -> ksp::math::Vec3
```

The relative prograde vector at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### radial_plus

```rust
orbit.radial_plus ( ut : float ) -> ksp::math::Vec3
```

The relative radial-plus vector at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### radius

```rust
orbit.radius ( ut : float ) -> float
```

Get the orbital radius (distance from center of body) at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### relative_position

```rust
orbit.relative_position ( ut : float ) -> ksp::math::Vec3
```

Get relative position at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### relative_position_for_true_anomaly

```rust
orbit.relative_position_for_true_anomaly ( trueAnomaly : float ) -> ksp::math::Vec3
```

Get relative position for a given `trueAnomaly`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
trueAnomaly | float |  | 

##### synodic_period

```rust
orbit.synodic_period ( other : ksp::orbit::Orbit ) -> float
```

Computes the period of the phase angle between orbiting objects of this orbit and and `other` orbit.
For noncircular orbits the time variation of the phase angle is only quasiperiodic
and for high eccentricities and/or large relative inclinations, the relative motion is
not really periodic at all.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
other | ksp::orbit::Orbit |  | 

##### time_of_ascending_node

```rust
orbit.time_of_ascending_node ( b : ksp::orbit::Orbit,
                               maybeUt : Option<float> ) -> float
```

Returns the next time at which a will cross its ascending node with b.
For elliptical orbits this is a time between UT and UT + a.period.
For hyperbolic orbits this can be any time, including a time in the past if
the ascending node is in the past.
NOTE: this function will throw an ArgumentException if a is a hyperbolic orbit and the ascending node
occurs at a true anomaly that a does not actually ever attain.
If `ut` is omitted the current time will be used.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
b | ksp::orbit::Orbit |  | 
maybeUt | Option<float> | x | 

##### time_of_descending_node

```rust
orbit.time_of_descending_node ( b : ksp::orbit::Orbit,
                                maybeUt : Option<float> ) -> float
```

Returns the next time at which a will cross its descending node with b.
For elliptical orbits this is a time between UT and UT + a.period.
For hyperbolic orbits this can be any time, including a time in the past if
the descending node is in the past.
NOTE: this function will throw an ArgumentException if a is a hyperbolic orbit and the descending node
occurs at a true anomaly that a does not actually ever attain.
If `ut` is omitted the current time will be used


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
b | ksp::orbit::Orbit |  | 
maybeUt | Option<float> | x | 

##### time_of_true_anomaly

```rust
orbit.time_of_true_anomaly ( trueAnomaly : float,
                             maybeUt : Option<float> ) -> float
```

Next time of a certain true anomaly after a given universal time `ut`.
If `ut` is omitted the current time will be used


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
trueAnomaly | float |  | 
maybeUt | Option<float> | x | 

##### to_fixed

```rust
orbit.to_fixed ( decimals : int ) -> string
```

Convert orbital parameter to string using specified number of `decimals`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
decimals | int |  | 

##### to_string

```rust
orbit.to_string ( ) -> string
```

Convert orbital parameters to string.


##### true_anomaly_at_radius

```rust
orbit.true_anomaly_at_radius ( radius : float ) -> float
```

Get the true anomaly of a radius.
If the radius is below the periapsis the true anomaly of the periapsis will be returned.
If it is above the apoapsis the true anomaly of the apoapsis is returned.
The returned value is always between 0 and 2pi.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
radius | float |  | 

##### true_anomaly_from_vector

```rust
orbit.true_anomaly_from_vector ( vec : ksp::math::Vec3 ) -> float
```

Converts a relative direction, into a true anomaly.
The vector is projected into the orbital plane and then the true anomaly is
computed as the angle this vector makes with the vector pointing to the periapsis.
The returned value is always between 0 and 2pi.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
vec | ksp::math::Vec3 |  | 

##### up

```rust
orbit.up ( ut : float ) -> ksp::math::Vec3
```

Relative up vector of the orbit at a given universal time `ut`


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
ut | float |  | 

##### ut_at_mean_anomaly

```rust
orbit.ut_at_mean_anomaly ( meanAnomaly : float,
                           ut : float ) -> float
```

The next time at which the orbiting object will reach the given mean anomaly.
For elliptical orbits, this will be a time between UT and UT + o.period.
For hyperbolic orbits, this can be any time, including a time in the past, if the given mean anomaly occurred in the past


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
meanAnomaly | float |  | 
ut | float |  | 

### PatchTransitionType

Transition type at the beginning or end of an orbit patch

#### Methods

##### to_string

```rust
patchtransitiontype.to_string ( ) -> string
```

String representation of the number

### PatchTransitionTypeConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
Collision | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Orbits collides with a (celestial) object
CompletelyOutOfFuel | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Planed maneuver will run out of fuel
Encounter | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Orbit enters a sphere of influence (SOI)
EndThrust | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | End of thrust of a planed maneuver
Escape | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Orbit leaves a sphere of influence (SOI)
Final | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Final transition (orbit ends here)
Initial | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Initial transition (orbit starts here)
Maneuver | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Orbit changes due to a planed maneuver
PartialOutOfFuel | [ksp::orbit::PatchTransitionType](/reference/ksp/orbit.md#patchtransitiontype) | R/O | Planed maneuver will partially run out of fuel

#### Methods

##### from_string

```rust
patchtransitiontypeconstants.from_string ( value : string ) -> Option<ksp::orbit::PatchTransitionType>
```

Parse from string

Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
value | string |  | Enum value to lookup

## Constants

Name | Type | Description
--- | --- | ---
PatchTransitionType | ksp::orbit::PatchTransitionTypeConstants | Transition type at the beginning or end of an orbit patch


## Functions


### find_body

```rust
pub sync fn find_body ( name : string ) -> Result<ksp::orbit::Body, string>
```

Find a body by name.


Parameters

Name | Type | Optional | Description
--- | --- | --- | ---
name | string |  | 

### galactic_origin

```rust
pub sync fn galactic_origin ( ) -> ksp::math::TransformFrame
```

Get the galactic celestial frame.

