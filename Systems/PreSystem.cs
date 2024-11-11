// <copyright file="Mod.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

using Lumina.Systems;
using Lumina.XML;
using System.IO;
using System;
using Unity.Entities;
using LuminaMod.XML;
using Colossal.IO.AssetDatabase;
using Game.Citizens;
using static HarmonyLib.Code;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Game.Pathfind;

namespace Lumina
{
    internal partial class PreSystem : SystemBase
    {


        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            ValidateLUTSDirectory();
            ValidateCubemapsDirectory();
            CheckForNullLUTName();

            if (GlobalVariables.Instance.ReloadAllPackagesOnRestart)
            {
                ClearPackagesDirectory();
                CheckAndReplaceAdditionalPackages();
            }

        }

        private void ClearPackagesDirectory()
        {
            var directories = new[]
            {
        GlobalPaths.LuminaLUTSDirectory,
        GlobalPaths.LuminaHDRIDirectory
    };

            foreach (var directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    try
                    {
                        // Get all files in the directory
                        var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);

                        // Delete each file
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }

                        // Log a single success message
                        Lumina.Mod.Log.Info($"All files in {directory} have been successfully deleted.");
                    }
                    catch (Exception ex)
                    {
                        // Log an error if any issue arises
                        Lumina.Mod.Log.Error($"Error clearing files in {directory}: {ex.Message}");
                    }
                }
                else
                {
                    Lumina.Mod.Log.Info($"Directory {directory} does not exist.");
                }
            }
        }


        private void CheckAndReplaceAdditionalPackages()
        {
            var packagesDirectory = GlobalPaths.PackagesDirectory;
            var localPackagesDirectory = GlobalPaths.LocalModsDirectory;
            var hdrDirectory = GlobalPaths.LuminaHDRIDirectory;
            var lutsDirectory = GlobalPaths.LuminaLUTSDirectory;

            // Check if both the packages and local packages directories exist
            if (!Directory.Exists(packagesDirectory))
            {
                Lumina.Mod.Log.Info($"Packages directory does not exist: {packagesDirectory}");
                return;
            }

            if (!Directory.Exists(localPackagesDirectory))
            {
                Lumina.Mod.Log.Info($"Local packages directory does not exist: {localPackagesDirectory}");
                return;
            }

            // Get all relevant files from both directories (.cube and .png)
            var packageFiles = Directory.GetFiles(packagesDirectory, "*.*", SearchOption.AllDirectories)
                                        .Where(f => (f.EndsWith(".cube", StringComparison.OrdinalIgnoreCase) ||
                                                     f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) &&
                                                    !IsExcludedFile(f));

            var localFiles = Directory.GetFiles(localPackagesDirectory, "*.*", SearchOption.AllDirectories)
                                      .Where(f => (f.EndsWith(".cube", StringComparison.OrdinalIgnoreCase) ||
                                                   f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) &&
                                                  !IsExcludedFile(f));

            // Combine files from both directories
            var allFiles = packageFiles.Concat(localFiles);

            // Process and copy the files to their respective destinations
            ProcessFiles(allFiles, hdrDirectory, lutsDirectory);
        }

        // Helper method to process files and copy them to the destination directories
        private void ProcessFiles(IEnumerable<string> files, string hdrDirectory, string lutsDirectory)
        {
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var fileDirectory = Path.GetDirectoryName(file); // Get the file's directory
                var baseFileName = Path.GetFileNameWithoutExtension(file); // Get the base name of the file without extension
                var luminaFileWithName = Path.Combine(fileDirectory, baseFileName + ".lumina"); // Construct the .lumina file path with name
                var luminaFileNoName = Path.Combine(fileDirectory, ".lumina"); // Construct the .lumina file path without name

                // Check if either .lumina file exists
                bool luminaFileExists = File.Exists(luminaFileWithName) || File.Exists(luminaFileNoName);

                // Ensure a .lumina file exists beside the file
                if (!luminaFileExists)
                {
                    Lumina.Mod.Log.Info($"Skipping {fileName} as no corresponding .lumina file was found.");
                    continue;
                }

                string destination = null;

                // Exclude .png files if maptextureconfig.json is in the same directory
                if (file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    var configPath = Path.Combine(fileDirectory, "maptextureconfig.json");
                    if (File.Exists(configPath))
                    {
                        Lumina.Mod.Log.Info($"Skipping {fileName} as maptextureconfig.json is found in the same directory.");
                        continue;
                    }
                    destination = Path.Combine(hdrDirectory, fileName);
                }
                else if (file.EndsWith(".cube", StringComparison.OrdinalIgnoreCase))
                {
                    destination = Path.Combine(lutsDirectory, fileName);
                }

                if (destination != null)
                {
                    try
                    {
                        File.Copy(file, destination, overwrite: true);
                        Lumina.Mod.Log.Info($"Copied {fileName} to {destination}");
                    }
                    catch (Exception ex)
                    {
                        Lumina.Mod.Log.Error($"Failed to copy {fileName} to {destination}: {ex.Message}");
                    }
                }
            }
        }

        // Helper method to check for excluded files based on their name
        private bool IsExcludedFile(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
            return fileName.Contains("screenshot") || fileName.Contains("preview") ||
                   fileName.Contains("photo-collage") || fileName.Contains("thumbnail");
        }


        /// <summary>
        /// Validates and ensures predetermined LUTs are available in ModsData.
        /// </summary>
        /// 
        public void ValidateLUTSDirectory()
        {
            try
            {
                // Ensure the directory path for LUTs is valid
                string directoryPath = GlobalPaths.LuminaLUTSDirectory;

                if (string.IsNullOrWhiteSpace(directoryPath))
                {
                    throw new InvalidOperationException("The directory path for LUTs is not set.");
                }

                // Create the directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    RenderEffectsSystem.LutFiles = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory);
                }


            }
            catch (Exception ex)
            {
                Mod.Log.Error($"Failed to validate or create the LUTs directory: {ex.Message}");
            }
        }

        private static void CheckForNullLUTName()
        {
            // Access the instance of GlobalVariables and check if LUTName is null or empty
            if (GlobalVariables.Instance == null)
            {
                Lumina.Mod.Log.Error("GlobalVariables.Instance is null.");
                return;
            }

            if (string.IsNullOrEmpty(GlobalVariables.Instance.LUTName))
            {
                // If LUTName is null or empty, assign it a default value
                GlobalVariables.Instance.LUTName = "None";
                Mod.Log.Info("LUTName was null or empty, assigned default value 'None'.");



            }
        }


        private void ValidateCubemapsDirectory()
        {
            // Ensure the directory path for LUTs is valid
            string directoryPath = GlobalPaths.LuminaHDRIDirectory;

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new InvalidOperationException("The directory path for Cubemaps is not set.");
            }

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

        }

        protected override void OnUpdate()
        {

        }
    }
}