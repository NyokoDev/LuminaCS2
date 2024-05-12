using LuminaMod.XML;
using System;

namespace Lumina.Systems
{
    internal class PlanetarySettingsMerger
    {
        internal static float LatitudeValue()
        {
            return GlobalVariables.Instance.Latitude;
        }

        internal static float LongitudeValue()
        {
            return GlobalVariables.Instance.Longitude;
        }

        internal static void SetLatitude(float obj)
        {
            GlobalVariables.Instance.Latitude = obj;
        }

        internal static void SetLongitude(float obj)
        {
            GlobalVariables.Instance.Longitude = obj;
        }
    }
}