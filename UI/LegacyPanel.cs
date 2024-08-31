using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

namespace Lumina.UI
{
    using Game.Rendering.CinematicCamera;
    using Game.UI;
    using Lumina.Systems;
    using Lumina.XML;
    using LuminaMod.XML;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;

    public class SliderPanel : MonoBehaviour
    {
        public static float slider1Value { get; set; } = GlobalVariables.Instance.PostExposure;
        public static float slider2Value { get; set; } = GlobalVariables.Instance.Contrast;
        public static float slider3Value { get; set; } = GlobalVariables.Instance.HueShift;
        public static float slider4Value { get; set; } = GlobalVariables.Instance.Saturation;
        public static float LongitudeValue { get; set; } = GlobalVariables.Instance.Longitude;
        public static float LatitudeValue { get; set; } = GlobalVariables.Instance.Latitude;
        public static float TemperatureSpace { get; set; } = GlobalVariables.Instance.Temperature;
        public static float TintSpace { get; set; } = GlobalVariables.Instance.Tint;
        public static float ShadowsSpace { get; set; } = GlobalVariables.Instance.Shadows;
        public static float MidtonesSpace { get; set; } = GlobalVariables.Instance.Midtones;


        public static bool showResetDialog = false;
        private static bool showPhotoModeSendDialog = false;
        public static bool Updating = false;
        public static bool OnReset = false;
        public static bool panelVisible = false;
        private bool secondpanelVisible = false;

        private Rect panelRect = new Rect(10, 10, 400, 610);
        private Rect buttonRect = new Rect(10, 10, 150, 30);
        private Vector2 panelOffset;
        private Vector2 buttonOffset;
        private bool isDraggingPanel = false;
        private bool isDraggingButton = false;
        private Vector2 buttonDragStartPosition;

        public static bool buttonVisible { get; set; }
        public static bool AllGood { get; set; }
        public static bool ImportPreset { get; set; }

        public static Dictionary<string, PhotoModeProperty> PhotoModePropertiesCopy = new Dictionary<string, PhotoModeProperty>();


