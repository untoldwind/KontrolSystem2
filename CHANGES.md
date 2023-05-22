# Changes

## 0.4.1.1 -> 0.4.1.2

* Fix parsing issue with line commends in if-else cases
* Add `MAINFRAME.available_processes` and `MAINFRAME.find_process` to launch scripts programatically
* Fix `orbit.semi_major_axis`

## 0.4.1 -> 0.4.1.1

* Add `get_vessels_in_range()` and `get_all_owned_vessels()` functions to `ksp::vessel`
* Add `Vessel.make_active()`

## ß.3.6 -> 0.4.1

* UI overhaul
* Add `ksp::ui` to create UI via scripts

## 0.3.5 -> 0.3.6

* Binding for FlightPlan mod

## 0.3.4 -> 0.3.5

* Ensure that `Vec3.to_direction()` and `GlobalVector.to_direction()` also work for "Up"
* Fix `vessel.available_thrust` to calculate the thrust in the current situation (i.e. atmosphere/vacuum)
* Support `+` and `+=` operators for array (append and concat)
* Add binding for control surfaces (e.g. `vessel.control_surfaces`)
* Support docking port target

## 0.3.3.3 -> 0.3.4

* Fix `vessel.global_facing` for space plane
  * Also ensure stable reference (internal) frame
* Add `time_of_ascending_node` and `time_of_decending_node` helper to orbit
* Improve docking port API:
  * Add `DockingState` enum
  * Get node type information
* Experimental additions:
  * Add `std::rendezvous::lib` for a simple orbital rendezvous maneuver
  * Add `std::rendezvous::dock` for simple docking (needs lots of tweaking/optimization)
* Add improved launch sequence (thanks to RobertoBiundo)
* Extend the resource transfer API to support transfer of specific resources (and specific number of units)
* Fix NPE in type checking
* Fix broken error reporting (issue #78)
* Fix invalid IL code exception (issue #79)

## 0.3.3.2 -> 0.3.3.3

* Fixed visible columns of console view
* Fix `vessel.angular_moment`
* Added helpers for steering:
  * `vessel.global_toment_of_inertia`
  * `vessel.total_torque`
* Add `std::control::steering` module to control a vessel without relying on the SAS system.

## 0.3.3.1 -> 0.3.3.2

* Fix inaccessible `..ctor()` exception (issue #70)

## 0.3.3 -> 0.3.3.1

* Prevent loading of files with "*.to2~" extension (issue #69) 

## 0.3.2 -> 0.3.3

* Fix incompatibility with 0.1.2.0 on maneuver node creation
* Add `activation_stage` and `decouple_stage` property to vessel part
* Ensure that resources are not created from nothing on resource transfer
  * ... still requires some additional testing

## 0.3.1 -> 0.3.2

* Cleanup calculation of `vessel.pitch_yaw_roll`
* Fix calculation of `vessel.heading_direction()`
* Remove `vessel.pitch_horizon_relative` and `vessel.roll_horizon_relative` as say were misleading and did not create "valid" results
* Added check for stack overflow of scripts
* Create bindings for all enums:
  * Instead of `vessel.autopilot.mode = "Prograde"` one should now use `vessel.autopilot.mode = AutopilotMode.Prograde`
* Basic resource transfer API `ksp::resource`

## 0.2.3 -> 0.3.1

* Telemetry

## 0.2.2 -> 0.2.3

* Fix comments in struct-impl (#57)
* Add "Copy to clipboard" to reboot errors and and console window
* Closed a backdoor allowing async functions to be injected into sync functions (with somewhat erratic results)
* Added an early version of run-command to the console-window

## 0.2.1 -> 0.2.2

* Make low and high gravity turn parameters tweakable in `launch_rocket.to2`
* Add `circularize_orbit_pe` to std-lib 
* Ensure that orbit relative coordinates are in main bodies reference frame (#54)
* Use coordinate independent vectors to check ship alignment in `exec_next_node` 
  * Also adding `global_ship_is_facing` helper to `std::utils`
* Fix struct type as type parameter issue (#55 + #56)
* Base the search interval of the `intercept` script on orbit period to make it more useful in interplanetary transitions

## 0.2.0.2 -> 0.2.1

Minor breaking changes to `ksp::orbit::Orbit`:
* `orbit.apoapsis` and `orbit.apoapsis_radius` are now `Option<float>` instead of just `float`
  * This is because apoapsis of a hyperbolic orbit is pretty much undefined.
  * If you are sure that you are only dealing with elliptic orbits, you can just replace `orbit.apoapsis` with `orbit.apoapsis.value` in your code.
  * This might by a small inconvenience, but should be an incentive to check for the `eccentricy` in your calculations
* `orbit.next_apoapsis_time()` and `orbit.next_time_of_radius()` now also return `Option<float>` to be consistent
* `orbit.u_t_at_mean_anomaly()` has been renamed to `orbit.ut_at_mean_anomaly()`
* Removed `orbit.absolute_position(ut)` as it was misleading. Use `orbit.global_position(ut)` instead.
* Fix orientation of the `orbit.normal`

Other fixes:
* Fixed access to struct field from async function (issue #47)
* CRITICAL: Fixed endless loop in IsAssignableFrom lookup (issue #51 ... may crash the game)
* Prevent the creation of multiple `ThrottleManager` for the same vessel (issue #37)
* Minor: Allow whitespace in `impl fn` (issue #49)
* Minor: Allow comments in record/tuple type declarations (issue #50)

## 0.2.0.1 -> 0.2.0.2

* Add debug billboards
* Add debug paths

## 0.1.9.3 -> 0.2.0.1

* Add API for coordinate system independent positions, velocities, vectors and rotations
* Debug vectors and ground markers
* More atmospheric telemetry data
* Support for string arguments
* Add binding for parachutes
* Add navball helper functions to `std::navball`

## 0.1.9.2 -> 0.1.9.3

* Fix main window rescale on resize
* Improve error message on invalid number of method arguments
* Add binding for decoupler and launch clamps
* Fixed mixing async function results with sync methods (issue 32)
* Add `pitch_yaw_roll` property to `vessel` reflecting the navball angles

## 0.1.9 -> 0.1.9.2

* Potentially fixed reload/revert issue
* Add bindings for solar panels
* Add bindings for fairings
* Improve handling of code generation bugs

## 0.1.8 -> 0.1.9

* Add basic ingame script editor (thanks to ThunderousEcho)
  * Note: There is an issue of IMGUI elements not capturing the keyboard focus. As a temporary workaround there is a toggle to disable keyboard control of the game
* Add the ability to add additional arguments to scripts (thanks to kbleeke).
  * As an example the `launch_rocket` script has now a `target_apoapsis` argument
* Experimental: Add `change_mode` to ModuleEngine
* Fix for `bool != bool`
* Check that it still works with game version 0.1.1.0
  * Known issue: After a reload it might happen that the reference to the active vessel is lost. A "Reboot" can fix that.

## 0.1.7 -> 0.1.8

* Add `.reverse()` on arrays
* Fix internal error if function is called with too many arguments (now correct error message is displayed)
* Update all the steering / throttle managers in `ksp::control` (see https://kontrolsystem2.readthedocs.io/en/latest/interact/control.html)

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
