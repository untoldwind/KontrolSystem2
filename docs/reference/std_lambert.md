---
title: "std::lambert"
---



# Functions


## solve_lambert

```rust
pub sync fn solve_lambert ( r1 : ksp::math::Vec3,
                            r2 : ksp::math::Vec3,
                            tof : float,
                            mu : float,
                            clockwise : bool ) -> (iters : int, v1 : ksp::math::Vec3, v2 : ksp::math::Vec3)
```

Solve Lambert's problem, i.e. calculate the Kepler orbit to get from point `r1`
to point `r2` in time `tof` (time of flight).

* `mu` is the standard gravitational parameter of the central body
* `clockwise` defines if a clockwise or counter-clockwise orbit should be calculated
* The result `v1` is the required velocity at `r1`
* The result `v2` is the required velocity at `r2`

This is based on the solver developed by Dario Izzo
Details can be found here: https://arxiv.org/pdf/1403.2705.pdf
Released under the GNU GENERAL PUBLIC LICENSE as part of the PyKEP library:
