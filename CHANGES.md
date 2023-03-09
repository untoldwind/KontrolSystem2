# Changes

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
