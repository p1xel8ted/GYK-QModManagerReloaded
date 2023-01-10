using System;
using System.Linq;
using System.Reflection;

namespace QModReloadedGUI
{
    internal class Loader : MarshalByRefObject
    {
        public void Load(FrmMain form, string file)
        {
            form.ModDomainLoaded = true;
            var modAssembly = Assembly.LoadFrom(file);
            foreach (var type in modAssembly.GetTypes().Where(t => t.FullName!.EndsWith("Config")))
            {
                var staticMethodInfo = type.GetMethod("GetOptions");
                if (staticMethodInfo == null)
                {
                    form.WriteLog(
                        $"Unable to find GetOptions method in {type.FullName}. Please run the game to generate a config or consult the author.",
                        true);
                    continue;
                }

                if (staticMethodInfo.GetParameters().Length == 1)
                {
                    staticMethodInfo.Invoke(null, new object[] {true});
                }
                else
                {
                    form.WriteLog(
                         $"GetOptions method in {type.FullName} doesn't have the correct amount of parameters. Please check for mod updates before contacting the author.",
                        true);
                }
            }
        }
    }
}