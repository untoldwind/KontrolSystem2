# ksp::testing



## Constants

Name | Type | Description
--- | --- | ---
IDENTITY_COORDINATE_SYSTEM | ksp::math::CoordinateSystem | 


## Functions


### assert_false

```rust
pub sync fn assert_false ( actual : bool ) -> Unit
```

Assert that `actual` is false (Test only)


### assert_float

```rust
pub sync fn assert_float ( expected : float,
                           actual : float,
                           delta : float ) -> Unit
```

Assert that `actual` float is almost equal to `expected` with an absolute tolerance of `delta` (Test only)


### assert_int

```rust
pub sync fn assert_int ( expected : int,
                         actual : int ) -> Unit
```

Assert that `actual` integer is equal to `expected` (Test only)


### assert_none

```rust
pub sync fn assert_none ( actual : Option<T> ) -> Unit
```



### assert_some

```rust
pub sync fn assert_some ( expected : T,
                          actual : Option<T> ) -> Unit
```



### assert_string

```rust
pub sync fn assert_string ( expected : string,
                            actual : string ) -> Unit
```

Assert that `actual` string is equal to `expected` (Test only)


### assert_true

```rust
pub sync fn assert_true ( actual : bool ) -> Unit
```

Assert that `actual` is true (Test only)


### assert_vec2

```rust
pub sync fn assert_vec2 ( expected : ksp::math::Vec2,
                          actual : ksp::math::Vec2,
                          delta : float ) -> Unit
```



### assert_vec3

```rust
pub sync fn assert_vec3 ( expected : ksp::math::Vec3,
                          actual : ksp::math::Vec3,
                          delta : float ) -> Unit
```



### assert_yield

```rust
pub sync fn assert_yield ( expected : int ) -> Unit
```

Assert that test case has yielded `expected` number of times already (Async test only)


### fail_test

```rust
pub sync fn fail_test ( message : string ) -> Unit
```

Fail the test case with a `message` (Test only).


### test_sleep

```rust
pub sync fn test_sleep ( millis : int ) -> Unit
```

Suspend execution for `millis`


### yield

```rust
pub fn yield ( ) -> Unit
```

Yield the test case (Async test only)

