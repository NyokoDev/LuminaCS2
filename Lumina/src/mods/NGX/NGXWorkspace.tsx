import React, { useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import "./NGXWorkspace.scss";

import NGXInspector from "./NGXInspector";


// ======================================================
// UNITY DATA TYPES
// ======================================================

export interface ComponentProperty {
    name: string;
    type: string;
    value: any;

    min?: number;
    max?: number;

    readOnly?: boolean;

    group?: string;

    // Enum values
    options?: string[];
}


export interface ComponentMetadata {

    component: string;

    properties: ComponentProperty[];

}


// ======================================================
// UNITY BINDINGS
// ======================================================

export const volumes =
    bindValue<string[]>(
        "Lumina",
        "GetVolumes"
    );


export const selectedVolume =
    bindValue<string>(
        "Lumina",
        "SelectedVolume"
    );


export const volumeComponents =
    bindValue<string[]>(
        "Lumina",
        "GetVolumeComponents"
    );


export const availableComponents =
    bindValue<string[]>(
        "Lumina",
        "GetAvailableComponents"
    );


export const selectedComponent =
    bindValue<string>(
        "Lumina",
        "SelectedComponent"
    );


export const componentProperties =
    bindValue<ComponentMetadata>(
        "Lumina",
        "GetComponentProperties"
    );



// ======================================================
// ACTIONS
// ======================================================

const SelectVolume = "SelectVolume";

const SelectComponent = "SelectComponent";

const AddComponent = "AddComponent";



// ======================================================
// NGX WORKSPACE
// ======================================================

export default function NGXWorkspace() {


    const [volumeSearch,setVolumeSearch]
        = useState("");

    const [componentSearch,setComponentSearch]
        = useState("");


    const volumeList =
        useValue(volumes) ?? [];


    const currentVolume =
        useValue(selectedVolume) ?? "";


    const components =
        useValue(volumeComponents) ?? [];


    const available =
        useValue(availableComponents) ?? [];


    const currentComponent =
        useValue(selectedComponent) ?? "";


    const metadata =
        useValue(componentProperties);



    const filteredVolumes =
        volumeList.filter(v =>
            v.toLowerCase()
             .includes(
                volumeSearch.toLowerCase()
             )
        );



    const filteredComponents =
        available.filter(c =>
            c.toLowerCase()
             .includes(
                componentSearch.toLowerCase()
             )
        );




    return (

<div className="NGXWorkspace">


{/* =====================================================
    LEFT PANEL
===================================================== */}

<section className="NGXPanel NGXVolumes">


<h2>
Volumes
</h2>


<div className="SearchBox">

<input

placeholder="Search volumes..."

value={volumeSearch}

onChange={(e)=>
    setVolumeSearch(
        e.target.value
    )
}

/>

</div>



<div className="VolumeList">


{
filteredVolumes.map(volume =>

<button

key={volume}

className={
currentVolume === volume
?
"active"
:
""
}

onClick={()=>

trigger(
    "Lumina",
    SelectVolume,
    volume
)

}

>

{volume}

</button>

)

}


</div>


</section>






{/* =====================================================
    CENTER PANEL
===================================================== */}

<section className="NGXPanel NGXComponents">


<h2>

{
currentVolume
?
currentVolume
:
"No Volume Selected"
}

</h2>



<h3>
Components
</h3>



<div className="ComponentList">


{

components.map(component =>


<button

key={component}


className={
currentComponent === component
?
"active"
:
""
}


onClick={()=>

trigger(
    "Lumina",
    SelectComponent,
    component
)

}


>

{component}

</button>


)


}


</div>




<h3>
Add Component
</h3>



<div className="SearchBox">


<input

placeholder="Search components..."

value={componentSearch}

onChange={(e)=>

setComponentSearch(
    e.target.value
)

}

/>


</div>




<div className="AddComponentList">


{

filteredComponents.map(component =>


<button

key={component}

onClick={()=>

trigger(
    "Lumina",
    AddComponent,
    component
)

}


>

+ {component}

</button>


)


}


</div>


</section>






{/* =====================================================
    RIGHT PANEL
===================================================== */}



<section className="NGXPanel NGXInspectorPanel">


{

currentComponent && metadata ?


<NGXInspector

component={currentComponent}

metadata={metadata}


/>


:

<div className="EmptyInspector">

Select a component

</div>


}


</section>



</div>


    );

}