# Changes

## 0.1.4 -> 0.1.5

* Minor language adjustments
  * Tweak precedence of the range operator `..` and `...` so that it more consistent
  * Handle final return statement in functions correctly
* Added null checks in the `AutopilotAdapter` (addressing issue #3) 

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
