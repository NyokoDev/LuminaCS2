using Lumina;
using Lumina.Systems;
using Lumina.Systems.TextureHelper;
using Lumina.XML;
using LuminaMod.XML;
using System.Drawing.Drawing2D;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

class CubemapLoader : MonoBehaviour
{
    static string cubemapFilePath = RenderEffectsSystem.cubemapFilePath;
    public static string IncomingCubemap;

    public static Cubemap LoadCubemap()
    {
        // Construct the full path to the cubemap PNG file
        var cubemapFilePath = Path.Combine(GlobalPaths.LuminaHDRIDirectory, IncomingCubemap + ".png");

        // Check if GlobalVariables.Instance.CubemapName is null or if IncomingCubemap is empty
        if (string.IsNullOrEmpty(IncomingCubemap) || GlobalVariables.Instance.CubemapName == null)
        {
             Mod.Log.Info("Cubemap name is none. Ignoring.");
            return null;
        }

         Mod.Log.Info("Starting to load cubemap from PNG file: " + cubemapFilePath);

        var texture2D = TextureStreamUtility.LoadTextureFromFile(cubemapFilePath, false);

        int textureWidth = texture2D.width;
        int textureHeight = texture2D.height;

         Mod.Log.Info($"Texture dimensions: Width = {textureWidth}, Height = {textureHeight}");


        // Create the cubemap based on the calculated face size
        Cubemap cubemap = new Cubemap(2048, TextureFormat.ARGB32, true)
        {
            name = "LuminaCubemap",
            wrapMode = TextureWrapMode.Clamp,
        };
        Texture2D combinedTexture = TextureStreamUtility.LoadTextureFromFile(cubemapFilePath);

        // Set the faces of the cubemap using the calculated face size
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.PositiveX, 2, 1);
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.PositiveY, 1, 0);
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.PositiveZ, 1, 1);
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.NegativeX, 0, 1);
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.NegativeY, 1, 2);
        SetCubemapFace(cubemap, combinedTexture, CubemapFace.NegativeZ, 3, 1);
        Object.Destroy(combinedTexture);

        // Set cubemap texture.
        cubemap.anisoLevel = 9;
        cubemap.filterMode = FilterMode.Trilinear;
        cubemap.SmoothEdges();
        cubemap.Apply();

         Mod.Log.Info("Created Cubemap from PNG file successfully.");
        return cubemap;
    }

    /// <summary>
    /// Sets a cubemap face.
    /// </summary>
    /// <param name="cubemap">Cubemap.</param>
    /// <param name="texture">Face texture.</param>
    /// <param name="face">Face to set.</param>
    /// <param name="positionX">Texture X position.</param>
    /// <param name="positionY">Texture Y position.</param>
    public static void SetCubemapFace(Cubemap cubemap, Texture2D texture, CubemapFace face, int positionX, int positionY)
    {
        // Iterate through each pixel and copy to cubemap.
        for (int x = 0; x < cubemap.width; ++x)
        {
            for (int y = 0; y < cubemap.height; ++y)
            {
                int xPos = positionX * cubemap.width + x;
                int yPos = (2 - positionY) * cubemap.height + (cubemap.height - y - 1);
                Color pixelColor = texture.GetPixel(xPos, yPos);
                cubemap.SetPixel(face, x, y, pixelColor);
            }
        }
    }
}
