# Modules

A module is just a TO2-file containing some public (`pub`) functions or types.

Currently all scripts are located in the plugin folder itself:
`BepInEx/plugins/KontrolSystem2/to2`

The name of the script becomes the name of the module. Subfolders will be prefixed with a `::` delimiter.

E.g.
```
to2/
| - intercept.to2                module name: "intercept"
| - std/
|   | - utils.to2                module name: "std::utils"
|   | - vac.to2                  module name: "std::vac"
|   | - numerics/
|   |   | - brent_optimize.to2   module name: "std::numerics::brent_optimize
```

## Importing for modules

Public functions can be used fully qualified:
E.g.:
```rust
std::utils::angle_to_360(520)
```
will call the `angle_to_360` function defined in the `std::utils` module.

Alternatively function can be import via the `use` keyword.
```rust
use { angle_to_360 } from std::utils

angle_to_360(520)
```

