import { ButtonTheme, Tooltip, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs, Dropdown, Icon } from "cs2/ui";
import { bindValue, trigger, useValue, } from "cs2/api";
import { game, tool, Theme, } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
import { Slider, PropsSlider, SliderValueTransformer } from "./slider";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import { useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import "../luminapanel.scss"; 
import { useState } from "react";
import { createPortal } from "react-dom";
import React, { Fragment } from 'react';
import {SketchPicker} from 'react-color';
import Box from "@mui/material/Box";
import { Slider as Slider2, TextField, createTheme } from '@mui/material';
import { ThemeProvider } from '@mui/material/styles';
import red from "@mui/material/colors/red";
import FilePicker from "./FilePicker";


export let isInstalled$ = false;
export let ColorAdjustmentsEnabled = true;
export let ToneMappingEnabled = false;


// ColorAdjustments
export const PostExposure$ = bindValue<number>(mod.id, 'PostExposure');
export const PostExposureActive$ = bindValue<boolean>(mod.id, 'GetPostExposureCheckbox');
export const Contrast$ = bindValue<number>(mod.id, 'GetContrast');
export const ContrastActive$ = bindValue<boolean>(mod.id, 'GetcontrastCheckbox');
export const HueShift$ = bindValue<number>(mod.id, 'GetHueShift');
export const hueshiftActive$ = bindValue<boolean>(mod.id, 'GethueshiftCheckbox');
export const Saturation$ = bindValue<number>(mod.id, 'GetSaturation');
export const saturationActive$ = bindValue<boolean>(mod.id, 'GetsaturationCheckbox');



// White Balance
export const Temperature$ = bindValue<number>(mod.id, 'GetTemperature');
export const TemperatureActive$ = bindValue<boolean>(mod.id, 'GetTempCheckbox');
export const Tint$ = bindValue<number>(mod.id, 'GetTint');
export const TintActive$ = bindValue<boolean>(mod.id, 'GetTintCheckbox');

// Shadows Midtones Highlights
export const Shadows$ = bindValue<number>(mod.id, 'GetShadows');
export const ShadowsActive$ = bindValue<boolean>(mod.id, 'GetShadowsCheckbox');
export const Midtones$ = bindValue<number>(mod.id, 'GetMidtones');
export const MidtonesActive$ = bindValue<boolean>(mod.id, 'GetMidtonesCheckbox');
export const Highlights$ =  bindValue<number>(mod.id, 'GetHighlights');
export const HighlightsActive$ =  bindValue<boolean>(mod.id, 'GetHighlightsCheckbox');


// Planetary Settings
export const LatitudeValue$ = bindValue<number>(mod.id, 'LatitudeValue');
export const LongitudeValue$ = bindValue<number>(mod.id, 'LongitudeValue');

//Tonemapping
export const LUTValue$ = bindValue<number>(mod.id, 'LUTValue');

const theme = createTheme({
  palette: {
    primary: {
      main: red[500],
    },
  },
});


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
  const PostExposureActive = useValue<boolean>(PostExposureActive$);
  const ContrastActive = useValue<boolean>(ContrastActive$);
  const COValue = useValue(Contrast$);
  const hueshiftActive = useValue<boolean>(hueshiftActive$);
  const HSValue = useValue(HueShift$);
  const SAValue = useValue(Saturation$);
  const saturationActive = useValue(saturationActive$);

  //Use localization
  const { translate } = useLocalization();


  // WhiteBalance
  const TempValue = useValue(Temperature$);
  const TempActive = useValue(TemperatureActive$);
  const TintValue = useValue(Tint$);
  const TintActive = useValue(TintActive$);


  // Shadows/Midtones/highlights
const ShadowsValue = useValue(Shadows$);
const ShadowsActive = useValue(ShadowsActive$);
const MidtonesValue = useValue(Midtones$);
const MidtonesActive = useValue(MidtonesActive$);
const HighlightsValue = useValue(Highlights$);
const HighlightsActive = useValue(HighlightsActive$);

// Planetary Settings
const LatitudeValue = useValue(LatitudeValue$);
const LongitudeValue = useValue(LongitudeValue$);

//Tonemapping
const LUTValue = useValue(LUTValue$);



// Initialize state variables using useState hook
const [ColorAdjustmentsEnabled$, setCA] = useState(true);
const [SettingsEnabled$, setSettings] = useState(false);
const [ToneMappingEnabled$, setTonemapping] = useState(false);
const [PlanetaryEnabled$, setPlanetaryTab] = useState(false);
const [OnImport, OnImportChange] = useState(false);



  const [tab1, setTab1] = useState(false);
  const moveItIconSrc = tab1 ? "coui://ui-mods/images/SendToLumina.svg" : "coui://ui-mods/images/SendToLumina.svg";

  const start = -1;
  const end = 1;
  const stepSize = 0.0001;

  const globalstart = -100;
  const globalend = 100;
  const globalstepSize = 0.0001;

  const globalnumberOfSteps = Math.floor((globalend - globalstart) / globalstepSize);

  const planetstart = -100;
  const planetend = 100;
  const planetstepSize = 0.0001;
  const planetnumberofsteps = Math.floor((planetend - planetstart) / planetstepSize);

 
  

  // Calculate the number of steps based on the range
  const numberOfSteps = Math.floor((end - start) / stepSize);
  
  const handleSliderChange = (value: number) => {
    // Round the value to the nearest step size of 0.001
    const roundedValue = Math.round(value / 0.001) * 0.001;
    // Convert the rounded value to an integer if necessary
    const id = globalstart + (value * globalstepSize);
    // Trigger the action with the adjusted value
    trigger(mod.id, 'SetPostExposure', id);
};

const handleContrast = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = globalstart + (value * globalstepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetContrast', id);
};





const handleHueShift = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = globalstart + (value * globalstepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetHueShift', id);
};

const handleSaturation = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = globalstart + (value * globalstepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetSaturation', id);
};


// WhiteBalance

const handleTemperature = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = globalstart + (value * globalstepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetTemperature', id);
};

const handleTint = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = globalstart + (value * globalstepSize);
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

function ImportPreset() {
  console.log("Import lumina preset button clicked.");

  trigger(mod.id, 'ImportLuminaPreset');
}

function ExportPreset() {
  console.log("Export lumina preset button clicked.");
  trigger(mod.id, 'ExportLuminaPreset');
}

function UpdatePresetName(value: string) {
  trigger(mod.id, 'UpdatePresetName', value);
}

function UpdateLUTName(value: string) {
  trigger(mod.id, 'UpdateLUTName', value);
}

  
  

// Shadows
const handleShadows = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = start + (value * stepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetShadows', id);
};

const handleMidtones = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = start + (value * stepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetMidtones', id);
};

const handleHighlights = (value: number) => {
  // Round the value to the nearest step size of 0.001
  const roundedValue = Math.round(value / 0.001) * 0.001;
  // Convert the rounded value to an integer if necessary
  const id = start + (value * stepSize);
  // Trigger the action with the adjusted value
  trigger(mod.id, 'SetHighlights', id);
};

    // LegacyUI
    const OpenLegacyUI = () => {
        trigger(mod.id, 'OpenLegacyUI');
    }


    const handlePostExposureCheckbox = () => {
      trigger(mod.id, 'SetPostExposureCheckbox'); // Assuming mod is declared somewhere outside this function
    }

    const handlecontrastCheckbox = () => {
      trigger(mod.id, 'SetcontrastCheckbox'); // Assuming mod is declared somewhere outside this function
    }

    const handlehueshiftCheckbox = () => {
      trigger(mod.id, 'SethueshiftCheckbox'); // Assuming mod is declared somewhere outside this function
    }

    const handlesaturationCheckbox = () => {
      trigger(mod.id, 'SetsaturationCheckbox'); // Assuming mod is declared somewhere outside this function
    }

    const handletemperatureCheckbox = () => {
      trigger(mod.id, 'SetTempCheckbox'); 
    }

    const handleTintCheckbox = () => {
      trigger(mod.id, 'SetTintCheckbox'); 
    }

    const handleShadowsCheckbox = () => {
      trigger(mod.id, 'SetShadowsCheckbox'); 
    }

    const handleMidtonesCheckbox = () => {
      trigger(mod.id, 'SetMidtonesCheckbox'); 
    }

    const handleHighlightsCheckbox = () => {
      trigger(mod.id, 'SetHighlightsCheckbox'); 
    }

    const OpenPresetFolder = () => {
      trigger(mod.id, 'OpenPresetFolder'); 
    }

    const UpdateLUT = () => {
      trigger(mod.id, 'UpdateLUT'); 
    }




    const handleLatitude = (value: number) => {
      const id = planetstart + (value * planetstepSize);
      trigger(mod.id, 'SetLatitude', id);
    };
    
    

    const handleLongitude = (value: number) => {
      const id = planetstart + (value * planetstepSize);
      trigger(mod.id, 'SetLongitude', id);
    };

    const handleLUTContribution = (value: number) => {
      const id = planetstart + (value * planetstepSize);
      trigger(mod.id, 'HandleLUTContribution', id);
    };

    





return (
  <div className="Global">


<div className="TabsRow">

    <Tooltip
  tooltip={translate("LUMINA.colortooltip")} // Specify the content of the tooltip
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
    setPlanetaryTab(false)
   }}>
