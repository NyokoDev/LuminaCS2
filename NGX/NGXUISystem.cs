using Colossal.UI.Binding;
using Lumina.Systems;
using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


namespace Lumina.NGX
{

    internal partial class NGXUISystem : ExtendedUISystemBase
    {

        private Volume selectedVolume;
        private bool ngxMode;
        private VolumeComponent selectedComponent;

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





        // ===============================
        // FIND ALL AVAILABLE COMPONENTS
        // ===============================

        private string[] GetAvailableComponents()
        {

            return typeof(VolumeComponent)
                .Assembly
                .GetTypes()
                .Where(type =>
                    type.IsSubclassOf(typeof(VolumeComponent))
                    &&
                    !type.IsAbstract
                )
                .Select(type => type.Name)
                .OrderBy(x => x)
                .ToArray();

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
                typeof(VolumeComponent)
                .Assembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.Name == componentName
                );


            if (componentType == null)
                return;



            if (!typeof(VolumeComponent)
                .IsAssignableFrom(componentType))
                return;



            selectedVolume.profile
                .Add(componentType, true);

        }

    }
}