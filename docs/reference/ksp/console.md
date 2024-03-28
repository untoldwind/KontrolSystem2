# ksp::console

Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.

Additionally there is support for displaying popup messages on the HUD.



## Types


### Console

Representation of a console


#### Fields

| Name       | Type | Read-only | Description |
| ---------- | ---- | --------- | ----------- |
| cursor_col | int  | R/O       |             |
| cursor_row | int  | R/O       |             |


#### Methods

##### clear

```rust
console.clear ( ) -> Unit
```

Clear the console of all its content and move cursor to (0, 0).


##### clear_line

```rust
console.clear_line ( row : int ) -> Unit
```

Clear a line


Parameters

| Name | Type | Optional | Description |
| ---- | ---- | -------- | ----------- |
| row  | int  |          |             |


##### move_cursor

```rust
console.move_cursor ( row : int,
                      column : int ) -> Unit
```

Move the cursor to a give `row` and `column`.


Parameters

| Name   | Type | Optional | Description |
| ------ | ---- | -------- | ----------- |
| row    | int  |          |             |
| column | int  |          |             |


##### print

```rust
console.print ( message : string ) -> Unit
```

Print a message at the current cursor position (and move cursor forward)


Parameters

| Name    | Type   | Optional | Description |
| ------- | ------ | -------- | ----------- |
| message | string |          |             |


##### print_at

```rust
console.print_at ( row : int,
                   column : int,
                   message : string ) -> Unit
```

Moves the cursor to the specified position, prints the message and restores the previous cursor position


Parameters

| Name    | Type   | Optional | Description |
| ------- | ------ | -------- | ----------- |
| row     | int    |          |             |
| column  | int    |          |             |
| message | string |          |             |


##### print_line

```rust
console.print_line ( message : string ) -> Unit
```

Print a message at the current cursor position and move cursor to the beginning of the next line.


Parameters

| Name    | Type   | Optional | Description |
| ------- | ------ | -------- | ----------- |
| message | string |          |             |


### RgbaColor

Interface color with alpha channel.


#### Fields

| Name  | Type  | Read-only | Description |
| ----- | ----- | --------- | ----------- |
| alpha | float | R/O       |             |
| blue  | float | R/O       |             |
| green | float | R/O       |             |
| red   | float | R/O       |             |


## Constants

| Name    | Type                    | Description   |
| ------- | ----------------------- | ------------- |
| BLACK   | ksp::console::RgbaColor | Color black   |
| BLUE    | ksp::console::RgbaColor | Color blue    |
| CONSOLE | ksp::console::Console   | Main console  |
| CYAN    | ksp::console::RgbaColor | Color cyan    |
| GREEN   | ksp::console::RgbaColor | Color green   |
| RED     | ksp::console::RgbaColor | Color red     |
| WHITE   | ksp::console::RgbaColor | Color red     |
| YELLOW  | ksp::console::RgbaColor | Color yellow  |


## Functions


### color

```rust
pub sync fn color ( red : float,
                    green : float,
                    blue : float,
                    alpha : float ) -> ksp::console::RgbaColor
```

Create a new color from `red`, `green`, `blue` and `alpha` (0.0 - 1.0).


Parameters

| Name  | Type  | Optional | Description |
| ----- | ----- | -------- | ----------- |
| red   | float |          |             |
| green | float |          |             |
| blue  | float |          |             |
| alpha | float | x        |             |