</button>
</Tooltip>

<Tooltip
  tooltip={translate("LUMINA.settingstooltip")}// Specify the content of the tooltip
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
    setPlanetaryTab(false)
    setTonemapping(false)
 ;}}>
</button>




</Tooltip>


<Tooltip
  tooltip={translate("LUMINA.planetarytooltip")}// Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'PlanetaryButtonDeselected' : 'PlanetaryButton'} 
  onSelect={() => {
    console.log("[LUMINA] Toggled Planetary panel.");
  }}
  onClick={() => { setCA(false)
    setSettings(false)
    setPlanetaryTab(true)
    setTonemapping(false)
 ;}}>
</button>
</Tooltip>

<Tooltip
  tooltip={translate("LUMINA.tonemappingtooltip")}// Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'TonemappingButtonDeselected' : 'TonemappingButton'} 
  onSelect={() => {
    console.log("[LUMINA] Toggled Planetary panel.");
  }}
  onClick={() => { setCA(false)
    setSettings(false)
    setPlanetaryTab(false)
    setTonemapping(true)
 ;}}>
</button>
</Tooltip>




    </div>




  <div  className="Panel">
    
   





    {ColorAdjustmentsEnabled$ && (
    <div className="ColorAdjustments">
 
        <label className="title_SVH title_zQN CALabel">{translate("LUMINA.coloradjustments")}</label>
        <label className="title_SVH title_zQN PostExposureLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.postexposure")}</label>
        <label className="title_SVH title_zQN PostExposureValue" >{PEValue.toString()} </label>


     
        <div className="pecheckbox">
  {PostExposureActive && (
    <div className="pecheckboximage" onClick={handlePostExposureCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ postexposurecheckbox2`}
    onClick={handlePostExposureCheckbox}
  ></button>
</div>

        





 
        <input
  type="range"
  className="toggle_cca item-mouse-states_Fmi toggle_th_"
  min={-5}
  max={1}
  value={PEValue.toString()}
  onChange={(event) => handleSliderChange(parseFloat(event.target.value))}
/>
        <Slider
                   value={(PEValue- globalstart) / globalstepSize}
                   start={-180}
                   end={globalnumberOfSteps}
          className="PostExposureSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleSliderChange(number)}
          />

        <label className="title_SVH title_zQN ContrastLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.contrast")}</label>
        <label className="title_SVH title_zQN ContrastValue" >{COValue.toString()} </label>

        <Slider
           value={(COValue- globalstart) / globalstepSize}
           start={-180}
           end={globalnumberOfSteps}
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

<div className="cocheckbox">
  {ContrastActive && (
    <div className="cocheckboximage" onClick={handlecontrastCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ contrastcheckbox2`}
    onClick={handlecontrastCheckbox}
  ></button>
</div>



<label className="title_SVH title_zQN HueshiftLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.hueshift")}</label>
        <label className="title_SVH title_zQN HueshiftValue" >{HSValue.toString()} </label>

        <div className="hscheckbox">
  {hueshiftActive && (
    <div className="hscheckboximage" onClick={handlehueshiftCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ hueshiftcheckbox2`}
    onClick={handlehueshiftCheckbox}
  ></button>
</div>



        <Slider
          value={(HSValue- globalstart) / globalstepSize}
          start={-180}
          end={globalnumberOfSteps}
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

<label className="title_SVH title_zQN SaturationLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.saturation")}</label>
        <label className="title_SVH title_zQN SaturationValue" >{SAValue.toString()} </label>

        <div className="satcheckbox">
  {saturationActive && (
    <div className="satcheckboximage" onClick={handlesaturationCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ satcheckbox2`}
    onClick={handlesaturationCheckbox}
  ></button>
</div>

        <Slider
          value={(SAValue - globalstart) / globalstepSize}
          start={-100}
          end={globalnumberOfSteps}
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

<label className="title_SVH title_zQN WBLabel">{translate("LUMINA.whitebalance")}</label>
<label className="title_SVH title_zQN TemperatureLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.temperature")}</label>
        <label className="title_SVH title_zQN TemperatureValue" >{TempValue.toString()} </label>

        <div className="tempcheckbox">
  {TempActive && (
    <div className="tempcheckboximage" onClick={handletemperatureCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ tempcheckbox2`}
    onClick={handletemperatureCheckbox}
  ></button>
</div>


        <Slider
          value={(TempValue- globalstart) / globalstepSize}
          start={-180}
          end={globalnumberOfSteps}
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

<label className="title_SVH title_zQN TintLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.tint")}</label>
        <label className="title_SVH title_zQN TintValue" >{TintValue.toString()} </label>


        <div className="tintcheckbox">
  {TintActive && (
    <div className="tintcheckboximage" onClick={handleTintCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ tintcheckbox2`}
    onClick={handleTintCheckbox}
  ></button>
</div>
        <Slider
           value={(TintValue- globalstart) / globalstepSize}
           start={-180}
           end={globalnumberOfSteps}
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


<label className="title_SVH title_zQN BLabel">{translate("LUMINA.brightness")}</label>

<label className="title_SVH title_zQN ShadowsLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.shadows")}</label>
        <label className="title_SVH title_zQN ShadowsValue" >{ShadowsValue.toString()} </label>

        <div className="shadowscheckbox">
  {ShadowsActive && (
    <div className="shadowscheckboximage" onClick={handleShadowsCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ shadowscheckbox2`}
    onClick={handleShadowsCheckbox}
  ></button>
</div>

        <Slider
          value={(ShadowsValue - start) / stepSize}
          start={-1}
          end={numberOfSteps}
          className="ShadowsSlider"
          gamepadStep={0.001}
     
        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleShadows(number)}
           // Set the step size here
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

        
<label className="title_SVH title_zQN MidtonesLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.midtones")}</label>
        <label className="title_SVH title_zQN MidtonesValue" >{MidtonesValue.toString()} </label>

        <div className="midtonescheckbox">
  {MidtonesActive && (
    <div className="midtonescheckboximage" onClick={handleMidtonesCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ midtonescheckbox2`}
    onClick={handleMidtonesCheckbox}
  ></button>
</div>


        <Slider
           value={(MidtonesValue - start) / stepSize}
           start={-1}
           end={numberOfSteps}
          className="MidtonesSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleMidtones(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />

<label className="title_SVH title_zQN HighlightsLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.highlights")}</label>
        <label className="title_SVH title_zQN HighlightsValue" >{HighlightsValue.toString()} </label>


        <div className="highlightscheckbox">
  {HighlightsActive && (
    <div className="highlightscheckboximage" onClick={handleHighlightsCheckbox}></div>
  )}

  <button
    className={`toggle_cca item-mouse-states_Fmi toggle_th_ highlightscheckbox2`}
    onClick={handleHighlightsCheckbox}
  ></button>
