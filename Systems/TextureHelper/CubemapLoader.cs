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
            if (!File.Exists(path)) throw new FileNotFoundException("Image rejected: PNG not found. Load the original file again or refresh the cubemap library.");
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path))) throw new InvalidDataException("Image rejected: PNG could not be decoded. Open the original image in an image editor and export it again as an RGB or RGBA PNG.");
            bool cross = texture.width % 4 == 0 && texture.height % 3 == 0 && texture.width / 4 == texture.height / 3;
            bool panorama = texture.width == texture.height * 2;
            if (!cross && !panorama) throw new InvalidDataException($"Image rejected: {texture.width} × {texture.height}. Use a 2:1 equirectangular panorama (4096 × 2048) or a 4:3 cubemap cross (4096 × 3072). Convert or export the original sky in one of these layouts; do not simply stretch it.");
            int size = cross ? texture.width / 4 : Mathf.Min(texture.width / 4, 2048);
            if (size < 1 || size > SystemInfo.maxCubemapSize) throw new InvalidDataException($"Image rejected: face size {size} exceeds supported limits or is invalid. Export a smaller sky, such as 4096 × 2048 (panorama) or 4096 × 3072 (cross).");
            cube = new Cubemap(size, TextureFormat.RGBA32, true) { name = "LuminaCubemap", wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Trilinear, anisoLevel = 9 };
            if (cross)
            {
                SetCubemapFace(cube, texture, CubemapFace.PositiveX, 2, 1);
                SetCubemapFace(cube, texture, CubemapFace.PositiveY, 1, 0);
                SetCubemapFace(cube, texture, CubemapFace.PositiveZ, 1, 1);
                SetCubemapFace(cube, texture, CubemapFace.NegativeX, 0, 1);
                SetCubemapFace(cube, texture, CubemapFace.NegativeY, 1, 2);
                SetCubemapFace(cube, texture, CubemapFace.NegativeZ, 3, 1);
            }
            else
            {
                texture.wrapModeU = TextureWrapMode.Repeat;
                texture.wrapModeV = TextureWrapMode.Clamp;
                foreach (CubemapFace face in new[] { CubemapFace.PositiveX, CubemapFace.NegativeX, CubemapFace.PositiveY, CubemapFace.NegativeY, CubemapFace.PositiveZ, CubemapFace.NegativeZ })
                    SetPanoramaFace(cube, texture, face);
            }
            cube.SmoothEdges();
            cube.Apply(true, true);
            var preview = new StringBuilder(48 * 28 * 6);
            for (int y = 0; y < 28; y++)
                for (int x = 0; x < 48; x++)
                {
                    Color32 color = cross
                        ? texture.GetPixel(size + x * size / 48, size + (27 - y) * size / 28)
                        : texture.GetPixelBilinear(0.375f + x / 47f * 0.25f, 0.75f - y / 27f * 0.5f);
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

    private static void SetPanoramaFace(Cubemap cube, Texture2D texture, CubemapFace face)
    {
        int size = cube.width;
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            float v = 2f * (y + 0.5f) / size - 1f;
            for (int x = 0; x < size; x++)
            {
                float u = 2f * (x + 0.5f) / size - 1f;
                Vector3 direction;
                switch (face)
                {
                    case CubemapFace.PositiveX: direction = new Vector3(1f, -v, -u); break;
                    case CubemapFace.NegativeX: direction = new Vector3(-1f, -v, u); break;
                    case CubemapFace.PositiveY: direction = new Vector3(u, 1f, v); break;
                    case CubemapFace.NegativeY: direction = new Vector3(u, -1f, -v); break;
                    case CubemapFace.PositiveZ: direction = new Vector3(u, -v, 1f); break;
                    default: direction = new Vector3(-u, -v, -1f); break;
                }
                float longitude = Mathf.Atan2(direction.x, direction.z) / (2f * Mathf.PI) + 0.5f;
                float latitude = Mathf.Asin(direction.y / direction.magnitude) / Mathf.PI + 0.5f;
                pixels[y * size + x] = texture.GetPixelBilinear(longitude, latitude);
            }
        }
        cube.SetPixels(pixels, face);
    }

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
