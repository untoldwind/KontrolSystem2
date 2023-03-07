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