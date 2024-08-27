// <copyright file="GlobalPaths.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina.XML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// GlobalPaths class. This class is a utility for common paths.
    /// </summary>
    public class GlobalPaths
    {
        private static string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string LocalLowDirectory = Path.Combine(localAppDataDirectory, "..", "LocalLow");

        /// <summary>
        /// AssemblyDirectory, returns mod path to Local App Data.
        /// </summary>
        public static string AssemblyDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", "ModsData", "Lumina");

        /// <summary>
        /// LuminaPresetsDirectory. Returns presets directory.
        /// </summary>
        public static string LuminaPresetsDirectory = Path.Combine(AssemblyDirectory, "LuminaPresets");

        /// <summary>
        /// Returns LUTS directory.
        /// </summary>
        public static string LuminaLUTSDirectory = Path.Combine(AssemblyDirectory, "LUTS");

        private static string assemblyPDXDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", "ModsData", "Lumina");
        private static string settingsFilePath = Path.Combine(AssemblyDirectory, "Lumina.xml");

        /// <summary>
        /// Returns the global mod saving path.
        /// </summary>
        public static string GlobalModSavingPath = Path.Combine(AssemblyDirectory, "Lumina.xml");

        /// <summary>
        /// Returns the Global Paradox Mod Saving path.
        /// </summary>
        public static string GlobalPDXModSavingPath = Path.Combine(assemblyPDXDirectory, "Lumina.xml");

        /// <summary>
        /// Returns the version.
        /// </summary>
        public static string Version = "1.5.7r1";
    }
}