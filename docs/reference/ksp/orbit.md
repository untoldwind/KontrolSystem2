# ksp::orbit



## Types


### Body

Represents an in-game celestial body.


#### Fields

Name | Type | Description
--- | --- | ---
atmosphere_depth | float | Depth/height of the atmosphere if present. 
grav_parameter | float | Standard gravitation parameter of the body. 
has_atmosphere | bool | `true` if the celestial body has an atmosphere to deal with. 
name | string | Name of the celestial body. 
orbit | ksp::orbit::Orbit | The orbit of the celestial body itself (around the parent body) 
radius | float | Radius of the body at sea level 
right | ksp::math::Vec3 | 
rotation_period | float | Rotation period of the planet. 
SOI_radius | float | Radius of the sphere of influence of the body 
up | ksp::math::Vec3 | 

#### Methods

##### create_orbit

```rust
body.create_orbit ( position : ksp::math::Vec3,
                    velocity : ksp::math::Vec3,
                    ut : float ) -> ksp::orbit::Orbit
```

Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`


##### surface_normal

```rust
body.surface_normal ( latitude : float,
                      longitude : float ) -> ksp::math::Vec3
```

Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate


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

Name | Type | Description
--- | --- | ---
body | ksp::orbit::Body | 
latitude | float | 
longitude | float | 
surface_normal | ksp::math::Vec3 | 
terrain_height | float | 

#### Methods

##### altitude_position

```rust
geocoordinates.altitude_position ( altitude : float ) -> ksp::math::Vec3
```



### Orbit

Represents an in-game orbit.


#### Fields

Name | Type | Description
--- | --- | ---
apoapsis | float | Apoapsis of the orbit above sealevel of the `reference_body`. 
apoapsis_radius | float | Radius of apoapsis of the orbit (i.e. from the center of the `reference_body') 
argument_of_periapsis | float | Argument of periapsis of the orbit. 
eccentricity | float | Eccentricity of the orbit. 
epoch | float | Orbit epoch. 
inclination | float | Inclination of the orbit in degree. 
LAN | float | Longitude of ascending node of the orbit in degree 
mean_anomaly_at_epoch | float | Mean anomaly of the orbit at `epoch` 
mean_motion | float | Mean motion of the orbit. 
orbit_normal | ksp::math::Vec3 | Normal vector perpendicular to orbital plane. 
periapsis | float | Periapsis of the orbit above sealevel of the `reference_body` 
periapsis_radius | float | Radius of periapsis of the orbit (i.e. from the center of the `reference_body') 
period | float | Orbital period. 
reference_body | ksp::orbit::Body | The celestial body the orbit is referenced on. 
semi_major_axis | float | Semi major axis of the orbit. 

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


