# ksp::math

Collection of KSP/Unity related mathematical functions.

## Types


### CoordinateSystem

Representation of a coordinate system

#### Fields

Name | Type | Description
--- | --- | ---
back | [ksp::math::Vector](/reference/ksp/math.md#vector) | backward vector of the coordinate system
down | [ksp::math::Vector](/reference/ksp/math.md#vector) | down vector of the coordinate system
forward | [ksp::math::Vector](/reference/ksp/math.md#vector) | forward vector of the coordinate system
left | [ksp::math::Vector](/reference/ksp/math.md#vector) | left vector of the coordinate system
right | [ksp::math::Vector](/reference/ksp/math.md#vector) | right vector of the coordinate system
up | [ksp::math::Vector](/reference/ksp/math.md#vector) | up vector of the coordinate system

#### Methods

##### to_local_direction

```rust
coordinatesystem.to_local_direction ( rotation : ksp::math::Rotation ) -> ksp::math::Direction
```

Get local direction of a rotation

##### to_local_position

```rust
coordinatesystem.to_local_position ( position : ksp::math::Position ) -> ksp::math::Vec3
```

Get local coordinates of a position

##### to_local_vector

```rust
coordinatesystem.to_local_vector ( position : ksp::math::Vector ) -> ksp::math::Vec3
```

Get local coordinates of a vector

### Direction

Represents the rotation from an initial coordinate system when looking down the z-axis and "up" being the y-axis

#### Fields

Name | Type | Description
--- | --- | ---
euler | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Euler angles in degree of the rotation
pitch | float | Pitch in degree
right_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Right vector of the rotation
roll | float | Roll in degree
up_vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Up vector of the rotation
vector | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Fore vector of the rotation (i.e. looking/facing direction
yaw | float | Yaw in degree

#### Methods

##### to_rotation

```rust
direction.to_rotation ( coordinate_system : ksp::math::CoordinateSystem ) -> ksp::math::Rotation
```

Associate this direction with a coordinate system

##### to_string

```rust
direction.to_string ( ) -> string
```

Convert the direction to string

### Matrix2x2

A 2-dimensional matrix.

#### Fields

Name | Type | Description
--- | --- | ---
a | float | a
b | float | b
c | float | c
d | float | d
determinant | float | Get determinant of matrix
inverse | [ksp::math::Matrix2x2](/reference/ksp/math.md#matrix2x2) | Invert matrix

### Position

A position in space. This is a 3-dimensional vector in a specific coordinate system

#### Methods

##### distance

```rust
position.distance ( other : ksp::math::Position ) -> float
```

Calculate the distance of `other` position.

##### distance_sqr

```rust
position.distance_sqr ( other : ksp::math::Position ) -> float
```

Calculate the squared distance of `other` position.

##### lerp_to

```rust
position.lerp_to ( other : ksp::math::Position,
                   t : float ) -> ksp::math::Position
```

Linear interpolate position between this and `other` position, where `t = 0.0` is this and `t = 1.0` is `other`.

##### to_local

```rust
position.to_local ( coordinate_system : ksp::math::CoordinateSystem ) -> ksp::math::Vec3
```

Get local vector in a coordinate system

### Vec2

A 2-dimensional vector.

#### Fields

Name | Type | Description
--- | --- | ---
magnitude | float | Magnitude/length of the vector
normalized | [ksp::math::Vec2](/reference/ksp/math.md#vec2) | Normalized vector (i.e. scaled to length 1)
sqr_magnitude | float | Squared magnitude of the vector
x | float | x-coordinate
y | float | y-coordinate

#### Methods

##### angle_to

```rust
vec2.angle_to ( other : ksp::math::Vec2 ) -> float
```

Calculate the angle in degree to `other` vector.

##### to_fixed

```rust
vec2.to_fixed ( decimals : int ) -> string
```

Convert the vector to string with fixed number of `decimals`.

##### to_string

```rust
vec2.to_string ( ) -> string
```

Convert the vector to string

### Vec3

A 3-dimensional vector.

#### Fields

Name | Type | Description
--- | --- | ---
magnitude | float | Magnitude/length of the vector
normalized | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Normalized vector (i.e. scaled to length 1)
sqr_magnitude | float | Squared magnitude of the vector
x | float | x-coordinate
xzy | [ksp::math::Vec3](/reference/ksp/math.md#vec3) | Swapped y- and z-coordinate
y | float | y-coordinate
z | float | z-coordinate

#### Methods

##### angle_to

```rust
vec3.angle_to ( other : ksp::math::Vec3 ) -> float
```

Calculate the angle in degree to `other` vector.

##### cross

```rust
vec3.cross ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Calculate the cross/other product with `other` vector.

##### distance_to

```rust
vec3.distance_to ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Calculate the distance between this and `other` vector.

##### dot

```rust
vec3.dot ( other : ksp::math::Vec3 ) -> float
```

Calculate the dot/inner product with `other` vector.

##### exclude_from

```rust
vec3.exclude_from ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Exclude this from `other` vector.

##### lerp_to

```rust
vec3.lerp_to ( other : ksp::math::Vec3,
               t : float ) -> ksp::math::Vec3
```

Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.

##### project_to

```rust
vec3.project_to ( other : ksp::math::Vec3 ) -> ksp::math::Vec3
```

Project this on `other` vector.

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

##### to_position

```rust
vec3.to_position ( coordinate_system : ksp::math::CoordinateSystem ) -> ksp::math::Position
```

Consider this vector as position in a coordinate system

##### to_string

```rust
vec3.to_string ( ) -> string
```

Convert vector to string.

##### to_vector

```rust
vec3.to_vector ( coordinate_system : ksp::math::CoordinateSystem ) -> ksp::math::Vector
```

Associate this vector with a coordinate system

### Vector

This is a 3-dimensional vector in a specific coordinate system

#### Fields

Name | Type | Description
--- | --- | ---
magnitude | float | Magnitude/length of the vector
normalized | [ksp::math::Vector](/reference/ksp/math.md#vector) | Normalized vector (i.e. scaled to length 1)
sqr_magnitude | float | Squared magnitude of the vector

#### Methods

##### cross

```rust
vector.cross ( other : ksp::math::Vector ) -> ksp::math::Vector
```

Calculate the cross/other product with `other` vector.

##### dot

```rust
vector.dot ( other : ksp::math::Vector ) -> float
```

Calculate the dot/inner product with `other` vector.

##### lerp_to

```rust
vector.lerp_to ( other : ksp::math::Vector,
                 t : float ) -> ksp::math::Vector
```

Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.

##### to_local

```rust
vector.to_local ( coordinate_system : ksp::math::CoordinateSystem ) -> ksp::math::Vec3
```

Get local vector in a coordinate system

## Functions


### angle_axis

```rust
pub sync fn angle_axis ( angle : float,
                         axis : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction from a given axis with rotation angle in degree

### angle_delta

```rust
pub sync fn angle_delta ( a : float,
                          b : float ) -> float
```

Calculate the difference between to angles in degree (-180 .. 180)

### euler

```rust
pub sync fn euler ( x : float,
                    y : float,
                    z : float ) -> ksp::math::Direction
```

Create a Direction from euler angles in degree

### from_vector_to_vector

```rust
pub sync fn from_vector_to_vector ( v1 : ksp::math::Vec3,
                                    v2 : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction to rotate from one vector to another

### look_dir_up

```rust
pub sync fn look_dir_up ( lookDirection : ksp::math::Vec3,
                          upDirection : ksp::math::Vec3 ) -> ksp::math::Direction
```

Create a Direction from a fore-vector and an up-vector

### matrix2x2

```rust
pub sync fn matrix2x2 ( a : float,
                        b : float,
                        c : float,
                        d : float ) -> ksp::math::Matrix2x2
```

Create a new 2-dimensional matrix

### vec2

```rust
pub sync fn vec2 ( x : float,
                   y : float ) -> ksp::math::Vec2
```

Create a new 2-dimensional vector

### vec3

```rust
pub sync fn vec3 ( x : float,
                   y : float,
                   z : float ) -> ksp::math::Vec3
```

Create a new 3-dimensional vector
