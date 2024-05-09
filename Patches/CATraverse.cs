using Game.Rendering;
using HarmonyLib;
using Lumina.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.HighDefinition;

namespace Lumina.Patches
{
        [HarmonyPatch(typeof(PhotoModeRenderSystem), "OnUpdate")]
        class PhotoModeRenderSystemPatch
        {
            static void Postfix(PhotoModeRenderSystem __instance)
            {
                // Retrieve the value of the private field m_ColorAdjustments
                ColorAdjustments colorAdjustments = Traverse.Create(__instance).Field("m_ColorAdjustments").GetValue<ColorAdjustments>();
                PhotoModeExtractor.SavedColorAdjustments = colorAdjustments;
          
            }
        }

    }

