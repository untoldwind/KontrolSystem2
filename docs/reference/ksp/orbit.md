# ksp::orbit



## Types


### Body

Represents an in-game celestial body.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
angular_velocity | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Angular velocity vector of the body 
atmosphere_depth | float | R/O | Depth/height of the atmosphere if present. 
body_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
celestial_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
global_angular_velocity | [ksp::math::GlobalAngularVelocity](/reference/ksp/math.md#globalangularvelocity) | R/O | Angular velocity vector of the body (coordinate system independent) 
global_position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/O | The current position of the body (coordinate system independent) 
global_right | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
global_up | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O | 
grav_parameter | float | R/O | Standard gravitation parameter of the body. 
has_atmosphere | bool | R/O | `true` if the celestial body has an atmosphere to deal with. 
name | string | R/O | Name of the celestial body. 
orbit | [ksp::orbit::Orbit](/reference/ksp/orbit.md#orbit) | R/O | The orbit of the celestial body itself (around the parent body) 
position | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | The current position of the body 
radius | float | R/O | Radius of the body at sea level 
right | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 
rotation_period | float | R/O | Rotation period of the planet. 
SOI_radius | float | R/O | Radius of the sphere of influence of the body 
up | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | 

#### Methods

##### create_orbit

```rust
body.create_orbit ( position : ksp::math::Vec3,
                    velocity : ksp::math::Vec3,
                    ut : float ) -> ksp::orbit::Orbit
```

Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`


##### geo_coordinates

```rust
body.geo_coordinates ( latitude : float,
                       longitude : float ) -> ksp::orbit::GeoCoordinates
```



##### global_surface_normal

```rust
body.global_surface_normal ( latitude : float,
                             longitude : float ) -> ksp::math::GlobalVector
```

Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate, coordinate system independent)


##### global_surface_position

```rust
body.global_surface_position ( latitude : float,
                               longitude : float,
                               altitude : float ) -> ksp::math::GlobalPosition
```



##### surface_normal

```rust
body.surface_normal ( latitude : float,
                      longitude : float ) -> ksp::math::Vec3
```

Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate)


##### surface_position

```rust
body.surface_position ( latitude : float,
                        longitude : float,
                        altitude : float ) -> ksp::math::Vec3
```



##### terrain_height

```rust
body.terrain_height ( lat : float,
                      lon : float ) -> float
```



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



##### global_altitude_position

```rust
geocoordinates.global_altitude_position ( altitude : float ) -> ksp::math::GlobalPosition
```



### Orbit

Represents an in-game orbit.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
apoapsis | float | R/O | Apoapsis of the orbit above sealevel of the `reference_body`. 
apoapsis_radius | float | R/O | Radius of apoapsis of the orbit (i.e. from the center of the `reference_body') 
argument_of_periapsis | float | R/O | Argument of periapsis of the orbit. 
eccentricity | float | R/O | Eccentricity of the orbit. 
end_ut | float | R/O | 
epoch | float | R/O | Orbit epoch. 
inclination | float | R/O | Inclination of the orbit in degree. 
LAN | float | R/O | Longitude of ascending node of the orbit in degree 
mean_anomaly_at_epoch | float | R/O | Mean anomaly of the orbit at `epoch` 
mean_motion | float | R/O | Mean motion of the orbit. 
orbit_normal | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O | Normal vector perpendicular to orbital plane. 
periapsis | float | R/O | Periapsis of the orbit above sealevel of the `reference_body` 
periapsis_radius | float | R/O | Radius of periapsis of the orbit (i.e. from the center of the `reference_body') 
period | float | R/O | Orbital period. 
reference_body | [ksp::orbit::Body](/reference/ksp/orbit.md#body) | R/O | The celestial body the orbit is referenced on. 
reference_frame | [ksp::math::TransformFrame](/reference/ksp/math.md#transformframe) | R/O | 
semi_major_axis | float | R/O | Semi major axis of the orbit. 
start_ut | float | R/O | 

#### Methods

##### absolute_position

```rust
orbit.absolute_position ( ut : float ) -> ksp::math::Vec3
```

Get the absolute position at a given universal time `ut`


##### get_eccentric_anomaly_at_true_anomaly

```rust
orbit.get_eccentric_anomaly_at_true_anomaly ( trueAnomaly : float ) -> float
```



##### get_mean_anomaly_at_eccentric_anomaly

```rust
orbit.get_mean_anomaly_at_eccentric_anomaly ( ecc : float ) -> float
```



##### global_position

```rust
orbit.global_position ( ut : float ) -> ksp::math::GlobalPosition
```

Get the absolute position at a given universal time `ut`


##### global_velocity

```rust
orbit.global_velocity ( ut : float ) -> ksp::math::GlobalVelocity
```



##### horizontal

```rust
orbit.horizontal ( ut : float ) -> ksp::math::Vec3
```



##### mean_anomaly_at_ut

```rust
orbit.mean_anomaly_at_ut ( ut : float ) -> float
```



##### next_apoapsis_time

```rust
orbit.next_apoapsis_time ( ut : Option<float> ) -> Result<float, string>
```



##### next_periapsis_time

```rust
orbit.next_periapsis_time ( ut : Option<float> ) -> float
```



##### next_time_of_radius

```rust
orbit.next_time_of_radius ( ut : float,
                            radius : float ) -> Result<float, string>
```



##### normal_plus

```rust
orbit.normal_plus ( ut : float ) -> ksp::math::Vec3
```



##### orbital_velocity

```rust
orbit.orbital_velocity ( ut : float ) -> ksp::math::Vec3
```



##### perturbed_orbit

```rust
orbit.perturbed_orbit ( ut : float,
                        dV : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```



##### prograde

```rust
orbit.prograde ( ut : float ) -> ksp::math::Vec3
```



##### radial_plus

```rust
orbit.radial_plus ( ut : float ) -> ksp::math::Vec3
```



##### radius

```rust
orbit.radius ( ut : float ) -> float
```



##### relative_position

```rust
orbit.relative_position ( ut : float ) -> ksp::math::Vec3
```



##### synodic_period

```rust
orbit.synodic_period ( other : ksp::orbit::Orbit ) -> float
```



##### time_of_true_anomaly

```rust
orbit.time_of_true_anomaly ( trueAnomaly : float,
                             ut : float ) -> float
```



##### to_fixed

```rust
orbit.to_fixed ( decimals : int ) -> string
```



##### to_string

```rust
orbit.to_string ( ) -> string
```



##### true_anomaly_at_radius

```rust
orbit.true_anomaly_at_radius ( radius : float ) -> float
```



##### u_t_at_mean_anomaly

```rust
orbit.u_t_at_mean_anomaly ( meanAnomaly : float,
                            ut : float ) -> float
```



##### up

```rust
orbit.up ( ut : float ) -> ksp::math::Vec3
```



## Functions


### find_body

```rust
pub sync fn find_body ( name : string ) -> Result<ksp::orbit::Body, string>
```


