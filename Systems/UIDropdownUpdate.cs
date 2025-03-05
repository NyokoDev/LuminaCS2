using Lumina.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Lumina.Systems
{
    partial class UIDropdownUpdate : SystemBase
    {
        /// <inheritdoc/>
        protected override void OnUpdate()
        {
        }

        public static string[] UpdateCubemapDropdown()
        {
            // Ensure CubemapFiles is reset before updating
            RenderEffectsSystem.CubemapFiles = Array.Empty<string>();

            // Ensure the cubemap directory exists
            if (!Directory.Exists(GlobalPaths.LuminaHDRIDirectory))
            {
                Mod.Log.Info($"Cubemaps directory not found: {GlobalPaths.LuminaHDRIDirectory}. Creating directory...");
                Directory.CreateDirectory(GlobalPaths.LuminaHDRIDirectory);
            }

            // Get all PNG files from the directory
            var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaHDRIDirectory, "*.png");

            // Extract only the file names without the extension
            RenderEffectsSystem.CubemapFiles = filesWithFullPath
                .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                .ToArray();

            // Log the updated cubemap files
            if (RenderEffectsSystem.CubemapFiles.Length > 0)
            {
                Mod.Log.Info("Cubemap files updated:\n" + string.Join(Environment.NewLine, RenderEffectsSystem.CubemapFiles));
            }
            else
            {
                Mod.Log.Info("No cubemap files found.");
                RenderEffectsSystem.CubemapFiles = Array.Empty<string>();
            }

            // Return the updated array
            return RenderEffectsSystem.CubemapFiles;
        }
    }
}

