// <copyright file="Mod.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

using Lumina.Systems;
using Lumina.XML;
using System.IO;
using System;
using Unity.Entities;

namespace Lumina
{
    internal partial class PreSystem : SystemBase
    {

        /// <summary>
        /// Validates and ensures predetermined LUTs are available in ModsData.
        /// </summary>
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

        protected override void OnCreate()
        {
            base.OnCreate();
            ValidateLUTSDirectory();
            ValidateCubemapsDirectory();
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