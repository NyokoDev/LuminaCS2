namespace Lumina.Systems.DynamicHdrp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lumina.XML;
    using LuminaMod.XML;
    using UnityEngine.Rendering;

    internal sealed class DynamicHdrpRegistry
    {
        private readonly Dictionary<string, IDynamicHdrpComponentModule> m_Modules;

        public DynamicHdrpRegistry(IEnumerable<IDynamicHdrpComponentModule> modules)
        {
            m_Modules = modules.ToDictionary(module => module.Descriptor.Id, module => module, StringComparer.OrdinalIgnoreCase);
        }

        public void Initialize(VolumeProfile profile)
        {
            foreach (var module in m_Modules.Values)
            {
                module.Initialize(profile);
                module.SetEnabled(GetStoredEnabledState(module.Descriptor.Id, module.Descriptor.DefaultEnabled));
            }
        }

        public void Apply(GlobalVariables settings)
        {
            foreach (var module in m_Modules.Values)
            {
                module.Apply(settings);
            }
        }

        public string GetMetadataJson()
        {
            return DynamicHdrpJson.BuildMetadataJson(m_Modules.Values.Select(module => module.Descriptor));
        }

        public string GetStateJson(GlobalVariables settings)
        {
            var state = m_Modules.Values.Select(module => new DynamicHdrpComponentRuntimeState
            {
                ComponentId = module.Descriptor.Id,
                Enabled = module.IsEnabled,
                Values = module.GetCurrentValues(settings),
            });

            return DynamicHdrpJson.BuildStateJson(state);
        }

        public void SetComponentEnabled(string componentId, bool enabled)
        {
            if (!m_Modules.TryGetValue(componentId, out var module))
            {
                Mod.Log.Info($"[LUMINA-DYNAMIC] Unknown component id '{componentId}'.");
                return;
            }

            module.SetEnabled(enabled);
            GlobalVariables.Instance.SetDynamicComponentState(componentId, enabled);

            if (GlobalVariables.Instance.SaveAutomatically)
            {
                GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
            }
        }

        public void SetPropertyValue(string componentId, string propertyId, string value)
        {
            if (!m_Modules.TryGetValue(componentId, out var module))
            {
                return;
            }

            if (!module.TrySetValue(propertyId, value))
            {
                return;
            }

            if (GlobalVariables.Instance.SaveAutomatically)
            {
                GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
            }
        }

        private static bool GetStoredEnabledState(string componentId, bool defaultValue)
        {
            bool? stored = GlobalVariables.Instance.GetDynamicComponentState(componentId);
            if (!stored.HasValue)
            {
                GlobalVariables.Instance.SetDynamicComponentState(componentId, defaultValue);
                return defaultValue;
            }

            return stored.Value;
        }
    }
}
