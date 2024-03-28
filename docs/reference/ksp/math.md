# ksp::math

Collection of KSP/Unity related mathematical functions.

## Types


### Direction

Represents the rotation from an initial coordinate system when looking down the z-axis and "up" being the y-axis

#### Fields

| Name         | Type                                                     | Read-only | Description                                                |
| ------------ | -------------------------------------------------------- | --------- | ---------------------------------------------------------- |
| euler        | [ksp::math::Vec3](/reference/ksp/math.md#vec3)           | R/W       | Euler angles in degree of the rotation                     |
| inverse      | [ksp::math::Direction](/reference/ksp/math.md#direction) | R/O       | Inverse direction                                          |
| pitch        | float                                                    | R/O       | Pitch in degree                                            |
| right_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3)           | R/O       | Right vector of the rotation                               |
| roll         | float                                                    | R/O       | Roll in degree                                             |
| up_vector    | [ksp::math::Vec3](/reference/ksp/math.md#vec3)           | R/O       | Up vector of the rotation                                  |
| vector       | [ksp::math::Vec3](/reference/ksp/math.md#vec3)           | R/W       | Fore vector of the rotation (i.e. looking/facing direction |
| yaw          | float                                                    | R/O       | Yaw in degree                                              |


#### Methods

##### to_global

```rust
direction.to_global ( frame : ksp::math::TransformFrame ) -> ksp::math::GlobalDirection
```

Associate this direction with a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
direction.to_string ( ) -> string
```

Convert the direction to string

### GlobalAngularVelocity

An angular velocity in space, that can be projected to a 3-dimensional vector in a specific frame of reference

#### Methods

##### relative_to

```rust
globalangularvelocity.relative_to ( frame : ksp::math::TransformFrame ) -> ksp::math::GlobalVector
```

Get relative angular velocity to a frame of reference

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_fixed

```rust
globalangularvelocity.to_fixed ( frame : ksp::math::TransformFrame,
                                 decimals : int ) -> string
```

Convert angular velocity to string with fixed number of `decimals` in a given coordinate system.

Parameters

| Name     | Type                      | Optional | Description        |
| -------- | ------------------------- | -------- | ------------------ |
| frame    | ksp::math::TransformFrame |          | Frame of reference |
| decimals | int                       |          | Number of decimals |


##### to_local

```rust
globalangularvelocity.to_local ( frame : ksp::math::TransformFrame ) -> ksp::math::Vec3
```

Get local angular velocity in a frame of reference

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
globalangularvelocity.to_string ( frame : ksp::math::TransformFrame ) -> string
```

Convert angular velocity to string in a given coordinate system.

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


### GlobalDirection

Represents the rotation from an initial coordinate system when looking down the z-axis and "up" being the y-axis

#### Fields

| Name         | Type                                                           | Read-only | Description                                                |
| ------------ | -------------------------------------------------------------- | --------- | ---------------------------------------------------------- |
| right_vector | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | Right vector of the rotation                               |
| up_vector    | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | Up vector of the rotation                                  |
| vector       | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/W       | Fore vector of the rotation (i.e. looking/facing direction |


#### Methods

##### euler

```rust
globaldirection.euler ( frame : ksp::math::TransformFrame ) -> ksp::math::Vec3
```

Get euler angles in a specific coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### pitch

```rust
globaldirection.pitch ( frame : ksp::math::TransformFrame ) -> float
```

Get pitch angle in a specific coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### roll

```rust
globaldirection.roll ( frame : ksp::math::TransformFrame ) -> float
```

Get roll angle in a specific coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_local

```rust
globaldirection.to_local ( frame : ksp::math::TransformFrame ) -> ksp::math::Direction
```

Get local direction in a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
globaldirection.to_string ( frame : ksp::math::TransformFrame ) -> string
```

Convert the direction to string

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### yaw

```rust
globaldirection.yaw ( frame : ksp::math::TransformFrame ) -> float
```

Get yaw angle in a specific coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


### GlobalPosition

A position in space that can be projected to a 3-dimensional vector in a specific coordinate system

#### Methods

##### distance

```rust
globalposition.distance ( other : ksp::math::GlobalPosition ) -> float
```

Calculate the distance of `other` position.

Parameters

| Name  | Type                      | Optional | Description    |
| ----- | ------------------------- | -------- | -------------- |
| other | ksp::math::GlobalPosition |          | Other position |


##### distance_sqr

```rust
globalposition.distance_sqr ( other : ksp::math::GlobalPosition ) -> float
```

Calculate the squared distance of `other` position.

Parameters

| Name  | Type                      | Optional | Description    |
| ----- | ------------------------- | -------- | -------------- |
| other | ksp::math::GlobalPosition |          | Other position |


##### lerp_to

```rust
globalposition.lerp_to ( other : ksp::math::GlobalPosition,
                         t : float ) -> ksp::math::GlobalPosition
```

Linear interpolate position between this and `other` position, where `t = 0.0` is this and `t = 1.0` is `other`.

Parameters

| Name  | Type                      | Optional | Description                                |
| ----- | ------------------------- | -------- | ------------------------------------------ |
| other | ksp::math::GlobalPosition |          | Other position                             |
| t     | float                     |          | Relative position of mid-point (0.0 - 1.0) |


##### to_fixed

```rust
globalposition.to_fixed ( frame : ksp::math::TransformFrame,
                          decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals` in a given coordinate system.

Parameters

| Name     | Type                      | Optional | Description        |
| -------- | ------------------------- | -------- | ------------------ |
| frame    | ksp::math::TransformFrame |          | Frame of reference |
| decimals | int                       |          | Number of decimals |


##### to_local

```rust
globalposition.to_local ( frame : ksp::math::TransformFrame ) -> ksp::math::Vec3
```

Get local vector in a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
globalposition.to_string ( frame : ksp::math::TransformFrame ) -> string
```

Convert vector to string in a given coordinate system.

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


### GlobalVector

Abstract vector in space that can be projected to a concrete 3-dimensional vector in a specific coordinate system

#### Fields

| Name          | Type                                                           | Read-only | Description                                 |
| ------------- | -------------------------------------------------------------- | --------- | ------------------------------------------- |
| magnitude     | float                                                          | R/O       | Magnitude/length of the vector              |
| normalized    | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | Normalized vector (i.e. scaled to length 1) |
| sqr_magnitude | float                                                          | R/O       | Squared magnitude of the vector             |


#### Methods

##### cross

```rust
globalvector.cross ( other : ksp::math::GlobalVector ) -> ksp::math::GlobalVector
```

Calculate the cross/other product with `other` vector.

Parameters

| Name  | Type                    | Optional | Description  |
| ----- | ----------------------- | -------- | ------------ |
| other | ksp::math::GlobalVector |          | Other vector |


##### dot

```rust
globalvector.dot ( other : ksp::math::GlobalVector ) -> float
```

Calculate the dot/inner product with `other` vector.

Parameters

| Name  | Type                    | Optional | Description  |
| ----- | ----------------------- | -------- | ------------ |
| other | ksp::math::GlobalVector |          | Other vector |


##### exclude_from

```rust
globalvector.exclude_from ( other : ksp::math::GlobalVector ) -> ksp::math::GlobalVector
```

Exclude this from `other` vector.

Parameters

| Name  | Type                    | Optional | Description  |
| ----- | ----------------------- | -------- | ------------ |
| other | ksp::math::GlobalVector |          | Other vector |


##### lerp_to

```rust
globalvector.lerp_to ( other : ksp::math::GlobalVector,
                       t : float ) -> ksp::math::GlobalVector
```

Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.

Parameters

| Name  | Type                    | Optional | Description                                |
| ----- | ----------------------- | -------- | ------------------------------------------ |
| other | ksp::math::GlobalVector |          | Other vector                               |
| t     | float                   |          | Relative position of mid-point (0.0 - 1.0) |


##### to_direction

```rust
globalvector.to_direction ( ) -> ksp::math::GlobalDirection
```

Convert the vector to a rotation/direction in space.

##### to_fixed

```rust
globalvector.to_fixed ( frame : ksp::math::TransformFrame,
                        decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals` in a given coordinate system.

Parameters

| Name     | Type                      | Optional | Description        |
| -------- | ------------------------- | -------- | ------------------ |
| frame    | ksp::math::TransformFrame |          | Frame of reference |
| decimals | int                       |          | Number of decimals |


##### to_local

```rust
globalvector.to_local ( frame : ksp::math::TransformFrame ) -> ksp::math::Vec3
```

Get local vector in a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
globalvector.to_string ( frame : ksp::math::TransformFrame ) -> string
```

Convert vector to string in a given coordinate system.

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


### GlobalVelocity

A velocity in space, that can be projected to a 3-dimensional vector in a specific frame of reference

#### Fields

| Name     | Type                                                               | Read-only | Description                           |
| -------- | ------------------------------------------------------------------ | --------- | ------------------------------------- |
| position | [ksp::math::GlobalPosition](/reference/ksp/math.md#globalposition) | R/W       | Position the velocity was measured at |
| vector   | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector)     | R/W       | Relative velocity vector              |


#### Methods

##### to_fixed

```rust
globalvelocity.to_fixed ( frame : ksp::math::TransformFrame,
                          decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals` in a given coordinate system.

Parameters

| Name     | Type                      | Optional | Description        |
| -------- | ------------------------- | -------- | ------------------ |
| frame    | ksp::math::TransformFrame |          | Frame of reference |
| decimals | int                       |          | Number of decimals |


##### to_local

```rust
globalvelocity.to_local ( frame : ksp::math::TransformFrame ) -> ksp::math::Vec3
```

Get local velocity in a frame of reference

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_relative

```rust
globalvelocity.to_relative ( frame : ksp::math::TransformFrame ) -> ksp::math::GlobalVector
```

Get relative velocity to frame of reference

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
globalvelocity.to_string ( frame : ksp::math::TransformFrame ) -> string
```

Convert vector to string in a given coordinate system.

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


### Matrix2x2

A 2-dimensional matrix.

#### Fields

| Name        | Type                                                     | Read-only | Description               |
| ----------- | -------------------------------------------------------- | --------- | ------------------------- |
| a           | float                                                    | R/O       | a                         |
| b           | float                                                    | R/O       | b                         |
| c           | float                                                    | R/O       | c                         |
| d           | float                                                    | R/O       | d                         |
| determinant | float                                                    | R/O       | Get determinant of matrix |
| inverse     | [ksp::math::Matrix2x2](/reference/ksp/math.md#matrix2x2) | R/O       | Invert matrix             |


### TransformFrame

Representation of a coordinate frame of reference

#### Fields

| Name    | Type                                                           | Read-only | Description                              |
| ------- | -------------------------------------------------------------- | --------- | ---------------------------------------- |
| back    | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | backward vector of the coordinate system |
| down    | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | down vector of the coordinate system     |
| forward | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | forward vector of the coordinate system  |
| left    | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | left vector of the coordinate system     |
| right   | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | right vector of the coordinate system    |
| up      | [ksp::math::GlobalVector](/reference/ksp/math.md#globalvector) | R/O       | up vector of the coordinate system       |


#### Methods

##### to_local_position

```rust
transformframe.to_local_position ( position : ksp::math::GlobalPosition ) -> ksp::math::Vec3
```

Get local coordinates of a position

Parameters

| Name     | Type                      | Optional | Description           |
| -------- | ------------------------- | -------- | --------------------- |
| position | ksp::math::GlobalPosition |          | Position to transform |


##### to_local_vector

```rust
transformframe.to_local_vector ( vector : ksp::math::GlobalVector ) -> ksp::math::Vec3
```

Get local coordinates of a vector

Parameters

| Name   | Type                    | Optional | Description         |
| ------ | ----------------------- | -------- | ------------------- |
| vector | ksp::math::GlobalVector |          | Vector to transform |


##### to_local_velocity

```rust
transformframe.to_local_velocity ( velocity : ksp::math::GlobalVelocity ) -> ksp::math::Vec3
```

Get local coordinates of a velocity

Parameters

| Name     | Type                      | Optional | Description           |
| -------- | ------------------------- | -------- | --------------------- |
| velocity | ksp::math::GlobalVelocity |          | Velocity to transform |


### Vec2

A 2-dimensional vector.

#### Fields

| Name          | Type                                           | Read-only | Description                                 |
| ------------- | ---------------------------------------------- | --------- | ------------------------------------------- |
| magnitude     | float                                          | R/O       | Magnitude/length of the vector              |
| normalized    | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | R/O       | Normalized vector (i.e. scaled to length 1) |
| sqr_magnitude | float                                          | R/O       | Squared magnitude of the vector             |
| x             | float                                          | R/W       | x-coordinate                                |
| y             | float                                          | R/W       | y-coordinate                                |


#### Methods

##### angle_to

```rust
vec2.angle_to ( other : ksp::math::Vec2 ) -> float
```

Calculate the angle in degree to `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec2 |          | Other vector |


##### to_fixed

```rust
vec2.to_fixed ( decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals`.

Parameters

| Name     | Type | Optional | Description        |
| -------- | ---- | -------- | ------------------ |
| decimals | int  |          | Number of decimals |


##### to_string

```rust
vec2.to_string ( ) -> string
```

Convert the vector to string

### Vec3

A 3-dimensional vector.

#### Fields

| Name          | Type                                           | Read-only | Description                                 |
| ------------- | ---------------------------------------------- | --------- | ------------------------------------------- |
| magnitude     | float                                          | R/O       | Magnitude/length of the vector              |
| normalized    | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O       | Normalized vector (i.e. scaled to length 1) |
| sqr_magnitude | float                                          | R/O       | Squared magnitude of the vector             |
| x             | float                                          | R/W       | x-coordinate                                |
| xzy           | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | R/O       | Swapped y- and z-coordinate                 |
| y             | float                                          | R/W       | y-coordinate                                |
| z             | float                                          | R/W       | z-coordinate                                |


#### Methods

##### angle_to

```rust
vec3.angle_to ( other : ksp::math::Vec3 ) -> float
```

Calculate the angle in degree to `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### cross

```rust
vec3.cross ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Calculate the cross/other product with `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### distance_to

```rust
vec3.distance_to ( other : ksp::math::Vec3 ) -> float
```

Calculate the distance between this and `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### dot

```rust
vec3.dot ( other : ksp::math::Vec3 ) -> float
```

Calculate the dot/inner product with `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### exclude_from

```rust
vec3.exclude_from ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Exclude this from `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### lerp_to

```rust
vec3.lerp_to ( other : ksp::math::Vec3,
               t : float ) -> ksp::math::Vec3
```

Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.

Parameters

| Name  | Type            | Optional | Description                                |
| ----- | --------------- | -------- | ------------------------------------------ |
| other | ksp::math::Vec3 |          | Other vector                               |
| t     | float           |          | Relative position of mid-point (0.0 - 1.0) |


##### project_to

```rust
vec3.project_to ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Project this on `other` vector.

Parameters

| Name  | Type            | Optional | Description  |
| ----- | --------------- | -------- | ------------ |
| other | ksp::math::Vec3 |          | Other vector |


##### to_direction

```rust
vec3.to_direction ( ) -> ksp::math::Direction
```

Point in direction of this vector.

##### to_fixed

```rust
vec3.to_fixed ( decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals`.

Parameters

| Name     | Type | Optional | Description        |
| -------- | ---- | -------- | ------------------ |
| decimals | int  |          | Number of decimals |


##### to_global

```rust
vec3.to_global ( frame : ksp::math::TransformFrame ) -> ksp::math::GlobalVector
```

Associate this vector with a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_position

```rust
vec3.to_position ( frame : ksp::math::TransformFrame ) -> ksp::math::GlobalPosition
```

Consider this vector as position in a coordinate system

Parameters

| Name  | Type                      | Optional | Description        |
| ----- | ------------------------- | -------- | ------------------ |
| frame | ksp::math::TransformFrame |          | Frame of reference |


##### to_string

```rust
vec3.to_string ( ) -> string
```

Convert vector to string.

## Functions


### angle_axis

```rust
pub sync fn angle_axis ( angle : float,
                         axis : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction from a given axis with rotation angle in degree

Parameters

| Name  | Type            | Optional | Description |
| ----- | --------------- | -------- | ----------- |
| angle | float           |          |             |
| axis  | ksp::math::Vec3 |          |             |


### angle_delta

```rust
pub sync fn angle_delta ( a : float,
                          b : float ) -> float
```

Calculate the difference between two angles in degree (-180 .. 180)

Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| a    | float |          |             |
| b    | float |          |             |


### euler

```rust
pub sync fn euler ( x : float,
                    y : float,
                    z : float ) -> ksp::math::Direction
```

Create a Direction from euler angles in degree

Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| x    | float |          |             |
| y    | float |          |             |
| z    | float |          |             |


### from_vector_to_vector

```rust
pub sync fn from_vector_to_vector ( v1 : ksp::math::Vec3,
                                    v2 : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction to rotate from one vector to another

Parameters

| Name | Type            | Optional | Description |
| ---- | --------------- | -------- | ----------- |
| v1   | ksp::math::Vec3 |          |             |
| v2   | ksp::math::Vec3 |          |             |


### global_angle_axis

```rust
pub sync fn global_angle_axis ( angle : float,
                                axis : ksp::math::GlobalVector ) -> ksp::math::GlobalDirection
```

Create a Direction from a given axis with rotation angle in degree

Parameters

| Name  | Type                    | Optional | Description |
| ----- | ----------------------- | -------- | ----------- |
| angle | float                   |          |             |
| axis  | ksp::math::GlobalVector |          |             |


### global_euler

```rust
pub sync fn global_euler ( frame : ksp::math::TransformFrame,
                           x : float,
                           y : float,
                           z : float ) -> ksp::math::GlobalDirection
```

Create a Direction from euler angles in degree

Parameters

| Name  | Type                      | Optional | Description |
| ----- | ------------------------- | -------- | ----------- |
| frame | ksp::math::TransformFrame |          |             |
| x     | float                     |          |             |
| y     | float                     |          |             |
| z     | float                     |          |             |


### global_from_vector_to_vector

```rust
pub sync fn global_from_vector_to_vector ( v1 : ksp::math::GlobalVector,
                                           v2 : ksp::math::GlobalVector ) -> ksp::math::GlobalDirection
```

Create a Direction to rotate from one vector to another

Parameters

| Name | Type                    | Optional | Description |
| ---- | ----------------------- | -------- | ----------- |
| v1   | ksp::math::GlobalVector |          |             |
| v2   | ksp::math::GlobalVector |          |             |


### global_look_dir_up

```rust
pub sync fn global_look_dir_up ( lookDirection : ksp::math::GlobalVector,
                                 upDirection : ksp::math::GlobalVector ) -> ksp::math::GlobalDirection
```

Create a Direction from a fore-vector and an up-vector

Parameters

| Name          | Type                    | Optional | Description |
| ------------- | ----------------------- | -------- | ----------- |
| lookDirection | ksp::math::GlobalVector |          |             |
| upDirection   | ksp::math::GlobalVector |          |             |


### look_dir_up

```rust
pub sync fn look_dir_up ( lookDirection : ksp::math::Vec3,
                          upDirection : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction from a fore-vector and an up-vector

Parameters

| Name          | Type            | Optional | Description |
| ------------- | --------------- | -------- | ----------- |
| lookDirection | ksp::math::Vec3 |          |             |
| upDirection   | ksp::math::Vec3 |          |             |


### matrix2x2

```rust
pub sync fn matrix2x2 ( a : float,
                        b : float,
                        c : float,
                        d : float ) -> ksp::math::Matrix2x2
```

Create a new 2-dimensional matrix

Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| a    | float |          |             |
| b    | float |          |             |
| c    | float |          |             |
| d    | float |          |             |


### matrix4x4

```rust
pub sync fn matrix4x4 ( ) -> ksp::math::Matrix4x4
```

Create a new 4-dimensional matrix

### vec2

```rust
pub sync fn vec2 ( x : float,
                   y : float ) -> ksp::math::Vec2
```

Create a new 2-dimensional vector

Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| x    | float |          |             |
| y    | float |          |             |


### vec3

```rust
pub sync fn vec3 ( x : float,
                   y : float,
                   z : float ) -> ksp::math::Vec3
```

Create a new 3-dimensional vector

Parameters

| Name | Type  | Optional | Description |
| ---- | ----- | -------- | ----------- |
| x    | float |          |             |
| y    | float |          |             |
| z    | float |          |             |

