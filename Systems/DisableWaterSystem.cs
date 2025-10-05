using Game;
using Game.Rendering;
using Game.SceneFlow;
using LuminaMod.XML;
using Unity.Entities;

namespace Lumina.Systems
{
    /// <summary>
    /// Automatically toggles water, lighting, terrain, and rendering systems based on game mode.
    /// SafeMode only disables water. Runs only in PerformanceMode.
    /// </summary>
    internal partial class DisableWaterSystem : SystemBase
    {
        private bool _lastState;       // Tracks last enabled state
        private GameMode _lastMode;    // Tracks last known game mode

        protected override void OnUpdate()
        {
            // Run only if PerformanceMode is on
            if (!GlobalVariables.Instance.PerformanceMode)
                return;

            var mode = GameManager.instance.gameMode;
            bool gameOrEditor = (mode & GameMode.GameOrEditor) != 0;

            // Only update if state or mode changed
            if (gameOrEditor == _lastState && mode == _lastMode)
                return;

            // Water system: disable if SafeMode is on
            var waterRenderSystem = World.GetExistingSystemManaged<WaterRenderSystem>();
            if (waterRenderSystem != null)
                waterRenderSystem.Enabled = !GlobalVariables.Instance.SafeMode && gameOrEditor;

            // Other systems: follow game mode
            var lightingSystem = World.GetExistingSystemManaged<LightingSystem>();
            if (lightingSystem != null)
                lightingSystem.Enabled = gameOrEditor;

            var renderingSystem = World.GetExistingSystemManaged<RenderingSystem>();
            if (renderingSystem != null)
                renderingSystem.Enabled = gameOrEditor;

            var terrainSystem = World.GetExistingSystemManaged<TerrainRenderSystem>();
            if (terrainSystem != null)
                terrainSystem.Enabled = gameOrEditor;

            _lastState = gameOrEditor;
            _lastMode = mode;
        }
    }
}
