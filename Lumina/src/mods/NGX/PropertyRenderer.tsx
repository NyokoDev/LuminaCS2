import React from "react";

import {
    ComponentProperty
} from "./NGXWorkspace";



interface Props {

    property: ComponentProperty;

    onChange(value:any):void;

}




export default function PropertyRenderer({

    property,

    onChange

}:Props){





switch(property.type)
{


// =====================================================
// BOOLEAN
// =====================================================

case "bool":

return (

<label className="Toggle">


<input

type="checkbox"

checked={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    e.target.checked
)

}

/>


<span className="SliderToggle"/>


</label>

);






// =====================================================
// STRING
// =====================================================

case "string":

return (

<input

className="PropertyInput"

type="text"

value={property.value ?? ""}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    e.target.value
)

}

/>

);






// =====================================================
// FLOAT
// =====================================================

case "float":

return (

<div className="SliderProperty">


<input

type="range"

min={property.min ?? 0}

max={property.max ?? 1}

step="0.01"

value={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    Number(
        e.target.value
    )
)

}

/>



<input

className="NumberValue"

type="number"

value={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    Number(
        e.target.value
    )
)

}

/>


</div>

);






// =====================================================
// INTEGER
// =====================================================

case "int":

return (

<input

className="PropertyInput"

type="number"

value={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    Number(
        e.target.value
    )
)

}

/>

);






// =====================================================
// ENUM
// =====================================================

case "enum":

return (

<select

className="PropertyInput"

value={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    e.target.value
)

}

>


{

property.options?.map(
(option:string)=>(

<option

key={option}

value={option}

>

{option}

</option>

)

)

}


</select>

);








// =====================================================
// VECTOR2
// =====================================================

case "Vector2":

return (

<VectorEditor

value={property.value}

axes={[
    "x",
    "y"
]}

onChange={onChange}

/>

);






// =====================================================
// VECTOR3
// =====================================================

case "Vector3":

return (

<VectorEditor

value={property.value}

axes={[
    "x",
    "y",
    "z"
]}

onChange={onChange}

/>

);






// =====================================================
// COLOR
// =====================================================

case "Color":

return (

<input

type="color"

value={property.value}

disabled={property.readOnly}

onChange={(e)=>

onChange(
    e.target.value
)

}

/>

);







// =====================================================
// FALLBACK
// =====================================================

default:

return (

<div className="Unsupported">

{String(property.value)}

</div>

);


}



}







// =====================================================
// VECTOR EDITOR
// =====================================================


function VectorEditor({

value,

axes,

onChange

}:any){


const update = (

axis:string,

newValue:number

)=>{


onChange({

...value,

[axis]:
newValue

});


};




return (

<div className="VectorEditor">


{

axes.map((axis:string)=>(


<div

key={axis}

>


<label>

{axis.toUpperCase()}

</label>



<input

type="number"

value={
    value?.[axis] ?? 0
}

onChange={(e)=>

update(

axis,

Number(
e.target.value
)

)

}

/>


</div>


))


}


</div>

);


}
export {};