# core::error

Error reporting and error handling.


## Types


### Error

Error information of a failed Result.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
message | string | R/O | 

#### Methods

##### to_string

```rust
error.to_string ( ) -> string
```



### StackEntry

Stacktrace entry.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
arguments | string[] | R/O | 
function_name | string | R/O | 
line | int | R/O | 
source_name | string | R/O | 
