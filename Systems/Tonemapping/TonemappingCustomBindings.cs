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

        internal static void UpdateTonemappingCustom()
        {
            GlobalVariables.Instance.ToeStrengthActive = IsToeStrengthActive;
        }

        internal static void HandleToeStrengthActive(float obj)
        {
            GlobalVariables.Instance.ToeStrengthValue = obj;
            UpdateTonemappingCustom();
            RenderEffectsSystem.SetTonemappingCustomModeProperties();
        }

        internal static void SetToeStrength()
        {
            IsToeStrengthActive = !IsToeStrengthActive;
            RenderEffectsSystem.SetTonemappingCustomModeProperties();
        }
    }
}
