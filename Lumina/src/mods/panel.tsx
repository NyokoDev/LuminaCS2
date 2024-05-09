import { ButtonTheme, Tooltip, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs, Dropdown, Icon } from "cs2/ui";
import { bindValue, trigger, useValue, } from "cs2/api";
import { game, tool, Theme, } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
import { Slider, PropsSlider, SliderValueTransformer } from "./slider";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import "../luminapanel.scss"; 
import { useState } from "react";
import { createPortal } from "react-dom";

export let isInstalled$ = false;
export let ColorAdjustmentsEnabled = true;


// ColorAdjustments
export const PostExposure$ = bindValue<number>(mod.id, 'PostExposure');
export const Contrast$ = bindValue<number>(mod.id, 'GetContrast');
export const HueShift$ = bindValue<number>(mod.id, 'GetHueShift');
export const Saturation$ = bindValue<number>(mod.id, 'GetSaturation');

// White Balance
export const Temperature$ = bindValue<number>(mod.id, 'GetTemperature');
export const Tint$ = bindValue<number>(mod.id, 'GetTint');



// const SliderTheme: Theme | any = getModule(
//   "game-ui/common/input/slider/themes/default.module.scss",
//   "classes"
//  );


function toggle_ToolEnabled() {
  ColorAdjustmentsEnabled = !ColorAdjustmentsEnabled;
}


let tab1 = false;
let tab2 = false;



export const YourPanelComponent: React.FC<any> = () => {
  // Values
  const PEValue = useValue(PostExposure$);
  const COValue = useValue(Contrast$);
  const HSValue = useValue(HueShift$);
  const SAValue = useValue(Saturation$);



  // WhiteBalance
  const TempValue = useValue(Temperature$);
  const TintValue = useValue(Tint$);

// Initialize state variables using useState hook
const [ColorAdjustmentsEnabled$, setCA] = useState(true);
const [SettingsEnabled$, setSettings] = useState(false);
  


  const [tab1, setTab1] = useState(false);
  const moveItIconSrc = tab1 ? "coui://ui-mods/images/SendToLumina.svg" : "coui://ui-mods/images/SendToLumina.svg";
  
  const handleSliderChange = (value: number) => {
    // Round the value to the nearest step size of 0.001
    const roundedValue = Math.round(value / 0.001) * 0.001;
    // Convert the rounded value to an integer if necessary
    const id = parseInt(roundedValue.toString());
    // Trigger the action with the adjusted value
    trigger(mod.id, 'SetPostExposure', id);
};

const handleContrast = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = parseInt(roundedValue.toString());
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetContrast', id);
};





const handleHueShift = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = parseInt(roundedValue.toString());
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetHueShift', id);
};

const handleSaturation = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = parseInt(roundedValue.toString());
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetSaturation', id);
};


// WhiteBalance

const handleTemperature = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = parseInt(roundedValue.toString());
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetTemperature', id);
};

const handleTint = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = parseInt(roundedValue.toString());
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetTint', id);
};


function SaveSettings() {
  console.log("Sent to Lumina button clicked.");
  trigger(mod.id, 'Save');
}

function ResetToDefault() {
  console.log("Sent to Lumina button clicked.");
  trigger(mod.id, 'ResetLuminaSettings');
}
  
return (
  <div className="Global">


<div className="TabsRow">

    <Tooltip
  tooltip="Color" // Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'ColorAdjustmentsButtonDeselected' : 'ColorAdjustmentsButton'} 
  onSelect={() => {
    setTab1(!tab1);
    console.log("[LUMINA] Toggled Color panel.");
  }}
  onClick={() => { setCA(true)
    setSettings(false)
   }}>
</button>
</Tooltip>

<Tooltip
  tooltip="Color" // Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'SettingsButtonDeselected' : 'SettingsButton'} 
  onSelect={() => {
    console.log("[LUMINA] Toggled Settings panel.");
  }}
  onClick={() => { setCA(false)
    setSettings(true)
 ;}}>
</button>




</Tooltip>





    </div>




  <div  className="Panel">
    
   





    {ColorAdjustmentsEnabled$ && (
    <div className="ColorAdjustments">
 
        <label className="title_SVH title_zQN CALabel"> ColorAdjustments</label>
        <label className="title_SVH title_zQN PostExposureLabel" style={{ whiteSpace: 'nowrap' }}>Post Exposure</label>
        <label className="title_SVH title_zQN PostExposureValue" >{PEValue.toString()} </label>

        <Slider
          value={PEValue}
          start={-5}
          end={1}
          className="PostExposureSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleSliderChange(number)}
          />

        <label className="title_SVH title_zQN ContrastLabel" style={{ whiteSpace: 'nowrap' }}>Contrast</label>
        <label className="title_SVH title_zQN ContrastValue" >{COValue.toString()} </label>

        <Slider
          value={COValue}
          start={-100}
          end={100}
          className="ContrastSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleContrast(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN HueshiftLabel" style={{ whiteSpace: 'nowrap' }}>Hue shift</label>
        <label className="title_SVH title_zQN HueshiftValue" >{HSValue.toString()} </label>

        <Slider
          value={HSValue}
          start={-180}
          end={180}
          className="HueshiftSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleHueShift(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN SaturationLabel" style={{ whiteSpace: 'nowrap' }}>Saturation</label>
        <label className="title_SVH title_zQN SaturationValue" >{SAValue.toString()} </label>

        <Slider
          value={SAValue}
          start={-100}
          end={100}
          className="SaturationSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleSaturation(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN WBLabel"> White Balance</label>
<label className="title_SVH title_zQN TemperatureLabel" style={{ whiteSpace: 'nowrap' }}>Temperature</label>
        <label className="title_SVH title_zQN TemperatureValue" >{TempValue.toString()} </label>

        <Slider
          value={TempValue}
          start={-100}
          end={100}
          className="TemperatureSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleTemperature(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN TintLabel" style={{ whiteSpace: 'nowrap' }}>Tint</label>
        <label className="title_SVH title_zQN TintValue" >{TintValue.toString()} </label>

        <Slider
          value={TintValue}
          start={-100}
          end={100}
          className="TintSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleTint(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN BLabel">v1.4 - Beta</label>
</div>
)}

{SettingsEnabled$ && (
    <div className="SettingsPanel">
    <button 
    onClick={SaveSettings}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaSaveButton">Save</button>
    <button 
    
    onClick={ResetToDefault}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaResetSettingsButton">Reset to Default</button>
    </div>
  )}



</div>

</div>
)}

