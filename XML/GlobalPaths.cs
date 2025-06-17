// <copyright file="GlobalPaths.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina.XML
{
    using Colossal.IO.AssetDatabase;
    using Game.Citizens;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static HarmonyLib.Code;
    using Unity.Entities.UniversalDelegates;
    using UnityEngine;

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

        /// <summary>
        /// Returns PDX mods packages directory.
        /// </summary>
        public static string PackagesDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", ".cache", "Mods", "mods_subscribed");

        /// <summary>
        /// Returns HDRI Directory.
        /// </summary>
        public static string LuminaHDRIDirectory = Path.Combine(AssemblyDirectory, "Cubemaps");

        public static string LocalModsDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", "Mods");

        private static string assemblyPDXDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", "ModsData", "Lumina");

        /// <summary>
        /// Returns the logs assembly directory.
        /// </summary>
        public static string logsassemblyPDXDirectory = Path.Combine(LocalLowDirectory, "Colossal Order", "Cities Skylines II", "Logs");

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
        public static string Version =
#if DEBUG
    "v2.2R5 - Experimental/Preview Test Version";
#else
            "v2.2R5";
#endif


        /// <summary>
        /// SupportedGameVersion, returns the supported game version.
        /// </summary>
        public static string SupportedGameVersion = "1.3.3f1 (1166.34240) [5841.24389]";

        /// <summary>
        /// Returns icon path for toast notifications.
        /// </summary>
        /// <returns>Returns icon path.</returns>
        internal static string GetIconPath()
        {
            string iconRelativePath = @"Icons\Lumina.ico";


            // Get the directory containing the DLL (Mod.ModPath points to DLL file)
            string baseDirectory = Path.GetDirectoryName(Mod.ModPath) ?? string.Empty;

            // Combine directory + relative icon path
            string iconFullPath = Path.Combine(baseDirectory, iconRelativePath);

            Lumina.Mod.Log.Info("Toast icon path: " + iconFullPath);

            return iconFullPath;
        }

        internal static string GetImagePath(string path)
        {
            // Use the provided relative path parameter instead of a hardcoded string
            string iconRelativePath = path;

            // Get the directory containing the DLL (Mod.ModPath points to the DLL file)
            string baseDirectory = Path.GetDirectoryName(Mod.ModPath) ?? string.Empty;

            // Combine base directory with the relative icon path to get full path
            string iconFullPath = Path.Combine(baseDirectory, iconRelativePath);

            Lumina.Mod.Log.Info("Requested image path: " + iconFullPath);

            return iconFullPath;
        }
    }
}