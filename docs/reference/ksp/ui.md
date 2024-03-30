# ksp::ui



## Types


### Align

Alignment of the element in off direction (horizontal for vertical container and vice versa)

#### Methods

##### to_string

```rust
align.to_string ( ) -> string
```

String representation of the number

### AlignConstants



#### Fields

| Name    | Type                                         | Read-only | Description                                           |
| ------- | -------------------------------------------- | --------- | ----------------------------------------------------- |
| Center  | [ksp::ui::Align](/reference/ksp/ui.md#align) | R/O       | Align the element to the center of the container.     |
| End     | [ksp::ui::Align](/reference/ksp/ui.md#align) | R/O       | Align the element to end of container (right/bottom). |
| Start   | [ksp::ui::Align](/reference/ksp/ui.md#align) | R/O       | Align the element to start of container (left/top).   |
| Stretch | [ksp::ui::Align](/reference/ksp/ui.md#align) | R/O       | Stretch the element to full size of container         |


#### Methods

##### from_string

```rust
alignconstants.from_string ( value : string ) -> Option<ksp::ui::Align>
```

Parse from string

Parameters

| Name  | Type   | Optional | Description          |
| ----- | ------ | -------- | -------------------- |
| value | string |          | Enum value to lookup |


### Button



#### Fields

| Name      | Type   | Read-only | Description                    |
| --------- | ------ | --------- | ------------------------------ |
| enabled   | bool   | R/W       | Enable/disable the button      |
| font_size | float  | R/W       | Font size of the button label  |
| label     | string | R/W       | Button label                   |


#### Methods

##### on_click

```rust
button.on_click ( onClick : sync fn() -> Unit ) -> Unit
```

Function to be called if button is clicked


Parameters

| Name    | Type              | Optional | Description |
| ------- | ----------------- | -------- | ----------- |
| onClick | sync fn() -> Unit |          |             |


##### remove

```rust
button.remove ( ) -> Unit
```



### Canvas



#### Fields

| Name     | Type                                           | Read-only | Description                                                             |
| -------- | ---------------------------------------------- | --------- | ----------------------------------------------------------------------- |
| height   | float                                          | R/O       | Current height of the canvas (determined by the surrounding container)  |
| min_size | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       | Minimum size of the canvas.                                             |
| width    | float                                          | R/O       | Current width of the canvas (determined by the surrounding container)   |


#### Methods

##### add_line

```rust
canvas.add_line ( points : ksp::math::Vec2[],
                  strokeColor : ksp::console::RgbaColor,
                  closed : bool,
                  thickness : float ) -> ksp::ui::Line2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |
| thickness   | float                   | x        |             |


##### add_pixel_line

```rust
canvas.add_pixel_line ( points : ksp::math::Vec2[],
                        strokeColor : ksp::console::RgbaColor,
                        closed : bool ) -> ksp::ui::PixelLine2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |


##### add_polygon

```rust
canvas.add_polygon ( points : ksp::math::Vec2[],
                     fillColor : ksp::console::RgbaColor ) -> ksp::ui::Polygon2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| points    | ksp::math::Vec2[]       |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rect

```rust
canvas.add_rect ( point1 : ksp::math::Vec2,
                  point2 : ksp::math::Vec2,
                  fillColor : ksp::console::RgbaColor ) -> ksp::ui::Rect2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| point1    | ksp::math::Vec2         |          |             |
| point2    | ksp::math::Vec2         |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rotate

```rust
canvas.add_rotate ( degrees : float ) -> ksp::ui::Rotate2D
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| degrees | float |          |             |


##### add_scale

```rust
canvas.add_scale ( scale : ksp::math::Vec2 ) -> ksp::ui::Scale2D
```



Parameters

| Name  | Type            | Optional | Description |
| ----- | --------------- | -------- | ----------- |
| scale | ksp::math::Vec2 |          |             |


##### add_text

```rust
canvas.add_text ( position : ksp::math::Vec2,
                  text : string,
                  fontSize : float,
                  color : ksp::console::RgbaColor,
                  degrees : float,
                  pivot : ksp::math::Vec2 ) -> ksp::ui::Text2D
```



Parameters

| Name     | Type                    | Optional | Description |
| -------- | ----------------------- | -------- | ----------- |
| position | ksp::math::Vec2         |          |             |
| text     | string                  |          |             |
| fontSize | float                   |          |             |
| color    | ksp::console::RgbaColor |          |             |
| degrees  | float                   | x        |             |
| pivot    | ksp::math::Vec2         | x        |             |


##### add_translate

```rust
canvas.add_translate ( translate : ksp::math::Vec2 ) -> ksp::ui::Translate2D
```



Parameters

| Name      | Type            | Optional | Description |
| --------- | --------------- | -------- | ----------- |
| translate | ksp::math::Vec2 |          |             |


##### add_value_raster

```rust
canvas.add_value_raster ( point1 : ksp::math::Vec2,
                          point2 : ksp::math::Vec2,
                          values : float[],
                          width : int,
                          height : int,
                          gradientWrapper : ksp::ui::Gradient ) -> ksp::ui::ValueRaster2D
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| point1          | ksp::math::Vec2   |          |             |
| point2          | ksp::math::Vec2   |          |             |
| values          | float[]           |          |             |
| width           | int               |          |             |
| height          | int               |          |             |
| gradientWrapper | ksp::ui::Gradient |          |             |


##### clear

```rust
canvas.clear ( ) -> Unit
```



##### remove

```rust
canvas.remove ( ) -> Unit
```



### ConsoleWindow

Represents the console window


#### Fields

| Name      | Type                                           | Read-only | Description                            |
| --------- | ---------------------------------------------- | --------- | -------------------------------------- |
| is_closed | bool                                           | R/O       | Check if the console window is closed  |
| min_size  | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/O       | Get minimum size of window             |
| position  | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       | Get or change position of window       |
| size      | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       | Get or change size of window           |


#### Methods

##### center

```rust
consolewindow.center ( ) -> Unit
```

Center window on the screen.


##### close

```rust
consolewindow.close ( ) -> Unit
```

Close the console window


##### open

```rust
consolewindow.open ( ) -> Unit
```

Open the console window


### Container



#### Methods

##### add_button

```rust
container.add_button ( label : string,
                       align : ksp::ui::Align,
                       stretch : float ) -> ksp::ui::Button
```

Add button to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the button in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_canvas

```rust
container.add_canvas ( minWidth : float,
                       minHeight : float,
                       align : ksp::ui::Align,
                       stretch : float ) -> ksp::ui::Canvas
```

Add canvas to the container


Parameters

| Name      | Type           | Optional | Description                                                          |
| --------- | -------------- | -------- | -------------------------------------------------------------------- |
| minWidth  | float          |          | Minimum width of the canvas                                          |
| minHeight | float          |          | Minimum height of the canvas                                         |
| align     | ksp::ui::Align | x        | Alignment of the canvas in its parent container                      |
| stretch   | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_float_input

```rust
container.add_float_input ( align : ksp::ui::Align,
                            stretch : float ) -> ksp::ui::FloatInputField
```

Add float input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal

```rust
container.add_horizontal ( gap : float,
                           align : ksp::ui::Align,
                           stretch : float ) -> ksp::ui::Container
```

Add sub container with horizontal layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the container                            |
| align   | ksp::ui::Align | x        | Alignment of the sub container in its parent container               |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal_panel

```rust
container.add_horizontal_panel ( gap : float,
                                 align : ksp::ui::Align,
                                 stretch : float ) -> ksp::ui::Container
```

Add sub panel with horizontal layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the panel                                |
| align   | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal_slider

```rust
container.add_horizontal_slider ( min : float,
                                  max : float,
                                  align : ksp::ui::Align,
                                  stretch : float ) -> ksp::ui::Slider
```

Add horizontal slider to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| min     | float          |          | Minimum value of the slider                                          |
| max     | float          |          | Maximum value of the slider                                          |
| align   | ksp::ui::Align | x        | Alignment of the slider in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_int_input

```rust
container.add_int_input ( align : ksp::ui::Align,
                          stretch : float ) -> ksp::ui::IntInputField
```

Add integer input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_label

```rust
container.add_label ( label : string,
                      align : ksp::ui::Align,
                      stretch : float ) -> ksp::ui::Label
```

Add label to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the label in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_spacer

```rust
container.add_spacer ( size : float,
                       stretch : float ) -> Unit
```

Add empty space between elements


Parameters

| Name    | Type  | Optional | Description                                                          |
| ------- | ----- | -------- | -------------------------------------------------------------------- |
| size    | float |          | Minimum amount of space between elements                             |
| stretch | float | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_string_input

```rust
container.add_string_input ( align : ksp::ui::Align,
                             stretch : float ) -> ksp::ui::StringInputField
```

Add string input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_toggle

```rust
container.add_toggle ( label : string,
                       align : ksp::ui::Align,
                       stretch : float ) -> ksp::ui::Toggle
```

Add toggle to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the toggle in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical

```rust
container.add_vertical ( gap : float,
                         align : ksp::ui::Align,
                         stretch : float ) -> ksp::ui::Container
```

Add sub container with vertical layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the container                            |
| align   | ksp::ui::Align | x        | Alignment of the sub container in its parent container               |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical_panel

```rust
container.add_vertical_panel ( gap : float,
                               align : ksp::ui::Align,
                               stretch : float ) -> ksp::ui::Container
```

Add sub panel with vertical layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the panel                                |
| align   | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical_scroll

```rust
container.add_vertical_scroll ( minWidth : float,
                                minHeight : float,
                                gap : float,
                                align : ksp::ui::Align,
                                stretch : float ) -> ksp::ui::Container
```

Add vertical scroll view to the container


Parameters

| Name      | Type           | Optional | Description                                                          |
| --------- | -------------- | -------- | -------------------------------------------------------------------- |
| minWidth  | float          |          | Minimum width of the scroll view                                     |
| minHeight | float          |          | Minimum height of the scroll view                                    |
| gap       | float          | x        | Gap between each element of the panel                                |
| align     | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch   | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### remove

```rust
container.remove ( ) -> Unit
```



### FloatInputField



#### Fields

| Name      | Type  | Read-only | Description |
| --------- | ----- | --------- | ----------- |
| enabled   | bool  | R/W       |             |
| font_size | float | R/W       |             |
| value     | float | R/W       |             |


#### Methods

##### bind

```rust
floatinputfield.bind ( boundValue : Cell<T> ) -> ksp::ui::FloatInputField
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |


##### on_change

```rust
floatinputfield.on_change ( onChange : sync fn(float) -> Unit ) -> Unit
```



Parameters

| Name     | Type                   | Optional | Description |
| -------- | ---------------------- | -------- | ----------- |
| onChange | sync fn(float) -> Unit |          |             |


##### remove

```rust
floatinputfield.remove ( ) -> Unit
```



### Gradient



#### Methods

##### add_color

```rust
gradient.add_color ( value : float,
                     color : ksp::console::RgbaColor ) -> bool
```



Parameters

| Name  | Type                    | Optional | Description |
| ----- | ----------------------- | -------- | ----------- |
| value | float                   |          |             |
| color | ksp::console::RgbaColor |          |             |


### IntInputField



#### Fields

| Name      | Type  | Read-only | Description |
| --------- | ----- | --------- | ----------- |
| enabled   | bool  | R/W       |             |
| font_size | float | R/W       |             |
| value     | int   | R/W       |             |


#### Methods

##### bind

```rust
intinputfield.bind ( boundValue : Cell<T> ) -> ksp::ui::IntInputField
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |


##### on_change

```rust
intinputfield.on_change ( onChange : sync fn(float) -> Unit ) -> Unit
```



Parameters

| Name     | Type                   | Optional | Description |
| -------- | ---------------------- | -------- | ----------- |
| onChange | sync fn(float) -> Unit |          |             |


##### remove

```rust
intinputfield.remove ( ) -> Unit
```



### Label



#### Fields

| Name      | Type   | Read-only | Description |
| --------- | ------ | --------- | ----------- |
| font_size | float  | R/W       |             |
| text      | string | R/W       |             |


#### Methods

##### bind

```rust
label.bind ( boundValue : Cell<T>,
             format : string ) -> ksp::ui::Label
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |
| format     | string  | x        |             |


##### remove

```rust
label.remove ( ) -> Unit
```



### Line2D



#### Fields

| Name         | Type                                                           | Read-only | Description |
| ------------ | -------------------------------------------------------------- | --------- | ----------- |
| closed       | bool                                                           | R/W       |             |
| points       | [ksp::math::Vec2](/reference/ksp/math.md#vec2)[]               | R/W       |             |
| stroke_color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       |             |
| thickness    | float                                                          | R/W       |             |


### PixelLine2D



#### Fields

| Name         | Type                                                           | Read-only | Description |
| ------------ | -------------------------------------------------------------- | --------- | ----------- |
| closed       | bool                                                           | R/W       |             |
| points       | [ksp::math::Vec2](/reference/ksp/math.md#vec2)[]               | R/W       |             |
| stroke_color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       |             |


### Polygon2D



#### Fields

| Name       | Type                                                           | Read-only | Description |
| ---------- | -------------------------------------------------------------- | --------- | ----------- |
| fill_color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       |             |
| points     | [ksp::math::Vec2](/reference/ksp/math.md#vec2)[]               | R/W       |             |


### Rect2D



#### Fields

| Name       | Type                                                           | Read-only | Description |
| ---------- | -------------------------------------------------------------- | --------- | ----------- |
| fill_color | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       |             |
| point1     | [ksp::math::Vec2](/reference/ksp/math.md#vec2)                 | R/W       |             |
| point2     | [ksp::math::Vec2](/reference/ksp/math.md#vec2)                 | R/W       |             |


### Rotate2D



#### Fields

| Name    | Type                                           | Read-only | Description |
| ------- | ---------------------------------------------- | --------- | ----------- |
| degrees | float                                          | R/W       |             |
| pivot   | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       |             |


#### Methods

##### add_line

```rust
rotate2d.add_line ( points : ksp::math::Vec2[],
                    strokeColor : ksp::console::RgbaColor,
                    closed : bool,
                    thickness : float ) -> ksp::ui::Line2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |
| thickness   | float                   | x        |             |


##### add_pixel_line

```rust
rotate2d.add_pixel_line ( points : ksp::math::Vec2[],
                          strokeColor : ksp::console::RgbaColor,
                          closed : bool ) -> ksp::ui::PixelLine2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |


##### add_polygon

```rust
rotate2d.add_polygon ( points : ksp::math::Vec2[],
                       fillColor : ksp::console::RgbaColor ) -> ksp::ui::Polygon2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| points    | ksp::math::Vec2[]       |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rect

```rust
rotate2d.add_rect ( point1 : ksp::math::Vec2,
                    point2 : ksp::math::Vec2,
                    fillColor : ksp::console::RgbaColor ) -> ksp::ui::Rect2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| point1    | ksp::math::Vec2         |          |             |
| point2    | ksp::math::Vec2         |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rotate

```rust
rotate2d.add_rotate ( degrees : float ) -> ksp::ui::Rotate2D
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| degrees | float |          |             |


##### add_scale

```rust
rotate2d.add_scale ( scale : ksp::math::Vec2 ) -> ksp::ui::Scale2D
```



Parameters

| Name  | Type            | Optional | Description |
| ----- | --------------- | -------- | ----------- |
| scale | ksp::math::Vec2 |          |             |


##### add_text

```rust
rotate2d.add_text ( position : ksp::math::Vec2,
                    text : string,
                    fontSize : float,
                    color : ksp::console::RgbaColor,
                    degrees : float,
                    pivot : ksp::math::Vec2 ) -> ksp::ui::Text2D
```



Parameters

| Name     | Type                    | Optional | Description |
| -------- | ----------------------- | -------- | ----------- |
| position | ksp::math::Vec2         |          |             |
| text     | string                  |          |             |
| fontSize | float                   |          |             |
| color    | ksp::console::RgbaColor |          |             |
| degrees  | float                   | x        |             |
| pivot    | ksp::math::Vec2         | x        |             |


##### add_translate

```rust
rotate2d.add_translate ( translate : ksp::math::Vec2 ) -> ksp::ui::Translate2D
```



Parameters

| Name      | Type            | Optional | Description |
| --------- | --------------- | -------- | ----------- |
| translate | ksp::math::Vec2 |          |             |


##### add_value_raster

```rust
rotate2d.add_value_raster ( point1 : ksp::math::Vec2,
                            point2 : ksp::math::Vec2,
                            values : float[],
                            width : int,
                            height : int,
                            gradientWrapper : ksp::ui::Gradient ) -> ksp::ui::ValueRaster2D
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| point1          | ksp::math::Vec2   |          |             |
| point2          | ksp::math::Vec2   |          |             |
| values          | float[]           |          |             |
| width           | int               |          |             |
| height          | int               |          |             |
| gradientWrapper | ksp::ui::Gradient |          |             |


##### clear

```rust
rotate2d.clear ( ) -> Unit
```



### Scale2D



#### Fields

| Name  | Type                                           | Read-only | Description |
| ----- | ---------------------------------------------- | --------- | ----------- |
| pivot | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       |             |
| scale | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       |             |


#### Methods

##### add_line

```rust
scale2d.add_line ( points : ksp::math::Vec2[],
                   strokeColor : ksp::console::RgbaColor,
                   closed : bool,
                   thickness : float ) -> ksp::ui::Line2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |
| thickness   | float                   | x        |             |


##### add_pixel_line

```rust
scale2d.add_pixel_line ( points : ksp::math::Vec2[],
                         strokeColor : ksp::console::RgbaColor,
                         closed : bool ) -> ksp::ui::PixelLine2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |


##### add_polygon

```rust
scale2d.add_polygon ( points : ksp::math::Vec2[],
                      fillColor : ksp::console::RgbaColor ) -> ksp::ui::Polygon2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| points    | ksp::math::Vec2[]       |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rect

```rust
scale2d.add_rect ( point1 : ksp::math::Vec2,
                   point2 : ksp::math::Vec2,
                   fillColor : ksp::console::RgbaColor ) -> ksp::ui::Rect2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| point1    | ksp::math::Vec2         |          |             |
| point2    | ksp::math::Vec2         |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rotate

```rust
scale2d.add_rotate ( degrees : float ) -> ksp::ui::Rotate2D
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| degrees | float |          |             |


##### add_scale

```rust
scale2d.add_scale ( scale : ksp::math::Vec2 ) -> ksp::ui::Scale2D
```



Parameters

| Name  | Type            | Optional | Description |
| ----- | --------------- | -------- | ----------- |
| scale | ksp::math::Vec2 |          |             |


##### add_text

```rust
scale2d.add_text ( position : ksp::math::Vec2,
                   text : string,
                   fontSize : float,
                   color : ksp::console::RgbaColor,
                   degrees : float,
                   pivot : ksp::math::Vec2 ) -> ksp::ui::Text2D
```



Parameters

| Name     | Type                    | Optional | Description |
| -------- | ----------------------- | -------- | ----------- |
| position | ksp::math::Vec2         |          |             |
| text     | string                  |          |             |
| fontSize | float                   |          |             |
| color    | ksp::console::RgbaColor |          |             |
| degrees  | float                   | x        |             |
| pivot    | ksp::math::Vec2         | x        |             |


##### add_translate

```rust
scale2d.add_translate ( translate : ksp::math::Vec2 ) -> ksp::ui::Translate2D
```



Parameters

| Name      | Type            | Optional | Description |
| --------- | --------------- | -------- | ----------- |
| translate | ksp::math::Vec2 |          |             |


##### add_value_raster

```rust
scale2d.add_value_raster ( point1 : ksp::math::Vec2,
                           point2 : ksp::math::Vec2,
                           values : float[],
                           width : int,
                           height : int,
                           gradientWrapper : ksp::ui::Gradient ) -> ksp::ui::ValueRaster2D
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| point1          | ksp::math::Vec2   |          |             |
| point2          | ksp::math::Vec2   |          |             |
| values          | float[]           |          |             |
| width           | int               |          |             |
| height          | int               |          |             |
| gradientWrapper | ksp::ui::Gradient |          |             |


##### clear

```rust
scale2d.clear ( ) -> Unit
```



### Slider



#### Fields

| Name    | Type  | Read-only | Description |
| ------- | ----- | --------- | ----------- |
| enabled | bool  | R/W       |             |
| value   | float | R/W       |             |


#### Methods

##### bind

```rust
slider.bind ( boundValue : Cell<T> ) -> ksp::ui::Slider
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |


##### on_change

```rust
slider.on_change ( onChange : sync fn(float) -> Unit ) -> Unit
```



Parameters

| Name     | Type                   | Optional | Description |
| -------- | ---------------------- | -------- | ----------- |
| onChange | sync fn(float) -> Unit |          |             |


##### remove

```rust
slider.remove ( ) -> Unit
```



### StringInputField



#### Fields

| Name      | Type   | Read-only | Description |
| --------- | ------ | --------- | ----------- |
| enabled   | bool   | R/W       |             |
| font_size | float  | R/W       |             |
| value     | string | R/W       |             |


#### Methods

##### bind

```rust
stringinputfield.bind ( boundValue : Cell<T> ) -> ksp::ui::StringInputField
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |


##### on_change

```rust
stringinputfield.on_change ( onChange : sync fn(string) -> Unit ) -> Unit
```



Parameters

| Name     | Type                    | Optional | Description |
| -------- | ----------------------- | -------- | ----------- |
| onChange | sync fn(string) -> Unit |          |             |


##### remove

```rust
stringinputfield.remove ( ) -> Unit
```



### Text2D



#### Fields

| Name      | Type                                                           | Read-only | Description |
| --------- | -------------------------------------------------------------- | --------- | ----------- |
| color     | [ksp::console::RgbaColor](/reference/ksp/console.md#rgbacolor) | R/W       |             |
| degrees   | float                                                          | R/W       |             |
| font_size | float                                                          | R/W       |             |
| pivot     | [ksp::math::Vec2](/reference/ksp/math.md#vec2)                 | R/W       |             |
| position  | [ksp::math::Vec2](/reference/ksp/math.md#vec2)                 | R/W       |             |
| text      | string                                                         | R/W       |             |


### Toggle



#### Fields

| Name      | Type   | Read-only | Description |
| --------- | ------ | --------- | ----------- |
| enabled   | bool   | R/W       |             |
| font_size | float  | R/W       |             |
| label     | string | R/W       |             |
| value     | bool   | R/W       |             |


#### Methods

##### bind

```rust
toggle.bind ( boundValue : Cell<T> ) -> ksp::ui::Toggle
```



Parameters

| Name       | Type    | Optional | Description |
| ---------- | ------- | -------- | ----------- |
| boundValue | Cell<T> |          |             |


##### on_change

```rust
toggle.on_change ( onChange : sync fn(bool) -> Unit ) -> Unit
```



Parameters

| Name     | Type                  | Optional | Description |
| -------- | --------------------- | -------- | ----------- |
| onChange | sync fn(bool) -> Unit |          |             |


##### remove

```rust
toggle.remove ( ) -> Unit
```



### Translate2D



#### Fields

| Name      | Type                                           | Read-only | Description |
| --------- | ---------------------------------------------- | --------- | ----------- |
| translate | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       |             |


#### Methods

##### add_line

```rust
translate2d.add_line ( points : ksp::math::Vec2[],
                       strokeColor : ksp::console::RgbaColor,
                       closed : bool,
                       thickness : float ) -> ksp::ui::Line2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |
| thickness   | float                   | x        |             |


##### add_pixel_line

```rust
translate2d.add_pixel_line ( points : ksp::math::Vec2[],
                             strokeColor : ksp::console::RgbaColor,
                             closed : bool ) -> ksp::ui::PixelLine2D
```



Parameters

| Name        | Type                    | Optional | Description |
| ----------- | ----------------------- | -------- | ----------- |
| points      | ksp::math::Vec2[]       |          |             |
| strokeColor | ksp::console::RgbaColor |          |             |
| closed      | bool                    | x        |             |


##### add_polygon

```rust
translate2d.add_polygon ( points : ksp::math::Vec2[],
                          fillColor : ksp::console::RgbaColor ) -> ksp::ui::Polygon2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| points    | ksp::math::Vec2[]       |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rect

```rust
translate2d.add_rect ( point1 : ksp::math::Vec2,
                       point2 : ksp::math::Vec2,
                       fillColor : ksp::console::RgbaColor ) -> ksp::ui::Rect2D
```



Parameters

| Name      | Type                    | Optional | Description |
| --------- | ----------------------- | -------- | ----------- |
| point1    | ksp::math::Vec2         |          |             |
| point2    | ksp::math::Vec2         |          |             |
| fillColor | ksp::console::RgbaColor |          |             |


##### add_rotate

```rust
translate2d.add_rotate ( degrees : float ) -> ksp::ui::Rotate2D
```



Parameters

| Name    | Type  | Optional | Description |
| ------- | ----- | -------- | ----------- |
| degrees | float |          |             |


##### add_scale

```rust
translate2d.add_scale ( scale : ksp::math::Vec2 ) -> ksp::ui::Scale2D
```



Parameters

| Name  | Type            | Optional | Description |
| ----- | --------------- | -------- | ----------- |
| scale | ksp::math::Vec2 |          |             |


##### add_text

```rust
translate2d.add_text ( position : ksp::math::Vec2,
                       text : string,
                       fontSize : float,
                       color : ksp::console::RgbaColor,
                       degrees : float,
                       pivot : ksp::math::Vec2 ) -> ksp::ui::Text2D
```



Parameters

| Name     | Type                    | Optional | Description |
| -------- | ----------------------- | -------- | ----------- |
| position | ksp::math::Vec2         |          |             |
| text     | string                  |          |             |
| fontSize | float                   |          |             |
| color    | ksp::console::RgbaColor |          |             |
| degrees  | float                   | x        |             |
| pivot    | ksp::math::Vec2         | x        |             |


##### add_translate

```rust
translate2d.add_translate ( translate : ksp::math::Vec2 ) -> ksp::ui::Translate2D
```



Parameters

| Name      | Type            | Optional | Description |
| --------- | --------------- | -------- | ----------- |
| translate | ksp::math::Vec2 |          |             |


##### add_value_raster

```rust
translate2d.add_value_raster ( point1 : ksp::math::Vec2,
                               point2 : ksp::math::Vec2,
                               values : float[],
                               width : int,
                               height : int,
                               gradientWrapper : ksp::ui::Gradient ) -> ksp::ui::ValueRaster2D
```



Parameters

| Name            | Type              | Optional | Description |
| --------------- | ----------------- | -------- | ----------- |
| point1          | ksp::math::Vec2   |          |             |
| point2          | ksp::math::Vec2   |          |             |
| values          | float[]           |          |             |
| width           | int               |          |             |
| height          | int               |          |             |
| gradientWrapper | ksp::ui::Gradient |          |             |


##### clear

```rust
translate2d.clear ( ) -> Unit
```



### ValueRaster2D



#### Fields

| Name          | Type                                               | Read-only | Description |
| ------------- | -------------------------------------------------- | --------- | ----------- |
| gradient      | [ksp::ui::Gradient](/reference/ksp/ui.md#gradient) | R/W       |             |
| point1        | [ksp::math::Vec2](/reference/ksp/math.md#vec2)     | R/W       |             |
| point2        | [ksp::math::Vec2](/reference/ksp/math.md#vec2)     | R/W       |             |
| raster_height | int                                                | R/O       |             |
| raster_width  | int                                                | R/O       |             |
| values        | float[]                                            | R/W       |             |


### Window



#### Fields

| Name      | Type                                           | Read-only | Description                                                     |
| --------- | ---------------------------------------------- | --------- | --------------------------------------------------------------- |
| is_closed | bool                                           | R/O       | Check if the window has been closed (either be user or script)  |
| min_size  | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/O       | Get minimum size of window                                      |
| position  | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       | Get or change position of window                                |
| size      | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/W       | Get or change size of window                                    |


#### Methods

##### add_button

```rust
window.add_button ( label : string,
                    align : ksp::ui::Align,
                    stretch : float ) -> ksp::ui::Button
```

Add button to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the button in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_canvas

```rust
window.add_canvas ( minWidth : float,
                    minHeight : float,
                    align : ksp::ui::Align,
                    stretch : float ) -> ksp::ui::Canvas
```

Add canvas to the container


Parameters

| Name      | Type           | Optional | Description                                                          |
| --------- | -------------- | -------- | -------------------------------------------------------------------- |
| minWidth  | float          |          | Minimum width of the canvas                                          |
| minHeight | float          |          | Minimum height of the canvas                                         |
| align     | ksp::ui::Align | x        | Alignment of the canvas in its parent container                      |
| stretch   | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_float_input

```rust
window.add_float_input ( align : ksp::ui::Align,
                         stretch : float ) -> ksp::ui::FloatInputField
```

Add float input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal

```rust
window.add_horizontal ( gap : float,
                        align : ksp::ui::Align,
                        stretch : float ) -> ksp::ui::Container
```

Add sub container with horizontal layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the container                            |
| align   | ksp::ui::Align | x        | Alignment of the sub container in its parent container               |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal_panel

```rust
window.add_horizontal_panel ( gap : float,
                              align : ksp::ui::Align,
                              stretch : float ) -> ksp::ui::Container
```

Add sub panel with horizontal layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the panel                                |
| align   | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_horizontal_slider

```rust
window.add_horizontal_slider ( min : float,
                               max : float,
                               align : ksp::ui::Align,
                               stretch : float ) -> ksp::ui::Slider
```

Add horizontal slider to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| min     | float          |          | Minimum value of the slider                                          |
| max     | float          |          | Maximum value of the slider                                          |
| align   | ksp::ui::Align | x        | Alignment of the slider in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_int_input

```rust
window.add_int_input ( align : ksp::ui::Align,
                       stretch : float ) -> ksp::ui::IntInputField
```

Add integer input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_label

```rust
window.add_label ( label : string,
                   align : ksp::ui::Align,
                   stretch : float ) -> ksp::ui::Label
```

Add label to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the label in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_spacer

```rust
window.add_spacer ( size : float,
                    stretch : float ) -> Unit
```

Add empty space between elements


Parameters

| Name    | Type  | Optional | Description                                                          |
| ------- | ----- | -------- | -------------------------------------------------------------------- |
| size    | float |          | Minimum amount of space between elements                             |
| stretch | float | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_string_input

```rust
window.add_string_input ( align : ksp::ui::Align,
                          stretch : float ) -> ksp::ui::StringInputField
```

Add string input field to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| align   | ksp::ui::Align | x        | Alignment of the input field in its parent container                 |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_toggle

```rust
window.add_toggle ( label : string,
                    align : ksp::ui::Align,
                    stretch : float ) -> ksp::ui::Toggle
```

Add toggle to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| label   | string         |          |                                                                      |
| align   | ksp::ui::Align | x        | Alignment of the toggle in its parent container                      |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical

```rust
window.add_vertical ( gap : float,
                      align : ksp::ui::Align,
                      stretch : float ) -> ksp::ui::Container
```

Add sub container with vertical layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the container                            |
| align   | ksp::ui::Align | x        | Alignment of the sub container in its parent container               |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical_panel

```rust
window.add_vertical_panel ( gap : float,
                            align : ksp::ui::Align,
                            stretch : float ) -> ksp::ui::Container
```

Add sub panel with vertical layout to the container


Parameters

| Name    | Type           | Optional | Description                                                          |
| ------- | -------------- | -------- | -------------------------------------------------------------------- |
| gap     | float          | x        | Gap between each element of the panel                                |
| align   | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### add_vertical_scroll

```rust
window.add_vertical_scroll ( minWidth : float,
                             minHeight : float,
                             gap : float,
                             align : ksp::ui::Align,
                             stretch : float ) -> ksp::ui::Container
```

Add vertical scroll view to the container


Parameters

| Name      | Type           | Optional | Description                                                          |
| --------- | -------------- | -------- | -------------------------------------------------------------------- |
| minWidth  | float          |          | Minimum width of the scroll view                                     |
| minHeight | float          |          | Minimum height of the scroll view                                    |
| gap       | float          | x        | Gap between each element of the panel                                |
| align     | ksp::ui::Align | x        | Alignment of the panel in its parent container                       |
| stretch   | float          | x        | Relative amount of available space to acquire (beyond minimal space) |


##### center

```rust
window.center ( ) -> Unit
```

Center window on the screen.


##### close

```rust
window.close ( ) -> Unit
```

Close the window


##### compact

```rust
window.compact ( ) -> Unit
```

Resize window to its minimum size


## Constants

| Name           | Type                    | Description                                                                                  |
| -------------- | ----------------------- | -------------------------------------------------------------------------------------------- |
| Align          | ksp::ui::AlignConstants | Alignment of the element in off direction (horizontal for vertical container and vice versa) |
| CONSOLE_WINDOW | ksp::ui::ConsoleWindow  | Main console window                                                                          |


## Functions


### gradient

```rust
pub sync fn gradient ( start : ksp::console::RgbaColor,
                       end : ksp::console::RgbaColor ) -> ksp::ui::Gradient
```



Parameters

| Name  | Type                    | Optional | Description |
| ----- | ----------------------- | -------- | ----------- |
| start | ksp::console::RgbaColor |          |             |
| end   | ksp::console::RgbaColor |          |             |


### open_centered_window

```rust
pub sync fn open_centered_window ( title : string,
                                   width : float,
                                   height : float ) -> ksp::ui::Window
```



Parameters

| Name   | Type   | Optional | Description |
| ------ | ------ | -------- | ----------- |
| title  | string |          |             |
| width  | float  |          |             |
| height | float  |          |             |


### open_window

```rust
pub sync fn open_window ( title : string,
                          x : float,
                          y : float,
                          width : float,
                          height : float ) -> ksp::ui::Window
```



Parameters

| Name   | Type   | Optional | Description |
| ------ | ------ | -------- | ----------- |
| title  | string |          |             |
| x      | float  |          |             |
| y      | float  |          |             |
| width  | float  |          |             |
| height | float  |          |             |


### screen_size

```rust
pub sync fn screen_size ( ) -> ksp::math::Vec2
```


