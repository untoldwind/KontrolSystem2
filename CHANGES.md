# Changes

## 0.5.7.3 -> 0.5.7.6

* The `?` operator can now also be used on `Option<T>` in functions that return a `Result<T>`
  * Example: 
    ```
    pub fn main_flight(vessel: Vessel) -> Result<Unit> = {
        const target = vessel.target? // <-- Will fail with error if vessel has no target
    }
    ```
* VSCode extension now tries to read `reference.json` from the currently installed version of `KontrolSystem2`
  * It will no longer be necessary to update the extension with every release to make a (small) API change visible in
    VSCode
* Add `.is_empty` and `.is_not_empty` to arrays
* Basic bindings for waypoints
  * Add `body.waypoints`
  * Add `ksp::orbit::find_waypoint(name: string)`
* Fix: `WheelThrottleManager` is now doing what its name suggests
* Make cleaner distinction between orbit and orbit patch:
  * `ksp::orbit::Orbit` is not more (or less) then a standalone Kepler orbit that can be used for orbital calculation
  * `ksp::orbit::OrbitPatch` is a section of a Kepler orbit that is part of a `ksp::orbit::Trajectory` of a vessel
  * `vessel.trajectory` now returns a `ksp::orbit::Trajectory`, for compatibility  `ksp::orbit::Trajectory` can be used
    (almost) like a `ksp::orbit::OrbitPatch[]` (i.e. it can be used in a `for` loop and `vessel.trajectory[0]` still works)
  * Properties ein `vessel.maneuver` have been updated accordingly
* Add `true_anomaly_at_ut(ut: float)` to `ksp::orbit::Orbit`
* Fix: Scroll wheel in UI does not affect camera zoom any more

## 0.5.6.3 -> 0.5.7.3

### `Result<T, E>` overhaul.

Background:
* The main use-case of `Result<T, E>` is to serve as return of a function that can either have a successful result of
  type `T` or an error of type `E`.
* In pretty much all cases the error is just a `string`, i.e. the common use is `Result<T, string>`

Changes:
* For simplification the type parameter for the error case is dropped, i.e. a result is just `Result<T>`
  * The old notation `Result<T, E>` (or rather `Result<T, string>`) will still work, but the second parameter is ignored
* The `result.error` field is now a newly introduced `core::error::Error`
  * For compatibility `core::error::Error` will be automatically converted to a string
  * Just like before an error `Result` can be created using the builtin `Err` function with an error message
* In addition to the error `message` the `core::error::Error` additional contains a `stack_trace` to help locating
  where the `Error` was created
* As an additional helper there this a `core::error::current_stack()` function to obtain the current stack trace for
  debugging purposes.

Support for script based logs:
* Add `MAIN_LOG` and `open_log_file(string)` to `ksp::debug`
  * Log files will be cared ein `logs` sub-folder of `to2Local`

General improvements:
* Add `vessel.has_launched`, `vessel.launch_time` and `vessel.time_since_launch` (the latter should be the mission time)
* Fix issue in `std::control::sterring` not working for "poorly" oriented command modules
* Basic support for aerodynamic drag and lift-forces:
  * Add `part.lifting_surface` and `part.dard`
  * Add drag, lift and body lift properties to `ModuleLiftingSurface`, `ModuleDrag` and `ModuleControlSurface`

## 0.5.6.2 -> 0.5.6.3

