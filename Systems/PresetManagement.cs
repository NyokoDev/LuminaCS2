// <copyright file="PresetManagement.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina.Systems
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Game.Pathfind;
    using Game.Prefabs;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using Unity.Collections;

    /// <summary>
    /// Preset Manager.
    /// </summary>
    internal class PresetManagement
    {
        /// <summary>
        /// Presets array of strings.
        /// </summary>
        public static string[] Presets;

        /// <summary>
        /// Selected name for the preset.
        /// </summary>
        public static string SelectedName;

        /// <summary>
        /// Executes the import function.
        /// </summary>
        internal static void ExecuteImport()
        {
            ImportPresets(SelectedName);
        }

        /// <summary>
        /// Exports the preset to a file.
        /// </summary>
        internal static void ExportLuminaPreset()
        {
            ExportPreset();
        }

        /// <summary>
        /// ExportPreset method creates the LuminaPresets directory, saves the current GlobalVariables XML to that location.
        /// </summary>
        internal static void ExportPreset()
        {
            string luminaPresetsDirectory = Path.Combine(GlobalPaths.AssemblyDirectory, "LuminaPresets");

            try
            {
                // Create the LuminaPresets directory if it doesn't exist
                Directory.CreateDirectory(luminaPresetsDirectory);

                Mod.Log.Info("LuminaPresets directory created (or already exists): " + luminaPresetsDirectory);

                // Generate a unique name for the preset based on timestamp and a unique identifier
                string uniqueName = SelectedName + ".xml";
                string uniquePresetPath = Path.Combine(luminaPresetsDirectory, uniqueName);

                Mod.Log.Info("Exporting preset: " + uniqueName);

                // Save the preset to a file
                GlobalVariables.SaveToFile(uniquePresetPath);
                Mod.Log.Info("Preset saved to: " + uniquePresetPath);

                // Open the saved preset file
                Process.Start(uniquePresetPath);
                Mod.Log.Info("Preset file opened: " + uniquePresetPath);

                Mod.Log.Info("Preset " + uniqueName + " has been saved.");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the export process
                Mod.Log.Info("Error exporting preset: " + ex.Message);
            }
        }

        /// <summary>
        /// Import presets looks for the selected preset in the folder <see cref="GlobalPaths.LuminaPresetsDirectory"/>.
        /// </summary>
        /// <param name="selectedName">Selected name for the preset.</param>
        internal static void ImportPresets(string selectedName)
        {
            string luminaPresetsDirectory = Path.Combine(GlobalPaths.AssemblyDirectory, "LuminaPresets");
            string uniqueName = selectedName + ".xml";
            string uniquePresetPath = Path.Combine(luminaPresetsDirectory, uniqueName);

            // Copy contents of uniquePresetPath to GlobalPaths.GlobalModSavingPath
            File.Copy(uniquePresetPath, GlobalPaths.GlobalModSavingPath, true);
            GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        /// <summary>
        /// Opens <see cref="GlobalPaths.LuminaPresetsDirectory"/>.
        /// </summary>
        internal static void OpenPresetFolder()
        {
            string luminaPresetsDirectory = Path.Combine(GlobalPaths.AssemblyDirectory, "LuminaPresets");
            Process.Start(luminaPresetsDirectory);
        }



     

        /// <summary>
        /// Updates the preset name, this comes from the <see cref="UISystem"/> that React uses.
        /// </summary>
        /// <param name="obj">String being passed to.</param>
        internal static void UpdatePresetName(string obj)
        {
            SelectedName = obj;
        }
    }
}