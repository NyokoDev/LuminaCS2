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

        // Cache original pixels to avoid compounding brightness/opacity on each update
        private Color[] originalColourPixels;

        protected override void OnCreate()
        {
            base.OnCreate();


            LogAllLaneMaterialsProperties();

            if (!Directory.Exists(GlobalPaths.TexturesPDXDirectory))
            {
                Directory.CreateDirectory(GlobalPaths.TexturesPDXDirectory);
                Mod.Log.Info("Created Textures directory at: " + GlobalPaths.TexturesPDXDirectory);
            }

            InitiallyCopyVanillaTextures();

            roadWearColourTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, true, true);
            roadWearNormalTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);
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
                // Only handle resources inside the Textures folder
                if (!resourceName.StartsWith("Lumina.Textures.")) continue;

                string fileName = resourceName.Substring("Lumina.Textures.".Length);
                string targetPath = Path.Combine(targetDir, fileName);

                if (File.Exists(targetPath))
                {
                    Mod.Log.Info($"Texture already exists, skipping: {fileName}");
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
                Mod.Log.Info($"Extracted: {fileName} → {targetPath}");
            }
        }

        public void SetAndApplyGlobalRoadValues(float brightness, float opacity, float smoothness)
        {
            Mod.Log.Info($"Setting global road values: Brightness={brightness}, Opacity={opacity}, Smoothness={smoothness}");

            // Step 1: Set global values
            GlobalVariables.Instance.TextureBrightness = brightness;
            GlobalVariables.Instance.TextureOpacity = opacity;
            GlobalVariables.Instance.RoadTextureSmoothness = smoothness;

            // Step 2: Apply them to the textures and materials
            ApplyValuesToMaterials(brightness, opacity, smoothness);

            Mod.Log.Info("Applied global road values to lane materials and textures.");
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

        public void ApplyRoadTextureValues()
        {
            float brightness = GlobalVariables.Instance.TextureBrightness;
            float opacity = GlobalVariables.Instance.TextureOpacity;
            float smoothness = GlobalVariables.Instance.RoadTextureSmoothness;

            ApplyBrightnessOpacityToTexture(brightness, opacity); // modifies the color texture
            ApplySmoothnessToMaterials(smoothness);               // modifies materials only

            Mod.Log.Info("Applied brightness, opacity, and smoothness to road textures and materials.");
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

            string colourFilePath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_colour.png");
            string normalFilePath = Path.Combine(GlobalPaths.TexturesPDXDirectory, fileName + "_normal.png");

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

            // Cache original pixels for later brightness/opacity adjustments
            originalColourPixels = roadWearColourTexture.GetPixels(0);

            // Apply initial brightness and opacity from global variables to cached pixels
            ApplyBrightnessOpacityToTexture(GlobalVariables.Instance.TextureBrightness, GlobalVariables.Instance.TextureOpacity);

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
                roadWearNormalTexture.Reinitialize(tempNormalTexture.width, tempNormalTexture.height, TextureFormat.ARGB32, true);
            }

            Graphics.CopyTexture(tempNormalTexture, roadWearNormalTexture);
            roadWearNormalTexture.Apply();
            Object.Destroy(tempNormalTexture);

            hasGeneratedTextures = true;
            return true;
        }

        protected override void OnUpdate()
        {
            if (GlobalVariables.Instance.UseRoadTextures)
            {
                FindLaneMaterialsOnce();

                if (!hasGeneratedTextures)
                {
                    RefreshRoadWearTextures();
                }
            }
        }



        private void ApplySmoothnessToMaterials(float smoothness)
        {
            if (carLaneMaterial != null)
                carLaneMaterial.SetFloat("_Smoothness", smoothness);

            if (gravelLaneMaterial != null)
                gravelLaneMaterial.SetFloat("_Smoothness", smoothness);
        }

        public void LogAllMaterialProperties(Material material, string materialName)
        {
            if (material == null)
            {
                Mod.Log.Info($"{materialName} is null.");
                return;
            }

            Shader shader = material.shader;
            int propertyCount = shader.GetPropertyCount();

            Mod.Log.Info($"Logging all properties for {materialName} (Shader: {shader.name}):");

            for (int i = 0; i < propertyCount; i++)
            {
                string propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);

                switch (propType)
                {
                    case UnityEngine.Rendering.ShaderPropertyType.Color:
                        Color colVal = material.GetColor(propName);
                        Mod.Log.Info($"  Color: {propName} = {colVal}");
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Vector:
                        Vector4 vecVal = material.GetVector(propName);
                        Mod.Log.Info($"  Vector: {propName} = {vecVal}");
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Float:
                    case UnityEngine.Rendering.ShaderPropertyType.Range:
                        float floatVal = material.GetFloat(propName);
                        Mod.Log.Info($"  Float: {propName} = {floatVal}");
                        break;

                    case UnityEngine.Rendering.ShaderPropertyType.Texture:
                        Texture texVal = material.GetTexture(propName);
                        Mod.Log.Info($"  Texture: {propName} = {(texVal != null ? texVal.name : "null")}");
                        break;

                    default:
                        Mod.Log.Info($"  Unknown type: {propName}");
                        break;
                }
            }
        }

        public void LogAllLaneMaterialsProperties()
        {
            LogAllMaterialProperties(carLaneMaterial, "CarLaneMaterial");
            LogAllMaterialProperties(gravelLaneMaterial, "GravelLaneMaterial");
        }

        public void LogAllLaneMaterialFloats()
        {
            void LogFloats(Material material, string name)
            {
                if (material == null)
                {
                    Mod.Log.Info($"{name} is null.");
                    return;
                }

                Shader shader = material.shader;
                int count = shader.GetPropertyCount();

                Mod.Log.Info($"Logging float properties for {name} (Shader: {shader.name}):");

                for (int i = 0; i < count; i++)
                {
                    var propType = shader.GetPropertyType(i);
                    if (propType == UnityEngine.Rendering.ShaderPropertyType.Float ||
                        propType == UnityEngine.Rendering.ShaderPropertyType.Range)
                    {
                        string propName = shader.GetPropertyName(i);
                        float val = material.GetFloat(propName);
                        Mod.Log.Info($"  {propName} = {val}");
                    }
                }
            }

            LogFloats(carLaneMaterial, "CarLaneMaterial");
            LogFloats(gravelLaneMaterial, "GravelLaneMaterial");
        }


        private void ApplyBrightnessOpacityToTexture(float brightness, float opacity)
        {
            if (roadWearColourTexture == null || originalColourPixels == null)
                return;

            Color[] modifiedPixels = new Color[originalColourPixels.Length];
            for (int i = 0; i < originalColourPixels.Length; i++)
            {
                Color c = originalColourPixels[i];
                c.r *= brightness;
                c.g *= brightness;
                c.b *= brightness;
                c.a *= opacity;
                modifiedPixels[i] = c;
            }

            roadWearColourTexture.SetPixels(modifiedPixels);
            roadWearColourTexture.Apply(true);

            Mod.Log.Info($"Applied brightness {brightness} and opacity {opacity} to roadWearColourTexture and materials.");
        }

        private void ApplyValuesToMaterials(float brightness, float opacity, float smoothness)
        {
            ApplyBrightnessOpacityToTexture(brightness, opacity);
            ApplySmoothnessToMaterials(smoothness);
        }

        public void ApplyTexturesToAllLaneMaterials()
        {
            ApplyTextures(carLaneMaterial);
            ApplyTextures(gravelLaneMaterial);
            Mod.Log.Info("Reapplied updated textures to lane materials.");
        }


        public void ReloadAndApplyRoadTextures()
        {
            ApplyTextures(carLaneMaterial);
            ApplyTextures(gravelLaneMaterial);
        }



        public static void SetAndApplyGlobalRoadValues(ReplaceRoadWearSystem system, float value)
        {
            Mod.Log.Info($"Setting global road values to {value}");

            GlobalVariables.Instance.TextureBrightness = value;
            GlobalVariables.Instance.TextureOpacity = value;
            GlobalVariables.Instance.RoadTextureSmoothness = value;

            if (system != null)
            {
                Mod.Log.Info("Applying values to lane materials");
                system.ApplyValuesToMaterials(value, value, value);
            }
            else
            {
                Mod.Log.Info("ReplaceRoadWearSystem instance is null, skipping material application");
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
