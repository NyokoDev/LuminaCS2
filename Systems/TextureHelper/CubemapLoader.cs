using Lumina;
using Lumina.XML;
using LuminaMod.XML;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

class CubemapLoader : MonoBehaviour
{
    public static string IncomingCubemap;
    public static string Preview { get; private set; } = "";
    public static string Error { get; private set; } = "";

    public static Cubemap LoadSavedCubemap() => Load(GlobalVariables.Instance.CubemapName);
    public static Cubemap LoadCubemap() => Load(IncomingCubemap);

    public static Cubemap Load(string name)
    {
        Texture2D texture = null;
        Cubemap cube = null;
        Error = "";
        try
        {
            if (string.IsNullOrWhiteSpace(name) || name != Path.GetFileName(name) || name == "None" || name == "Select Cubemap")
                throw new ArgumentException("Select a cubemap first.");
            string path = Path.Combine(GlobalPaths.LuminaHDRIDirectory, name + ".png");
            if (!File.Exists(path)) throw new FileNotFoundException("Cubemap PNG not found.");
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path))) throw new InvalidDataException("Invalid PNG.");
            int size = texture.width / 4;
            if (size < 1 || texture.width != size * 4 || texture.height != size * 3 || size > SystemInfo.maxCubemapSize)
                throw new InvalidDataException("Expected a 4 x 3 cubemap cross with square faces.");
            cube = new Cubemap(size, TextureFormat.RGBA32, true) { name = "LuminaCubemap", wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Trilinear, anisoLevel = 9 };
            SetCubemapFace(cube, texture, CubemapFace.PositiveX, 2, 1);
            SetCubemapFace(cube, texture, CubemapFace.PositiveY, 1, 0);
            SetCubemapFace(cube, texture, CubemapFace.PositiveZ, 1, 1);
            SetCubemapFace(cube, texture, CubemapFace.NegativeX, 0, 1);
            SetCubemapFace(cube, texture, CubemapFace.NegativeY, 1, 2);
            SetCubemapFace(cube, texture, CubemapFace.NegativeZ, 3, 1);
            cube.SmoothEdges();
            cube.Apply(true, true);
            var preview = new StringBuilder(48 * 28 * 6);
            for (int y = 0; y < 28; y++)
                for (int x = 0; x < 48; x++)
                {
                    Color32 color = texture.GetPixel(size + x * size / 48, size + (27 - y) * size / 28);
                    preview.Append(color.r.ToString("X2")).Append(color.g.ToString("X2")).Append(color.b.ToString("X2"));
                }
            Preview = preview.ToString();
            return cube;
        }
        catch (Exception e)
        {
            if (cube != null) UnityEngine.Object.Destroy(cube);
            Error = e.Message;
            Mod.Log.Warn("[Sky & Fog] " + Error);
            return null;
        }
        finally { if (texture != null) UnityEngine.Object.Destroy(texture); }
    }

    public static void ClearPreview() { Preview = ""; Error = ""; }

    public static void SetCubemapFace(Cubemap cube, Texture2D texture, CubemapFace face, int x, int y)
    {
        int size = cube.width;
        Color[] pixels = texture.GetPixels(x * size, (2 - y) * size, size, size);
        for (int row = 0; row < size / 2; row++)
        {
            int a = row * size, b = (size - row - 1) * size;
            for (int col = 0; col < size; col++)
            {
                Color temp = pixels[a + col];
                pixels[a + col] = pixels[b + col];
                pixels[b + col] = temp;
            }
        }
        cube.SetPixels(pixels, face);
    }
}
