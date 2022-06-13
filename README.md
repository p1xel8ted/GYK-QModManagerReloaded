# QMod Manager Reloaded (Harmony2)

Please ensure it's installed into "..\Graveyard Keeper\Graveyard Keeper_Data\Managed" directory and you've clicked the "Checklist" button at the top before asking for support.

**Important**

QMod Reloaded can permanently patch your ultrawide resolution in and get rid of intros, but if you want to try and maintain a cleaner game file, they're available as separate mods now.

Res: Now available as a standalone mod if you don't want to patch the file permanently. Do not use both...
Intros: Now available as a standalone mod if you don't want to patch the file permanently. Do not use both...

**Continuation of QMod Manager**

- Removed console app, entirely GUI-based.
- Can patch resolutions in.
- Installation and removal of mods via the GUI. Will create valid JSONS if one isn't found.
- Added patch to remove intros. This is permanent and will require a re-download of Assembly-CSharp.dll (and subsequent re-patching)
- Toggle mods on/off
- QOL features such as the direct opening of mod and game directory and log files.
- Can start the game via GUI. Launches via steam:// first, and then EXE directly if it fails.
- Implemented mod load order
- Doesn't rely on the correct entry point being entered in JSON to load. It will search DLLs directly.
- Backup/restore clean Assembly-CSharp.dll files.

# Mod Developers

.NET Framework 4.6 - 4.8
Harmony 2.2+ (installed by QMod Manager Reloaded - copy out of game directory into a lib folder and reference.)
Class name must be MainPatcher, and patch method must be called Patch to be picked up by the loader.

```c#
using HarmonyLib;
using Debug = UnityEngine.Debug;

namespace YourNameSpace
{
    public class MainPatcher
    {
        public static void Patch()
        {
            var harmony = new Harmony("your.entirely.100%.unique.id");
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);
        }
    }
}
```
Mod.json OR Info.json, but may as well do Mod.json as it will get converted anyway.

```json
{
  "Id": "MiscBitsAndBobs",
  "DisplayName": "Misc. Bits & Bobs",
  "Author": "p1xel8ted",
  "Version": "1.2",
  "Enable": true,
  "AssemblyName": "MiscBitsAndBobs.dll",
  "EntryMethod": "MiscBitsAndBobs.MainPatcher.Patch",
  "LoadOrder": 6
}
```
Config files, preferred is **Config.ini** - if you need a semi-decent INI handler that doesnt need external libraries, Config.cs and ConfigReader.cs can be taken from any of my mods.
