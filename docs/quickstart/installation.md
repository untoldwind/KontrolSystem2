
# Installation

* Install [BepInEx](https://github.com/BepInEx/BepInEx) and [SpaceWarp](https://github.com/SpaceWarpDev/SpaceWarp) v0.4.0
  * ... or just SpaceWarp with bundled BepInEx
* Unpack the one of the [releases](https://github.com/untoldwind/KontrolSystem2/releases) in the same directory as the KSP2 executable
  * Directory should look like:
    ```
    KSP2_x64.exe
    BepInEx
    |- plugins
    |  |- KontrolSystem2
    |  |  |- KontrolSystemSpaceWarpMod.dll
    |  |  |- ...
    |  |- SpaceWarp
    |  |- ...
    |- ...
    ```

In flight there should now be an additional menu entry:

![Menu](menu1.png)

which should open the main dialog to start scripts:

![Dialog](dialog1.png)

The "Manage" button shows all errors if the system failed to reboot. If everything is ok it should be like this:

![Manager](manager1.png)

And there is a console where scripts can write helpful (or not so helpful) stuff to:

![Console](onsole1.png)
