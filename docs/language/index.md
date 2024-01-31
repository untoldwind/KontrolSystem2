
# The TO2 language

TO2 should be read as "twelve-O-2" resp `1202`. If you have some historic knowledge about the first moon landing you probably get the reference.

Core features:

* TO2 is strictly typed (as you do not want a typo to mess up your expensive rocket)
* It is also functional by nature, which is a nice thing to have these days
* When rebooting the Kontrol-System every script is recompiled
  * ... to IL code for <s>maximum</s> acceptable performance
* By default every function is asynchronous so that it can run as a Unity-coroutine
  * ... i.e. a running script does not block the main thread of the game (which would be rather bad)

```{toctree}
:maxdepth: 2

conditionals
entrypoint
function
loops
modules
types
operators
special_types/index
structs
examples/index
```