        /// <summary>
        /// Toggle panel with ALT + C
        /// </summary>
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    panelVisible = !panelVisible;

                }
            }
        }

        void Start()
        {
            // Find all GameObjects with Volume components
            Volume[] volumes = FindObjectsOfType<Volume>();

            // Iterate through each volume and log its information
            foreach (Volume volume in volumes)
            {
                Mod.Log.Info("Volume found: " + volume.gameObject.name);

                // You can access and log various properties of the volume here
                // For example, you can access its profile, parameters, etc.
            }



        }

        private void OnGUI()
        {

            if (ImportPreset)
            {

            }

            if (buttonVisible)
            {
                buttonRect = GUI.Window(1, buttonRect, ButtonWindow, "Lumina");
            }

            if (panelVisible)
            {
                GUIStyle blackWindowStyle = new GUIStyle(GUI.skin.window);
                blackWindowStyle.normal.textColor = Color.white;

                panelRect = GUI.Window(0, panelRect, PanelWindow, "Lumina", blackWindowStyle);
            }

            // Add another if statement here
            if (secondpanelVisible)
            {
                GUIStyle blackWindowStyle = new GUIStyle(GUI.skin.window);
                blackWindowStyle.normal.textColor = Color.white;
                panelRect = GUI.Window(0, panelRect, SecondPanel, "Lumina", blackWindowStyle);
            }

            if (showResetDialog)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 75, 300, 150), ResetDialogWindow, "Confirmation");
            }

            if (showPhotoModeSendDialog)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 75, 400, 150), SendSettingsToLuminaConfirmation, "Confirmation");
            }







            // else block can be omitted if you don't want to do anything when panelVisible is false
        }


        private void SendSettingsToLuminaConfirmation(int windowID)
        {
            GUI.Label(new Rect(50, 30, 400, 30), "Send settings to Lumina? This will override existing settings.");

            if (GUI.Button(new Rect(50, 80, 80, 30), "Yes"))
            {
                PhotoModeExtractor.ExtractColorAdjustments();
                showPhotoModeSendDialog = false;
            }

            if (GUI.Button(new Rect(170, 80, 80, 30), "No"))
            {
                showPhotoModeSendDialog = false;
                // Do nothing
            }
        }





        void ButtonWindow(int windowID)
        {

            // Draw the button
            if (GUI.Button(new Rect(0, 0, buttonRect.width, buttonRect.height), "Lumina"))
            {
                panelVisible = !panelVisible; // Toggle panel visibility
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                isDraggingButton = true;
                buttonDragStartPosition = Event.current.mousePosition - buttonRect.position;
            }

            if (isDraggingButton)
            {
                buttonRect.position = Event.current.mousePosition - buttonDragStartPosition;
            }

        }



        void PanelWindow(int windowID)
        {
            float YControl = 20;
            GUI.DragWindow(new Rect(0, 0, panelRect.width, 30));

            // Add 'x' button at the top-right corner
            if (GUI.Button(new Rect(panelRect.width - 20, YControl, 19, 20), "X"))
            {
                // Close the window
                panelVisible = false;
                secondpanelVisible = false;
            }

            if (GUI.Button(new Rect(20, YControl, 100, 20), "Shadows"))
            {
                if (panelVisible)
                {
                    panelVisible = false;
                    secondpanelVisible = true;
                }
            }


            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Post Exposure");
            Rect Slider1Rect = new Rect(20, 40, 300, 20);
            slider1Value = GUI.HorizontalSlider(Slider1Rect, slider1Value, -20f, 15f);
            slider1Value = Mathf.Round(slider1Value * 1000f) / 1000f; // Set step size of 0.001f
            GlobalVariables.Instance.PostExposure = slider1Value;

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Contrast");
            Rect Slider2Rect = new Rect(20, YControl += 30, 300, 20);
            slider2Value = GUI.HorizontalSlider(Slider2Rect, slider2Value, -100f, 100f);
            slider2Value = Mathf.Round(slider2Value * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Contrast = slider2Value;

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Hue Shift");
            Rect Slider3Rect = new Rect(20, YControl += 30, 300, 20);
            slider3Value = GUI.HorizontalSlider(Slider3Rect, slider3Value, -180f, 180f);
            slider3Value = Mathf.Round(slider3Value * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.HueShift = slider3Value;

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Saturation");
            Rect Slider4Rect = new Rect(20, YControl += 30, 300, 20);
            slider4Value = GUI.HorizontalSlider(Slider4Rect, slider4Value, -100f, 100f);
            slider4Value = Mathf.Round(slider4Value * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Saturation = slider4Value;


            // Planetary Settings

            GUI.skin.label.wordWrap = false;
            GUI.skin.label.fontSize = 14; // Or any other size you prefer
            GUI.Label(new Rect(20, YControl += 31, 100, 20), "Planet Settings");

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Longitude");
            Rect LongitudeRect = new Rect(20, YControl += 20, 300, 20);
            LongitudeValue = GUI.HorizontalSlider(LongitudeRect, LongitudeValue, -180f, 180f);
            LongitudeValue = Mathf.Round(LongitudeValue * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Longitude = LongitudeValue;


            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Latitude");
            Rect LatitudeRect = new Rect(20, YControl += 20, 300, 20);
            LatitudeValue = GUI.HorizontalSlider(LatitudeRect, LatitudeValue, -90f, 90f);
            LatitudeValue = Mathf.Round(LatitudeValue * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Latitude = LatitudeValue;





            string newText1Value = GUI.TextField(new Rect(330, Slider1Rect.y, 40, 20), slider1Value.ToString());
            if (float.TryParse(newText1Value, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue1))
            {
                slider1Value = parsedValue1;
            }

            string newText2Value = GUI.TextField(new Rect(330, Slider2Rect.y, 40, 20), slider2Value.ToString());
            if (float.TryParse(newText2Value, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue2))
            {
                slider2Value = parsedValue2;
            }

            string newText3Value = GUI.TextField(new Rect(330, Slider3Rect.y, 40, 20), slider3Value.ToString());
            if (float.TryParse(newText3Value, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue3))
            {
                slider3Value = parsedValue3;
            }

            string newText4Value = GUI.TextField(new Rect(330, Slider4Rect.y, 40, 20), slider4Value.ToString());
            if (float.TryParse(newText4Value, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue4))
            {
                slider4Value = parsedValue4;
            }

            string LongitudeValueParsed = GUI.TextField(new Rect(330, LongitudeRect.y, 40, 20), LongitudeValue.ToString());
            if (float.TryParse(LongitudeValueParsed, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue5))
            {
                LongitudeValue = parsedValue5;
            }

            string LatitudeValueParsed = GUI.TextField(new Rect(330, LatitudeRect.y, 40, 20), LatitudeValue.ToString());
            if (float.TryParse(LatitudeValueParsed, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out float parsedValue6))
            {
                LatitudeValue = parsedValue6;
            }

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "White Balance");
            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Temperature");
            Rect TemperatureRect = new Rect(20, YControl += 20, 300, 20);
            TemperatureSpace = GUI.HorizontalSlider(TemperatureRect, TemperatureSpace, -100f, 100f);
            TemperatureSpace = Mathf.Round(TemperatureSpace * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Temperature = TemperatureSpace;

            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Tint");
            Rect TintRect = new Rect(20, YControl += 20, 300, 20);
            TintSpace = GUI.HorizontalSlider(TintRect, TintSpace, -100f, 100f);
            TintSpace = Mathf.Round(TintSpace * 1000f) / 1000f; // Set step size
            GlobalVariables.Instance.Tint = TintSpace;

            TintSpace = float.TryParse(GUI.TextField(new Rect(330, TintRect.y, 40, 20), TintSpace.ToString()), out float parsedTintSpace) ? parsedTintSpace : TintSpace;
            TemperatureSpace = float.TryParse(GUI.TextField(new Rect(330, TemperatureRect.y, 40, 20), TemperatureSpace.ToString()), out float parsedTemperatureSpace) ? parsedTemperatureSpace : TemperatureSpace;




#if DEBUG
            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Sha/Mid/HighL");
            GUI.Label(new Rect(20, YControl += 30, 100, 20), "Shadows");

            Rect ShadowsSpaceRect = new Rect(20, YControl += 20, 300, 20);
            Vector4 oldShadowsValue = Shadows.value;
            oldShadowsValue.x = GUI.HorizontalSlider(ShadowsSpaceRect, oldShadowsValue.x, -10000f, 10000f);
            Shadows.value = oldShadowsValue;
            GlobalVariables.Instance.Shadows = Shadows;
#endif
            Rect Button1Rect = (new Rect(20, YControl += 30, 100, 20));
            if (GUI.Button(Button1Rect, "Save"))
            {
                GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
            }


            if (GUI.Button(new Rect(Button1Rect.x, YControl += 30, 100, 20), "Reset Settings"))
            {
                showResetDialog = true;

            }





            GUI.Label(new Rect(20, YControl += 30, 100, 20), "v" + GlobalPaths.Version);










            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                isDraggingPanel = true;
                panelOffset = panelRect.position - Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingPanel = false;
            }




        }












        private void ResetDialogWindow(int windowID)
        {
            GUI.Label(new Rect(50, 30, 200, 30), "Are you sure you want to reset settings?");

            if (GUI.Button(new Rect(50, 80, 80, 30), "Yes"))
            {
                ResetSettings();
                showResetDialog = false;
            }

            if (GUI.Button(new Rect(170, 80, 80, 30), "No"))
            {
                showResetDialog = false;
            }
        }




        /// <summary>
        /// Second Panel. Shadows MidTones etc.
        /// </summary>
        /// <param name="windowID"></param>

        void SecondPanel(int windowID)
        {
            float YControl = 20;
            GUI.DragWindow(new Rect(0, 0, panelRect.width, 30));

            // Add 'x' button at the top-right corner
            if (GUI.Button(new Rect(panelRect.width - 20, YControl, 19, 20), "X"))
            {
                // Close the window
                panelVisible = false;
                secondpanelVisible = false;
            }

            if (GUI.Button(new Rect(20, YControl, 100, 20), "Go Back"))
            {
                if (secondpanelVisible)
                {
                    panelVisible = true;
                    secondpanelVisible = false;
                }
            }

            GUI.Label(new Rect(20, YControl += 30, 150, 20), "Shadow Intensity");
            Rect Slider1Rect = new Rect(20, YControl += 20, 300, 20);
            ShadowsSpace = Mathf.Round(GUI.HorizontalSlider(Slider1Rect, ShadowsSpace, -1f, 1f) * 1000f) / 1000f;
            GlobalVariables.Instance.Shadows = ShadowsSpace;

            ShadowsSpace = float.TryParse(GUI.TextField(new Rect(330, Slider1Rect.y, 40, 20), ShadowsSpace.ToString()), out float parsedShadowsSpace) ? parsedShadowsSpace : ShadowsSpace;



            //MidtonesGame.UI.InGame.PhotoModeUISystem

            GUI.Label(new Rect(20, YControl += 30, 150, 20), "Midtones");
            Rect Slider2Rect = new Rect(20, YControl += 20, 300, 20);
            MidtonesSpace = Mathf.Round(GUI.HorizontalSlider(Slider2Rect, MidtonesSpace, -1f, 1f) * 1000f) / 1000f;
            GlobalVariables.Instance.Midtones = MidtonesSpace;

            MidtonesSpace = float.TryParse(GUI.TextField(new Rect(330, Slider2Rect.y, 40, 20), MidtonesSpace.ToString()), out float parsedMidtonesSpace) ? parsedMidtonesSpace : MidtonesSpace;
        }

        public static void ResetSettings()
        {

            GlobalVariables.Instance.PostExposure =  0f;
            GlobalVariables.Instance.PostExposureActive =  false;
            GlobalVariables.Instance.Contrast =  0f;
            GlobalVariables.Instance.ContrastActive = false;
            GlobalVariables.Instance.HueShift = 0f;
            GlobalVariables.Instance.HueShiftActive =  false;
            GlobalVariables.Instance.Saturation =   0f;
            GlobalVariables.Instance.SaturationActive =  false;

            GlobalVariables.Instance.Latitude = GetLatitude();
            GlobalVariables.Instance.Longitude = GetLongitude();

            GlobalVariables.Instance.Temperature = 0f;
            GlobalVariables.Instance.TemperatureActive = false;
            GlobalVariables.Instance.Tint = 0f;
            GlobalVariables.Instance.TintActive = false;

            GlobalVariables.Instance.Shadows = 0f;
            GlobalVariables.Instance.ShadowsActive = false;
            GlobalVariables.Instance.Midtones = 0f;
            GlobalVariables.Instance.MidtonesActive = false;
            GlobalVariables.Instance.Highlights = 0f;
            GlobalVariables.Instance.HighlightsActive = false;

            // Tonemapping
            GlobalVariables.Instance.TonemappingMode = TonemappingMode.None;
            GlobalVariables.Instance.LUTContribution = 0f;
            GlobalVariables.Instance.LUTName = "None";
            GlobalVariables.Instance.SceneFlowCheckerEnabled = false;

            GlobalVariables.Instance.ToeStrengthActive = false;
            GlobalVariables.Instance.ToeStrengthValue = 0f;
            GlobalVariables.Instance.ToeLengthActive = false;
            GlobalVariables.Instance.ToeLengthValue = 0f;

            GlobalVariables.Instance.shoulderStrengthActive = false;
            GlobalVariables.Instance.shoulderStrengthValue = 0f;
            GlobalVariables.Instance.shoulderLengthActive = false;
            GlobalVariables.Instance.shoulderAngleActive = false;
            GlobalVariables.Instance.shoulderAngleValue = 0f;

            GlobalVariables.Instance.TonemappingGammaActive = false;
            GlobalVariables.Instance.TonemappingGammaValue = 0f;
            GlobalVariables.Instance.SaveAutomatically = true;
            GlobalVariables.Instance.CubemapName = "None";
            GlobalVariables.Instance.spaceEmissionMultiplier = 100f;
            GlobalVariables.Instance.HDRISkyEnabled = false;

            GlobalVariables.Instance.CustomSunEnabled = false;
            GlobalVariables.Instance.AngularDiameter = 0f;
            GlobalVariables.Instance.SunIntensity = 0f;
            GlobalVariables.Instance.SunFlareSize = 0f;

            RenderEffectsSystem.DisableCubemap(); // Disable cubemap.

            // Save changes
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        private static float GetLongitude()
        {
            return PlanetarySettingsMerger.CurrentLongitude;
        }

        private static float GetLatitude()
        {
            return PlanetarySettingsMerger.CurrentLatitude;
        }

        internal static void Toggle()
        {
            panelVisible = !panelVisible;
        }

        internal static void SendToLumina()
        {
            showPhotoModeSendDialog = true;

        }

        public static void CheckUp()
        {
            // Set AllGood to false initially
            SliderPanel.AllGood = false;

            // Iterate through each property in PhotoModePropertiesCopy
            foreach (KeyValuePair<string, PhotoModeProperty> kvp in SliderPanel.PhotoModePropertiesCopy)
            {
                string propertyName = kvp.Key;
                PhotoModeProperty property = kvp.Value;

                // Check if the property matches any condition and SliderPanel is updating
                if (SliderPanel.Updating)
                {
                    if (propertyName == "ShadowsMidtonesHighlights.shadows")
                    {
                        // Handle ShadowsMidtonesHighlights.shadows property
                    }
                    else if (propertyName == "ColorAdjustments.postExposure")
                    {
                        // Handle ColorAdjustments.postExposure property
                    }
                    else if (propertyName == "ColorAdjustments.contrast")
                    {
                        // Handle ColorAdjustments.contrast property
                        float contrastValue = property.getValue();
                        Mod.Log.Info("Contrast value " + contrastValue);
                    }
                    else if (propertyName == "ColorAdjustments.hueShift")
                    {
                        // Handle ColorAdjustments.hueShift property
                        float value = property.getValue();
                        SliderPanel.slider3Value = value;
                    }
                    else if (propertyName == "ColorAdjustments.saturation")
                    {
                        // Handle ColorAdjustments.saturation property
                        float value = property.getValue();
                        SliderPanel.slider4Value = value;
                    }
                    else if (propertyName == "WhiteBalance.temperature")
                    {
                        // Handle WhiteBalance.temperature property
                        float value = property.getValue();
                        SliderPanel.TemperatureSpace = value;
                    }
                    else if (propertyName == "ShadowsMidtonesHighlights.midtones")
                    {
                        // Handle ShadowsMidtonesHighlights.midtones property
                        float value = property.getValue();
                        SliderPanel.MidtonesSpace = value;
                    }
                    else if (propertyName == "WhiteBalance.tint")
                    {
                        // Handle WhiteBalance.tint property
                        float value = property.getValue();
                        SliderPanel.TintSpace = value;
                    }
                }
            }

            // Set AllGood to true after all properties have been checked

            SliderPanel.Updating = false;
            SliderPanel.AllGood = true;
        }

    }
}
