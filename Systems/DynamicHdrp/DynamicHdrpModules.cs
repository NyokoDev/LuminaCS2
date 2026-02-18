namespace Lumina.Systems.DynamicHdrp
{
    using System;
    using System.Collections.Generic;
    using LuminaMod.XML;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;

    internal interface IDynamicHdrpComponentModule
    {
        DynamicHdrpComponentDescriptor Descriptor { get; }

        bool IsEnabled { get; }

        void Initialize(VolumeProfile profile);

        void SetEnabled(bool enabled);

        void Apply(GlobalVariables settings);

        Dictionary<string, object> GetCurrentValues(GlobalVariables settings);

        bool TrySetValue(string propertyId, string value);
    }

    internal abstract class DynamicHdrpComponentModuleBase<TComponent> : IDynamicHdrpComponentModule
        where TComponent : VolumeComponent
    {
        private readonly Dictionary<string, Func<GlobalVariables, object>> m_Readers = new Dictionary<string, Func<GlobalVariables, object>>();
        private readonly Dictionary<string, Action<string>> m_Writers = new Dictionary<string, Action<string>>();

        protected TComponent Component;

        protected DynamicHdrpComponentModuleBase(DynamicHdrpComponentDescriptor descriptor)
        {
            Descriptor = descriptor;
            IsEnabled = descriptor.DefaultEnabled;
        }

        public DynamicHdrpComponentDescriptor Descriptor { get; }

        public bool IsEnabled { get; private set; }

        public void Initialize(VolumeProfile profile)
        {
            if (!profile.TryGet(out Component))
            {
                Component = profile.Add<TComponent>();
            }

            Component.active = IsEnabled;
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            if (Component != null)
            {
                Component.active = enabled;
            }
        }

        public void Apply(GlobalVariables settings)
        {
            if (Component == null)
            {
                return;
            }

            Component.active = IsEnabled;
            if (!IsEnabled)
            {
                return;
            }

            ApplyInternal(settings);
        }

        public Dictionary<string, object> GetCurrentValues(GlobalVariables settings)
        {
            var values = new Dictionary<string, object>();
            foreach (var descriptor in Descriptor.Properties)
            {
                if (m_Readers.TryGetValue(descriptor.Id, out var reader))
                {
                    values[descriptor.Id] = reader(settings);
                }
            }

            return values;
        }

        public bool TrySetValue(string propertyId, string value)
        {
            if (!m_Writers.TryGetValue(propertyId, out var writer))
            {
                return false;
            }

            writer(value);
            return true;
        }

        protected void RegisterFloat(string propertyId, Func<GlobalVariables, float> reader, Action<float> writer)
        {
            m_Readers[propertyId] = settings => reader(settings);
            m_Writers[propertyId] = raw =>
            {
                if (float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    writer(parsed);
                }
            };
        }

        protected void RegisterBool(string propertyId, Func<GlobalVariables, bool> reader, Action<bool> writer)
        {
            m_Readers[propertyId] = settings => reader(settings);
            m_Writers[propertyId] = raw =>
            {
                if (bool.TryParse(raw, out var parsed))
                {
                    writer(parsed);
                }
                else if (raw == "1")
                {
                    writer(true);
                }
                else if (raw == "0")
                {
                    writer(false);
                }
            };
        }

        protected abstract void ApplyInternal(GlobalVariables settings);
    }

    internal sealed class ColorAdjustmentsModule : DynamicHdrpComponentModuleBase<ColorAdjustments>
    {
        public ColorAdjustmentsModule()
            : base(new DynamicHdrpComponentDescriptor
            {
                Id = "colorAdjustments",
                DisplayName = "Color Adjustments",
                Category = "Color",
                DefaultEnabled = true,
                Properties = new List<DynamicHdrpPropertyDescriptor>
                {
                    new DynamicHdrpPropertyDescriptor { Id = "postExposure", DisplayName = "Post Exposure", Type = DynamicHdrpPropertyType.Float, Min = -1f, Max = 1f, Step = 0.0001f },
                    new DynamicHdrpPropertyDescriptor { Id = "postExposureActive", DisplayName = "Post Exposure Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "contrast", DisplayName = "Contrast", Type = DynamicHdrpPropertyType.Float, Min = -100f, Max = 100f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "contrastActive", DisplayName = "Contrast Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "hueShift", DisplayName = "Hue Shift", Type = DynamicHdrpPropertyType.Float, Min = -100f, Max = 100f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "hueShiftActive", DisplayName = "Hue Shift Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "saturation", DisplayName = "Saturation", Type = DynamicHdrpPropertyType.Float, Min = -100f, Max = 100f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "saturationActive", DisplayName = "Saturation Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                },
            })
        {
            RegisterFloat("postExposure", settings => settings.PostExposure, value => GlobalVariables.Instance.PostExposure = value);
            RegisterBool("postExposureActive", settings => settings.PostExposureActive, value => GlobalVariables.Instance.PostExposureActive = value);
            RegisterFloat("contrast", settings => settings.Contrast, value => GlobalVariables.Instance.Contrast = value);
            RegisterBool("contrastActive", settings => settings.ContrastActive, value => GlobalVariables.Instance.ContrastActive = value);
            RegisterFloat("hueShift", settings => settings.HueShift, value => GlobalVariables.Instance.HueShift = value);
            RegisterBool("hueShiftActive", settings => settings.HueShiftActive, value => GlobalVariables.Instance.HueShiftActive = value);
            RegisterFloat("saturation", settings => settings.Saturation, value => GlobalVariables.Instance.Saturation = value);
            RegisterBool("saturationActive", settings => settings.SaturationActive, value => GlobalVariables.Instance.SaturationActive = value);
        }

        protected override void ApplyInternal(GlobalVariables settings)
        {
            Component.postExposure.Override(settings.PostExposure);
            Component.postExposure.overrideState = settings.PostExposureActive;
            Component.contrast.Override(settings.Contrast);
            Component.contrast.overrideState = settings.ContrastActive;
            Component.hueShift.Override(settings.HueShift);
            Component.hueShift.overrideState = settings.HueShiftActive;
            Component.saturation.Override(settings.Saturation);
            Component.saturation.overrideState = settings.SaturationActive;
        }
    }

    internal sealed class WhiteBalanceModule : DynamicHdrpComponentModuleBase<WhiteBalance>
    {
        public WhiteBalanceModule()
            : base(new DynamicHdrpComponentDescriptor
            {
                Id = "whiteBalance",
                DisplayName = "White Balance",
                Category = "Color",
                DefaultEnabled = true,
                Properties = new List<DynamicHdrpPropertyDescriptor>
                {
                    new DynamicHdrpPropertyDescriptor { Id = "temperature", DisplayName = "Temperature", Type = DynamicHdrpPropertyType.Float, Min = -100f, Max = 100f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "temperatureActive", DisplayName = "Temperature Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "tint", DisplayName = "Tint", Type = DynamicHdrpPropertyType.Float, Min = -100f, Max = 100f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "tintActive", DisplayName = "Tint Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                },
            })
        {
            RegisterFloat("temperature", settings => settings.Temperature, value => GlobalVariables.Instance.Temperature = value);
            RegisterBool("temperatureActive", settings => settings.TemperatureActive, value => GlobalVariables.Instance.TemperatureActive = value);
            RegisterFloat("tint", settings => settings.Tint, value => GlobalVariables.Instance.Tint = value);
            RegisterBool("tintActive", settings => settings.TintActive, value => GlobalVariables.Instance.TintActive = value);
        }

        protected override void ApplyInternal(GlobalVariables settings)
        {
            Component.temperature.Override(settings.Temperature);
            Component.temperature.overrideState = settings.TemperatureActive;
            Component.tint.Override(settings.Tint);
            Component.tint.overrideState = settings.TintActive;
        }
    }

    internal sealed class ShadowsMidtonesHighlightsModule : DynamicHdrpComponentModuleBase<ShadowsMidtonesHighlights>
    {
        public ShadowsMidtonesHighlightsModule()
            : base(new DynamicHdrpComponentDescriptor
            {
                Id = "shadowsMidtonesHighlights",
                DisplayName = "Shadows Midtones Highlights",
                Category = "Color",
                DefaultEnabled = true,
                Properties = new List<DynamicHdrpPropertyDescriptor>
                {
                    new DynamicHdrpPropertyDescriptor { Id = "shadows", DisplayName = "Shadows", Type = DynamicHdrpPropertyType.Float, Min = -1f, Max = 2f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "shadowsActive", DisplayName = "Shadows Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "midtones", DisplayName = "Midtones", Type = DynamicHdrpPropertyType.Float, Min = -1f, Max = 2f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "midtonesActive", DisplayName = "Midtones Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                    new DynamicHdrpPropertyDescriptor { Id = "highlights", DisplayName = "Highlights", Type = DynamicHdrpPropertyType.Float, Min = -1f, Max = 2f, Step = 0.001f },
                    new DynamicHdrpPropertyDescriptor { Id = "highlightsActive", DisplayName = "Highlights Enabled", Type = DynamicHdrpPropertyType.Bool, Min = 0f, Max = 1f, Step = 1f },
                },
            })
        {
            RegisterFloat("shadows", settings => settings.Shadows, value => GlobalVariables.Instance.Shadows = value);
            RegisterBool("shadowsActive", settings => settings.ShadowsActive, value => GlobalVariables.Instance.ShadowsActive = value);
            RegisterFloat("midtones", settings => settings.Midtones, value => GlobalVariables.Instance.Midtones = value);
            RegisterBool("midtonesActive", settings => settings.MidtonesActive, value => GlobalVariables.Instance.MidtonesActive = value);
            RegisterFloat("highlights", settings => settings.Highlights, value => GlobalVariables.Instance.Highlights = value);
            RegisterBool("highlightsActive", settings => settings.HighlightsActive, value => GlobalVariables.Instance.HighlightsActive = value);
        }

        protected override void ApplyInternal(GlobalVariables settings)
        {
            Component.shadows.Override(new Vector4(settings.Shadows, settings.Shadows, settings.Shadows, settings.Shadows));
            Component.shadows.overrideState = settings.ShadowsActive;
            Component.midtones.Override(new Vector4(settings.Midtones, settings.Midtones, settings.Midtones, settings.Midtones));
            Component.midtones.overrideState = settings.MidtonesActive;
            Component.highlights.Override(new Vector4(settings.Highlights, settings.Highlights, settings.Highlights, settings.Highlights));
            Component.highlights.overrideState = settings.HighlightsActive;
        }
    }
}
