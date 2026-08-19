using Colossal.UI.Binding;
using Game.Settings;
using Lumina.Systems;
using Lumina.XML;
using LuminaMod.XML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


namespace Lumina.NGX
{

    internal partial class NGXUISystem : ExtendedUISystemBase
    {

        private Volume selectedVolume;
        private bool ngxMode;
        private VolumeComponent selectedComponent;
        private string selectedProperty;

        private const int NGXSaveFormatVersion = 1;
        private const string NGXSaveExtension = ".ngx";
        private bool isApplyingNGXSave;
        private NGXSaveFile trackedNGXChanges = new NGXSaveFile();

        // ===============================
        // PERFORMANCE CACHES
        // ===============================
        private string[] cachedVolumes = Array.Empty<string>();
        private Dictionary<string, Volume> cachedVolumeObjects = new Dictionary<string, Volume>(StringComparer.OrdinalIgnoreCase);
        private bool volumesDirty = true;

        private string[] cachedNGXSaves = Array.Empty<string>();
        private bool ngxSavesDirty = true;

        private string[] cachedVolumeComponents = Array.Empty<string>();
        private string[] cachedComponentStates = Array.Empty<string>();
        private bool componentsDirty = true;

        private string[] cachedComponentProperties = Array.Empty<string>();
        private bool componentPropertiesDirty = true;

        private Dictionary<string, Type> cachedVolumeComponentTypes;
        private string[] cachedAvailableComponents;

        [Serializable]
        public sealed class NGXSaveFile
        {
            public int formatVersion = NGXSaveFormatVersion;
            public List<NGXVolumeChange> volumes = new List<NGXVolumeChange>();
        }

        [Serializable]
        public sealed class NGXVolumeChange
        {
            public string volumeName = string.Empty;

            public List<NGXComponentChange> components =
                new List<NGXComponentChange>();
        }

        [Serializable]
        public sealed class NGXComponentChange
        {
            public string componentName = string.Empty;

            // -1 unchanged
            //  0 removed
            //  1 exists
            public int existence = -1;

            public bool hasActiveState;
            public bool activeState;

            public List<NGXPropertyChange> properties =
                new List<NGXPropertyChange>();
        }

        [Serializable]
        public sealed class NGXPropertyChange
        {
            public string propertyName = string.Empty;
            public string value = string.Empty;
            public bool overrideState;
        }

        /// <summary>
        /// Gets the properties for the currently selected component.
        /// </summary>
        /// <returns></returns>
        private string[] GetComponentProperties()
        {
            if (!componentPropertiesDirty)
                return cachedComponentProperties;

            if (selectedComponent == null)
            {
                cachedComponentProperties = Array.Empty<string>();
                componentPropertiesDirty = false;
                return cachedComponentProperties;
            }

            cachedComponentProperties = selectedComponent
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => typeof(VolumeParameter).IsAssignableFrom(f.FieldType))
                .Select(f =>
                {
                    var parameter = (VolumeParameter)f.GetValue(selectedComponent);
                    string value = "";

                    if (parameter != null)
                    {
                        var valueProperty = parameter.GetType().GetProperty("value");
                        if (valueProperty != null && valueProperty.CanRead)
                            value = valueProperty.GetValue(parameter)?.ToString() ?? "";
                    }

                    return $"{f.Name}|{parameter?.GetType().Name}|{value}";
                })
                .ToArray();

            componentPropertiesDirty = false;
            return cachedComponentProperties;
        }

