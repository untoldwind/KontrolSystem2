---
title: "core::math"
---

Collection of basic mathematical functions.

# Types


## Random

Random number generator

### Methods

#### next_float

```rust
random.next_float ( ) -> float
```

Get next random number between 0.0 and 1.0

#### next_gaussian

```rust
random.next_gaussian ( mu : float,
                       sigma : float ) -> float
```

Get next gaussian distributed random number

#### next_int

```rust
random.next_int ( min : int,
                  max : int ) -> int
```

Get next random number between `min` and `max`

# Constants

Name | Type | Description
--- | --- | ---
DEG_TO_RAD | float | Multiplicator to convert an angle of degree to radian.
E | float | Represents the natural logarithmic base, specified by the e constant,
EPSILON | float | Machine epsilon, i.e lowest possible resolution of a floating point number.
MAX_FLOAT | float | Maximum possible floating point number.
MAX_INT | int | Maximum possible integer number.
MIN_FLOAT | float | Minimum possible floating point number.
MIN_INT | int | Minimum possible integer number.
PI | float | Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
RAD_TO_DEG | float | Multiplicator to convert an angle of radian to degree.


# Functions


## abs

```rust
pub sync fn abs ( value : float ) -> float
```

Returns the absolute value of a number.

## acos

```rust
pub sync fn acos ( d : float ) -> float
```

Returns the angle in radian whose cosine is the specified number.

## acos_deg

```rust
pub sync fn acos_deg ( x : float ) -> float
```

Returns the angle in degree whose cosine is the specified number.

## acosh

```rust
pub sync fn acosh ( x : float ) -> float
```

Returns the angle whose hyperbolic cosine is the specified number.

## asin

```rust
pub sync fn asin ( d : float ) -> float
```

Returns the angle in radian whose sine is the specified number.

## asin_deg

```rust
pub sync fn asin_deg ( x : float ) -> float
```

Returns the angle in degree whose sine is the specified number.

## asinh

```rust
pub sync fn asinh ( x : float ) -> float
```

Returns the angle whose hyperbolic sine is the specified number.

## atan

```rust
pub sync fn atan ( d : float ) -> float
```

Returns the angle in radian whose tanget is the specified number.

## atan_deg

```rust
pub sync fn atan_deg ( x : float ) -> float
```

Returns the angle in degree whose tangent is the specified number.

## atan2

```rust
pub sync fn atan2 ( y : float,
                    x : float ) -> float
```

Returns the angle in redian whose tangent is the quotient of two specified numbers.

## atan2_deg

```rust
pub sync fn atan2_deg ( y : float,
                        x : float ) -> float
```

Returns the angle in degree whose tangent is the quotient of two specified numbers.

## atanh

```rust
pub sync fn atanh ( x : float ) -> float
```

Returns the angle whose hyperbolic tanget is the specified number.

## ceiling

```rust
pub sync fn ceiling ( a : float ) -> float
```

Returns the smallest integral value that is greater than or equal to the specified number.

## clamp

```rust
pub sync fn clamp ( x : float,
                    min : float,
                    max : float ) -> float
```

Clamp a number between a given minimum and maximum

## clamp_degrees180

```rust
pub sync fn clamp_degrees180 ( angle : float ) -> float
```

Clamp an angle between -180 and 180 degree

## clamp_degrees360

```rust
pub sync fn clamp_degrees360 ( angle : float ) -> float
```

Clamp an angle between 0 and 360 degree

## clamp_radians_pi

```rust
pub sync fn clamp_radians_pi ( angle : float ) -> float
```

Clamp an angle between -π and π

## clamp_radians2_pi

```rust
pub sync fn clamp_radians2_pi ( angle : float ) -> float
```

Clamp an angle between 0 and 2π

## cos

```rust
pub sync fn cos ( d : float ) -> float
```

Returns the cosine of the specified angle in redian.

## cos_deg

```rust
pub sync fn cos_deg ( x : float ) -> float
```

Returns the cosine of the specified angle in degree.

## cosh

```rust
pub sync fn cosh ( value : float ) -> float
```

Returns the hyperbolic cosine of the specified angle.

## exp

```rust
pub sync fn exp ( d : float ) -> float
```

Returns e raised to the specified power.

## floor

```rust
pub sync fn floor ( d : float ) -> float
```

Returns the largest integral value less than or equal to the specified number.

## log

```rust
pub sync fn log ( d : float ) -> float
```

Returns the natural (base e) logarithm of a specified number.

## log10

```rust
pub sync fn log10 ( d : float ) -> float
```

Returns the base 10 logarithm of a specified number.

## max

```rust
pub sync fn max ( val1 : float,
                  val2 : float ) -> float
```

Returns the larger of two decimal numbers.

## min

```rust
pub sync fn min ( val1 : float,
                  val2 : float ) -> float
```

Returns the smaller of two decimal numbers.

## pow

```rust
pub sync fn pow ( x : float,
                  y : float ) -> float
```

Returns a specified number raised to the specified power.

## random

```rust
pub sync fn random ( ) -> core::math::Random
```

New random number generator

## random_from_seed

```rust
pub sync fn random_from_seed ( seed : int ) -> core::math::Random
```

New random number generator from given seed

## round

```rust
pub sync fn round ( a : float ) -> float
```

Rounds a decimal value to the nearest integral value, and rounds midpoint values to the nearest even number.

## sin

```rust
pub sync fn sin ( a : float ) -> float
```

Returns the sine of the specified angle in redian.

## sin_deg

```rust
pub sync fn sin_deg ( x : float ) -> float
```

Returns the sine of the specified angle in degree.

## sinh

```rust
pub sync fn sinh ( value : float ) -> float
```

Returns the hyperbolic sine of the specified angle.

## sqrt

```rust
pub sync fn sqrt ( d : float ) -> float
```

Returns the square root of a specified number.

## tan

```rust
pub sync fn tan ( a : float ) -> float
```

Returns the sine of the specified angle in redian.

## tan_deg

```rust
pub sync fn tan_deg ( x : float ) -> float
```

Returns the sine of the specified angle in degree.

## tanh

```rust
pub sync fn tanh ( value : float ) -> float
```

Returns the hyperbolic tangent of the specified angle.

## truncate

```rust
pub sync fn truncate ( d : float ) -> float
```

Calculates the integral part of a specified number.
