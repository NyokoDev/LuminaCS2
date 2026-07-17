using Colossal.UI.Binding;
using Game.Settings;
using Lumina.Systems;
using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets the properties for the currently selected component.
        /// </summary>
        /// <returns></returns>
        private string[] GetComponentProperties()
        {
            if (selectedComponent == null)
                return Array.Empty<string>();

            return selectedComponent
                .GetType()
                .GetFields(
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public
                )
                .Where(f => typeof(VolumeParameter).IsAssignableFrom(f.FieldType))
                .Select(f =>
                {
                    var parameter = (VolumeParameter)f.GetValue(selectedComponent);

                    string value = "";

                    if (parameter != null)
                    {
                        var valueField = parameter.GetType().GetField(
                            "m_Value",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic
                        );

                        if (valueField != null)
                            value = valueField.GetValue(parameter)?.ToString() ?? "";
                    }

                    return $"{f.Name}|{parameter?.GetType().Name}|{value}";
                })
                .ToArray();
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


            selectedVolume.profile.components.Remove(component);


            selectedComponent = null;
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
        }

        // ===============================
        // DYNAMIC VOLUME DISCOVERY
        // ===============================

        private string[] GetVolumes()
        {
            return UnityEngine.Object
                .FindObjectsOfType<Volume>()
                .Select(x => x.name)
                .ToArray();
        }





        private string GetSelectedVolume()
        {
            return selectedVolume != null
                ? selectedVolume.name
                : "";
        }

        // COMPONENT STATES
        private string[] GetComponentStates()
        {
            if (selectedVolume == null)
                return Array.Empty<string>();

            if (selectedVolume.profile == null)
                return Array.Empty<string>();


            return selectedVolume.profile.components
                .Select(x =>
                    $"{x.GetType().Name}|{x.active}"
                )
                .ToArray();
        }





        // ===============================
        // READ CURRENT PROFILE
        // ===============================

        private string[] GetVolumeComponents()
        {

            if (selectedVolume == null)
                return Array.Empty<string>();


            if (selectedVolume.profile == null)
                return Array.Empty<string>();


            return selectedVolume.profile.components
                .Select(x => x.GetType().Name)
                .ToArray();

        }




        // Cached Available Components (SAVE RAM)
        private string[] cachedAvailableComponents;

        // ===============================
        // FIND ALL AVAILABLE COMPONENTS
        // ===============================

        private string[] GetAvailableComponents()
        {
            if (cachedAvailableComponents != null)
                return cachedAvailableComponents;


            cachedAvailableComponents =
                AppDomain.CurrentDomain
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
                    typeof(VolumeComponent).IsAssignableFrom(type)
                    &&
                    !type.IsAbstract
                )
                .Select(type => type.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();


            return cachedAvailableComponents;
        }



        // ===============================
        // SELECT VOLUME
        // ===============================

        private void SelectVolume(string name)
        {

            selectedVolume =
                UnityEngine.Object
                .FindObjectsOfType<Volume>()
                .FirstOrDefault(
                    x => x.name == name
                );

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
// ADD COMPONENT DYNAMICALLY
// ===============================

private void AddComponent(string componentName)
        {
            if (selectedVolume == null)
                return;


            if (selectedVolume.profile == null)
                selectedVolume.profile =
                    ScriptableObject.CreateInstance<VolumeProfile>();


            Type componentType =
                AppDomain.CurrentDomain
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
                .FirstOrDefault(type =>
                    type.Name == componentName &&
                    typeof(VolumeComponent).IsAssignableFrom(type)
                );


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


            Mod.Log.Info(
                "NGX Added: " + componentName
            );
        }
    }
}