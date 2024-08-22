using Colossal.PSI.Common;
using LuminaMod.XML;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class CubeLutLoader : MonoBehaviour
{
    public static Texture3D LutTexture { get; set; } // Static property to access the texture
    public static TextureFormat TextureFormat { get; set; }

    // Public method to load a LUT from a .cube file
    public static Texture3D LoadLutFromFile(string cubeFilePath)
    {
        if (File.Exists(cubeFilePath))
        {
            var result = LoadCubeFile(cubeFilePath);
            if (result != null)
            {
                var texture = new Texture3D(result.LutSize, result.LutSize, result.LutSize, TextureFormat, false);
                texture.SetPixels(result.Pixels);
                texture.Apply();

                Lumina.Mod.Log.Info($"LUT Texture created successfully with size {result.LutSize}x{result.LutSize}x{result.LutSize} with texture format " + GlobalVariables.Instance.TextureFormat.ToString());

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