</div>



        <Slider
           value={(HighlightsValue - start) / stepSize}
           start={-1}
           end={numberOfSteps}
          className="HighlightsSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleHighlights(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />








</div>


)}

{SettingsEnabled$ && (
    <div className="SettingsPanel">

    <button 
  
    onClick={SaveSettings}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaSaveButton">{translate("LUMINA.save")}</button>


    
    <button 

    
    onClick={ResetToDefault}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaResetSettingsButton">{translate("LUMINA.resettodefault")}</button>

<h1 className="title_SVH title_zQN PresetManagementLabel">{translate("LUMINA.presetmanagement")}</h1>

<button
onClick={() => OnImportChange(true)}
className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaImportPresetButton">{translate("LUMINA.importpreset")}
  
  
   </button>

   <button
onClick={ExportPreset}
className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaExportPresetButton">{translate("LUMINA.exportpresetlabel")}
   </button>

   {OnImport && (
  <div className="PresetConfirmation">
    <ConfirmationDialog
      onConfirm={() => {
        ImportPreset();
        OnImportChange(false);
      }}
      onCancel={() => OnImportChange(false)}
      message={translate("LUMINA.confirmationonpreset")}
    />
  </div>
)}



<input
  type="text"
  onChange={(event) => UpdatePresetName(String(event.target.value))}
  className="toggle_cca item-mouse-states_Fmi toggle_th_ PresetInputText"
/>

<button
onClick={OpenPresetFolder}
className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaOpenFolder">{translate("LUMINA.openfolderlabel")}
   </button>



      


<div className="LuminaVersion_Image">
</div>





    </div>

    



  )}

