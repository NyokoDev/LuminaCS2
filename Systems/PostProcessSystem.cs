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
using static UnityEngine.Rendering.HighDefinition.VolumetricClouds;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.HighDefinition.WindParameter;

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
        public VolumetricClouds m_VolumetricClouds;

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
                    colorAdjustments.postExposure.overrideState = GlobalVariables.Instance.PostExposureActive;


                    colorAdjustments.contrast.Override(GlobalVariables.Instance.Contrast);
                    colorAdjustments.contrast.overrideState = GlobalVariables.Instance.ContrastActive;

                    colorAdjustments.hueShift.Override(GlobalVariables.Instance.HueShift);
                    colorAdjustments.hueShift.overrideState = GlobalVariables.Instance.HueShiftActive;

                    colorAdjustments.saturation.Override(GlobalVariables.Instance.Saturation);
                    colorAdjustments.saturation.overrideState = GlobalVariables.Instance.SaturationActive;

                }
            }
        }
        private void WhiteBalance()
        {
            m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
            m_WhiteBalance.temperature.overrideState = GlobalVariables.Instance.TemperatureActive;
            m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);
            m_WhiteBalance.tint.overrideState = GlobalVariables.Instance.TintActive;
        }

        private void ShadowsMidTonesHighlights()
        {
            m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
            m_ShadowsMidtonesHighlights.shadows.overrideState = GlobalVariables.Instance.ShadowsActive;
            m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));
            m_ShadowsMidtonesHighlights.midtones.overrideState = GlobalVariables.Instance.MidtonesActive;
            m_ShadowsMidtonesHighlights.highlights.Override(new Vector4(GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights));
            m_ShadowsMidtonesHighlights.highlights.overrideState = GlobalVariables.Instance.HighlightsActive;


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
                LuminaVolume.priority = 1980f;
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

                // Add Volumetric Clouds
                SetUpVolumetricClouds();

    
                // Add and configure White Balance effect
                m_WhiteBalance = m_Profile.Add<WhiteBalance>();
                m_WhiteBalance.active = true;
                m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
                m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);



               



                // Add and configure Color Adjustments effect
                m_ColorAdjustments = m_Profile.Add<ColorAdjustments>();
                m_ColorAdjustments.colorFilter.Override(new Color(1f, 1f, 1f));

                m_SetupDone = true;

                m_ShadowsMidtonesHighlights = m_Profile.Add<ShadowsMidtonesHighlights>(); // Shadows, midtones, highlights
                m_ShadowsMidtonesHighlights.active = true;
                m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
                m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));

                Mod.Log.Info("[LUMINA] Successfully added HDRP volume.");
            }
        }

        public void SetUpVolumetricClouds()
        {
            m_VolumetricClouds = m_Profile.Add<VolumetricClouds>();

            // Enable/Disable the volumetric clouds effect
            m_VolumetricClouds.enable.value = VolumetricCloudsData.vmcactive;

            // Renders the volumetric clouds effect pre or post transparent
            m_VolumetricClouds.renderHook.value = VolumetricCloudsData.cloudhook;

            // Controls whether clouds are local or part of the skybox
            m_VolumetricClouds.localClouds.Override(VolumetricCloudsData.localclouds);
            Mod.Log.Info("Override");

            // Controls the curvature of the cloud volume which defines the distance at which the clouds intersect with the horizon
            m_VolumetricClouds.earthCurvature.value = VolumetricCloudsData.earthCurvature;
            Mod.Log.Info("earthcurvature");

            // Tiling (x,y) of the cloud map
            m_VolumetricClouds.cloudTiling.value = VolumetricCloudsData.cloudtiling;

            // Offset (x,y) of the cloud map
            m_VolumetricClouds.cloudOffset.value = VolumetricCloudsData.cloudOffset;

            // Controls the altitude of the bottom of the volumetric clouds volume in meters
            m_VolumetricClouds.bottomAltitude.value = VolumetricCloudsData.bottomAltitude;

            // Controls the size of the volumetric clouds volume in meters
            m_VolumetricClouds.altitudeRange.value = VolumetricCloudsData.altitudeRange;

            // Controls the mode in which the clouds fade in when close to the camera's near plane
            m_VolumetricClouds.fadeInMode.value = VolumetricCloudsData.fadeInMode;

            // Controls the minimal distance at which clouds start appearing
            m_VolumetricClouds.fadeInStart.value = VolumetricCloudsData.fadeInStart;

            // Controls the distance that it takes for the clouds to reach their complete density
            m_VolumetricClouds.fadeInDistance.value = VolumetricCloudsData.fadeInDistance;

            // Controls the number of steps when evaluating the clouds' transmittance
            m_VolumetricClouds.numPrimarySteps.value = VolumetricCloudsData.numPrimarySteps;

            // Controls the number of steps when evaluating the clouds' lighting
            m_VolumetricClouds.numLightSteps.value = VolumetricCloudsData.numLightSteps;

            // Specifies the cloud map - Coverage (R), Rain (G), Type (B)
            m_VolumetricClouds.cloudMap.value = VolumetricCloudsData.cloudMap;

            // Specifies the lookup table for the clouds - Profile Coverage (R), Erosion (G), Ambient Occlusion (B)
            m_VolumetricClouds.cloudLut.value = VolumetricCloudsData.cloudLut;

            // Specifies the cloud control Mode: Simple, Advanced or Manual
            m_VolumetricClouds.cloudControl.value = VolumetricCloudsData.cloudControl;

            // Specifies the lower cloud layer distribution in the advanced mode
            m_VolumetricClouds.cumulusMap.value = VolumetricCloudsData.cumulusMap;

            // Overrides the coverage of the lower cloud layer specified in the cumulus map in the advanced mode
            m_VolumetricClouds.cumulusMapMultiplier.value = 1f;

            // Specifies the higher cloud layer distribution in the advanced mode
            m_VolumetricClouds.altoStratusMap.value = VolumetricCloudsData.altoStratusMap;

            // Overrides the coverage of the higher cloud layer specified in the alto stratus map in the advanced mode
            m_VolumetricClouds.altoStratusMapMultiplier.value = 1f;

            // Specifies the anvil shaped clouds distribution in the advanced mode
            m_VolumetricClouds.cumulonimbusMap.value = VolumetricCloudsData.cumulonimbusMap;

            // Overrides the coverage of the anvil shaped clouds specified in the cumulonimbus map in the advanced mode
            m_VolumetricClouds.cumulonimbusMapMultiplier.value = 1f;

            // Specifies the rain distribution in the advanced mode
            m_VolumetricClouds.rainMap.value = VolumetricCloudsData.rainMap;

            // Specifies the internal texture resolution used for the cloud map in the advanced mode
            m_VolumetricClouds.cloudMapResolution.value = VolumetricCloudsData.cloudMapResolution;

            // Controls the density (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.densityCurve.value = VolumetricCloudsData.densityCurve;

            // Controls the erosion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.erosionCurve.value = VolumetricCloudsData.erosionCurve;

            // Controls the ambient occlusion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.ambientOcclusionCurve.value = VolumetricCloudsData.ambientOcclusionCurve;

            // Specifies the tint of the cloud scattering color
            m_VolumetricClouds.scatteringTint.value = VolumetricCloudsData.scatteringTint;

            // Controls the amount of local scattering in the clouds
            m_VolumetricClouds.powderEffectIntensity.value = VolumetricCloudsData.powderEffectIntensity;

            // Controls the amount of multi-scattering inside the cloud
            m_VolumetricClouds.multiScattering.value = VolumetricCloudsData.multiScattering;

            // Controls the global density of the cloud volume
            m_VolumetricClouds.densityMultiplier.value = VolumetricCloudsData.densityMultiplier;

            // Controls the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeFactor.value = VolumetricCloudsData.shapeFactor;

            // Controls the size of the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeScale.value = VolumetricCloudsData.shapeScale;

            // Controls the world space offset applied when evaluating the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeOffset.value = VolumetricCloudsData.shapeOffset;

            // Controls the smaller noise on the edge of the clouds
            m_VolumetricClouds.erosionFactor.value = VolumetricCloudsData.erosionFactor;

            // Controls the size of the smaller noise passing through the cloud coverage
            m_VolumetricClouds.erosionScale.value = VolumetricCloudsData.erosionScale;

            // Controls the type of noise used to generate the smaller noise passing through the cloud coverage
            m_VolumetricClouds.erosionNoiseType.value = VolumetricCloudsData.erosionNoiseType;

            // Controls the influence of the light probes on the cloud volume
            m_VolumetricClouds.ambientLightProbeDimmer.value = VolumetricCloudsData.ambientLightProbeDimmer;

            // Controls the influence of the sun light on the cloud volume
            m_VolumetricClouds.sunLightDimmer.value = VolumetricCloudsData.sunLightDimmer;

            // Controls how much Erosion Factor is taken into account when computing ambient occlusion
            m_VolumetricClouds.erosionOcclusion.value = VolumetricCloudsData.erosionOcclusion;

            // Sets the global horizontal wind speed in kilometers per hour
            m_VolumetricClouds.globalWindSpeed.value = VolumetricCloudsData.globalWindSpeed;

            // Controls the orientation of the wind relative to the X world vector
            m_VolumetricClouds.orientation.value = VolumetricCloudsData.orientation;

            // Controls the intensity of the wind-based altitude distortion of the clouds
            m_VolumetricClouds.altitudeDistortion.value = VolumetricCloudsData.altitudeDistortion;

            // Controls the multiplier to the speed of the cloud map
            m_VolumetricClouds.cloudMapSpeedMultiplier.value = VolumetricCloudsData.cloudMapSpeedMultiplier;

            // Controls the multiplier to the speed of the larger cloud shapes
            m_VolumetricClouds.shapeSpeedMultiplier.value = VolumetricCloudsData.shapeSpeedMultiplier;

            // Controls the multiplier to the speed of the erosion cloud shapes
            m_VolumetricClouds.erosionSpeedMultiplier.value = VolumetricCloudsData.erosionSpeedMultiplier;

            // Controls the vertical wind speed of the larger cloud shapes
            m_VolumetricClouds.verticalShapeWindSpeed.value = VolumetricCloudsData.verticalShapeWindSpeed;

            // Controls the vertical wind speed of the erosion cloud shapes
            m_VolumetricClouds.verticalErosionWindSpeed.value = VolumetricCloudsData.verticalErosionWindSpeed;

            // Temporal accumulation increases the visual quality of clouds by decreasing the noise
            m_VolumetricClouds.temporalAccumulationFactor.value = VolumetricCloudsData.temporalAccumulationFactor;

            // Enable/Disable the volumetric clouds ghosting reduction
            m_VolumetricClouds.ghostingReduction.value = VolumetricCloudsData.ghostingReduction;

            // Specifies the strength of the perceptual blending for the volumetric clouds
            m_VolumetricClouds.perceptualBlending.value = VolumetricCloudsData.perceptualBlending;

            // Enable/Disable the volumetric clouds shadow
            m_VolumetricClouds.shadows.value = VolumetricCloudsData.shadows;

            // Specifies the resolution of the volumetric clouds shadow map
            m_VolumetricClouds.shadowResolution.value = VolumetricCloudsData.shadowResolution;

            // Controls the vertical offset applied to compute the volumetric clouds shadow in meters
            m_VolumetricClouds.shadowPlaneHeightOffset.value = VolumetricCloudsData.shadowPlaneHeightOffset;

            // Sets the size of the area covered by shadow around the camera
            m_VolumetricClouds.shadowDistance.value = VolumetricCloudsData.shadowDistance;

            // Controls the opacity of the volumetric clouds shadow
            m_VolumetricClouds.shadowOpacity.value = VolumetricCloudsData.shadowOpacity;

            // Controls the shadow opacity when outside the area covered by the volumetric clouds shadow
            m_VolumetricClouds.shadowOpacityFallback.value = VolumetricCloudsData.shadowOpacityFallback;
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

