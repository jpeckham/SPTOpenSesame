# OpenSesame — local SPT 5.0 / EFT 48146 port

This branch ports dwesterwick's OpenSesame from upstream commit `86e785d60e636adb08f34ca9366baef616f70e91`. No public SPT 5.0 port was found in the author's branches, PRs, or three listed forks when checked on October 28–29, 2026. This is an independent experimental port, not an upstream release or a claim of compatibility with every SPT 5.0 build.

Fork: https://github.com/jpeckham/SPTOpenSesame/tree/spt-5.0-48146

## Build and install

Requires .NET 10 SDK and an initialized SPT 5.0 installation with BepInXDX/IL2CPP and generated `BepInEx/interop` assemblies for EFT **1.2.5.0.48146**. The installation must already have launched successfully. References come from that installation; no game assemblies are distributed here.

```powershell
dotnet build SPTOpenSesame.csproj -c Release '-p:SPTPath=C:\sptarkov\SPTarkov 5.0.0-local'
```

With the client closed, copy `Build/BepInEx/plugins/OpenSesame/SPTOpenSesame.dll` to the same relative folder in your SPT installation. Remove any older OpenSesame DLL elsewhere in `BepInEx/plugins` to avoid duplicate plugins. To uninstall, close the client and remove this DLL.

The port changes the project from .NET Framework 4.7.1/BepInEx 5 to .NET 10/BepInEx 6 IL2CPP, uses `SPTushonka.Reflection.Patching`, registers the power-switch Unity component with an `IntPtr` constructor, replaces the native singleton of a managed logger with a managed singleton, uses native interaction lists and native loot-container casts, and inserts translations into `EFT.Locale` individually. Action lists are initialized even when Do Nothing is disabled or an existing state has a null list. Original resources, interaction targets, and power-switch IDs are retained. Version **4.0.0** identifies this fork's port.

Settings are in `BepInEx/config/com.danw.opensesame.cfg` (also accessible through F12): `Enabled Features = All`, `Enabled Debug Messages = 0`. Open Sesame, Turn On Power, and the default Do Nothing action are enabled. Upstream's switch-specific feature-gating behavior is retained; use the default All setting for initial testing.

## Validation and limits

Built successfully with 0 errors and 9 CS8981 warnings from upstream's lowercase generated resource class names. Client startup loaded all four patches and registered the Unity component. A temporary runtime test plugin passed checks for all ten resource locales, unknown-locale English fallback, null interaction state/list initialization, the Do Nothing toggle, and invoking a native action callback after managed garbage collection. The test DLL was removed afterward.

To reproduce the runtime checks, build `tests/RuntimeTests.csproj` with the same `SPTPath`, temporarily install only its `OpenSesame.RuntimeTests.dll` alongside the production DLL, launch a disposable test profile, and look for `OpenSesame Runtime Tests` / `PASS:` in `BepInEx/LogOutput.log`. The harness briefly changes the feature setting and restores its original value in `finally`; do not use it during normal gameplay. Close the game and remove the test DLL after checking. This is an in-client test, not a `dotnet test` project.

**Not yet verified:** opening an actual locked door/keycard door/container in a raid, power-switch discovery/use, D2 behavior, and changing languages through the game's UI. Startup and helper tests do not establish those gameplay results. Test those interactions before treating the port as validated for gameplay.

## Original upstream documentation

A tool for developers (and cheaters) to open locked doors, safes, etc. without requiring the key.

This mod will add a new action called "Open Sesame" to their context menus to allow you to open them without requiring the key.

It also adds a "Turn On Power" action to doors that require the map's power switch to be turned on before they can be unlocked.

To prevent you from accidentally unlocking things, a "Do Nothing" action is added first (so it's the default action). This can be disabled in the Configuration Manager.

You can enable options in the Configuration Manager to write debug messages when the context menu opens or when you select the "Open Sesame" or "Turn on Power" actions. Enabling these will allow you to see the door ID (when the context menu opens), key ID (when you unlock a door via the "Open Sesame" action), or switch ID (when you turn on the power switch).

You can also prevent this mod from adding actions to context menus via a Configuration Manager option.

I wrote this to be agnostic of the SPT/EFT version, so you should rarely (if ever) have to update it.

Translations for the new context-menu actions exist for:
* English
* Chinese
* French
* German
* Korean
* Portuguese
* Russian
* Spanish

If you would like to help me include other languages, please post a comment with the locale ID in *Aki_Data\Server\database\locales\global* and translations for:
* "Do Nothing"
* "Open Sesame"
* "Turn On Power"

Known Issues:
* Cannot directly open the saferoom door in Interchange. You can only open it via the keypad in the Burger Spot restroom. 