        /// <summary>
        /// Gets the parameter value for a given VolumeParameter using reflection to access the private m_Value field.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private string GetParameterValue(VolumeParameter parameter)
        {
            var valueField = parameter
                .GetType()
                .GetField(
                    "m_Value",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic
                );

            if (valueField != null)
            {
                object value = valueField.GetValue(parameter);
                return value?.ToString() ?? "";
            }

            return parameter.ToString();
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddBinding(new TriggerBinding<string>(Mod.MODUI, "SaveNGX", SaveNGX));
            AddBinding(new TriggerBinding<string>(Mod.MODUI, "LoadNGX", LoadNGX));
            AddBinding(new TriggerBinding<string>(Mod.MODUI, "DeleteNGX", DeleteNGX));
            AddUpdateBinding(
                new GetterValueBinding<string[]>(
                    Mod.MODUI,
                    "GetNGXSaves",
                    GetNGXSaves,
                    new ArrayWriter<string>()
                )
            );

            AddBinding(
    new TriggerBinding<string>(
        Mod.MODUI,
        "InspectProperty",
        InspectProperty
    )
);



            AddBinding(
               new TriggerBinding<string>(
                   Mod.MODUI,
                   "SetProperty",
                   SetProperty
               )
           );

            AddUpdateBinding(
    new GetterValueBinding<string[]>(
        Mod.MODUI,
        "GetComponentStates",
        GetComponentStates,
        new ArrayWriter<string>()
    )
);


            AddBinding(
    new TriggerBinding<string>(
        Mod.MODUI,
        "ToggleComponent",
        ToggleComponent
    )
);


            AddBinding(
             new TriggerBinding<string>(
                 Mod.MODUI,
                 "RemoveComponent",
                 RemoveComponent
             )
         );



            AddUpdateBinding(
    new GetterValueBinding<string[]>(
        Mod.MODUI,
        "GetComponentProperties",
        GetComponentProperties,
        new ArrayWriter<string>()
    )
);
            AddBinding(
    new TriggerBinding<string>(
        Mod.MODUI,
        "SelectComponent",
        SelectComponent
    )
);

            AddUpdateBinding(
            new GetterValueBinding<bool>(
                Mod.MODUI,
                "NGXMode",
                () => GlobalVariables.Instance.NGXMode
            )
        );

            AddUpdateBinding(
      new GetterValueBinding<string[]>(
          Mod.MODUI,
          "GetVolumes",
          GetVolumes,
          new ArrayWriter<string>()
      )
  );

            AddUpdateBinding(
                new GetterValueBinding<string>(
                    Mod.MODUI,
                    "SelectedVolume",
                    GetSelectedVolume
                )
            );

            AddUpdateBinding(
                new GetterValueBinding<string[]>(
                    Mod.MODUI,
                    "GetVolumeComponents",
                    GetVolumeComponents,
                    new ArrayWriter<string>()
                )
            );

            AddUpdateBinding(
                new GetterValueBinding<string[]>(
                    Mod.MODUI,
                    "GetAvailableComponents",
                    GetAvailableComponents,
                    new ArrayWriter<string>()
                )
            );

            AddBinding(
    new TriggerBinding<string>(
        Mod.MODUI,
        "SelectVolume",
        SelectVolume
    )
);

            AddBinding(
                new TriggerBinding<string>(
                    Mod.MODUI,
                    "AddComponent",
                    AddComponent
                )
            );
        }


        // ===============================
        // SET / EDIT PROPERTY FROM CONSOLE
        // Format:
        // Volume|Component.Property|Value
        // ===============================

        private void SetProperty(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;


            string[] args = data.Split('|');


            if (args.Length != 3)
            {
                Mod.Log.Info(
                    "NGX: Invalid SetProperty format"
                );

                return;
            }


            string volumeName = args[0];
            string propertyPath = args[1];
            string value = args[2];


            Volume volume =
    UnityEngine.Object
    .FindObjectsOfType<Volume>()
    .FirstOrDefault(
        x => x.name.Equals(
            volumeName,
            StringComparison.OrdinalIgnoreCase
        )
    );


            if (volume == null)
            {
                Mod.Log.Info(
                    "NGX: Volume not found " + volumeName
                );

                return;
            }


            if (volume.profile == null)
                return;


            string[] propertyParts =
                propertyPath.Split('.');


            if (propertyParts.Length != 2)
            {
                Mod.Log.Info(
                    "NGX: Invalid property path"
                );

                return;
            }


            string componentName = propertyParts[0];
            string propertyName = propertyParts[1];


            VolumeComponent component =
    volume.profile.components
    .FirstOrDefault(
        x => x.GetType().Name.Equals(
            componentName,
            StringComparison.OrdinalIgnoreCase
        )
    );

            if (component == null)
            {
                Mod.Log.Info(
                    "NGX: Component not found " + componentName
                );

                return;
            }


            var field =
    component.GetType()
    .GetField(
        propertyName,
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.Public |
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.IgnoreCase
    );


            if (field == null)
            {
                Mod.Log.Info(
                    "NGX: Property not found " + propertyName
                );

                return;
            }


            object parameter = field.GetValue(component);

            if (parameter is VolumeParameter volumeParameter)
            {
                if (TrySetParameterValue(volumeParameter, value))
                {
                    TrackPropertyChange(volumeName, componentName, propertyName, volumeParameter);
                    componentPropertiesDirty = true;

                    Mod.Log.Info(
                        $"NGX: {componentName}.{propertyName} = {value}"
                    );
                }
                else
                {
                    Mod.Log.Info(
                        $"NGX: Unsupported parameter type ({parameter.GetType().Name})"
                    );
                }

                return;
            }

            Mod.Log.Info(
                $"NGX: {propertyName} is not a VolumeParameter.");

        }





