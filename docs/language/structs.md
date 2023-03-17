# Structs

Sometime it is necessary to have a complex state that is shared between multiple functions and using a [`Cell`](special_types/cell.md) is just to cumbersome. In that case it is possible to define a `struct` (in object-oriented terms this would be called a class).

```rust
struct NameOfStruct(param1: paramType1, param2: paramType2) {
    field1 : fieldType1 = initial_value
    field2 : fieldType1 = initial_value
}
```
will implicitly create a `NameOfStruct(param1: paramType1, param2: paramType2)` function creating a new instance. To make this available to other modules this can be prefixed with `pub`.

Example: 
```rust
struct Counter(initial_name: string) {
    name  : string = initial_name
    count : int   = 0
}
```
then a new `Counter` struct can be created with
```rust
let my_counter = Counter("My counter")

CONSOLE.print_line("Name : " + my_counter.name)              // Will be "Name : My counter"
CONSOLE.print_line("Count: " + my_counter.count.to_string)   // Will be "Count: 0"
```

It is possible to define methods to operate on that struct:

```rust
impl NameOfStruct {
  fn method_name(self, param1: type1, param2: type1) -> return_type = {
     ... implementation where "self" is a reference to the struct itself
  }
}
```

In the example above
```rust
impl Counter {
    fn to_string(self) -> string = {
        "Counter(" + self.name + ", " + self.count.to_string() + ")"
    }
    
    fn inc(self) -> Unit = {
        self.count = self.count + 1
    }
}
```
then the following can be done:
```rust
let my_counter = Counter("My counter")

CONSOLE.print_line(my_counter.to_string())   // Will be "Counter(My counter, 0)"

my_counter.inc()

CONSOLE.print_line(my_counter.to_string())   // Will be "Counter(My counter, 1)"
```