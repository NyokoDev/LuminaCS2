using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Systems.Tonemapping_Custom
{
    public class TonemappingCustomBindings
    {
        public static bool IsToeStrengthActive { get; set; }


        internal static void SetToeStrength()
        {
            GlobalVariables.Instance.ToeStrengthActive = !GlobalVariables.Instance.ToeStrengthActive;
            IsToeStrengthActive = GlobalVariables.Instance.ToeStrengthActive;
        }





    }
}
