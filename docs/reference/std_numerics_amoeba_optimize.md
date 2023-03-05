---
title: "std::numerics::amoeba_optimize"
---



# Functions


## amoeba_optimize

```rust
pub sync fn amoeba_optimize ( func : fn(float, float) -> float,
                              start_points : ksp::math::Vec2[],
                              tolerance : float,
                              max_iters : int ) -> Result<(iters : int, x : float, y : float), string>
```



## amoeba_optimize_perturbation

```rust
pub sync fn amoeba_optimize_perturbation ( func : fn(float, float) -> float,
                                           guess : ksp::math::Vec2,
                                           perturbation : ksp::math::Vec2,
                                           tolerance : float,
                                           max_iters : int ) -> Result<(iters : int, x : float, y : float), string>
```


