using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Lumina;
using Lumina.Systems;
using Lumina.Systems.TextureHelper;
using LuminaMod.XML;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CubeLutLoader : MonoBehaviour
{

    /// <summary>
    /// Loads LUT as GraphicsFormat.R16G16B16A16_SFloat.
    /// </summary>
    /// <param name="cubeFilePath">Cube file path.</param>
    /// <returns>Returns a texture 3d.</returns>
    public static Texture3D LoadLutFromFile(string cubeFilePath)
    {
        if (File.Exists(cubeFilePath))
        {
            var result = LoadCubeFile(cubeFilePath);
            if (result != null)
            {

                // Create and configure the texture

                var texture = new Texture3D(result.LutSize, result.LutSize, result.LutSize, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None)
                {
                    filterMode = FilterMode.Bilinear, // Properties from CubeLutImporter
                    wrapMode = TextureWrapMode.Clamp,
                };

                texture.SetPixels(result.Pixels);
                texture.Apply();

                 Mod.Log.Info($"LUT Texture created: {result.LutSize}³, format: {texture.graphicsFormat}");

                return texture;
            }
            else
            {
                 Mod.Log.Info("Failed to parse the .cube file.");
                return null;
            }
        }
        else
        {
             Mod.Log.Info($"File not found: {cubeFilePath}");
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
            Mod.Log.Info(msg);
            return false;
        }

        var lines = File.ReadAllLines(filePath);
        int lutSize = -1;
        var table = new List<Color>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = FilterLine(lines[i]);

            if (string.IsNullOrEmpty(line) || line.StartsWith("TITLE") || line.StartsWith("DOMAIN_"))
                continue;

            if (line.StartsWith("LUT_3D_SIZE"))
            {
                var sizeStr = line.Substring(11).TrimStart();
                if (!int.TryParse(sizeStr, out var size))
                {
                    Mod.Log.Info($"Invalid LUT_3D_SIZE on line {i}");
                    return null;
                }

                if (size != 32 && size != 33)
                {
                    Mod.Log.Info($"Unsupported LUT size: {size}. Only 32 and 33 supported.");
                    return null;
                }

                lutSize = size;
                continue;
            }

            var row = line.Split();
            if (row.Length != 3)
            {
                Mod.Log.Info($"Invalid data on line {i}");
                return null;
            }

            var color = Color.black;
            for (int j = 0; j < 3; j++)
            {
                if (!float.TryParse(row[j], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out var d))
                {
                    Mod.Log.Info($"Invalid data on line {i}");
                    return null;
                }

                color[j] = d;
            }

            table.Add(color);
        }

        if (lutSize == -1)
        {
            Mod.Log.Info("Missing LUT_3D_SIZE.");
            return null;
        }

        int expectedCount = lutSize * lutSize * lutSize;
        if (table.Count != expectedCount)
        {
            Mod.Log.Info($"Wrong table size - Expected {expectedCount} elements, got {table.Count}");
            return null;
        }

        if (lutSize == 33)
        {
            Mod.Log.Info("Converting LUT_3D_SIZE 33 → 32 via trilinear resampling...");
            var resizedPixels = ResampleCubeLUT(table.ToArray(), 33, 32);
            return new ParseResult { LutSize = 32, Pixels = resizedPixels };
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

    /// <summary>
    /// Converts a 33x33x33 LUT to a 32x32x32 LUT using trilinear resampling.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="srcSize"></param>
    /// <param name="dstSize"></param>
    /// <returns></returns>
    private static Color[] ResampleCubeLUT(Color[] input, int srcSize, int dstSize)
    {
        Color[,,] src = new Color[srcSize, srcSize, srcSize];
        int index = 0;
        for (int b = 0; b < srcSize; b++)
            for (int g = 0; g < srcSize; g++)
                for (int r = 0; r < srcSize; r++)
                    src[r, g, b] = input[index++];

        Color[] output = new Color[dstSize * dstSize * dstSize];
        index = 0;

        for (int b = 0; b < dstSize; b++)
            for (int g = 0; g < dstSize; g++)
                for (int r = 0; r < dstSize; r++)
                {
                    float fr = r / (float)(dstSize - 1) * (srcSize - 1);
                    float fg = g / (float)(dstSize - 1) * (srcSize - 1);
                    float fb = b / (float)(dstSize - 1) * (srcSize - 1);
                    output[index++] = TrilinearSample(src, fr, fg, fb, srcSize);
                }

        return output;
    }

    private static Color TrilinearSample(Color[,,] lut, float x, float y, float z, int size)
    {
        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);
        int z0 = Mathf.FloorToInt(z);
        int x1 = Mathf.Min(x0 + 1, size - 1);
        int y1 = Mathf.Min(y0 + 1, size - 1);
        int z1 = Mathf.Min(z0 + 1, size - 1);

        float tx = x - x0;
        float ty = y - y0;
        float tz = z - z0;

        Color c000 = lut[x0, y0, z0];
        Color c100 = lut[x1, y0, z0];
        Color c010 = lut[x0, y1, z0];
        Color c110 = lut[x1, y1, z0];
        Color c001 = lut[x0, y0, z1];
        Color c101 = lut[x1, y0, z1];
        Color c011 = lut[x0, y1, z1];
        Color c111 = lut[x1, y1, z1];

        Color c00 = Color.Lerp(c000, c100, tx);
        Color c01 = Color.Lerp(c001, c101, tx);
        Color c10 = Color.Lerp(c010, c110, tx);
        Color c11 = Color.Lerp(c011, c111, tx);

        Color c0 = Color.Lerp(c00, c10, ty);
        Color c1 = Color.Lerp(c01, c11, ty);

        return Color.Lerp(c0, c1, tz);
    }


    // Private class to hold the LUT data
    private class ParseResult
    {
        public int LutSize { get; set; }
        public Color[] Pixels { get; set; }
    }
}