* Add `ObjectAssemblyEngineDeltaV.engine` and `ObjectAssemblyEngineDeltaV.part` field
* Add `StageDeltaV.parts` and `ObjectAssemblyStageDeltaV.parts` fields
* Add `StageDeltaV.parts` and `ObjectAssemblyStageDeltaV.parts` fields`
* Add additional `ksp::vessel::Part.engine` field to make naming compatible with OAB (and other modules)
  * `.module_engine` is now supposed to be deprecated

## 0.5.6.1 -> 0.5.6.2

* Add `body.atmosphere_pressure_kpa(altitude : float)` method
* Add `body.atmosphere_temperature(altitude : float)` method
* Add `body.atmosphere_density(altitude : float)` method
* Add `engine.calc_max_thrust_output_atm()` method
* Fix `GeoCoordinates.terrain_height` and `body.terrain_height` to include offset from scenery

## 0.5.6.0 -> 0.5.6.1

* Improve parser support for string-interpolation
* Improve language documentation

## 0.5.5.5 -> 0.5.6.0

* Add `part.part_title` and `part.part_description` to `ksp::vessel` and `ksp::oab`
* Support for c# style string interpolation
  * Instead of `"Hello " + a.to_string() + " world " + b.to_string()` one can now just write `$"Hello {a} world {b}"`
  * Formatting is supported via `{<interpolationExpression>[,<alignment>][:<formatString>]}`
  * See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated for more details

## 0.5.4.1 -> 0.5.5.5

* Add `engineMode.propellant` field
* Protentially breaking changes:
  * Cleanup `ModuleCommand`: Remove fields that belong to `Part` to access those use `moduleCommand.part.field`
  * Cleanup `ModuleDockingNode`: Remove fields that belong to `Part` to access those use `dockingNode.part.field`
* Add `.part` field to all `Module*` types
* Add `moduleCommand.required_resources`
* Add `moduleSolarPanel.resource_setting`
* Add `moduleHeatshield.required_resources`
* Add binding for transmitter modules
* Add binding for lighting modules
* Add binding for generator modules
* Support air intake, command, decoupler, docking node, generator, light, transmitter, science_experiment in VAB
* Make resource container information available in VAB
* Add `.flat_map` and `.filter_map` to ranges
* Make `warp_to`, `cancel_warp` and `set_warp_index` in `ksp::game::warp` available in sync context (i.e. inside UI callback)
  * This is in part a rollback of the previous change to `warp_to`
* Add binding for RCS modules
* Add binding for ReactionWheel modules
* Add `.active_transmitter` field to `ScienceStorage`
  * `ScienceStorage` had to be moved from `ksp::science` to `ksp::vessel`
* Add `.connection_status` to vessel

## 0.5.4.0 -> 0.5.4.1

Hotfix:
* Fix coordinate system mismatch in `GeoCoordinate.surface_position`
* Fix broken `GeoCoordinate.global_surface_normal`


## 0.5.3.4 -> 0.5.4.0

* Extend array functionality:
  * `array.flat_map`: Map with function that returns an array for each element.
    Result will be appended.
    Example `[ 1, 2, 3 ].flat_map(fn(i) -> [0.1 * i, 0.01 * i]) == [ 0.1, 0.01, 0.2, 0.02, 0.3, 0.03]`
  * `array.filter_map`: Map with function that return an option for each element.
    Result will only contain values where option is defined.
    Example `vessel.parts.filter_map(fn(part) -> part.solar_panel)` to get an array of all
    solar panels of the vessel.
* Add solar panel flow information:
  * `base_flow_rate` (available in VAB)
  * `efficiency_multiplier` (available in VAB)
  * `star_energy_scale`
  * `max_flow` shorthand for `base_flow_rate * efficiency_multiplier * star_energy_scale`
* Add `fairing.enabled` toggle
* Add `deployable.extended` toggle
* Add heatshield information:
  * `heatshield.is_deployed`
  * `heatshield.is_ablating`
  * `heatshield.is_ablator_exhausted`
  * `heatshield.ablator_ratio` (0.0 - 1.0)
* Add propellant information to engine via `engine.current_propellant` and `engine.propellants`
  * Extend `ResourceDefinition` by `is_recipe` and `recipe_ingredients`
* Add `engine.thrust_limiter` property
* Add `part.is_cargo_bay`
* Add `engine.has_fairing` and `engine.fairing` to access engine specific fairings
  * `part.fairing` will now only cover non-engine parts
* Add `set_warp_index(int)`, `max_warp_index()`, `is_warping()` and `is_physics_time_warp()` to `ksp::game::warp`
  * `warp_to(float)` and `set_warp_index(int)` can not be used in a sync function
* `ModuleDeployable.deploy_state` is now a proper enumeration and not just a string
* Improve available engine information in VAB

## 0.5.3.3 -> 0.5.3.4 

Hotfix:
* Re-add accidentally dropped `research_report.start_transmission()` (#125) 

## 0.5.3.2 -> 0.5.3.3

* Fix stuck in rebooting error if there is an initial compilation failure
* Support independent throttle for engines via `engine.independent_throttle_enabled` and `engine.independent_throttle` (#128)
* Support control for engine gimbal via `engine.gimbal` (#128)
* Add basic delta-v information for `PartAssembly` in `ksp::oab` (#127)

## 0.5.3.1 -> 0.5.3.2

* Convert `EngineDeltaV.get_thrust_vector()` to celestial frame (#128)
* Add coordinate independent variant `EngineDeltaV.get_global_thrust_vector()`
* Add corresponding fields to `ModuleEngine`
  * `thrust_direction`
  * `global_thrust_direction`
* Add `ksp::ui::CONSOLE_WINDOW` to control the console window via script (#129)
  * Methods: `CONSOLE_WINDOW.open()`, `CONSOLE_WINDOW.open()`, `CONSOLE_WINDOW.center()`
  * Fields: `CONSOLE_WINDOW.is_closed`, `CONSOLE_WINDOW.size`, `CONSOLE_WINDOW.min_size`, `CONSOLE_WINDOW.position`
* Improve bindings for science data (#125)
  * Get a list of all completed research reports via `ksp::science::get_completed_research_reports()`
* Experimental bindings for object assembly (VAB, #127)

## 0.5.2.8 -> 0.5.3.1

* Internal updates
  * Migrate code to netstandard2.1 (except unit tests)
  * Update C# version and enable nullable feature (improving overall code quality)
* Add methods to manipulate window position/size after creation
  * `window.position` property (writable) to get current position of window or move it
  * `window.size` property (writable) to get current size of window or resize it
  * `window.min_size` property (read-only) to get minimum size of window 
  * `window.compact()` resize window to its minimum size
  * `window.center()` center window on screen
* Fix an issue when trying to display empty telemetry timeseries
* Fix issue using inside a lambda-function (#118)
* Fix compilation error of if-else-return cascade (#119)
* Fix reference rendering of debug vectors in flight view

## 0.5.2.7 -> 0.5.2.8

* Improve error handling in struct field initializers (#119)
* Improve docutmation of function/method parameters (#126)
* Fix size calculation in UI vertical layout (#126)
* Add `add_spacer` to UI layout containers (#126)

## 0.5.2.6 -> 0.5.2.7

* Fix type generation of structs in generic parameters (#76)
* Improve error reporting of missing variables (#119)
* Fix bug in if-case (#119)
* Add binding for science action group
* Fix default values in user-struct methods (#119)
* Remove `condition_met` from experiment (for now?) as it seems to have a different meaning
* Add `vessel.research_location` that can be compared with `experiment.experiment_location` (#125)
* Add `vessel.science_storage` to access science storage/resource inventory (#125)

## 0.5.2.5 -> 0.5.2.6

* Improve error handling on missing imports (#119)
* Add button to VAB (#121)
  * Note: Most binding are still untested for VAB
* Fix: Handle line comments in record creation (#113)
* Add helper methods to string (#123):
  * `index_of`: `"abcdefgh".index_of("g") == 6` or `"abcdefghabcdefgh".index_of("g", 7) == 14`
  * `slice`: `"abcdefgh".slice(3) == "defgh"` or `"abcdefgh".slice(3, 5) == "de"`
  * `replace`: `"abcdefgh".replace("de", "en") == "abcenfgh"`
  * `split`: `"a,b,c,de,fgh".split(",") == ["a", "b", "c", "de", "fgh"]`

## 0.5.2.4 -> 0.5.2.5

* Add `body.parent_body` and `body.orbiting_bodies` fields
* Add `reduce` and `reverse` method to ranges (#114)
* Fix `orbit.global_velocity` to use correct frame of reference (#116)
* Fix internal type comparison/generation of structs (#118)
* Add `.sort_by` and `.sort_with` to array

## 0.5.2.2 -> 0.5.2.4

* Fix/tweak update of maneuver node data (#111)
* Add `.sort` method for array (#114). Example: `[10,6,5].sort() == [5,6,10]`
* Add `.slice` method for array (#114). Examples:
    * `[00, 11, 22, 33, 44].slice(2) == [22,33,44]`
    * `[00, 11, 22, 33, 44].slice(1, 3) == [11,22]`
* Add `.reduce` method for array (#114): Example: `[1,2,3,4].reduce(0, fn(a, e) -> a + e) == 10`
* Fix handling of comments in chained method calls (#113)
* Fix type resolution for imported constants (#107)
* Ensure that non-global coordinates are in the celestial frame of the main body (addresses #117)
* Breaking change: Fix take motion of parent body into account for `orbit.global_position(ut)` (#116)
  * If you have used `orbit.global_position(ut)` before you might want to use `orbit.global_relative_position` instead

## 0.5.2 -> 0.5.2.2

* Fix update for maneuver node data in map-view (#111)

## 0.5.1.2 -> 0.5.2

* Improve `vessel.maneuver.add` and `vessel.maneuver.add_burn_vector` to use the
  correct orbit patch in case multiple maneuvers are planed ahead.
* Add `vessel.maneuver.remove_all()` helper to remove all maneuver nodes at once
* Add `vessel.trajectory` field containing all the orbit patches of the current
  trajectory of the vessel (i.e. where the vessel will end up if it stays on its
  current course without intervention).
* Add `vessel.maneuver.trajectory` field containing all the orbit patches if all
  maneuvers are successfully executed.
  * This list will always start after the first maneuver node. I.e. if there are
    no planed maneuvers it will be empty.
* Add `orbit.start_transition` and `orbit.end_transition` fields representing the
  patch transition at the start and end.
* Add `orbit.previous_patch` and `orbit.next_patch` fields (both `Option<Orbit>`)
  to get the previous/next patch if available.

## 0.5.1.1 -> 0.5.1.2

* Fix `vessel.maneuver.add` (#108)

## 0.5.1 -> 0.5.1.1

* Improve vscode extension/lsp-server
* Improve type inference in structs (#104)
* Improve handling of if ... else if ... (#105)

## 0.5.0.1 -> 0.5.1

* Tweak SteeringManager to also control grid fins (#101)
  * ... might require some further tweaking
* Improve binding for science experiments
  * Also allowing science experiments to be run via script

## 0.5.0 -> 0.5.0.1

* Fix part resources fields (#99)
* Fix exception in maneuver node creation (#100)
* Basic bindings for science parts

## 0.4.4 -> 0.5.0

* Compatibility with v0.2.0.0

## 0.4.3 -> 0.4.4

* Compatibility with v0.1.4.0

## 0.4.2.4 -> 0.4.3

* Cleanup ambiguity of the the `^` operator
  * `^` is now bitwise xor and only defined for integers with the same precedence as `&` and `|`
  * For float and integer there is a new `**` operator which is a shorthand for `pow` and has a higher precedence than
    `*` and `/`

## 0.4.2.3 -> 0.4.2.4

* Fix blurry labels in telemetry display (addresses #98) 

## 0.4.2.2 -> 0.4.2.3

* Fix `ksp::telemetry::add_time_series` (telemetry)

## 0.4.2.1 -> 0.4.2.2

* Ensure that logging backend is only invoked in main thread
  * This prevents dead-lock if logging from backend thread
* `^` operator now behaves like `pow` instead of bit-xor. Also has higher precedence over `*` and `/` (addresses #95)
* Fix minimum dependency to spacewarp

## 0.4.2.1

Bug fixes:
* Further improve type-inference (#91)
* Fix match_inclination detla-v calculation (#94)

## 0.4.2

* Compatibility with 0.1.3.0

## 0.4.1.2 -> 0.4.1.3

* Add additional part information
  * `splashed`, `temperature`, `max_temperature`, `dry_mass`, `resource_mass`, `total_mass`
* Fix function type aliases (#91)
* Fix usage of constants in default parameters (#92)
* Support inlay hints in LSP server

## 0.4.1.1 -> 0.4.1.2

* Fix parsing issue with line commends in if-else cases
* Add `MAINFRAME.available_processes` and `MAINFRAME.find_process` to launch scripts programatically
* Fix `orbit.semi_major_axis`

## 0.4.1 -> 0.4.1.1

* Add `get_vessels_in_range()` and `get_all_owned_vessels()` functions to `ksp::vessel`
* Add `Vessel.make_active()`

## 0.3.6 -> 0.4.1

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
