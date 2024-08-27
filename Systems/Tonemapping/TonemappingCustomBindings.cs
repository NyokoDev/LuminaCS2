namespace Lumina.Systems.Tonemapping_Custom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LuminaMod.XML;

    public class TonemappingCustomBindings
    {
        public static bool IsToeStrengthActive = GlobalVariables.Instance.ToeStrengthActive;
        public static float ToeStrengthValue = GlobalVariables.Instance.ToeStrengthValue;

        internal static void UpdateTonemappingCustom()
        {
            GlobalVariables.Instance.ToeStrengthActive = IsToeStrengthActive;
        }

        internal static void HandleToeStrengthActive(float obj)
        {
            GlobalVariables.Instance.ToeStrengthValue = obj;
            RenderEffectsSystem.SetToeStrengthValue();
        }

        internal static void SetToeStrength()
        {
            IsToeStrengthActive = !IsToeStrengthActive;
            RenderEffectsSystem.ToggleToeStrength();

        }

        internal static void SetToeLengthActive()
        {
            GlobalVariables.Instance.ToeLengthActive = !GlobalVariables.Instance.ToeLengthActive;
            RenderEffectsSystem.ToggleToeLength();
        }

        internal static void HandleToeLengthActive(float obj)
        {
            GlobalVariables.Instance.ToeLengthValue = obj;
            RenderEffectsSystem.SetToeLengthValue();
        }

        internal static void SetShoulderStrengthActive()
        {
            RenderEffectsSystem.SetShoulderStrengthActive();
        }

        internal static void handleShoulderStrength(float obj)
        {
            GlobalVariables.Instance.shoulderStrengthValue = obj;
            RenderEffectsSystem.handleShoulderStrength();
        }
    }
}
