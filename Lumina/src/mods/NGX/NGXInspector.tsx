import React, { useState } from "react";
import { trigger } from "cs2/api";

import {
    ComponentMetadata,
    ComponentProperty
} from "./NGXWorkspace";
import "./NGXWorkspace.scss";
import "./PropertyRenderer";
import PropertyRenderer from "./PropertyRenderer";



interface Props {

    component:string;

    metadata:ComponentMetadata;

}




export default function NGXInspector({

    component,
    metadata

}:Props){


    const [search,setSearch]
        = useState("");



    const filteredProperties =

        metadata.properties.filter(property =>

            property.name
            .toLowerCase()
            .includes(
                search.toLowerCase()
            )

        );





    function updateProperty(
        property:ComponentProperty,
        value:any
    ){

        trigger(

            "Lumina",

            "SetComponentProperty",

            JSON.stringify({

                component,

                property:
                    property.name,

                value

            })

        );

    }





    return (

<div className="NGXInspector">


<header className="InspectorHeader">


<h2>

{component}

</h2>


<button

className="ResetButton"

>

Reset

</button>


</header>





<div className="PropertySearch">


<input

placeholder="Search properties..."

value={search}

onChange={(e)=>

setSearch(
    e.target.value
)

}

/>


</div>







{

filteredProperties.length === 0 ?


<p>

No editable properties

</p>


:

filteredProperties.map(property =>



<div

className="PropertyCard"

key={property.name}

>



<div className="PropertyHeader">


<span>

{property.name}

</span>



{

property.readOnly &&

<span className="ReadOnly">

Read Only

</span>

}



</div>





<PropertyRenderer


property={property}


onChange={(value)=>

updateProperty(
    property,
    value
)

}


/>



</div>



)


}



</div>


    );

}