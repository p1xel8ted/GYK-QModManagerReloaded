using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QModReloaded;

public class QModLoader
{
    private static readonly string QModBaseDir = Environment.CurrentDirectory + "\\QMods";
    private static readonly string DisableMods = QModBaseDir + "\\disable";

    public static void Patch()
    {
        LoadHelper();
        Logger.WriteLog("Assembly-CSharp.dll has been patched, (otherwise you wouldn't see this message.");
        if (File.Exists(DisableMods))
        {
            Logger.WriteLog("Game has been launched via Load Modless. Disabling mods.");
            return;
        }
        Logger.WriteLog("Patch method called. Attempting to load mods.");
        var dllFiles =
            Directory.EnumerateDirectories(QModBaseDir).SelectMany(
                directory => Directory.EnumerateFiles(directory, "*.dll"));

        var mods = new List<QMod>();
        foreach (var dllFile in dllFiles)
        {
            var directoryName = new FileInfo(dllFile).DirectoryName;
            var jsonPath = Path.Combine(directoryName!, "mod.json");
            if (!new FileInfo(jsonPath).Exists)
            {
                var created = CreateJson(dllFile);
                if (created)
                {
                    Logger.WriteLog(
                        $"No mod.json found for {dllFile}. Have created one, attempting to load. Note if I couldn't automatically determine the entry method, a best guess is used.");
                    jsonPath = Path.Combine(directoryName, "mod.json");
                }
                else
                {
                    Logger.WriteLog(
                        $"No mod.json found for {dllFile}. Failed to create one automatically. Usually indicates an issue with the DLL.", true);
                    continue;
                }
            }

            var modToAdd = QMod.FromJsonFile(jsonPath);

            modToAdd.LoadedAssembly = Assembly.LoadFrom(dllFile);
            modToAdd.ModAssemblyPath = dllFile;
            mods.Add(modToAdd);
        }



        mods.Sort((m1, m2) => m1.LoadOrder.CompareTo(m2.LoadOrder));

        foreach (var mod in mods)
        {
            if (mod.Description.Contains("QModHelper"))
            {
                mod.Enable = true;
            }
            if (mod.Enable)
                LoadMod(mod);
            else
                Logger.WriteLog($"{mod.DisplayName} has been disabled in config. Skipping.");
        }
    }

    private static bool CreateJson(string file)
    {
        var sFile = new FileInfo(file);
        var path = new FileInfo(file).DirectoryName;
        var fileNameWithoutExt = sFile.Name.Substring(0, sFile.Name.Length - 4);
        var (namesp, type, method, found) = GetModEntryPoint(file);

        var modInfo = FileVersionInfo.GetVersionInfo(path!);

        var newMod = new QMod
        {
            DisplayName = modInfo.ProductName,
            Enable = true,
            ModAssemblyPath = path,
            Description = modInfo.FileDescription,
            AssemblyName = AssemblyName.GetAssemblyName(sFile.FullName).Name,
            Author = modInfo.CompanyName,
            NexusId = -1,
            Id = fileNameWithoutExt,
            EntryMethod = found ? $"{namesp}.{type}.{method}" : $"Couldn't find a PatchAll. Not a valid mod.",
            Version = modInfo.ProductVersion
        };
        newMod.SaveJson();
        var files = new FileInfo(Path.Combine(path, "mod.json"));
        return files.Exists;
    }

    private static void LoadHelper()
    {
        try
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Graveyard Keeper_Data\\Managed\\Helper.dll");
            var assembly = Assembly.LoadFile(path);
            var m = GetModEntryPoint(path);
            var methodToLoad = assembly.GetType(m.namesp + "." + m.type).GetMethod(m.method);
            methodToLoad?.Invoke(m, Array.Empty<object>());
            Logger.WriteLog("Successfully invoked QMod Helper entry method.");
        }
        catch (FileNotFoundException ex)
        {
            Logger.WriteLog($"Could not find QMod Helper. {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.WriteLog($"Error invoking QMod Helper. {ex.Message}");
        }
    }

    private static (string namesp, string type, string method, bool found) GetModEntryPoint(string mod)
    {
        try
        {
            var modAssembly = AssemblyDefinition.ReadAssembly(mod);

            var toInspect = modAssembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                    .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            // toInspect = toInspect.Where(x => x.m.Name is "Patch");

            foreach (var method in toInspect)
                if (method.m.Body.Instructions.Where(instruction => instruction.Operand != null)
                    .Any(instruction => instruction.Operand.ToString().Contains("PatchAll")))
                    return (method.t.Namespace, method.t.Name, method.m.Name, true);
        }
        catch (Exception ex)
        {
            Logger.WriteLog($"GetModEntryPoint(): Error, {ex.Message}", true);
        }

        return (null, null, null, false);
    }

    private static bool IsModCompatible(string mod)
    {
        try
        {
            var modAssembly = AssemblyDefinition.ReadAssembly(mod);

            var toInspect = modAssembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                    .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            // toInspect = toInspect.Where(x => x.m.Name is "Patch");

            if (toInspect.Any(method => method.m.Body.Instructions.Where(instruction => instruction.Operand != null)
                    .Any(instruction => instruction.OpCode == OpCodes.Newobj && instruction.Operand.ToString().Contains("HarmonyLib.Harmony"))))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLog($"GetModEntryPoint(): Error, {ex.Message}", true);
        }

        return false;
    }

    private static void LoadMod(QMod mod)
    {
        try
        {
            MethodInfo methodToLoad;
            var jsonEntrySplit = mod.EntryMethod.Split('.');
            var m = GetModEntryPoint(mod.ModAssemblyPath);
            if (!IsModCompatible(mod.ModAssemblyPath))
            {
                Logger.WriteLog($"{mod.Id} is not Harmony2 enabled, and as such, is not compatible.", true);
                return;
            }
            var jsonEntry = $"{jsonEntrySplit[0]}.{jsonEntrySplit[1]}.{jsonEntrySplit[2]}";
            var foundEntry = $"{m.namesp}.{m.type}.{m.method}";

            if (!jsonEntry.Equals(foundEntry, StringComparison.Ordinal))
            {
                Logger.WriteLog(
                    $"Found entry point in {mod.AssemblyName} does not match what's in the JSON. Ignoring JSON and loading found entry method.");
                methodToLoad = mod.LoadedAssembly.GetType(m.namesp + "." + m.type).GetMethod(m.method);
            }
            else
            {
                methodToLoad = mod.LoadedAssembly.GetType($"{jsonEntrySplit[0]}.{jsonEntrySplit[1]}")
                    .GetMethod(jsonEntrySplit[2]);
            }

            methodToLoad?.Invoke(m, Array.Empty<object>());
            Logger.WriteLog($"Load order: {mod.LoadOrder}, successfully invoked {mod.DisplayName} entry method.");
        }
        catch (TargetInvocationException)
        {
            Logger.WriteLog($"Invoking the specified EntryMethod {mod.EntryMethod} failed for {mod.Id}. Is the mod Harmony2.0 compatible?", true);
        }
        catch (NullReferenceException nullEx)
        {
            Logger.WriteLog(nullEx.Message, true);
        }
        catch (Exception finalEx)
        {
            Logger.WriteLog($"LoadMod():{finalEx.Message}, Source: {finalEx.Source}, Trace: {finalEx.StackTrace}", true);
        }
    }
}