{PlanetaryEnabled$ && 
<div className="PlanetaryPanel">
  
  
<label className="title_SVH title_zQN LatitudeLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.latitude")}</label>
<Slider
           value={(LatitudeValue - planetstart) / planetstepSize}
           start={-100}
           end={planetnumberofsteps}
          className="LatitudeSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleLatitude(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />
  
  <input
  value={LatitudeValue}
  type="range"
  className="toggle_cca item-mouse-states_Fmi toggle_th_ LatitudeInput"
  onChange={(event) => handleLatitude(Number(event.target.value))}
/>

<label className="title_SVH title_zQN LongitudeLabel" style={{ whiteSpace: 'nowrap' }}>{translate("LUMINA.longitude")}</label>
<Slider
           value={(LongitudeValue - planetstart) / planetstepSize}
           start={-100}
           end={planetnumberofsteps}
          className="LongitudeSlider"
          gamepadStep={0.001}

        
          valueTransformer={SliderValueTransformer.intTransformer}
          disabled={false}
          noFill={false}
          onChange={(number) => handleLongitude(number)}
          // onDragStart={() => console.log("onDragStart")}
          // onDragEnd={() => console.log("onDragEnd")}
          // onMouseOver={() => console.log("onMouseOver")}
          // onMouseLeave={() => console.log("onMouseLeave")}
        />
  
  <input
  value={LongitudeValue}
  type="range"
  className="toggle_cca item-mouse-states_Fmi toggle_th_ LongitudeInput"
  onChange={(event) => handleLongitude(Number(event.target.value))}
/>


   </div>
}



{ToneMappingEnabled$ && 
  <div className="TonemappingPanel">


<input
  type="text"
  onChange={(event) => UpdateLUTName(String(event.target.value))}
  className="toggle_cca item-mouse-states_Fmi toggle_th_ LUTInputText"
/>


<button
onClick={UpdateLUT}
className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LoadLUTButton">{translate("LUMINA.loadlutbutton")}
   </button>



</div>

}
</div>



</div>
)}

