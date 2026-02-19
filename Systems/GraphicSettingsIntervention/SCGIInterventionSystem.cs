using LuminaMod.XML;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Lumina.Systems
{
    public partial class GraphicSettingsInterventionSystem : SystemBase
    {
        private GlobalIllumination m_GlobalIllumination;

        public void ToggleGlobalIllumination()
        {
            // Get the desired SSGI state
            bool luminaState = GlobalVariables.Instance?.IsSSGIInterventionEnabled ?? false;

            // Access the static volume
            Volume volume = RenderEffectsSystem.LuminaVolume;
            if (volume == null)
            {
                Lumina.Mod.Log.Info("[Intervention] No volume found, skipping SSGI toggle.");
                return;
            }

            // Get the profile safely
            VolumeProfile profile = volume.profile ?? volume.sharedProfile;
            if (profile == null)
            {
                Lumina.Mod.Log.Info("[Intervention] No volume profile found, skipping SSGI toggle.");
                return;
            }

            // Ensure the GlobalIllumination component exists
            if (!profile.TryGet(out m_GlobalIllumination) || m_GlobalIllumination == null)
            {
                m_GlobalIllumination = profile.Add<GlobalIllumination>();
                Lumina.Mod.Log.Info("[Intervention] GlobalIllumination override added.");
            }

            // Set the component state
            m_GlobalIllumination.active = luminaState;
            m_GlobalIllumination.enable.overrideState = luminaState;

            if (m_GlobalIllumination.enable.value != luminaState)
            {
                m_GlobalIllumination.enable.value = luminaState;
                Lumina.Mod.Log.Info($"[Intervention] SSGI enforced → {luminaState}");
            }
        }


        protected override void OnUpdate()
        {
            // Optionally, you could call ToggleGlobalIllumination() here
            // every frame if you want real-time sync with the bool
        }
    }
}
