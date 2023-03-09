# Building

## Common

* A copy of the game is required (obviously)
* [BepInEx](https://github.com/BepInEx/BepInEx) and [SpaceWarp](https://github.com/SpaceWarpDev/SpaceWarp) v0.4.0 should be installed
* By default the game is expected to be installed in
  `C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program 2` if it is somewhere else you have to set a `KSP2_BASE_DIR` enviroment variable or provide it as a global property to msbuild

## Windows

* Install Visual Studio 2022 Community edition (or bigger)
  * Add Commandline Tools and support for `.NET 4.7.2` target

In the 'VS` commmand line or powershell

```
msbuild -t:build,test -restore -Property:Configuration=Release
```
should build everything to the `dist` folder

There are powershell helper script `build.ps1`  and `clean.ps1` to do this with slightly better control.
