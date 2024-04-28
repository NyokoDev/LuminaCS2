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
#if DEBUG
            ShadowsMidTonesHighlights();
#endif
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

                    // Accessing the postExposure field
                    float exposureValue = colorAdjustments.postExposure.value;
                    float contrastvalue = colorAdjustments.contrast.value;

                }
            }
        }
        private void WhiteBalance() {




            /// White Balance retrieval
            Type photoModeRenderSystemType = typeof(PhotoModeRenderSystem);
            FieldInfo whiteBalanceField = photoModeRenderSystemType.GetField("m_WhiteBalance", BindingFlags.NonPublic | BindingFlags.Instance);

            if (whiteBalanceField != null) // Checking if the field is found
            {
                PhotoModeRenderSystem photoModeRenderSystemInstance = World.GetExistingSystemManaged<PhotoModeRenderSystem>();
                if (photoModeRenderSystemInstance != null)
                {
                    UnityEngine.Rendering.HighDefinition.WhiteBalance whiteBalanceValue = (UnityEngine.Rendering.HighDefinition.WhiteBalance)whiteBalanceField.GetValue(photoModeRenderSystemInstance);


                    // Using White Balance same as Lumina.
                    if (whiteBalanceValue != null)
                    {

                        whiteBalanceValue.temperature.Override(GlobalVariables.Instance.Temperature);
                        whiteBalanceValue.tint.Override(GlobalVariables.Instance.Tint);
                    }



                }

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

                                Mod.log.Info("Set Latitude: " + planetarySystemInstance.latitude);
                                Mod.log.Info("Set Longitude: " + planetarySystemInstance.longitude);
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

#if DEBUG
        private void ShadowsMidTonesHighlights()
        {




            /// White Balance retrieval
            Type photoModeRenderSystemType = typeof(PhotoModeRenderSystem);
            FieldInfo m_ShadowsMidtonesHighlightsField = photoModeRenderSystemType.GetField("m_ShadowsMidtonesHighlights", BindingFlags.NonPublic | BindingFlags.Instance);

            if (m_ShadowsMidtonesHighlightsField != null) // Checking if the field is found
            {
                PhotoModeRenderSystem photoModeRenderSystemInstance = World.GetExistingSystemManaged<PhotoModeRenderSystem>();
                if (photoModeRenderSystemInstance != null)
                {
                    UnityEngine.Rendering.HighDefinition.ShadowsMidtonesHighlights SMHValue = (UnityEngine.Rendering.HighDefinition.ShadowsMidtonesHighlights)m_ShadowsMidtonesHighlightsField.GetValue(photoModeRenderSystemInstance);


                    // Using White Balance same as Lumina.
                    if (SMHValue != null)
                    {

                        SMHValue.shadows.Override((Vector4)GlobalVariables.Instance.Shadows);
                        SMHValue.midtones.Override((Vector4)GlobalVariables.Instance.Midtones);
                        SMHValue.highlights.Override((Vector4)GlobalVariables.Instance.Highlights);
                        SMHValue.shadowsStart.Override((float)GlobalVariables.Instance.ShadowsStart);
                        SMHValue.shadowsEnd.Override((float)GlobalVariables.Instance.ShadowsEnd);
                        SMHValue.highlightsStart.Override((float)GlobalVariables.Instance.highlightsStart);
                        SMHValue.highlightsEnd.Override((float)GlobalVariables.Instance.highlightsEnd);
                    }



                }

            }
        #endif

        }

    }
    



