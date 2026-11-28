namespace Lumina.ManagerSystems
{
    using Lumina.Systems;
    using Unity.Entities;

    internal partial class CustomSunManager : SystemBase
    {
        protected override void OnUpdate()
        {
            RenderEffectsSystem.AdjustAngularDiameter();
        }
    }
}
