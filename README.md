# QMod Manager Reloaded (Harmony2)

Please ensure it's installed into "..\Graveyard Keeper\Graveyard Keeper_Data\Managed" directory and you've clicked the "Checklist" button at the top before asking for support.

**Important**

QMod Reloaded can permanently patch your ultrawide resolution in and get rid of intros, but if you want to try and maintain a cleaner game file, they're available as separate mods now.

Resolution patch: Now available as a standalone mod if you don't want to patch the file permanently. 
https://www.nexusmods.com/graveyardkeeper/mods/45

No Intros: Now available as a standalone mod if you don't want to patch the file permanently.
https://www.nexusmods.com/graveyardkeeper/mods/47

**Continuation of QMod Manager**

- Removed console app, entirely GUI-based.
- Mod.json files are generated entirely by QMR. Information is taken from the DLL AssemblyInfo directly, and the entry point is auto-detected.
- Can patch resolutions in, replaces the 2560x1440 option. Designed for ultra-wide users.
- Installation and removal of mods via the GUI. Will create valid JSONS if one isn't found.
- Added patch to remove intros. This is permanent and will require a re-download of Assembly-CSharp.dll (and subsequent re-patching).
- Toggle mods on/off.
- QOL features such as the direct opening of mod and game directory and log files.
- Editing of mod configs within the application.
- Can start the game via GUI. Trys to detect if you're using Steam/GOG or standalone and launch accordingly.
- Implemented mod load order (drag and drop the list).
- Doesn't rely on the correct entry point being entered in JSON to load. It will search DLLs directly for .PatchAll and run from there.
- Backup/restore clean Assembly-CSharp.dll files. GOG and Steam as of 1.405. 


![alt text](https://github.com/p1xel8ted/GYK-QModManagerReloaded/blob/44c7f0b61b33de21a64b2ad7b12dd0aac2b3021e/main_ui_new.png)


# Mod Developers

.NET Framework 4.6 - 4.8
Harmony 2.2+ (installed by QMod Manager Reloaded - copy out of game directory into a lib folder and reference.)
The loader will find the entry method itself by searching for .PatchAll - so don't deviate from the patching process or the mod wont load. For the sake of
keeping everything the same across mods, I suggest using the class and method names below.

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
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
```
Ensure you're making use of AssemblyInfo in its entirety and QMod Manager Reloaded will generate a proper mod.json file for you. Even if you write one yourself, the information will overwritten with what it finds inside the DLL.
```c#
//json Id = the dll filename
[assembly: AssemblyTitle("Misc. Bits & Bobs")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("p1xel8ted")] //json Author
[assembly: AssemblyProduct("Misc. Bits & Bobs")] //json DisplayName
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.3.0.0")]
[assembly: AssemblyFileVersion("1.3.0.0")] //json Version
[assembly: NeutralResourcesLanguage("en")]
```
The output of above, looks like:
```json
{
  "Id": "MiscBitsAndBobs",
  "DisplayName": "Misc. Bits & Bobs",
  "Author": "p1xel8ted",
  "Version": "1.3",
  "Enable": true,
  "AssemblyName": "MiscBitsAndBobs.dll",
  "EntryMethod": "MiscBitsAndBobs.MainPatcher.Patch",
  "LoadOrder": 6
}
```
Config files, preferred is **Config.ini** - if you need a semi-decent INI handler that doesnt need external libraries, Config.cs and ConfigReader.cs can be taken from any of my mods.