        private bool TrySetParameterValue(VolumeParameter parameter, string value)
        {



            var culture = System.Globalization.CultureInfo.InvariantCulture;

            var valueProperty = parameter.GetType().GetProperty("value");
            Mod.Log.Info("Parameter Type: " + parameter.GetType().FullName);

            if (valueProperty == null)
            {
                Mod.Log.Info("valueProperty == NULL");
                return false;
            }

            Mod.Log.Info("Value Property Type: " + valueProperty.PropertyType.FullName);
            Mod.Log.Info("Incoming Value: " + value);

            if (valueProperty == null || !valueProperty.CanWrite)
                return false;

            Type valueType = valueProperty.PropertyType;

            try
            {
                if (valueType == typeof(float))
                {
                    valueProperty.SetValue(parameter,
                        float.Parse(value, culture));
                    return true;
                }

                if (valueType == typeof(int))
                {
                    valueProperty.SetValue(parameter,
                        int.Parse(value, culture));
                    return true;
                }

                if (valueType == typeof(bool))
                {
                    valueProperty.SetValue(parameter,
                        bool.Parse(value));
                    return true;
                }

                if (valueType == typeof(Color))
                {
                    Mod.Log.Info("Entered Color parser");
                    string colorValue = value.Trim();

                    // HTML colors (#FF0000)
                    if (ColorUtility.TryParseHtmlString(colorValue, out Color htmlColor))
                    {
                        valueProperty.SetValue(parameter, htmlColor);
                        return true;
                    }

                    // Remove wrappers
                    colorValue = colorValue
                        .Replace("RGBA(", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("RGB(", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("(", "")
                        .Replace(")", "")
                        .Trim();

                    string[] split = colorValue
                        .Split(',')
                        .Select(x => x.Trim())
                        .ToArray();

                    if (split.Length == 3 || split.Length == 4)
                    {
                        float r = float.Parse(split[0], culture);
                        float g = float.Parse(split[1], culture);
                        float b = float.Parse(split[2], culture);

                        float a = split.Length == 4
                            ? float.Parse(split[3], culture)
                            : 1f;

                        // Convert 255 colors into 0-1 automatically
                        if (r > 1f || g > 1f || b > 1f || a > 1f)
                        {
                            r /= 255f;
                            g /= 255f;
                            b /= 255f;
                            a /= 255f;
                        }

                        valueProperty.SetValue(parameter,
                            new Color(r, g, b, a));

                        Mod.Log.Info("Successfully assigned color");
                        return true;
                    }

                    return false;
                }

                if (valueType == typeof(Vector2))
                {
                    string[] split = value.Split(',');

                    if (split.Length == 2)
                    {
                        valueProperty.SetValue(parameter,
                            new Vector2(
                                float.Parse(split[0].Trim(), culture),
                                float.Parse(split[1].Trim(), culture)
                            ));

                        return true;
                    }

                    return false;
                }

                if (valueType == typeof(Vector3))
                {
                    string[] split = value.Split(',');

                    if (split.Length == 3)
                    {
                        Vector3 vector = new Vector3(
                            float.Parse(split[0].Trim(), culture),
                            float.Parse(split[1].Trim(), culture),
                            float.Parse(split[2].Trim(), culture)
                        );

                        valueProperty.SetValue(parameter, vector);

                        return true;
                    }

                    return false;
                }

                if (valueType == typeof(Vector4))
                {
                    string[] split = value.Split(',');

                    if (split.Length == 4)
                    {
                        valueProperty.SetValue(parameter,
                            new Vector4(
                                float.Parse(split[0].Trim(), culture),
                                float.Parse(split[1].Trim(), culture),
                                float.Parse(split[2].Trim(), culture),
                                float.Parse(split[3].Trim(), culture)
                            ));

                        return true;
                    }

                    return false;
                }

                if (valueType == typeof(LayerMask))
                {
                    valueProperty.SetValue(parameter,
                        (LayerMask)int.Parse(value, culture));
                    return true;
                }

                if (valueType.IsEnum)
                {
                    valueProperty.SetValue(parameter,
                        Enum.Parse(valueType, value, true));

                    return true;
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Info(ex.ToString());
                return false;
            }

            Mod.Log.Info($"Unsupported value type: {valueType.FullName}");

            return false;
        }

        // REMOVE COMPONENT
        // REMOVE COMPONENT
        private void RemoveComponent(string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
                return;


            if (selectedVolume == null)
                return;


            if (selectedVolume.profile == null)
                return;


            var component = selectedVolume.profile.components
                .FirstOrDefault(x => x.GetType().Name == componentName);


            if (component == null)
                return;

            TrackComponentExistence(selectedVolume.name, componentName, false);
            selectedVolume.profile.components.Remove(component);

            selectedComponent = null;
            componentsDirty = true;
            componentPropertiesDirty = true;
        }

        // SELECT COMPONENT
        private void SelectComponent(string name)
        {
            if (selectedVolume == null)
                return;

            if (selectedVolume.profile == null)
                return;

            selectedComponent =
                selectedVolume.profile.components
                .FirstOrDefault(x => x.GetType().Name == name);

            componentPropertiesDirty = true;
        }

        // ===============================
        // DYNAMIC VOLUME DISCOVERY
        // ===============================

        private string[] GetVolumes()
        {
            if (!volumesDirty)
                return cachedVolumes;

            Volume[] sceneVolumes = UnityEngine.Object.FindObjectsOfType<Volume>();

            cachedVolumeObjects = sceneVolumes
                .Where(x => x != null && !string.IsNullOrEmpty(x.name))
                .GroupBy(x => x.name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group.First(),
                    StringComparer.OrdinalIgnoreCase);

            cachedVolumes = cachedVolumeObjects.Keys.ToArray();

            volumesDirty = false;
            return cachedVolumes;
        }





        private string GetSelectedVolume()
        {
            return selectedVolume != null
                ? selectedVolume.name
                : "";
        }

        // COMPONENT STATES
        private void RefreshSelectedVolumeCaches()
        {
            if (selectedVolume == null || selectedVolume.profile == null)
            {
                cachedVolumeComponents = Array.Empty<string>();
                cachedComponentStates = Array.Empty<string>();
                componentsDirty = false;
                return;
            }

            var components = selectedVolume.profile.components;

            cachedVolumeComponents = components
                .Where(x => x != null)
                .Select(x => x.GetType().Name)
                .ToArray();

            cachedComponentStates = components
                .Where(x => x != null)
                .Select(x => $"{x.GetType().Name}|{x.active}")
                .ToArray();

            componentsDirty = false;
        }

        private string[] GetComponentStates()
        {
            if (componentsDirty)
                RefreshSelectedVolumeCaches();

            return cachedComponentStates;
        }

        // ===============================
        // READ CURRENT PROFILE
        // ===============================

        private string[] GetVolumeComponents()
        {
            if (componentsDirty)
                RefreshSelectedVolumeCaches();

            return cachedVolumeComponents;
        }

        // ===============================
        // AVAILABLE COMPONENT TYPE CACHE
        // ===============================

        private void EnsureVolumeComponentTypeCache()
        {
            if (cachedVolumeComponentTypes != null)
                return;

            cachedVolumeComponentTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(type =>
                    type != null &&
                    !type.IsAbstract &&
                    typeof(VolumeComponent).IsAssignableFrom(type))
                .GroupBy(type => type.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group.First(),
                    StringComparer.OrdinalIgnoreCase);

            cachedAvailableComponents = cachedVolumeComponentTypes.Keys
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private string[] GetAvailableComponents()
        {
            EnsureVolumeComponentTypeCache();
            return cachedAvailableComponents ?? Array.Empty<string>();
        }

        // ===============================
        // SELECT VOLUME
        // ===============================

        private void SelectVolume(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                selectedVolume = null;
                selectedComponent = null;
                componentsDirty = true;
                componentPropertiesDirty = true;
                return;
            }

            if (volumesDirty)
                GetVolumes();

            if (!cachedVolumeObjects.TryGetValue(name, out selectedVolume))
            {
                volumesDirty = true;
                GetVolumes();
                cachedVolumeObjects.TryGetValue(name, out selectedVolume);
            }

            selectedComponent = null;
            componentsDirty = true;
            componentPropertiesDirty = true;
        }

        // TOGGLE COMPONENT ACTIVE OR INACTIVE
        // TOGGLE COMPONENT ACTIVE AND INTERNAL ENABLE PARAMETER
        private void ToggleComponent(string componentName)
        {
            if (selectedVolume == null || selectedVolume.profile == null)
                return;


            var component = selectedVolume.profile.components
                .FirstOrDefault(x => x.GetType().Name == componentName);


            if (component == null)
                return;


            // Toggle component active state
            bool newState = !component.active;

            component.active = newState;
            TrackComponentActiveState(selectedVolume.name, componentName, newState);

            // Toggle internal "enable" BoolParameter if it exists
            var enableField = component.GetType().GetField(
                "enable",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic
            );


            if (enableField != null)
            {
                var parameter = enableField.GetValue(component);

                if (parameter is BoolParameter boolParameter)
                {
                    boolParameter.value = newState;
                }
            }

            componentsDirty = true;
            componentPropertiesDirty = true;
        }

        // INSPECT PROPERTY
        public void InspectProperty(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;


            string[] split = path.Split('.');


            if (split.Length != 2)
            {
                Mod.Log.Info(
                    "NGX: Invalid inspect path"
                );

                return;
            }


            string componentName = split[0];
            string propertyName = split[1];


            Type componentType =
                AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x =>
                {
                    try
                    {
                        return x.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }

                })
                .FirstOrDefault(x =>
                    x.Name == componentName &&
                    typeof(VolumeComponent)
                    .IsAssignableFrom(x)
                );


            if (componentType == null)
            {
                Mod.Log.Info(
                    "NGX: Component not found"
                );

                return;
            }


            var field =
                componentType.GetField(
                    propertyName,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic
                );


            if (field == null)
            {
                Mod.Log.Info(
                    "NGX: Property not found"
                );

                return;
            }


            Type valueType =
                field.FieldType
                .GetProperty("value")
                .PropertyType;


            Mod.Log.Info(
                $"AVAILABLE ADJUSTMENTS FOR {path}:"
            );


            if (valueType.IsEnum)
            {
                foreach (var value in Enum.GetNames(valueType))
                {
                    Mod.Log.Info(value);
                }

                return;
            }


            if (valueType == typeof(bool))
            {
                Mod.Log.Info("true");
                Mod.Log.Info("false");

                return;
            }


            if (valueType == typeof(Color))
            {
                Mod.Log.Info(
                    "Format: RGBA(r,g,b,a)"
                );

                return;
            }


            if (valueType == typeof(Vector2))
            {
                Mod.Log.Info(
                    "Format: (x,y)"
                );

                return;
            }


            if (valueType == typeof(Vector3))
            {
                Mod.Log.Info(
                    "Format: (x,y,z)"
                );

                return;
            }


            if (valueType == typeof(Vector4))
            {
                Mod.Log.Info(
                    "Format: (x,y,z,w)"
                );

                return;
            }


            Mod.Log.Info(
                "Numeric value"
            );
        }




        // ===============================
        // NGX SAVE / LOAD
        // ===============================

        private string NGXSaveDirectory
        {
            get { return Path.Combine(GlobalPaths.AssemblyDirectory, "NGXPresets"); }
        }

        private string[] GetNGXSaves()
        {
            if (!ngxSavesDirty)
                return cachedNGXSaves;

            try
            {
                if (!Directory.Exists(NGXSaveDirectory))
                {
                    cachedNGXSaves = Array.Empty<string>();
                }
                else
                {
                    cachedNGXSaves = Directory
                        .GetFiles(NGXSaveDirectory, "*" + NGXSaveExtension)
                        .Select(Path.GetFileNameWithoutExtension)
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                }
            }
            catch
            {
                cachedNGXSaves = Array.Empty<string>();
            }

            ngxSavesDirty = false;
            return cachedNGXSaves;
        }

        private void SaveNGX(string saveName)
        {
            try
            {
                string safeName = SanitizeSaveName(saveName);

                if (string.IsNullOrEmpty(safeName))
                    safeName = "NGX Preset";

                Directory.CreateDirectory(NGXSaveDirectory);

                NGXSaveFile save = CaptureCurrentNGXState();

                string json = JsonConvert.SerializeObject(
                    save,
                    Formatting.Indented
                );

                string path = Path.Combine(
                    NGXSaveDirectory,
                    safeName + NGXSaveExtension
                );

                string tempPath = path + ".tmp";

                File.WriteAllText(tempPath, json);

                if (File.Exists(path))
                    File.Delete(path);

                File.Move(tempPath, path);

                trackedNGXChanges = save;
                ngxSavesDirty = true;

                Mod.Log.Info(
                    $"NGX preset saved: {safeName} | Volumes: {save.volumes.Count}"
                );
            }
            catch (Exception ex)
            {
                Mod.Log.Info(
                    "NGX save failed: " + ex
                );
            }
        }

        private NGXSaveFile CaptureCurrentNGXState()
        {
            NGXSaveFile save = new NGXSaveFile
            {
                formatVersion = NGXSaveFormatVersion,
                volumes = new List<NGXVolumeChange>()
            };

            Volume[] sceneVolumes =
                UnityEngine.Object.FindObjectsOfType<Volume>();

            foreach (Volume volume in sceneVolumes)
            {
                if (volume == null)
                    continue;

                if (volume.profile == null)
                    continue;

                if (string.IsNullOrEmpty(volume.name))
                    continue;

                NGXVolumeChange volumeSave = new NGXVolumeChange
                {
                    volumeName = volume.name,
                    components = new List<NGXComponentChange>()
                };

                foreach (VolumeComponent component in volume.profile.components)
                {
                    if (component == null)
                        continue;

                    NGXComponentChange componentSave =
                        new NGXComponentChange
                        {
                            componentName = component.GetType().Name,

                            existence = 1,

                            hasActiveState = true,
                            activeState = component.active,

                            properties = new List<NGXPropertyChange>()
                        };

                    FieldInfo[] fields =
                        component.GetType().GetFields(
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic
                        );

                    foreach (FieldInfo field in fields)
                    {
                        if (!typeof(VolumeParameter)
                            .IsAssignableFrom(field.FieldType))
                        {
                            continue;
                        }

                        VolumeParameter parameter =
                            field.GetValue(component) as VolumeParameter;

                        if (parameter == null)
                            continue;

                        if (!TrySerializeParameterValue(
                            parameter,
                            out string serializedValue))
                        {
                            continue;
                        }

                        componentSave.properties.Add(
                            new NGXPropertyChange
                            {
                                propertyName = field.Name,
                                value = serializedValue,
                                overrideState = parameter.overrideState
                            }
                        );
                    }

                    volumeSave.components.Add(componentSave);
                }

                save.volumes.Add(volumeSave);
            }

            return save;
        }

        private void LoadNGX(string saveName)
        {
            string safeName = SanitizeSaveName(saveName);

            if (string.IsNullOrEmpty(safeName))
                return;

            string path = Path.Combine(
                NGXSaveDirectory,
                safeName + NGXSaveExtension
            );

            if (!File.Exists(path))
                return;

            try
            {
                string json = File.ReadAllText(path);

                NGXSaveFile save =
                    JsonConvert.DeserializeObject<NGXSaveFile>(json);

                if (save == null || save.volumes == null)
                    return;

                if (save.formatVersion > NGXSaveFormatVersion)
                {
                    Mod.Log.Info(
                        "NGX preset uses a newer save format and cannot be loaded safely."
                    );

                    return;
                }

                isApplyingNGXSave = true;

                ApplyNGXSave(save);

                trackedNGXChanges = save;
                NormalizeSaveData(trackedNGXChanges);

                volumesDirty = true;
                componentsDirty = true;
                componentPropertiesDirty = true;

                Mod.Log.Info(
                    "NGX preset loaded: " + safeName
                );
            }
            catch (Exception ex)
            {
                Mod.Log.Info(
                    "NGX load failed: " + ex.Message
                );
            }
            finally
            {
                isApplyingNGXSave = false;
            }
        }

        private void DeleteNGX(string saveName)
        {
            try
            {
                string safeName = SanitizeSaveName(saveName);
                if (string.IsNullOrEmpty(safeName))
                    return;

                string path = Path.Combine(NGXSaveDirectory, safeName + NGXSaveExtension);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    ngxSavesDirty = true;
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Info("NGX delete failed: " + ex.Message);
            }
        }

        private void ApplyNGXSave(NGXSaveFile save)
        {
            Volume[] sceneVolumes = UnityEngine.Object.FindObjectsOfType<Volume>();

            foreach (NGXVolumeChange volumeChange in save.volumes)
            {
                if (volumeChange == null || string.IsNullOrEmpty(volumeChange.volumeName))
                    continue;

                Volume volume = sceneVolumes.FirstOrDefault(x =>
                    x != null && x.name.Equals(volumeChange.volumeName, StringComparison.OrdinalIgnoreCase));

                if (volume == null)
                {
                    Mod.Log.Info("NGX load skipped missing volume: " + volumeChange.volumeName);
                    continue;
                }

                if (volume.profile == null)
                    volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

                if (volumeChange.components == null)
                    continue;

                foreach (NGXComponentChange componentChange in volumeChange.components)
                {
                    if (componentChange == null || string.IsNullOrEmpty(componentChange.componentName))
                        continue;

                    VolumeComponent component = volume.profile.components.FirstOrDefault(x =>
                        x != null && x.GetType().Name.Equals(
                            componentChange.componentName,
                            StringComparison.OrdinalIgnoreCase));

                    if (componentChange.existence == 0)
                    {
                        if (component != null)
                            volume.profile.components.Remove(component);

                        continue;
                    }

                    // Added components must be recreated. Also recreate a missing component when
                    // the preset contains state/property edits for it, so those edits can be restored.
                    bool needsComponent =
                        componentChange.existence == 1 ||
                        componentChange.hasActiveState ||
                        (componentChange.properties != null && componentChange.properties.Count > 0);

                    if (component == null && needsComponent)
                        component = EnsureComponent(volume, componentChange.componentName);

                    if (component == null)
                        continue;

                    if (componentChange.hasActiveState)
                    {
                        component.active = componentChange.activeState;
                        SetInternalEnableParameter(component, componentChange.activeState);
                    }

                    if (componentChange.properties == null)
                        continue;

                    foreach (NGXPropertyChange propertyChange in componentChange.properties)
                    {
                        if (propertyChange == null || string.IsNullOrEmpty(propertyChange.propertyName))
                            continue;

                        FieldInfo field = component.GetType().GetField(
                            propertyChange.propertyName,
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.IgnoreCase);

                        if (field == null)
                            continue;

                        VolumeParameter parameter = field.GetValue(component) as VolumeParameter;
                        if (parameter == null)
                            continue;

                        if (TrySetParameterValue(parameter, propertyChange.value))
                            parameter.overrideState = propertyChange.overrideState;
                    }
                }
            }
        }

        private VolumeComponent EnsureComponent(Volume volume, string componentName)
        {
            if (volume == null || volume.profile == null)
                return null;

            VolumeComponent existing = volume.profile.components.FirstOrDefault(x =>
                x != null && x.GetType().Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing;

            Type componentType = FindVolumeComponentType(componentName);
            if (componentType == null)
                return null;

            VolumeComponent added = volume.profile.Add(componentType, true);
            added.active = true;
            SetInternalEnableParameter(added, true);
            return added;
        }

        private Type FindVolumeComponentType(string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
                return null;

            EnsureVolumeComponentTypeCache();

            cachedVolumeComponentTypes.TryGetValue(componentName, out Type type);
            return type;
        }

        private void SetInternalEnableParameter(VolumeComponent component, bool state)
        {
            if (component == null)
                return;

            FieldInfo enableField = component.GetType().GetField(
                "enable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (enableField == null)
                return;

            BoolParameter boolParameter = enableField.GetValue(component) as BoolParameter;
            if (boolParameter != null)
                boolParameter.value = state;
        }

        private void TrackPropertyChange(
            string volumeName,
            string componentName,
            string propertyName,
            VolumeParameter parameter)
        {
            if (isApplyingNGXSave || parameter == null)
                return;

            string serializedValue;
            if (!TrySerializeParameterValue(parameter, out serializedValue))
                return;

            NGXComponentChange componentChange = GetOrCreateComponentChange(volumeName, componentName);
            NGXPropertyChange propertyChange = componentChange.properties.FirstOrDefault(x =>
                x.propertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (propertyChange == null)
            {
                propertyChange = new NGXPropertyChange();
                propertyChange.propertyName = propertyName;
                componentChange.properties.Add(propertyChange);
            }

            propertyChange.value = serializedValue;
            propertyChange.overrideState = parameter.overrideState;
        }

        private void TrackComponentActiveState(string volumeName, string componentName, bool active)
        {
            if (isApplyingNGXSave)
                return;

            NGXComponentChange change = GetOrCreateComponentChange(volumeName, componentName);
            change.hasActiveState = true;
            change.activeState = active;
        }

        private void TrackComponentExistence(string volumeName, string componentName, bool exists)
        {
            if (isApplyingNGXSave)
                return;

            NGXComponentChange change = GetOrCreateComponentChange(volumeName, componentName);
            change.existence = exists ? 1 : 0;
        }

        private NGXComponentChange GetOrCreateComponentChange(string volumeName, string componentName)
        {
            NormalizeSaveData(trackedNGXChanges);

            NGXVolumeChange volumeChange = trackedNGXChanges.volumes.FirstOrDefault(x =>
                x.volumeName.Equals(volumeName, StringComparison.OrdinalIgnoreCase));

            if (volumeChange == null)
            {
                volumeChange = new NGXVolumeChange();
                volumeChange.volumeName = volumeName;
                trackedNGXChanges.volumes.Add(volumeChange);
            }

            NGXComponentChange componentChange = volumeChange.components.FirstOrDefault(x =>
                x.componentName.Equals(componentName, StringComparison.OrdinalIgnoreCase));

            if (componentChange == null)
            {
                componentChange = new NGXComponentChange();
                componentChange.componentName = componentName;
                volumeChange.components.Add(componentChange);
            }

            return componentChange;
        }

        private void NormalizeSaveData(NGXSaveFile save)
        {
            if (save == null)
                return;

            if (save.volumes == null)
                save.volumes = new List<NGXVolumeChange>();

            foreach (NGXVolumeChange volume in save.volumes)
            {
                if (volume.components == null)
                    volume.components = new List<NGXComponentChange>();

                foreach (NGXComponentChange component in volume.components)
                {
                    if (component.properties == null)
                        component.properties = new List<NGXPropertyChange>();
                }
            }
        }

        private bool TrySerializeParameterValue(VolumeParameter parameter, out string value)
        {
            value = null;

            PropertyInfo valueProperty = parameter.GetType().GetProperty("value");
            if (valueProperty == null || !valueProperty.CanRead)
                return false;

            object raw = valueProperty.GetValue(parameter);
            if (raw == null)
            {
                value = string.Empty;
                return true;
            }

            CultureInfo culture = CultureInfo.InvariantCulture;

            if (raw is float)
            {
                value = ((float)raw).ToString("R", culture);
                return true;
            }

            if (raw is int)
            {
                value = ((int)raw).ToString(culture);
                return true;
            }

            if (raw is bool)
            {
                value = ((bool)raw) ? "true" : "false";
                return true;
            }

            if (raw is Color)
            {
                Color c = (Color)raw;
                value = string.Format(culture, "{0:R},{1:R},{2:R},{3:R}", c.r, c.g, c.b, c.a);
                return true;
            }

            if (raw is Vector2)
            {
                Vector2 v = (Vector2)raw;
                value = string.Format(culture, "{0:R},{1:R}", v.x, v.y);
                return true;
            }

            if (raw is Vector3)
            {
                Vector3 v = (Vector3)raw;
                value = string.Format(culture, "{0:R},{1:R},{2:R}", v.x, v.y, v.z);
                return true;
            }

            if (raw is Vector4)
            {
                Vector4 v = (Vector4)raw;
                value = string.Format(culture, "{0:R},{1:R},{2:R},{3:R}", v.x, v.y, v.z, v.w);
                return true;
            }

            if (raw is LayerMask)
            {
                value = ((LayerMask)raw).value.ToString(culture);
                return true;
            }

            Type rawType = raw.GetType();
            if (rawType.IsEnum)
            {
                value = raw.ToString();
                return true;
            }

            return false;
        }

        private string SanitizeSaveName(string saveName)
        {
            string name = (saveName ?? string.Empty).Trim();

            foreach (char invalid in Path.GetInvalidFileNameChars())
                name = name.Replace(invalid.ToString(), string.Empty);

            if (name.EndsWith(NGXSaveExtension, StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - NGXSaveExtension.Length);

            return name.Trim();
        }


        // ===============================
        // ADD COMPONENT DYNAMICALLY
        // ===============================

        private void AddComponent(string componentName)
        {
            if (selectedVolume == null)
                return;


            if (selectedVolume.profile == null)
                selectedVolume.profile =
                    ScriptableObject.CreateInstance<VolumeProfile>();


            Type componentType = FindVolumeComponentType(componentName);


            if (componentType == null)
            {
                Mod.Log.Info(
                    "NGX: Could not find component " + componentName
                );
                return;
            }


            if (selectedVolume.profile.components
                .Any(x => x.GetType() == componentType))
            {
                Mod.Log.Info(
                    "NGX: Component already exists"
                );
                return;
            }


            var added =
    selectedVolume.profile.Add(
        componentType,
        true
    );


            // Enable VolumeComponent
            added.active = true;


            // Enable internal enable parameter if available
            var enableField = componentType.GetField(
                "enable",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic
            );

            if (enableField != null)
            {
                var parameter = enableField.GetValue(added);

                if (parameter is BoolParameter boolParameter)
                {
                    boolParameter.value = true;
                }
            }


            selectedComponent = added;
            TrackComponentExistence(selectedVolume.name, componentName, true);
            TrackComponentActiveState(selectedVolume.name, componentName, true);

            componentsDirty = true;
            componentPropertiesDirty = true;

            Mod.Log.Info(
                "NGX Added: " + componentName
            );
        }
    }
}