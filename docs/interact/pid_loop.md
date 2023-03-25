# PIDLoop

## Example

Here is a simple example how to create and use a [PIDLoop](../reference/ksp/control.md#pidloop):

```rust
use { Vessel } from ksp::vessel
use { CONSOLE } from ksp::console
use { sleep, current_time } from ksp::game
use { pid_loop } from ksp::control

pub fn main_flight( vessel: Vessel) -> Result<Unit, string> = {
    let tKp = 0.1		//throttle hold PID control variables
    let tKi = 0.0002
    let tKd = 0.01
    let tmin = 0.0
    let tmax = 1.0
    let throttlePID = pid_loop(tKp, tKi, tKd, tmin, tmax)

    let fake_speed = 0.0

    throttlePID.setpoint = 100   // desired speed

    CONSOLE.clear()
    for(i in 0..1000) {
        sleep(0.2)
        const throttle = throttlePID.update(current_time(), fake_speed)

        fake_speed += throttle * 10    // super-simple acceleration
        fake_speed -= 2                // super-simple drag

        CONSOLE.print_line("S: " + fake_speed.to_fixed(3) + " T: " + throttle.to_fixed(3))
    }
}
```