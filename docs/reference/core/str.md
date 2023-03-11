# core::str

Provided basic string manipulation and formatting.


## Functions


### format

```rust
pub sync fn format ( format : string,
                     items : T ) -> string
```

Format items using C# format strings (https://learn.microsoft.com/en-us/dotnet/api/system.string.format). Items can be either a single value, an array or a tuple.


### join

```rust
pub sync fn join ( separator : string,
                   items : string[] ) -> string
```

Join an array of string with a separator.

