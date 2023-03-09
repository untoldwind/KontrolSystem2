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

## Linux

Note: Running the game on linux is officially not supported.

If you have installed the game via steam/proton you should set an environment variable:

```
export KSP2_BASE_DIR=$HOME/.local/share/Steam/steamapps/common/Kerbal\ Space\ Program\ 2
```

* Install mono and msbuild for mono.
  * Package names will vary on distribution. For ArchLinux based distributions this would be `mono` and `mono-msbuild`

```
msbuild -t:build,test -restore -Property:Configuration=Release
```
should build everything to the `dist` folder

There are some helper scripts `build.sh` and `clean.sh` to do this with slightly better control.

## IDE

Depending on your IDE you might have to set a global msbuild property `KSP2_BASE_DIR`. Otherwise I had no problem importing the solution to VS Code 2022 or Rider.
