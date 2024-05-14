using Game.Pathfind;
using Game.Prefabs;
using Lumina.UI;
using Lumina.XML;
using LuminaMod.XML;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.Collections;

namespace Lumina.Systems
{
    internal class PresetManagement
    {
        public static string[] Presets;
        public static string SelectedName;



        internal static void ExecuteImport()
        {
            ImportPresets(SelectedName);

        }

        internal static void ExportLuminaPreset()
        {
            ExportPreset();
        }

        internal static void ExportPreset()
        {
            string luminaPresetsDirectory = Path.Combine(GlobalPaths.assemblyDirectory, "LuminaPresets");

            try
            {
                // Create the LuminaPresets directory if it doesn't exist
                Directory.CreateDirectory(luminaPresetsDirectory);

                Mod.log.Info("LuminaPresets directory created (or already exists): " + luminaPresetsDirectory);

                // Generate a unique name for the preset based on timestamp and a unique identifier
                string uniqueName = SelectedName + ".xml";
                string uniquePresetPath = Path.Combine(luminaPresetsDirectory, uniqueName);

                Mod.log.Info("Exporting preset: " + uniqueName);

                // Save the preset to a file
                GlobalVariables.SaveToFile(uniquePresetPath);
                Mod.log.Info("Preset saved to: " + uniquePresetPath);

                // Open the saved preset file
                Process.Start(uniquePresetPath);
                Mod.log.Info("Preset file opened: " + uniquePresetPath);

                Mod.log.Info("Preset " + uniqueName + " has been saved.");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the export process
                Mod.log.Info("Error exporting preset: " + ex.Message);
            }
        }


internal static void ImportPresets(string SelectedName)
    {
        string luminaPresetsDirectory = Path.Combine(GlobalPaths.assemblyDirectory, "LuminaPresets");
        string uniqueName = SelectedName + ".xml";
        string uniquePresetPath = Path.Combine(luminaPresetsDirectory, uniqueName);

        // Copy contents of uniquePresetPath to GlobalPaths.GlobalModSavingPath
        File.Copy(uniquePresetPath, GlobalPaths.GlobalModSavingPath, true);
        GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);
        GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);

        }


    internal static void OpenPresetFolder()
        {
            Process.Start(GlobalPaths.assemblyDirectory);
        }

        internal static void UpdatePresetName(string obj)
        {
            SelectedName = obj;
        }
    }
}