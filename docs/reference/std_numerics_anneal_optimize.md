---
title: "std::numerics::anneal_optimize"
---



# Functions


## anneal_optimize

```rust
pub sync fn anneal_optimize ( func : fn(float, float) -> float,
                              min : ksp::math::Vec2,
                              max : ksp::math::Vec2,
                              max_temp : float,
                              iters : int,
                              num_particles : int,
                              cooling_rate : float ) -> (best : (f : float, x : float, y : float), points : (f : float, x : float, y : float)[])
```


