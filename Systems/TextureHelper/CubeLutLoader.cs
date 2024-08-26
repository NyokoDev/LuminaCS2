using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Colossal.PSI.Common;
using Lumina.Systems;
using Lumina.Systems.TextureHelper;
using LuminaMod.XML;
using UnityEngine;

public class CubeLutLoader : MonoBehaviour
{

    /// <summary>
    /// Loads LUT as RGBA64.
    /// </summary>
    /// <param name="cubeFilePath">Cube file path.</param>
    /// <returns>Returns a texture 3d.</returns>
    public static Texture3D LoadLutFromFile(string cubeFilePath)
    {
        // Force resource unloading
        Resources.UnloadUnusedAssets();

        if (File.Exists(cubeFilePath))
        {
            var result = LoadCubeFile(cubeFilePath);
            if (result != null)
            {
                // Check if the desired texture format is supported
                var desiredFormat = RenderEffectsSystem.TextureFormat;
                var supportedFormats = new[] { TextureFormat.RGBAHalf, TextureFormat.RGBA64 }; // Add other formats if needed

                if (!supportedFormats.Contains(desiredFormat))
                {
                    Lumina.Mod.Log.Error($"Desired texture format {desiredFormat} is not supported.");
                    return null;
                }

                // Create and configure the texture
                var texture = new Texture3D(result.LutSize, result.LutSize, result.LutSize, desiredFormat, false);

                // Check if the texture format matches the desired format
                if (texture.format != desiredFormat)
                {
                    Lumina.Mod.Log.Error($"Texture format mismatch. Expected: {desiredFormat}, but got: {texture.format}");
                    return null;
                }

                texture.SetPixels(result.Pixels);
                texture.Apply();

                Lumina.Mod.Log.Info($"LUT Texture created: {result.LutSize}³, format: {texture.format}");

                return texture;
            }
            else
            {
                Lumina.Mod.Log.Info("Failed to parse the .cube file.");
                return null;
            }
        }
        else
        {
            Lumina.Mod.Log.Info($"File not found: {cubeFilePath}");
            return null;
        }
    }


    // Load and process the LUT file
    private static ParseResult LoadCubeFile(string path)
    {
        
        return ParseCubeData(path);
    }

    // Refactored to return a result object instead of using out parameters
    private static ParseResult ParseCubeData(string filePath)
    {
        bool Error(string msg)
        {
            Lumina.Mod.Log.Info(msg);
            return false;
        }

        var lines = File.ReadAllLines(filePath);
        int lutSize = -1;
        int sizeCube = -1;
        var table = new List<Color>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = FilterLine(lines[i]);

            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith("TITLE"))
                continue;

            if (line.StartsWith("LUT_3D_SIZE"))
            {
                var sizeStr = line.Substring(11).TrimStart();

                if (!int.TryParse(sizeStr, out var size))
                {
                    Lumina.Mod.Log.Info($"Invalid data on line {i}");
                    return null;
                }

                lutSize = size;
                sizeCube = size * size * size;

                continue;
            }

            if (line.StartsWith("DOMAIN_"))
                continue;

            var row = line.Split();

            if (row.Length != 3)
            {
                Lumina.Mod.Log.Info($"Invalid data on line {i}");
                return null;
            }

            var color = Color.black;

            for (int j = 0; j < 3; j++)
            {
                if (!float.TryParse(row[j], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out var d))
                {
                    Lumina.Mod.Log.Info($"Invalid data on line {i}");
                    return null;
                }

                color[j] = d;
            }

            table.Add(color);
        }

        if (sizeCube != table.Count)
        {
            Lumina.Mod.Log.Info($"Wrong table size - Expected {sizeCube} elements, got {table.Count}");
            return null;
        }

        return new ParseResult { LutSize = lutSize, Pixels = table.ToArray() };
    }

    private static string FilterLine(string line)
    {
        var filtered = new StringBuilder();
        line = line.TrimStart().TrimEnd();
        int len = line.Length;
        int o = 0;

        while (o < len)
        {
            char c = line[o];
            if (c == '#') break;
            filtered.Append(c);
            o++;
        }

        return filtered.ToString();
    }

    // Private class to hold the LUT data
    private class ParseResult
    {
        public int LutSize { get; set; }
        public Color[] Pixels { get; set; }
    }
}
