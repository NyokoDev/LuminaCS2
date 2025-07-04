using Game;
using Game.Objects;
using Game.SceneFlow;
using Game.Simulation;
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

        private Color[] originalColourPixels;
        private const string fileName = "RoadWearTexture";

        public bool AppliedYet = false;
        public Texture replacementTexture; // Optional: assign via Inspector

        public static void FindAndModifyMaterials()
        {
            string shaderName = "BH/NetCompositionMeshLitShader";
            Shader targetShader = Shader.Find(shaderName);

            if (targetShader == null)
            {
                Mod.Log.Info($"Shader '{shaderName}' not found.");
                return;
            }

            Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (Material mat in allMaterials)
            {
                if (mat.shader == targetShader)
                {
                    Mod.Log.Info($"Found Material: {mat.name} using Shader: {shaderName}");

                    if (mat.HasProperty("_BaseColor"))
                    {
                        mat.SetColor("_BaseColor", GlobalVariables.Instance.PrimaryRoadColor);
                        Mod.Log.Info($"  → Changed _BaseColor to {GlobalVariables.Instance.PrimaryRoadColor}");
                    }

                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.SetColor("_EmissionColor", GlobalVariables.Instance.SecondaryRoadColor);
                        mat.EnableKeyword("_EMISSION");
                        Mod.Log.Info($"  → Changed _EmissionColor to {GlobalVariables.Instance.SecondaryRoadColor}");
                    }

                    // Check common texture properties
                    string[] textureProps = new[] { "_BaseMap", "_MainTex", "_EmissionMap" };

                    foreach (string texProp in textureProps)
                    {
                        if (mat.HasProperty(texProp))
                        {
                            Texture tex = mat.GetTexture(texProp);
                            Mod.Log.Info($"  Texture Slot '{texProp}': {(tex != null ? tex.name : "None")}");

#if DEBUG
                            // If a replacement texture is set, replace it
                            if (replacementTexture != null)
                            {
                                mat.SetTexture(texProp, replacementTexture);
                                Mod.Log.Info($"  → Replaced {texProp} with '{replacementTexture.name}'");
                            }
#endif
                        }
                    }
                }
            }
        }


        protected override void OnCreate()
        {
            base.OnCreate();

            FindAndModifyMaterials();

            roadWearColourTexture = new Texture2D(1024, 1024);
            roadWearNormalTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);

            InitiallyCopyVanillaTextures();
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
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith("Lumina.Textures.")) continue;

                string file = resourceName.Substring("Lumina.Textures.".Length);
                string targetPath = Path.Combine(targetDir, file);

                if (File.Exists(targetPath))
                {
                    Mod.Log.Info($"Texture already exists, skipping: {file}");
                    continue;
                }

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Mod.Log.Info($"Failed to get stream for resource: {resourceName}");
                    continue;
                }

                using var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
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
            originalColourPixels = roadWearColourTexture.GetPixels(0);

            byte[] normalData = File.ReadAllBytes(normalPath);
            roadWearNormalTexture.LoadImage(normalData);
            roadWearNormalTexture.Apply();

            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;

            Color[] modifiablePixels = new Color[originalColourPixels.Length];
            for (int i = 0; i < originalColourPixels.Length; i++)
            {
                var px = originalColourPixels[i];
                px.r *= brightness;
                px.g *= brightness;
                px.b *= brightness;
                px.a *= opacity;
                modifiablePixels[i] = px;
            }

            roadWearColourTexture.SetPixels(modifiablePixels);
            roadWearColourTexture.Apply(true);
        }

        /// <summary>
        /// Initializes the road wear textures and replaces the existing textures in the game.
        /// </summary>
        public void InitialLoad()
        {
            Mod.Log.Info("Reloading all road wear textures");
            UpdateStoredTextures();
            ReplaceTextures();
            Mod.Log.Info("Reload complete.");
            AppliedYet = true;
        }

        public void ReloadTexturesCompletely()
        {
            Mod.Log.Info("Reloading all road wear textures");
            UpdateStoredTextures();
            ReplaceTextures();
            Mod.Log.Info("Reload complete.");
        }

        private void EnsureOriginalTexturesCached()
        {
            if (originalCarLaneBaseTexture != null && originalGravelLaneBaseTexture != null)
                return;

            var materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var m in materials)
            {
                if (!m.HasTexture("_BaseColorMap")) continue;

                Texture baseTex = m.GetTexture("_BaseColorMap");
                if (baseTex == null) continue;

                if (baseTex.name.StartsWith("CarLane_BaseColor") && originalCarLaneBaseTexture == null)
                {
                    originalCarLaneBaseTexture = baseTex;
                    originalCarLaneNormalTexture = m.GetTexture("_NormalMap");
                    carLaneMaterial = m;
                }
                else if (baseTex.name.StartsWith("GravelLane_BaseColor") && originalGravelLaneBaseTexture == null)
                {
                    originalGravelLaneBaseTexture = baseTex;
                    originalGravelLaneNormalTexture = m.GetTexture("_NormalMap");
                    gravelLaneMaterial = m;
                }

                if (originalCarLaneBaseTexture != null && originalGravelLaneBaseTexture != null)
                    break;
            }
        }

        public void ReplaceTextures()
        {
            var materials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (var m in materials)
            {
                if (!m.HasTexture("_BaseColorMap")) continue;
                Texture baseTex = m.GetTexture("_BaseColorMap");
                if (baseTex == null) continue;

                if (baseTex.name.StartsWith("CarLane_BaseColor") && !ReferenceEquals(baseTex, roadWearColourTexture))
                {
                    originalCarLaneBaseTexture = baseTex;
                    originalCarLaneNormalTexture = m.GetTexture("_NormalMap");
                    m.SetTexture("_BaseColorMap", roadWearColourTexture);
                    m.SetTexture("_NormalMap", roadWearNormalTexture);
                    carLaneMaterial = m;
                }
                else if (baseTex.name.StartsWith("GravelLane_BaseColor") && !ReferenceEquals(baseTex, roadWearColourTexture))
                {
                    originalGravelLaneBaseTexture = baseTex;
                    originalGravelLaneNormalTexture = m.GetTexture("_NormalMap");
                    m.SetTexture("_BaseColorMap", roadWearColourTexture);
                    m.SetTexture("_NormalMap", roadWearNormalTexture);
                    gravelLaneMaterial = m;
                }

            }
        }

        public void RevertTextures()
        {
            Mod.Log.Info("Forceful road-wear revert: scanning all materials…");

            EnsureOriginalTexturesCached();
            var materials = Resources.FindObjectsOfTypeAll<Material>();

            foreach (var m in materials)
            {
                if (!m.HasTexture("_BaseColorMap")) continue;

                Texture currentBase = m.GetTexture("_BaseColorMap");
                if (currentBase == null) continue;

                bool isInjected =
                    ReferenceEquals(currentBase, roadWearColourTexture) ||
                    currentBase.name == roadWearColourTexture.name;

                if (!isInjected) continue;

                if (m.name.Contains("CarLane") && originalCarLaneBaseTexture != null)
                {
                    m.SetTexture("_BaseColorMap", originalCarLaneBaseTexture);
                    m.SetTexture("_NormalMap", originalCarLaneNormalTexture);
                    Mod.Log.Info($"Reverted CarLane material: {m.name}");
                }
                else if (m.name.Contains("GravelLane") && originalGravelLaneBaseTexture != null)
                {
                    m.SetTexture("_BaseColorMap", originalGravelLaneBaseTexture);
                    m.SetTexture("_NormalMap", originalGravelLaneNormalTexture);
                    Mod.Log.Info($"Reverted GravelLane material: {m.name}");
                }
            }

            Mod.Log.Info("Forceful revert complete.");
        }
        protected override void OnUpdate()
        {
            if (GlobalVariables.Instance.UseRoadTextures)
            {
                float smoothness = GlobalVariables.Instance.RoadTextureSmoothness;

                carLaneMaterial?.SetFloat("_Smoothness", smoothness);
                gravelLaneMaterial?.SetFloat("_Smoothness", smoothness);

 

                if (!AppliedYet &&
                    (GameManager.instance.gameMode == GameMode.Game || GameManager.instance.gameMode == GameMode.Editor))
                {
                    Mod.Log.Info("[RoadTextureUpdater] Calling InitialLoad()");
                    InitialLoad();
                }
            }
            else
            {
#if DEBUG
                Mod.Log.Info("[RoadTextureUpdater] UseRoadTextures is false, skipping all logic.");
#endif
            }
        }


        protected override void OnDestroy()
        {
            Mod.Log.Info("Cleaning up");
            Object.Destroy(roadWearColourTexture);
            Object.Destroy(roadWearNormalTexture);
        }

        internal static void UpdateColors()
        {
            FindAndModifyMaterials();
        }
    }
}
