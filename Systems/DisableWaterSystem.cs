using Game;
using Game.Rendering;
using Game.SceneFlow;
using Unity.Entities;

namespace Lumina.Systems
{
    /// <summary>
    /// Automatically toggles water and lighting systems based on game mode.
    /// Disabled in menus/loading screens, enabled in Game or Editor mode.
    /// </summary>
    internal partial class DisableWaterSystem : SystemBase
    {
        private bool _lastState;       // Tracks last enabled state
        private GameMode _lastMode;    // Tracks last known game mode

        protected override void OnUpdate()
        {
            var mode = GameManager.instance.gameMode;
            bool shouldEnable = (mode & GameMode.GameOrEditor) != 0;

            // Only update if state or mode changed
            if (shouldEnable == _lastState && mode == _lastMode)
                return;

            // Water system
            var waterRenderSystem = World.GetExistingSystemManaged<WaterRenderSystem>();
            if (waterRenderSystem != null)
                waterRenderSystem.Enabled = shouldEnable;

            // Lighting system
            var lightingSystem = World.GetExistingSystemManaged<LightingSystem>();
            if (lightingSystem != null)
                lightingSystem.Enabled = shouldEnable;

            _lastState = shouldEnable;
            _lastMode = mode;
        }
    }
}
