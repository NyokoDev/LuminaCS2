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
            public static string assemblyDirectory = Path.Combine(localLowDirectory, "Colossal Order", "Cities Skylines II", "ModsData", "Lumina");
            public static string luminaPresetsDirectory = Path.Combine(assemblyDirectory, "LuminaPresets");
            private static string assemblyPDXDirectory = Path.Combine(localLowDirectory, "Colossal Order", "Cities Skylines II", "ModsData", "Lumina");
            private static string settingsFilePath = Path.Combine(assemblyDirectory, "Lumina.xml");
            public static string GlobalModSavingPath = Path.Combine(assemblyDirectory, "Lumina.xml");
            public static string GlobalPDXModSavingPath = Path.Combine(assemblyPDXDirectory, "Lumina.xml");
            public static string Version = "1.5.2";
    }
    }

