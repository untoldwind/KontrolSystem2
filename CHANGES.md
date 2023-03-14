# Changes

## 0.1.6 -> 0.1.7

* `vessel.actions.sas = true/false` now working
* `vessel.actions.rcs = true/false` now working
* Add `vessel.actions.custom1` .. `vessel.actions.custom10` (addresses #10)
* Support array of arrays types (addresses #12)
* Add engine mode information (addresses #12)


## 0.1.5 -> 0.1.6

* Add `vessel.situtation` binding (Addressing #8)
* Made coordinate transformation more stable (try always to be in coordinate frame of the main body, addresses #4)
* Optimize the parsing/compiling speed
* Add addition scripting director "localTo2" to cleanly distinguish between scripts provided by by the plugin and scripts you write your own
  * This can be modified in the public configuration `/BepInEx/config/com.github.untoldwind.KontrolSystem2.cfg`

## 0.1.4 -> 0.1.5

* Minor language adjustments
  * Tweak precedence of the range operator `..` and `...` so that it more consistent
  * Handle final return statement in functions correctly
* Added null checks in the `AutopilotAdapter` (addressing issue #3) 
* Added `core::std` package with string `join` and `format` functions
* The formatting is based on C# `String.Format`, so something like this works:
  
```format("First: {0,7:F3}, Second: {1,7:F2}, Third: {2}", (1.1234567, 12.345678, "Hallo")))```
gives "First:   1.123, Second:   12.35, Third: Hallo"


## 0.1.3 -> 0.1.4

* Combined Toolbar window and Modulemanager window to one
  * ... i.e. got rid of the window that had little to no purpose
* Added an `exepect_orbit` to the maneuvering node
* Added `parts_in_stage` to staging information
* Improved the `exec_node` script (a lot)

## 0.1.2 -> 0.1.3

* Close windows on ESC menu
* Fix `vessel.stating.next()` to actually work
* Made a `launch_rocket` script working (for at least one rocket)

## 0.1.1 -> 0.1.2

* Re-adding the unit tests that where failing on windows
* Add hotkey "Alt-K" to open the main window even if toolbar is not present/visible
  * Can be toggled of in BepInEx configuration manager
* Increase internal line length of console
* Add `ksp::vessel::Vessel.staging.next()` method to trigger staging
* Fix throttle_manager unhook problem
  * Should eventually work for all other control managers as well
* Ignore empty lines on console resize
* Add indicator light to main window
* Additional vessel information (thanks EloxZ) 
