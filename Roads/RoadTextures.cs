using Game;
using Game.Objects;
using Game.SceneFlow;
using Game.UI.InGame;
using Lumina;
using Lumina.XML;
using LuminaMod.XML;
using System;
using System.IO;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace RoadWearAdjuster.Systems
{
    public partial class ReplaceRoadWearSystem : GameSystemBase
    {
        private Texture originalCarLaneBaseTexture;
        private Texture originalCarLaneNormalTexture;

        private Texture originalGravelLaneBaseTexture;
        private Texture originalGravelLaneNormalTexture;


        private Texture2D roadWearColourTexture;
        private Texture2D roadWearNormalTexture;

        private Material carLaneMaterial;
        private Material gravelLaneMaterial;

        private bool hasGeneratedTextures = false;
        private bool hasReplacedCarLaneRoadWearTexture = false;
        private bool hasReplacedGravelLaneRoadWearTexture = false;

        private const string fileName = "RoadWearTexture";

        protected override void OnCreate()
        {
            base.OnCreate();

            roadWearColourTexture = new Texture2D(1024, 1024);
            roadWearNormalTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);

            InitiallyCopyVanillaTextures();
        }

        private void InitiallyCopyVanillaTextures()
        {
            string targetDir = GlobalPaths.TexturesPDXDirectory;

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
                Mod.Log.Info("Created Textures directory at: " + targetDir);
            }

            var assembly = typeof(ReplaceRoadWearSystem).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (!resourceName.StartsWith("Lumina.Textures.")) continue;

                string file = resourceName.Substring("Lumina.Textures.".Length);
                string targetPath = Path.Combine(targetDir, file);

                if (File.Exists(targetPath))
                {
                    Mod.Log.Info($"Texture already exists, skipping: {file}");
                    continue;
                }

                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Mod.Log.Info($"Failed to get stream for resource: {resourceName}");
                    continue;
                }

                using FileStream fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fs);
                Mod.Log.Info($"Extracted: {file} → {targetPath}");
            }
        }

        public void UpdateStoredTextures()
        {
            string colourPath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_colour.png");
            string normalPath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_normal.png");

            if (!File.Exists(colourPath) || !File.Exists(normalPath))
            {
                Mod.Log.Error("Missing required texture files.");
                return;
            }

            byte[] colourData = File.ReadAllBytes(colourPath);
            roadWearColourTexture.LoadImage(colourData);

            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;

            Color[] pixels = roadWearColourTexture.GetPixels(0);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r *= brightness;
                pixels[i].g *= brightness;
                pixels[i].b *= brightness;
                pixels[i].a *= opacity;
            }
            roadWearColourTexture.SetPixels(pixels);
            roadWearColourTexture.Apply(true);

            byte[] normalData = File.ReadAllBytes(normalPath);
            Texture2D tempNormal = new Texture2D(2, 2);
            tempNormal.LoadImage(normalData);
            Graphics.CopyTexture(tempNormal, roadWearNormalTexture);
            roadWearNormalTexture.Apply();
            Object.Destroy(tempNormal);

            hasGeneratedTextures = true;
        }

        /// <summary>
        /// Completely reloads the textures and reapplies them to materials.
        /// Use this if textures or their properties changed at runtime.
        /// </summary>
        public void ReloadTexturesCompletely()
        {
            Object.Destroy(roadWearColourTexture);
            Object.Destroy(roadWearNormalTexture);

            roadWearColourTexture = new Texture2D(1024, 1024);
            roadWearNormalTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);

            hasGeneratedTextures = false;
            hasReplacedCarLaneRoadWearTexture = false;
            hasReplacedGravelLaneRoadWearTexture = false;

            UpdateStoredTextures();
            ReplaceTextures();
        }

        private void RevertTextures()
        {
            if (hasReplacedCarLaneRoadWearTexture && carLaneMaterial != null)
            {
                carLaneMaterial.SetTexture("_BaseColorMap", originalCarLaneBaseTexture);
                carLaneMaterial.SetTexture("_NormalMap", originalCarLaneNormalTexture);
                hasReplacedCarLaneRoadWearTexture = false;
            }

            if (hasReplacedGravelLaneRoadWearTexture && gravelLaneMaterial != null)
            {
                gravelLaneMaterial.SetTexture("_BaseColorMap", originalGravelLaneBaseTexture);
                gravelLaneMaterial.SetTexture("_NormalMap", originalGravelLaneNormalTexture);
                hasReplacedGravelLaneRoadWearTexture = false;
            }

            hasGeneratedTextures = false;
        }


        public void ReplaceTextures()
        {
            if (hasReplacedCarLaneRoadWearTexture && hasReplacedGravelLaneRoadWearTexture)
                return;

            var materials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (var material in materials)
            {
                if (!material.HasTexture("_BaseColorMap")) continue;
                Texture baseTex = material.GetTexture("_BaseColorMap");
                if (baseTex == null) continue;

                if (baseTex.name.StartsWith("CarLane_BaseColor") && !hasReplacedCarLaneRoadWearTexture)
                {
                    originalCarLaneBaseTexture = baseTex;
                    originalCarLaneNormalTexture = material.GetTexture("_NormalMap");

                    material.SetTexture("_BaseColorMap", roadWearColourTexture);
                    material.SetTexture("_NormalMap", roadWearNormalTexture);
                    carLaneMaterial = material;
                    hasReplacedCarLaneRoadWearTexture = true;
                }
                else if (baseTex.name.StartsWith("GravelLane_BaseColor") && !hasReplacedGravelLaneRoadWearTexture)
                {
                    originalGravelLaneBaseTexture = baseTex;
                    originalGravelLaneNormalTexture = material.GetTexture("_NormalMap");

                    material.SetTexture("_BaseColorMap", roadWearColourTexture);
                    material.SetTexture("_NormalMap", roadWearNormalTexture);
                    gravelLaneMaterial = material;
                    hasReplacedGravelLaneRoadWearTexture = true;
                }
            }
        }

        public void RequestReload()
        {
            reloadRequested = true;
        }

        private bool reloadRequested = false;
        protected override void OnUpdate()
        {
            if (!GlobalVariables.Instance.UseRoadTextures)
            {
                RevertTextures();
                return;
            }

            // Always update and apply brightness/opacity directly to texture pixels
            UpdateStoredTextures(); // this handles pixel modification

            // Only replace materials once (avoids redundant work)
            ReplaceTextures();

            // Only Smoothness is a shader float – apply it every frame
            float smoothness = GlobalVariables.Instance.RoadTextureSmoothness;
            carLaneMaterial?.SetFloat("_Smoothness", smoothness);
            gravelLaneMaterial?.SetFloat("_Smoothness", smoothness);

            if (reloadRequested)
            {
                ReloadTexturesCompletely();
                reloadRequested = false;
            }

        }

        private void UpdateAllMaterialFloats()
        {
            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;
            float smoothness = GlobalVariables.Instance.RoadTextureSmoothness;

            if (carLaneMaterial != null)
            {
                carLaneMaterial.SetFloat("_Smoothness", smoothness);
                carLaneMaterial.SetFloat("_Brightness", brightness);
                carLaneMaterial.SetFloat("_Opacity", opacity);
            }

            if (gravelLaneMaterial != null)
            {
                gravelLaneMaterial.SetFloat("_Smoothness", smoothness);
                gravelLaneMaterial.SetFloat("_Brightness", brightness);
                gravelLaneMaterial.SetFloat("_Opacity", opacity);
            }
        }


        protected override void OnDestroy()
        {
            Mod.Log.Info("Cleaning up");
            Object.Destroy(roadWearColourTexture);
            Object.Destroy(roadWearNormalTexture);
        }
    }
}