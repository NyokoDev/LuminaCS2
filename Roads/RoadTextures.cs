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

            // Always do one scan to save vanilla textures even if we're not going to apply them yet
            ReloadTexturesCompletely();
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

        private Color[] originalColourPixels;
        private bool hasLoadedOriginalData = false;


        public void UpdateStoredTextures()
        {
            string colourPath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_colour.png");
            string normalPath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_normal.png");

            if (!File.Exists(colourPath) || !File.Exists(normalPath))
            {
                Mod.Log.Error("Missing required texture files.");
                return;
            }

            if (!hasLoadedOriginalData || originalColourPixels == null || originalColourPixels.Length == 0)
            {
                Mod.Log.Info("Reloading texture from disk into memory...");
                byte[] colourData = File.ReadAllBytes(colourPath);
                roadWearColourTexture.LoadImage(colourData);
                originalColourPixels = roadWearColourTexture.GetPixels(0);
                hasLoadedOriginalData = true;

                byte[] normalData = File.ReadAllBytes(normalPath);
                roadWearNormalTexture.LoadImage(normalData);
                roadWearNormalTexture.Apply();
            }


            // Modify cached pixels per-frame
            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;

            Color[] modifiablePixels = new Color[originalColourPixels.Length];
            for (int i = 0; i < originalColourPixels.Length; i++)
            {
                Color px = originalColourPixels[i];
                px.r *= brightness;
                px.g *= brightness;
                px.b *= brightness;
                px.a *= opacity;
                modifiablePixels[i] = px;
            }

            roadWearColourTexture.SetPixels(modifiablePixels);
            roadWearColourTexture.Apply(true);

            hasGeneratedTextures = true;
        }


        /// <summary>
        /// Completely reloads the textures and reapplies them to materials.
        /// Use this if textures or their properties changed at runtime.
        /// </summary>
        public void ReloadTexturesCompletely()
        {
            Mod.Log.Info("Reloading all road wear textures");

            // ✅ Reset everything
            originalColourPixels = null;
            hasLoadedOriginalData = false;
            hasGeneratedTextures = false;
            hasReplacedCarLaneRoadWearTexture = false;
            hasReplacedGravelLaneRoadWearTexture = false;

            UpdateStoredTextures(); // re-load image and cache pixels
            ReplaceTextures();      // rebind textures to materials

            Mod.Log.Info("Reload complete: textures reloaded and replaced.");
        }

        /// Make sure original textures & materials are captured
        private void EnsureOriginalTexturesCached()
        {
            if (originalCarLaneBaseTexture != null &&
                originalGravelLaneBaseTexture != null)
                return; // already cached

            var materials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (var m in materials)
            {
                if (!m.HasTexture("_BaseColorMap")) continue;

                Texture baseTex = m.GetTexture("_BaseColorMap");
                if (baseTex == null) continue;

                if (baseTex.name.StartsWith("CarLane_BaseColor") &&
                    originalCarLaneBaseTexture == null)
                {
                    originalCarLaneBaseTexture = baseTex;
                    originalCarLaneNormalTexture = m.GetTexture("_NormalMap");
                    carLaneMaterial = m;  // remember it for later
                }
                else if (baseTex.name.StartsWith("GravelLane_BaseColor") &&
                         originalGravelLaneBaseTexture == null)
                {
                    originalGravelLaneBaseTexture = baseTex;
                    originalGravelLaneNormalTexture = m.GetTexture("_NormalMap");
                    gravelLaneMaterial = m;
                }

                // Early-out once both are cached
                if (originalCarLaneBaseTexture != null &&
                    originalGravelLaneBaseTexture != null)
                    break;
            }
        }

        /// ***FORCEFUL*** revert — no flag checks, no early returns
        public void RevertTextures()
        {
            Mod.Log.Info("Forceful road-wear revert: scanning all materials…");

            // 1️⃣ Make sure originals exist
            EnsureOriginalTexturesCached();

            // 2️⃣ Swap back on every material that still uses the injected texture
            var materials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (var m in materials)
            {
                if (!m.HasTexture("_BaseColorMap")) continue;

                Texture currentBase = m.GetTexture("_BaseColorMap");
                if (currentBase == null) continue;

                // Is this one of OUR injected textures?
                bool isInjected =
                    ReferenceEquals(currentBase, roadWearColourTexture) ||
                    currentBase.name == roadWearColourTexture.name;

                if (!isInjected) continue;

                if (m.name.Contains("CarLane"))
                {
                    if (originalCarLaneBaseTexture != null &&
                        originalCarLaneNormalTexture != null)
                    {
                        m.SetTexture("_BaseColorMap", originalCarLaneBaseTexture);
                        m.SetTexture("_NormalMap", originalCarLaneNormalTexture);
                        Mod.Log.Info($"Reverted CarLane material: {m.name}");
                    }
                    else Mod.Log.Warn($"Missing originals for {m.name}");
                }
                else if (m.name.Contains("GravelLane"))
                {
                    if (originalGravelLaneBaseTexture != null &&
                        originalGravelLaneNormalTexture != null)
                    {
                        m.SetTexture("_BaseColorMap", originalGravelLaneBaseTexture);
                        m.SetTexture("_NormalMap", originalGravelLaneNormalTexture);
                        Mod.Log.Info($"Reverted GravelLane material: {m.name}");
                    }
                    else Mod.Log.Warn($"Missing originals for {m.name}");
                }
                else
                {
                    // If needed, handle other road types here
                    Mod.Log.Info($"Skipped non-lane material still using injected texture: {m.name}");
                }
            }

            // 3️⃣ Reset internal state unconditionally
            hasReplacedCarLaneRoadWearTexture = false;
            hasReplacedGravelLaneRoadWearTexture = false;
            hasGeneratedTextures = false;

            Mod.Log.Info("Forceful revert complete.");
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
                return;
            }

            // Only Smoothness is a shader float – apply it every frame
            float smoothness = GlobalVariables.Instance.RoadTextureSmoothness;
            carLaneMaterial?.SetFloat("_Smoothness", smoothness);
            gravelLaneMaterial?.SetFloat("_Smoothness", smoothness);

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