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
            // Ensure the LUT directory exists
            if (!Directory.Exists(GlobalPaths.LuminaHDRIDirectory))
            {
                Mod.Log.Warn($"Cubemaps directory not found: {GlobalPaths.LuminaHDRIDirectory}. Creating directory...");
                Directory.CreateDirectory(GlobalPaths.LuminaHDRIDirectory);
            }

            // Populate RenderEffectsSystem.CubemapFiles with files from the specified directory
            var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaHDRIDirectory, "*.png");

            // Extract only the file names without the extension
            var fileNames = filesWithFullPath
                .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                .ToArray();

            // Ensure CubemapFiles is not null before logging
            if (RenderEffectsSystem.CubemapFiles != null)
            {
                // Log the result for debugging
                Mod.Log.Info("Cubemap files updated:\n" + string.Join(Environment.NewLine, RenderEffectsSystem.CubemapFiles));
            }
            else
            {
                Mod.Log.Warn("CubemapFiles is null after update.");
            }
            // Return the array of file names
            return fileNames;
        }
    }
}
