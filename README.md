# Kontrol System 2

A scripting framework for autopilots in Kerbal Space Programm 2.

This is the successor of [KontrolSystem](https://github.com/untoldwind/KontrolSystem) which in turn was (supposed to by) the spiritual offspring of the wonderful [KOS](https://github.com/KSP-KOS/KOS) mod.

## Installation

... (tbd)

## Building

Currently there is a hard requirment to [BepInEx](https://github.com/BepInEx/BepInEx) and [SpaceWarp](https://github.com/SpaceWarpDev/SpaceWarp). Both need to be installed to your local copy of KSP2.

If KSP2 is installed somewhere else than `C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program 2` you have to set a `KSP2_BASE_DIR` environment variable with the correct path (or somehow set this as a global property for `msbuild`).

Convenient build scripts: `build.ps1` (Windows), `build.sh` (Linux)

## Known issues

* The unit tests are currently not working on windows with a standard Visual Studio setup as `msbuild` uses some incompatible system libraries. They should run fine in a mono-setup
  * It probably requires migration to a different testing framework to fix this
