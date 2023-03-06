# ksp::math

Collection of KSP/Unity related mathematical functions.

## Types


### Direction

Represents the rotation from an initial coordinate system when looking down the z-axis and "up" being the y-axis

#### Fields

Name | Type | Description
--- | --- | ---
euler | ksp::math::Vec3 | Euler angles in degree of the rotation
pitch | float | Pitch in degree
right_vector | ksp::math::Vec3 | Right vector of the rotation
roll | float | Roll in degree
up_vector | ksp::math::Vec3 | Up vector of the rotation
vector | ksp::math::Vec3 | Fore vector of the rotation (i.e. looking/facing direction
yaw | float | Yaw in degree

#### Methods

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
inverse | ksp::math::Matrix2x2 | Invert matrix

### Vec2

A 2-dimensional vector.

#### Fields

Name | Type | Description
--- | --- | ---
magnitude | float | Magnitude/length of the vector
normalized | ksp::math::Vec2 | Normalized vector (i.e. scaled to length 1)
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
normalized | ksp::math::Vec3 | Normalized vector (i.e. scaled to length 1)
sqr_magnitude | float | Squared magnitude of the vector
x | float | x-coordinate
xzy | ksp::math::Vec3 | Swapped y- and z-coordinate
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
