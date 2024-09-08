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
            ClearPackagesDirectory();
            CheckAndReplaceAdditionalPackages();
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

            // Check if the packages directory exists
            if (!Directory.Exists(packagesDirectory))
            {
                Lumina.Mod.Log.Info($"Packages directory does not exist: {packagesDirectory}");
                return;
            }

            // Get all files in the directory and its subdirectories
            var files = Directory.GetFiles(packagesDirectory, "*.*", SearchOption.AllDirectories)
                                 .Where(f => (f.EndsWith(".cube", StringComparison.OrdinalIgnoreCase) ||
                                              f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) &&
                                             !Path.GetFileNameWithoutExtension(f).ToLower().Contains("screenshot") &&
                                             !Path.GetFileNameWithoutExtension(f).ToLower().Contains("preview") &&
                                             !Path.GetFileNameWithoutExtension(f).ToLower().Contains("thumbnail"));


            // Process normal files
            ProcessFiles(files);

            // Load assemblies and check for embedded resources
            var assemblies = Directory.GetFiles(packagesDirectory, "*.dll", SearchOption.AllDirectories);
            foreach (var assemblyPath in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyPath);
                    ProcessEmbeddedResources(assembly);
                }
                catch (Exception ex)
                {
                    Lumina.Mod.Log.Error($"[HARMLESS] Not loaded {assemblyPath}: {ex.Message}");
                }
            }
        }

        private void ProcessFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                try
                {
                    var extension = Path.GetExtension(file).ToLowerInvariant();
                    string destinationPath = string.Empty;

                    // If it's a .png file, check for maptextureconfig.json in the same directory
                    if (extension == ".png")
                    {
                        var directory = Path.GetDirectoryName(file);
                        var configFilePath = Path.Combine(directory, "maptextureconfig.json");

                        // Skip this .png file if maptextureconfig.json exists in the same directory
                        if (File.Exists(configFilePath))
                        {
                            Lumina.Mod.Log.Info($"Skipped {file} because maptextureconfig.json was found in the same directory.");
                            continue;
                        }

                        destinationPath = Path.Combine(GlobalPaths.LuminaHDRIDirectory, Path.GetFileName(file));
                    }
                    else if (extension == ".cube")
                    {
                        destinationPath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, Path.GetFileName(file));
                    }

                    // Ensure the destination directory exists
                    var destinationDirectory = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Copy the file to the destination
                    File.Copy(file, destinationPath, overwrite: true);
                    Lumina.Mod.Log.Info($"Copied {file} to {destinationPath}");
                }
                catch (Exception ex)
                {
                    Lumina.Mod.Log.Error($"Error copying file {file}: {ex.Message}");
                }
            }
        }

        private void ProcessEmbeddedResources(Assembly assembly)
        {
            if (assembly == null)
            {
                Lumina.Mod.Log.Error("Assembly is null.");
                return;
            }

            try
            {
                var resourceNames = assembly.GetManifestResourceNames();
                if (resourceNames == null || resourceNames.Length == 0)
                {
                    Lumina.Mod.Log.Info($"No resources found in assembly {assembly.FullName}");
                    return;
                }

                foreach (var resourceName in resourceNames)
                {
                    try
                    {
                        // Check for .cube or .png files within the embedded resources
                        if (resourceName.EndsWith(".cube", StringComparison.OrdinalIgnoreCase) ||
                            resourceName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var stream = assembly.GetManifestResourceStream(resourceName))
                            {
                                if (stream == null)
                                {
                                    Lumina.Mod.Log.Info($"Resource stream is null for {resourceName} in assembly {assembly.FullName}");
                                    continue; // Skip this resource if the stream is null
                                }

                                var extension = Path.GetExtension(resourceName).ToLowerInvariant();
                                string destinationPath = string.Empty;

                                if (extension == ".png")
                                {
                                    destinationPath = Path.Combine(GlobalPaths.LuminaHDRIDirectory, Path.GetFileName(resourceName));
                                }
                                else if (extension == ".cube")
                                {
                                    destinationPath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, Path.GetFileName(resourceName));
                                }

                                // Ensure the destination directory exists
                                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                                if (!Directory.Exists(destinationDirectory))
                                {
                                    Directory.CreateDirectory(destinationDirectory);
                                }

                                // Save the embedded resource to the destination path
                                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                                {
                                    stream.CopyTo(fileStream);
                                }

                                Lumina.Mod.Log.Info($"Extracted embedded resource {resourceName} to {destinationPath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Lumina.Mod.Log.Error($"Error processing embedded resource {resourceName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Lumina.Mod.Log.Error($"[HARMLESS] Error processing assembly {assembly.FullName}: {ex.Message}");
            }
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

                // Copy all embedded resources
                CopyAllEmbeddedResourcesToDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                Mod.Log.Error($"Failed to validate or create the LUTs directory: {ex.Message}");
            }
        }

        private void CopyAllEmbeddedResourcesToDirectory(string directoryPath)
        {
            var assembly = GetType().Assembly;
            var resourceNamespace = "Lumina.LUTS"; // Replace with your actual namespace

            // Get all resource names
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                // Check if the resource belongs to the correct namespace
                if (resourceName.StartsWith(resourceNamespace))
                {
                    using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream == null)
                        {
                            Mod.Log.Error($"Embedded resource '{resourceName}' not found.");
                            continue;
                        }

                        // Determine the destination path
                        var relativePath = resourceName.Substring(resourceNamespace.Length + 1); // Remove namespace prefix
                        var destinationPath = Path.Combine(directoryPath, relativePath);

                        // Create the directory if it doesn't exist
                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        // Copy the resource to the destination
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }

                    }
                }
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


        private void CopyCubemapsToDirectory(string directoryPath)
        {
            var assembly = GetType().Assembly;
            var resourceNamespace = "Lumina.Cubemaps"; // Replace with your actual namespace

            // Get all resource names
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                // Check if the resource belongs to the correct namespace
                if (resourceName.StartsWith(resourceNamespace))
                {
                    using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream == null)
                        {
                            Mod.Log.Error($"Embedded resource '{resourceName}' not found.");
                            continue;
                        }

                        // Determine the destination path
                        var relativePath = resourceName.Substring(resourceNamespace.Length + 1); // Remove namespace prefix
                        var destinationPath = Path.Combine(directoryPath, relativePath);

                        // Create the directory if it doesn't exist
                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        // Copy the resource to the destination
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }

                    }
                }
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
            // Copy all embedded resources
            CopyCubemapsToDirectory(directoryPath);

        }

        protected override void OnUpdate()
        {

        }
    }
}