# std::staging

Collection of helper functions to control staging of a vessel

## Functions


### has_engine_in_stage

```rust
pub sync fn has_engine_in_stage ( vessel : ksp::vessel::Vessel,
                                  stage : int ) -> bool
```



### has_flameout

```rust
pub sync fn has_flameout ( vessel : ksp::vessel::Vessel ) -> bool
```



### has_ignited

```rust
pub sync fn has_ignited ( vessel : ksp::vessel::Vessel ) -> bool
```



### has_still_running

```rust
pub sync fn has_still_running ( vessel : ksp::vessel::Vessel ) -> bool
```



### trigger_staging

```rust
pub fn trigger_staging ( vessel : ksp::vessel::Vessel ) -> bool
```

Helper function to automatically trigger staging during a burn.

This function is just checking if one of the ignited engines has has a flameout,
which in most cases means that the current stage has burned out.

Will return `true` if stating has been triggered.
