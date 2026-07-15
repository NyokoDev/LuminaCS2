using Colossal.UI.Binding;
using Lumina.Systems;
using LuminaMod.XML;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;


namespace Lumina.NGX
{

    [Serializable]
    public class ComponentPropertyData
    {
        public string name;
        public string type;

        public object value;

        public float min;
        public float max;

        public bool hasRange;

        public bool readOnly;

        public string[] options;

        public string group;
    }



    [Serializable]
    public class ComponentMetadata
    {
        public string component;

        public List<ComponentPropertyData> properties =
            new();
    }



    [Serializable]
    public class PropertyUpdate
    {
        public string component;
        public string property;
        public string value;
    }




    internal partial class NGXUISystem : ExtendedUISystemBase
    {

        private Volume selectedVolume;

        private VolumeComponent selectedComponent;



        protected override void OnCreate()
        {
            base.OnCreate();



            // ===============================
            // COMPONENT INSPECTOR
            // ===============================


            AddUpdateBinding(
                new GetterValueBinding<string>(
                    Mod.MODUI,
                    "SelectedComponent",
                    GetSelectedComponent
                )
            );



            AddUpdateBinding(
                new GetterValueBinding<ComponentMetadata>(
                    Mod.MODUI,
                    "GetComponentProperties",
                    GetComponentProperties
                )
            );



            AddBinding(
                new TriggerBinding<string>(
                    Mod.MODUI,
                    "SelectComponent",
                    SelectComponent
                )
            );



            AddBinding(
                new TriggerBinding<string>(
                    Mod.MODUI,
                    "SetComponentProperty",
                    SetComponentProperty
                )
            );





            // ===============================
            // MODE
            // ===============================


            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    Mod.MODUI,
                    "NGXMode",
                    () => GlobalVariables.Instance.NGXMode
                )
            );






            // ===============================
            // VOLUMES
            // ===============================


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







        // ======================================================
        // COMPONENT SELECTION
        // ======================================================


        private void SelectComponent(string name)
        {

            if (selectedVolume == null)
                return;


            if (selectedVolume.profile == null)
                return;



            selectedComponent =
                selectedVolume.profile.components
                .FirstOrDefault(
                    x => x.GetType().Name == name
                );

        }





        private string GetSelectedComponent()
        {

            return selectedComponent != null
                ? selectedComponent.GetType().Name
                : "";

        }








        // ======================================================
        // COMPONENT METADATA
        // ======================================================


        private ComponentMetadata GetComponentProperties()
        {

            ComponentMetadata metadata =
                new();


            if (selectedComponent == null)
                return metadata;



            metadata.component =
                selectedComponent.GetType().Name;



            foreach (PropertyInfo property in
                selectedComponent.GetType()
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance
                ))
            {


                if (!property.CanRead)
                    continue;



                ComponentPropertyData data =
                    new();



                data.name =
                    property.Name;


                data.type =
                    NormalizeType(
                        property.PropertyType
                    );


                data.readOnly =
                    !property.CanWrite;



                try
                {
                    data.value =
                        property.GetValue(
                            selectedComponent
                        );
                }
                catch
                {
                    continue;
                }




                // ENUM SUPPORT

                if (property.PropertyType.IsEnum)
                {
                    data.options =
                        Enum.GetNames(
                            property.PropertyType
                        );
                }



                // RANGE ATTRIBUTE

                RangeAttribute range =
                    property.GetCustomAttribute<RangeAttribute>();


                if (range != null)
                {

                    data.hasRange = true;

                    data.min =
                        range.min;

                    data.max =
                        range.max;

                }




                metadata.properties.Add(data);

            }



            return metadata;

        }







        private string NormalizeType(Type type)
        {

            if (type == typeof(float))
                return "float";


            if (type == typeof(int))
                return "int";


            if (type == typeof(bool))
                return "bool";


            if (type == typeof(string))
                return "string";


            if (type == typeof(Color))
                return "Color";


            if (type == typeof(Vector2))
                return "Vector2";


            if (type == typeof(Vector3))
                return "Vector3";


            if (type.IsEnum)
                return "enum";


            return type.Name;

        }









        // ======================================================
        // UPDATE PROPERTY
        // ======================================================


        private void SetComponentProperty(string json)
        {

            if (selectedComponent == null)
                return;



            PropertyUpdate update =
    JsonConvert.DeserializeObject<PropertyUpdate>(json);



            PropertyInfo property =
                selectedComponent.GetType()
                .GetProperty(
                    update.property
                );



            if (property == null)
                return;



            if (!property.CanWrite)
                return;



            object value =
                ConvertValue(
                    update.value,
                    property.PropertyType
                );



            property.SetValue(
                selectedComponent,
                value
            );

        }






        private object ConvertValue(
            string value,
            Type type
        )
        {

            if (type == typeof(float))
                return float.Parse(value);


            if (type == typeof(int))
                return int.Parse(value);


            if (type == typeof(bool))
                return bool.Parse(value);


            if (type == typeof(string))
                return value;


            if (type.IsEnum)
                return Enum.Parse(
                    type,
                    value
                );


            return value;

        }









        // ======================================================
        // VOLUMES
        // ======================================================


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







        private string[] GetVolumeComponents()
        {

            if (selectedVolume == null)
                return Array.Empty<string>();


            if (selectedVolume.profile == null)
                return Array.Empty<string>();



            return selectedVolume.profile.components
                .Select(
                    x => x.GetType().Name
                )
                .ToArray();

        }








        private string[] GetAvailableComponents()
        {

            return typeof(VolumeComponent)
                .Assembly
                .GetTypes()

                .Where(type =>
                    type.IsSubclassOf(
                        typeof(VolumeComponent)
                    )

                    &&

                    !type.IsAbstract
                )

                .Select(
                    type => type.Name
                )

                .Distinct()

                .OrderBy(
                    x => x
                )

                .ToArray();

        }








        private void SelectVolume(string name)
        {

            selectedVolume =
                UnityEngine.Object
                .FindObjectsOfType<Volume>()

                .FirstOrDefault(
                    x => x.name == name
                );



            selectedComponent = null;

        }









        // ======================================================
        // ADD COMPONENT
        // ======================================================


        private void AddComponent(string componentName)
        {

            if (selectedVolume == null)
                return;



            if (selectedVolume.profile == null)

                selectedVolume.profile =
                    ScriptableObject
                    .CreateInstance<VolumeProfile>();




            Type componentType =
                typeof(VolumeComponent)
                .Assembly
                .GetTypes()

                .FirstOrDefault(
                    type =>
                    type.Name == componentName
                );



            if (componentType == null)
                return;



            if (!typeof(VolumeComponent)
                .IsAssignableFrom(componentType))
                return;



            if (selectedVolume.profile.components
                .Any(
                    x =>
                    x.GetType() == componentType
                ))
                return;




            selectedVolume.profile
                .Add(
                    componentType,
                    true
                );


        }


    }

}