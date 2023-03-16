# Entrypoints

An entrypoint is just a TO2-script file (aka. module) with a main function. These will appear in the main UI window and can be started as a process.

There are different main functions for each game situation.

## Flight view

The entrypoint for the flight view is:

```rust
use { CONSOLE } from ksp::console

pub fn main_flight() -> Unit = {
    CONSOLE.print_line("Hello world")
}
```

For convenience the currently active vessel can be injected as parameter:

```rust
use { CONSOLE } from ksp::console
use { Vessel } from ksp::vessel

pub fn main_flight(vessel: Vessel) -> Unit = {
    CONSOLE.print_line("Hello " + vessel.name)
}
```

Entrypoints can have additional parameters beyond the active vessel. These serve as inputs to the process and can be overwritten in-game. 
Currently, `int`, `float` and `bool` types are supported. 
You should provide a default value for your extra parameters, otherwise a zero value will be chosen.

```rust
use { CONSOLE } from ksp::console
use { Vessel } from ksp::vessel

pub fn main_flight(vessel: Vessel, apoapsis: int = 1000, inclination: float = 1.5, circularize: bool = true) -> Unit = {
    CONSOLE.print_line("Hello " + vessel.name)
    CONSOLE.print_line("Launching to " + apoapsis.to_string() + "km, inclination" + inclination.to_string() + "°. circularize=" + circularize.to_string())
}
```

----
**NOTE** all the following entrypoints are essentially untested and might not work.

----

## Tracking station

The entrypoint for the Tracking station is:
```rust
use { CONSOLE } from ksp::console

pub fn main_tracking() -> Unit = {
    CONSOLE.print_line("Hello world")
}
```

## VAB

The entrypoint for the VAB is:
```rust
use { CONSOLE } from ksp::console

pub fn main_editor() -> Unit = {
    CONSOLE.print_line("Hello world")
}
```

## KSC

The entrypoint for the KSC is:
```rust
use { CONSOLE } from ksp::console

pub fn main_ksc() -> Unit = {
    CONSOLE.print_line("Hello world")
}
```