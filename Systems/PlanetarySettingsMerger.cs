using Game.SceneFlow;
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
        if (!GlobalVariables.Instance.LatLongEnabled)
        {
        
            string errorMessage = "Please enable 'Latitude and Longitude Adjustments' in the options menu and try again.";

            Mod.Log.Info(errorMessage);

            var dialog = new SimpleMessageDialog(errorMessage);
            GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);
            return;
        }
        GlobalVariables.Instance.Latitude = obj;
    }

    internal static void SetLongitude(float obj)
    {
        if (!GlobalVariables.Instance.LatLongEnabled)
        {
            string errorMessage = "Please enable 'Latitude and Longitude Adjustments' in the options menu and try again.";

            Mod.Log.Info(errorMessage);

            var dialog = new SimpleMessageDialog(errorMessage);
            GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);
            return;
        }
        GlobalVariables.Instance.Longitude = obj;
    }


    protected override void OnUpdate()
    {
        // Your update logic here
    }
}
