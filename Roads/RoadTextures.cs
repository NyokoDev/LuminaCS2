/// <summary>
/// Several parts of this code are thanks to the RoadWearAdjuster project. https://github.com/HX2003/RoadWearAdjuster
/// <summary>

using Game;
using Game.Objects;
using Lumina;
using Lumina.XML;
using LuminaMod.XML;
using System.IO;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace RoadWearAdjuster.Systems
{
    public partial class ReplaceRoadWearSystem : GameSystemBase
    {
        private Texture2D roadWearColourTexture;
        private Texture2D roadWearNormalTexture;

        private Material carLaneMaterial;
        private Material gravelLaneMaterial;

        private bool hasGeneratedTextures = false;
        private const string fileName = "RoadWearTexture"; 

        protected override void OnCreate()
        {
            base.OnCreate();

            roadWearColourTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, true, true);
            roadWearNormalTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);
        }

        private void FindLaneMaterialsOnce()
        {
            if (carLaneMaterial != null && gravelLaneMaterial != null)
                return;

            foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (!material.HasTexture("_BaseColorMap")) continue;

                Texture baseTex = material.GetTexture("_BaseColorMap");
                if (baseTex == null) continue;

                if (baseTex.name.StartsWith("CarLane_BaseColor"))
                {
                    Mod.Log.Info("Caching car lane material: " + material.name);
                    carLaneMaterial = material;
                }
                else if (baseTex.name.StartsWith("GravelLane_BaseColor"))
                {
                    Mod.Log.Info("Caching gravel lane material: " + material.name);
                    gravelLaneMaterial = material;
                }
            }
        }

        private void ApplyTextures(Material material)
        {
            if (material == null) return;

            material.SetTexture("_BaseColorMap", roadWearColourTexture);
            material.SetTexture("_NormalMap", roadWearNormalTexture);
            material.SetFloat("_Smoothness", GlobalVariables.Instance.RoadTextureSmoothness);
        }

        public void RefreshRoadWearTextures()
        {
            Mod.Log.Info("Refreshing road wear textures on demand");

            if (UpdateStoredTextures())
            {
                ApplyTextures(carLaneMaterial);
                ApplyTextures(gravelLaneMaterial);
            }
        }

        public bool UpdateStoredTextures()
        {
            Mod.Log.Info("Generating and updating stored textures");

            string colourFilePath = Path.Combine(GlobalPaths.GlobalModSavingPath, fileName + "_colour.png");
            string normalFilePath = Path.Combine(GlobalPaths.GlobalModSavingPath, fileName + "_normal.png");

            if (!File.Exists(colourFilePath) || !File.Exists(normalFilePath))
            {
                Mod.Log.Info("One or both texture files are missing: " + colourFilePath + " or " + normalFilePath);
                return false;
            }

            // Load colour texture
            byte[] colourData = File.ReadAllBytes(colourFilePath);
            if (!roadWearColourTexture.LoadImage(colourData))
            {
                Mod.Log.Info("Failed to load colour image");
                return false;
            }

            Color[] pixels = roadWearColourTexture.GetPixels(0);
            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r *= brightness;
                pixels[i].g *= brightness;
                pixels[i].b *= brightness;
                pixels[i].a *= opacity;
            }

            roadWearColourTexture.SetPixels(pixels);
            roadWearColourTexture.Apply(true);

            // Load normal texture
            byte[] normalData = File.ReadAllBytes(normalFilePath);
            Texture2D tempNormalTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tempNormalTexture.LoadImage(normalData))
            {
                Mod.Log.Info("Failed to load normal image");
                Object.Destroy(tempNormalTexture);
                return false;
            }

            if (tempNormalTexture.width != roadWearNormalTexture.width || tempNormalTexture.height != roadWearNormalTexture.height)
            {
                roadWearNormalTexture.Resize(tempNormalTexture.width, tempNormalTexture.height, TextureFormat.ARGB32, true);
            }

            Graphics.CopyTexture(tempNormalTexture, roadWearNormalTexture);
            roadWearNormalTexture.Apply();
            Object.Destroy(tempNormalTexture);

            hasGeneratedTextures = true;
            return true;
        }

        protected override void OnUpdate()
        {
            FindLaneMaterialsOnce();

            if (!hasGeneratedTextures)
            {
                RefreshRoadWearTextures();
            }
        }

        protected override void OnDestroy()
        {
            Mod.Log.Info("Cleaning up road wear textures");
            Object.Destroy(roadWearColourTexture);
            Object.Destroy(roadWearNormalTexture);
        }
    }
}
