# Asynchronous execution

By default all scripts are running asynchronously in the game, so in many cases triggering an action will not produce an immediate change.

Instead you should use one of the following functions in [`ksp::game`](../reference/ksp/game.md) to give the game a little time to catch up.

## Yield

The `yield()` function is the most basic form ot "interruption", it will pause the script for the shortest amount possible and wait for the next update loop of the game.

Essentially it is a `sleep(0)` (see below).

## Sleep

The `sleep(seconds)` function will pause the script for a given amount of seconds. 

Due to the nature of the game loop this is just a rough estimate. If you need a precise amount of seconds passed for some calculation you should recheck the `current_time()`. 

## Wait while/until

`wait_while(condition)` pauses the script while a given condition is true and correspondingly `wait_until(condition)` pauses while the condition is false (until it becomes true).

In both cases `condition` is a function that returns a boolean (`fn() -> bool`). So a `sleep(12.34)` could be also written as:
```
const ut = current_time()

wait_while(fn() -> current_time() - ut < 12.34)
```
or
```
const ut = current_time()

wait_until(fn() -> current_time() - ut >= 12.34)
```

Note that the `condition` is checked on every game loop, so it should be as simple as possible. Doing a complex calculation here can have a serious impact on the game.
