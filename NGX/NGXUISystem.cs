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



        protected override void OnCreate()
        {
            base.OnCreate();

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