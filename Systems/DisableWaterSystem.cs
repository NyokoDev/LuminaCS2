using Game;
using Game.Rendering;
using Game.SceneFlow;
using Unity.Entities;

namespace Lumina.Systems
{
    /// <summary>
    /// “Automatically turns water rendering on when you’re playing or editing the map, and off when you’re not (like in menus or loading).”
    /// </summary>
    internal partial class DisableWaterSystem : SystemBase
    {
        /// <summary>
        /// / On update , check the current game mode and enable or disable water rendering accordingly.
        /// </summary>
        protected override void OnUpdate()
        {
            var waterRenderSystem = World.GetExistingSystemManaged<WaterRenderSystem>();
            if (waterRenderSystem == null)
                return;

            // Retrieve the current game mode safely
            var mode = GameManager.instance.gameMode;

            // Disable water rendering unless in Game or Editor mode
            if ((mode & GameMode.GameOrEditor) != 0)
            {
                waterRenderSystem.Enabled = true;
            }
            else
            {
                waterRenderSystem.Enabled = false;
            }
        }
    }
}
