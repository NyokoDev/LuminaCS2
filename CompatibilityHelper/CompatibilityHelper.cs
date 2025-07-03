using Game.Modding;
using Game.SceneFlow;
using System;

namespace Lumina
{
    internal static class CompatibilityHelper
    {
        public static bool CheckForIncompatibleMods(params string[] modNameFragments)
        {
            var modManager = GameManager.instance?.modManager;
            if (modManager == null)
            {
#if DEBUG
                Mod.Log.Info("ModManager instance is null.");
#endif
                return false;
            }

#if DEBUG
            Mod.Log.Info("=== All Mods in ModManager ===");
#endif
            foreach (var modInfo in modManager)
            {
                string name = modInfo.name ?? "(null)";
                string state = modInfo.state.ToString();
                bool isLoaded = modInfo.isLoaded;
                bool isValid = modInfo.isValid;
                string error = modInfo.loadError ?? "No error";

#if DEBUG
                Mod.Log.Info($"Mod Name: {name}");
                Mod.Log.Info($"  State: {state}");
                Mod.Log.Info($"  Loaded: {isLoaded}");
                Mod.Log.Info($"  Valid: {isValid}");
                Mod.Log.Info($"  Load Error: {error}");
#endif 

                // Check for incompatible fragments in mod name
                foreach (var fragment in modNameFragments)
                {
                    if (name.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
#if DEBUG
                        Mod.Log.Info($"Incompatible mod detected: {name}");
#endif
                        return true;
                    }
                }
            }

#if DEBUG
            Mod.Log.Info("=== End of All Mods ===");
#endif

            return false;
        }
    }
}
