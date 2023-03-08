# Hello world

A wee bit more than just that.

## Code

```rust
use { Vessel } from ksp::vessel
use { CONSOLE } from ksp::console

pub fn main_flight(vessel: Vessel) -> Unit = {
    CONSOLE.clear()
    CONSOLE.print_line("Hello world: " + vessel.name)
}
```

## Annotated

```rust
use { Vessel } from ksp::vessel
```

* Import the [`Vessel`](../../reference/ksp/vessel.md#vessel) type from the builtin `ksp::vessel` module.
* The `Vessel` type contains many properties and methods to interact with a vessel in the game
* Please to not interact with Kerbals this way
  * ... I mean ... you potentially could, but it is probably not very nice

```rust
use { CONSOLE } from ksp::console
```

* Import the [`CONSOLE`](../../reference/ksp/console.md) type from the builtin `ksp::console` module.

```rust
pub fn main_flight(vessel: Vessel) -> Unit = {
```

* The entrypoint for flight-mode, i.e. the script will appear on the main window if the game is in flight-mode
* `vessel` will be a reference to the currently active vessel

```rust
    CONSOLE.clear()
```

* Clear the console

```rust
    CONSOLE.print_line("Hello world:" + vessel.name)
```

* Will print "Hello world:" followed by the name of the active vessel
