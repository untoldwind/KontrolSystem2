# std::vac



## Functions


### estimate_burn_time

```rust
pub sync fn estimate_burn_time ( vessel : ksp::vessel::Vessel,
                                 delta_v : float,
                                 stage_delay : float,
                                 throttle_limit : float ) -> (burn_time : float, half_burn_time : float)
```

Estimate the required burn time for a desired `delta_v` in vacuum.

* `stage_delay` is the assumed amount of seconds required for staging
* `throttle_limit` is a limit for the throttle to be considered

### exec_next_node

```rust
pub fn exec_next_node ( vessel : ksp::vessel::Vessel ) -> Result<Unit, string>
```

Execute the next planed maneuver node.

Will result in an error if there are no planed maneuver nodes.
