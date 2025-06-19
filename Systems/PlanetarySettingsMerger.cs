using Lumina;
using Lumina.Systems;
using LuminaMod.XML;

internal partial class PlanetarySettingsMerger : RenderEffectsSystem
{
    public static float CurrentLatitude { get; set; }
    public static float CurrentLongitude { get; set; }


    protected override void OnCreate()
    {
        base.OnCreate();
    }

    internal static float LatitudeValue() => GlobalVariables.Instance.Latitude;
    internal static float LongitudeValue() => GlobalVariables.Instance.Longitude;

    internal static void SetLatitude(float obj)
    {
        GlobalVariables.Instance.Latitude = obj;
    }

    internal static void SetLongitude(float obj)
    {
        GlobalVariables.Instance.Longitude = obj;
    }


    protected override void OnUpdate()
    {
        // Your update logic here
    }
}
