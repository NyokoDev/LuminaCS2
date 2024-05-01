using Game.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using LuminaMod.XML;
using Lumina.UI;
using Game.Simulation;
using JetBrains.Annotations;
using Game.Assets;
using System.Drawing.Text;

namespace Lumina.Systems
{

    internal partial class PostProcessSystem : SystemBase
    {

        bool m_SetupDone = false;
        Volume LuminaVolume;
        private VolumeProfile m_Profile;

        public Exposure m_Exposure;
        public Vignette m_Vignette;
        public ColorAdjustments m_ColorAdjustments;
        private WhiteBalance m_WhiteBalance;
        private ShadowsMidtonesHighlights m_ShadowsMidtonesHighlights;

        private UnityEngine.Rendering.HighDefinition.ColorAdjustments colorAdjustments;
        private PhotoModeRenderSystem PhotoModeRenderSystem;

        public static bool InitializedVolume = false;
        public static bool PanelInitialized = false;

        public static bool Panel = true;
        protected override void OnUpdate()
        {

            if (Panel)
            {
                GameObject newGameObject = new GameObject();
                newGameObject.AddComponent<SliderPanel>();
                PanelInitialized = true;
                Panel = false;
            }


            PlanetarySettings();
            ColorAdjustments();
            WhiteBalance();
            ShadowsMidTonesHighlights();

        }

        private void ColorAdjustments()
        {
            // Use reflection to get the private ColorAdjustments field from LightingSystem
            Type lightingSystemType = typeof(LightingSystem);
            FieldInfo colorAdjustmentsField = lightingSystemType.GetField("m_ColorAdjustments", BindingFlags.NonPublic | BindingFlags.Instance);

            if (colorAdjustmentsField != null)
            {
                LightingSystem lightingSystem = World.GetExistingSystemManaged<LightingSystem>();
                colorAdjustments = (UnityEngine.Rendering.HighDefinition.ColorAdjustments)colorAdjustmentsField.GetValue(lightingSystem);

                if (colorAdjustments != null)
                {
                    // Set the exposure value to 0 using Override method
                    colorAdjustments.postExposure.Override(GlobalVariables.Instance.PostExposure);
                    colorAdjustments.contrast.Override(GlobalVariables.Instance.Contrast);
                    colorAdjustments.hueShift.Override(GlobalVariables.Instance.hueShift);
                    colorAdjustments.saturation.Override(GlobalVariables.Instance.Saturation);
                }
            }
        }
        private void WhiteBalance()
        {
            m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
            m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);
        }

        private void ShadowsMidTonesHighlights()
        {
            m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
            m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));


        }










        protected override void OnCreate()
        {
            base.OnCreate();
            ConvertToHDRP();
        }

        public void ConvertToHDRP()
        {
            if (!m_SetupDone)
            {
                // Create new Global Volume GameObject
                GameObject globalVolume = new GameObject("Lumina");
                UnityEngine.Object.DontDestroyOnLoad(globalVolume);

                // Add Volume component
                LuminaVolume = globalVolume.AddComponent<Volume>();
                LuminaVolume.priority = 2000f;
                LuminaVolume.enabled = true;

                // Access the Volume Profile
                m_Profile = LuminaVolume.profile;

                // Add and configure Exposure effect
                m_Exposure = m_Profile.Add<Exposure>();
                m_Exposure.active = true;
                m_Exposure.mode.value = ExposureMode.Automatic;
                m_Exposure.limitMin.Override(-5f);
                m_Exposure.limitMax.Override(14f);
                m_Exposure.fixedExposure.Override(100f);

                // Add and configure White Balance effect
                m_WhiteBalance = m_Profile.Add<WhiteBalance>();
                m_WhiteBalance.active = true;
                m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
                m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);



                // Add and configure Color Adjustments effect
                m_ColorAdjustments = m_Profile.Add<ColorAdjustments>();
                m_ColorAdjustments.colorFilter.Override(new Color(1f, 1f, 1f));
                m_ColorAdjustments.contrast.Override(0f);

                m_SetupDone = true;

                m_ShadowsMidtonesHighlights = m_Profile.Add<ShadowsMidtonesHighlights>(); // Shadows, midtones, highlights
                m_ShadowsMidtonesHighlights.active = true;
                m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
                m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));

                Mod.log.Info("[LUMINA] Successfully added HDRP volume.");
            }
        }



        private void PlanetarySettings()
        {
            try
            {
#if DEBUG
                Mod.log.Info("Entered PlanetarySettings");
#endif
                LightingSystem lightingSystemInstance = World.GetExistingSystemManaged<LightingSystem>();
                if (lightingSystemInstance != null)
                {
                    Type lightingSystemType = typeof(LightingSystem);
                    FieldInfo planetarySystemField = lightingSystemType.GetField("m_PlanetarySystem", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (planetarySystemField != null)
                    {
                        PlanetarySystem planetarySystemInstance = (PlanetarySystem)planetarySystemField.GetValue(lightingSystemInstance);

                        if (planetarySystemInstance != null)
                        {


                            Type planetarySystemType = typeof(PlanetarySystem);
                            FieldInfo latitudeField = planetarySystemType.GetField("m_Latitude", BindingFlags.NonPublic | BindingFlags.Instance);
                            FieldInfo longitudeField = planetarySystemType.GetField("m_Longitude", BindingFlags.NonPublic | BindingFlags.Instance);

                            if (latitudeField != null && longitudeField != null)
                            {
                                float newLatitude = GlobalVariables.Instance.Latitude;
                                float newLongitude = GlobalVariables.Instance.Longitude;

                                latitudeField.SetValue(planetarySystemInstance, newLatitude);
                                longitudeField.SetValue(planetarySystemInstance, newLongitude);


                            }
                            else
                            {
#if DEBUG
                                Mod.log.Info("Latitude or longitude field not found.");
#endif
                            }
                        }
                        else
                        {
#if DEBUG
                            Mod.log.Info("PlanetarySystemInstance is null.");
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        Mod.log.Info("m_PlanetarySystem field not found in LightingSystem.");
#endif
                    }
                }
                else
                {
#if DEBUG
                    Mod.log.Info("LightingSystemInstance is null.");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Mod.log.Info("An error occurred: " + ex.Message);
#endif
            }
        }
    }
}

