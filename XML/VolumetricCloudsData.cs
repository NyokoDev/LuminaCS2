using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace LuminaMod.XML
{
    public class VolumetricCloudsData
    {
        public static bool vmcactive { get; internal set; }
        public static VolumetricClouds.CloudHook cloudhook { get; internal set; }
        public static bool localclouds { get; internal set; }
        public static float earthCurvature { get; internal set; }
        public static Vector2 cloudtiling { get; internal set; }
        public static Vector2 cloudOffset { get; internal set; }
        public static float bottomAltitude { get; internal set; }
        public static float altitudeRange { get; internal set; }
        public static VolumetricClouds.CloudFadeInMode fadeInMode { get; internal set; }
        public static float fadeInStart { get; internal set; }
        public static float fadeInDistance { get; internal set; }
        public static int numPrimarySteps { get; internal set; }
        public static int numLightSteps { get; internal set; }
        public static Texture cloudMap { get; internal set; }
        public static Texture cloudLut { get; internal set; }
        public static VolumetricClouds.CloudControl cloudControl { get; internal set; }
        public static Texture cumulusMap { get; internal set; }
        public static Texture altoStratusMap { get; internal set; }
        public static Texture cumulonimbusMap { get; internal set; }
        public static Texture rainMap { get; internal set; }
        public static VolumetricClouds.CloudMapResolution cloudMapResolution { get; internal set; }
        public static AnimationCurve densityCurve { get; internal set; }
        public static AnimationCurve erosionCurve { get; internal set; }
        public static AnimationCurve ambientOcclusionCurve { get; internal set; }
        public static Color scatteringTint { get; internal set; }
        public static float powderEffectIntensity { get; internal set; }
        public static float multiScattering { get; internal set; }
        public static float densityMultiplier { get; internal set; }
        public static float shapeFactor { get; internal set; }
        public static float shapeScale { get; internal set; }
        public static Vector3 shapeOffset { get; internal set; }
        public static float erosionFactor { get; internal set; }
        public static float erosionScale { get; internal set; }
        public static VolumetricClouds.CloudErosionNoise erosionNoiseType { get; internal set; }
        public static float ambientLightProbeDimmer { get; internal set; }
        public static float sunLightDimmer { get; internal set; }
        public static float erosionOcclusion { get; internal set; }
        public static WindParameter.WindParamaterValue globalWindSpeed { get; internal set; }
        public static WindParameter.WindParamaterValue orientation { get; internal set; }
        public static float altitudeDistortion { get; internal set; }
        public static float cloudMapSpeedMultiplier { get; internal set; }
        public static float shapeSpeedMultiplier { get; internal set; }
        public static float erosionSpeedMultiplier { get; internal set; }
        public static float verticalShapeWindSpeed { get; internal set; }
        public static float verticalErosionWindSpeed { get; internal set; }
        public static float temporalAccumulationFactor { get; internal set; }
        public static bool ghostingReduction { get; internal set; }
        public static float perceptualBlending { get; internal set; }
        public static bool shadows { get; internal set; }
        public static VolumetricClouds.CloudShadowResolution shadowResolution { get; internal set; }
        public static float shadowPlaneHeightOffset { get; internal set; }
        public static float shadowDistance { get; internal set; }
        public static float shadowOpacity { get; internal set; }
        public static float shadowOpacityFallback { get; internal set; }
    }
}