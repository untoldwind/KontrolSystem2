# Thinks to improve aka. pain points

The following list is be no means priority sorted and might jump from "probably trivial" to megalomania.

## Extend the `std::` lib

The plugin ships with a set of example scripts including a `std::` lib containing helpers for various common maneuvers.
There are still a lot maneuvers missing and the existing ones have a lot of short comings

* `launch_rocket` is very basic and does not do a real gravity turn. It also might also end in a orbit where the periapsis is inside the atmosphere
* ... there is no "launch space plane" script
* `exec_node` does not work well when there is a SOI change
* ... there very little support to calculate SOI exit orbits (e.g. planing an ideal Duna transfer)
* ... there is no docking support
  * Rendezvous calculation/execution could also be improved on
* ... there is not atmospheric land for rockets (the Elon-style ... not the "Drop the boosters and open parachute"-style)
* ... there is landing for planes
* ... and what about rovers?

## Extend the `ksp::vessel` bindings

* Bindings for the vessel and the various modules is progressing nicely, but there are probably still a lot of telemetry data that might be interesting to have in a script
* ... there is not binding to initiate resource transfer
* ... mode switching for engines needs to be tested
* ... make individual engine thrust available (to make something like throttle controlled avionics at least possible)

## Extend the `ksp::game` bindings

* Improve time-wrap functionality (switching between rails- and physics-mode)

## General additions

* Some sort of storage per vessel for scripts to store/restore some sort of state (e.g. where to pick up after hibernation)

## UI improvements

* Syntax highlighting for the in-game script editor
* Highlight compilation errors in in-game editor
* On that note: Find a reliable way to disable game-input while an IMGUI text field has focus
  * ... maybe migrate away from IMGUI?
* Have keyboard input in the CONSOLE (like `kOS` had)
* On that note: Have some sort of REPL mode to just evaluate expressions

## UI for scripts?

* Allow scripts to open there own windows/dialogs?
* ... dare I say: Graphs ... that would be cool

## Improve tooling

* Add some basic autocompletion to VS-code
  * Probably not an lsp-server (cough), but using a generated json containing all the type names and pre-defined functions/modules