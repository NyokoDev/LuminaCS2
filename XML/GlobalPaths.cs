using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.XML
{
        public class GlobalPaths
        {
            private static string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            private static string localLowDirectory = Path.Combine(localAppDataDirectory, "..", "LocalLow");
            private static string assemblyDirectory = Path.Combine(localLowDirectory, "Colossal Order", "Cities Skylines II", "Mods", "Lumina");
            private static string settingsFilePath = Path.Combine(assemblyDirectory, "Lumina.xml");
            public static string GlobalModSavingPath = Path.Combine(assemblyDirectory, "Lumina.xml");
        }
    }

