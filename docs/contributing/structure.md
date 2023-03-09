# Project structure

The project is separeted in the follow sub-projects/assemblies

* `Parsing`
    * This is a pretty simple and generic parser combinator library (i.e. a library supporting the creation of aand kind of text-based parsers)
    * Indipendent to the game or the unity engine
* `Parsing-Test`
    * All the unit tests for the `Parsing` library
* `TO2`
    * The core of the TO2 language containing
      * Parser
      * Compiler
        * Which is done by parsing the script to an abstract syntax tree which then generates the IL-code via `System.Reflection.Emit`
      * Bindings to the `core` moddule
    * Has a dependeny to the `Parsing` library
    * Indipendent to the game or the unity engine
* `TO2-Test`
    * All the unit tests for the `TO2` library
* `KSP2Runtime`
    * The bindings to Kerbal Space Progam 2
      * This is the `ksp` module
    * Has a dependeny to the `TO2` library
    * Also contains the `std` module and example scrips written in TO2 itself
    * Only has dependencies to the game and the unity engine. Should remain indipendent to the modding framework
* `KSP2Runtime-Test`
    * All the unit tests for the `KSP2Runtime` library
* `SpaceWarpMod`
    * The `SpaceWarp` mod tying it all together
    * Has a dependeny to the `KSP2Runtime` library
    * ... will most likely be replaced once there is an official mod loader