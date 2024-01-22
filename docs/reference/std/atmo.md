# std::atmo



## Functions


### atmo_launch

```rust
pub fn atmo_launch ( vessel : ksp::vessel::Vessel,
                     target_apoapsis : float,
                     heading : float,
                     low_turn : float,
                     high_turn : float ) -> Result<Unit, string>
```

Automatically launch a rocket from an atmosphere to a circular orbit.

### atmo_launch_ascent

```rust
pub fn atmo_launch_ascent ( vessel : ksp::vessel::Vessel,
                            target_apoapsis : float,
                            heading : float,
                            low_turn : float,
                            high_turn : float ) -> Unit
```


