import { ButtonTheme, Tooltip, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs, Dropdown, Icon } from "cs2/ui";
import { bindValue, trigger, useValue, } from "cs2/api";
import SimpleBar from 'simplebar-react';
import 'simplebar-react/dist/simplebar.min.css';
import { game, tool, Theme, } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
import { Slider, PropsSlider, SliderValueTransformer } from "./slider";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import { useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import "../luminapanel.scss"; 
import { useRef, useState } from "react";
import { createPortal } from "react-dom";
import React, { Fragment } from 'react';
import {SketchPicker} from 'react-color';
import FilePicker from "./FilePicker";
import { TonemappingDropdown } from "./TonemappingDropdown";
import { LUTSDropdown } from "./LUTSDropdown";
import { ToeStrengthCheckbox } from "./Checkboxes/ToeStrengthCheckbox";
import './Checkboxes/CheckboxesStyle.scss'
import ToeLengthCheckbox from "./Checkboxes/ToeLengthCheckbox";
import ShoulderStrengthCheckbox from "./Checkboxes/ShoulderStrengthCheckbox";
import { CubemapsDropdown } from "./Cubemaps/CubemapsDropdown";
import SpaceEmissionCheckbox from "./Checkboxes/UseHDRISky";
import CustomSunCheckbox from "./Checkboxes/UseCustomSunCheckbox";
import { LUTContributionSlider } from "./Sliders/LutContributionSlider";
import { OpenFileDialogButton } from "./Buttons/UploadFileButton";
import LuminaVolumeCheckbox from "./Components/UseLuminaVolumeCheckbox";
import './Cubemaps/Cubemaps.scss'
import DragButton from "./DraggableButton/DragButton";
import './RoadPanel/roadpanel.scss';
import { RoadPanelBase } from "./RoadPanel/RoadPanelBase";

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

// Tonemapping

const TonemappingMode$ = bindValue<string>(mod.id, "TonemappingMode");
const TextureFormatMode$ = bindValue<string>(mod.id, "TextureFormat");
const ExternalModeActivated$ = bindValue<boolean>(mod.id, "IsExternal");
const CustomModeActivated$ = bindValue<boolean>(mod.id, "IsCustom");

// Planetary Settings

export const LatitudeValue$ = bindValue<number>(mod.id, 'LatitudeValue');
export const LongitudeValue$ = bindValue<number>(mod.id, 'LongitudeValue');

//Tonemapping
export const LUTName$ = bindValue<string>(mod.id, 'LUTName');
export const ToeStrengthValue$ = bindValue<number>(mod.id, "ToeStrengthValue")
export const ToeLengthValue$ = bindValue<number>(mod.id, "ToeLengthValue");

export const ShoulderStrengthValue$ = bindValue<number>(mod.id, "ShoulderStrengthValue");

export const EmissionMultiplier$ = bindValue<number>(mod.id, "EmissionMultiplier");
export const SunDiameter$ = bindValue<number>(mod.id, "SunDiameter");
export const SunIntensity$ = bindValue<number>(mod.id, "SunIntensity");
export const SunFlareSize$ = bindValue<number>(mod.id, "SunFlareSize");




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
  const [position, setPosition] = useState({ x: 0, y: 0 });
  
  const isDragging = useRef(false);
  const velocity = useRef({ x: 0, y: 0 });


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
const LUTName = useValue(LUTName$);
const TonemappingMode = useValue(TonemappingMode$);
const ExternalModeActivated = useValue(ExternalModeActivated$);
const CustomModeActivated = useValue(CustomModeActivated$);
const TextureFormatMode = useValue(TextureFormatMode$);
const ToeStrengthValue = useValue(ToeStrengthValue$);
const ToeLengthValue = useValue(ToeLengthValue$);

const ShoulderStrengthValue = useValue(ShoulderStrengthValue$);

const EmissionMultiplier = useValue(EmissionMultiplier$);
const SunDiameter = useValue(SunDiameter$);
const SunIntensity = useValue(SunIntensity$);
const SunFlareSize = useValue(SunFlareSize$);


// Initialize state variables using useState hook
const [ColorAdjustmentsEnabled$, setCA] = useState(true);
const [SettingsEnabled$, setSettings] = useState(false);
const [SkyAndFogEnabled$, setSkyAndFog] = useState(false);
const [ToneMappingEnabled$, setTonemapping] = useState(false);
const [PlanetaryEnabled$, setPlanetaryTab] = useState(false);
const [OnImport, OnImportChange] = useState(false);
const [RoadPanel, setRoadPanel] = useState(false);

const [IsClicked, setIsClicked] = useState(false);




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


  const StepSizeStart = 0.0001;
  const StepSizeEnd = 1;
  const stepSizeNew = 0.0001; // Define a small step size for precision
 
  

  // Calculate the number of steps based on the range
  const numberOfSteps = Math.floor((end - start) / stepSize);
  
  const handleSliderChange = (value: number) => {

    // Trigger the action with the adjusted value
    trigger(mod.id, 'SetPostExposure', value);
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
  console.log("Reset settings clicked");
  return (
    <div>
      <p>{translate("LUMINA.resettodefault")}</p>
      <ConfirmationDialog
        message={translate("LUMINA.resettodefault")}
        onCancel={() => console.log("Dialog canceled.")}
        onConfirm={(dismiss: boolean): void => {
          trigger(mod.id, 'ResetLuminaSettings');
        }}
      />
    </div>
  );
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

    const OpenLUTFolder= () => {
      trigger(mod.id, 'OpenLUTFolder'); 
    }






const isValidCoordinate = (value: any, min: number, max: number): value is number => {
  return typeof value === 'number' && !isNaN(value) && value >= min && value <= max;
};

const handleLatitude = (raw: any) => {
  const value = Number(raw);
  if (!isValidCoordinate(value, -90, 90)) return;

  trigger(mod.id, 'SetLatitude', value);
};

const handleLongitude = (raw: any) => {
  const value = Number(raw);
  if (!isValidCoordinate(value, -180, 180)) return;

  trigger(mod.id, 'SetLongitude', value);
};


    const handleLUTContribution = (value: number) => {
      const id = planetstart + (value * planetstepSize);
      trigger(mod.id, 'HandleLUTContribution', id);
    };

    
    const handleToeStrength = (value: number) => {
      trigger(mod.id, 'HandleToeStrengthActive', value);
    };

    const handleEmissionMultiplier = (value: number) => {
      trigger(mod.id, 'handleEmissionMultiplier', value);
    };

    const handleToeLength = (value: number) => {
      trigger(mod.id, 'HandleToeLengthActive', value);
    };
    
    const handleShoulderStrength = (value: number) => {
      trigger(mod.id, 'handleShoulderStrength', value);
    };

    
    const handleAngularDiameter = (value: number) => {
      trigger(mod.id, 'handleAngularDiameter', value);
    };

    
    const handleSunIntensity = (value: number) => {
      trigger(mod.id, 'handleSunIntensity', value);
    };

    
    const handleSunFlareSize = (value: number) => {
      trigger(mod.id, 'handleSunFlareSize', value);
    };

return (

  <div className="Global"
id="Global"
  

      style={{
        transform: `translate(${position.x}px, ${position.y}px)`, // Use transform for smooth movement
        transition: 'transform 0.02s ease', // Smooth transition for dragging
        cursor: 'move',
        position: 'absolute',
        left: position.x,
        top: position.y,
      }}
    >

<DragButton />





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
    setTonemapping(false)
    setSkyAndFog(false)
    setRoadPanel(false)
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
    setSkyAndFog(false)
    setRoadPanel(false)
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
    setSkyAndFog(false)
    setRoadPanel(false)
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
    setSkyAndFog(false)
    setRoadPanel(false)
 ;}}>
</button>
</Tooltip>

<Tooltip
  tooltip={translate("LUMINA.skyandfogtooltip")}// Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'SkyAndFogButtonDeselected' : 'SkyAndFogButton'} 
  onSelect={() => {
    console.log("[LUMINA] Toggled Sky and Fog panel.");
  }}
  onClick={() => { setCA(false)
    setSettings(false)
    setPlanetaryTab(false)
    setTonemapping(false)
    setSkyAndFog(true)
    setRoadPanel(false)
 ;}}>
</button>
</Tooltip>


    <Tooltip
  tooltip={translate("LUMINA.roadconfig")} // Specify the content of the tooltip
  disabled={false} // Specify whether the tooltip is disabled (default: false)
  alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
  className="custom-tooltip" // Specify additional class names for styling purposes
>
<button 
  className={tab1 ? 'RoadButtonSelect' : 'RoadButtonSelect'} 
  onSelect={() => {
    setRoadPanel(true);
    console.log("[LUMINA] Toggled Road panel.");
  }}
  onClick={() => { setCA(false)
    setSettings(false)
    setPlanetaryTab(false)
    setTonemapping(false)
    setSkyAndFog(false)
    setRoadPanel(true)
   }}>
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
                   value={PEValue}
                   start={-5}
                   end={3}
          className="PostExposureSlider"
          gamepadStep={0.001}
          step={0.001}

        
          valueTransformer={SliderValueTransformer.floatTransformer}
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
      <label className="TextUseLumina">{translate("LUMINA.enableluminavolume")}</label>
  <LuminaVolumeCheckbox />
    <button 
  
    onClick={SaveSettings}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaSaveButton">{translate("LUMINA.save")}</button>

{IsClicked && (
  <div className="ResetConfirmationDialog">
  <p>{translate("LUMINA.resettodefaultconfirmation")}</p>
  <ConfirmationDialog
    message={translate("LUMINA.resettodefaultconfirmation")}
    onCancel={() => {
      console.log("Dialog canceled.");
      setIsClicked(false); // Reset state when dialog is canceled
    }}
    onConfirm={(dismiss: boolean): void => {
      trigger(mod.id, 'ResetLuminaSettings');
      setIsClicked(false); // Reset state after confirming
    }}
  />
</div>

)}
    
    <button 

    onClick={() => setIsClicked(true)}
    className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LuminaResetSettingsButton">

      
      {translate("LUMINA.resettodefault")}</button>

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
  <div className="Version_Text"
  ><h1></h1> 
  
  </div>
</div>





    </div>

    



  )}

{PlanetaryEnabled$ && 
<div className="PlanetaryPanel">

  {/* Latitude */}
  <label className="title_SVH title_zQN LatitudeLabel" style={{ whiteSpace: 'nowrap' }}>
    {translate("LUMINA.latitude")}
  </label>

  <Slider
    value={LatitudeValue}
    start={-90}
    end={90}
    step={0.000000000000000001}
    className="LatitudeSlider"
    gamepadStep={0.000000000000000001}
    valueTransformer={SliderValueTransformer.floatTransformer}
    disabled={false}
    noFill={false}
    onChange={handleLatitude}
  />

  <input
    value={LatitudeValue}
    type="range"
    min={-90}
    max={90}
    step={0.000000000000000001}
    className="toggle_cca item-mouse-states_Fmi toggle_th_ LatitudeInput"
    onChange={(e) => handleLatitude(parseFloat(e.target.value))}
  />

  {/* Longitude */}
  <label className="title_SVH title_zQN LongitudeLabel" style={{ whiteSpace: 'nowrap' }}>
    {translate("LUMINA.longitude")}
  </label>

  <Slider
    value={LongitudeValue}
    start={-180}
    end={180}
    step={0.000000000000000001}
    className="LongitudeSlider"
    gamepadStep={0.000000000000000001}
    valueTransformer={SliderValueTransformer.floatTransformer}
    disabled={false}
    noFill={false}
    onChange={handleLongitude}
  />

  <input
    value={LongitudeValue}
    type="range"
    min={-180}
    max={180}
    step={0.000000000000000001}
    className="toggle_cca item-mouse-states_Fmi toggle_th_ LongitudeInput"
 onChange={(e) => handleLongitude(parseFloat(e.target.value))}
  />

  <div style={{
    marginTop: '16px',
    padding: '1px',
    borderRadius: '6px',
    fontSize: '14px',
    color: 'white',
    textAlign: 'center'
  }}>
    Please ensure latitude and longitude adjustments are turned on in the game settings menu.
  </div>
</div>

}



{ToneMappingEnabled$ && 
  <div className="TonemappingPanel">




<Tooltip tooltip={translate("LUMINA.tonemappingmodedropdowntooltip")}>
<div className="TonemappingDropdown">
<TonemappingDropdown />
</div>
</Tooltip>

<label className="title_SVH title_zQN ModeLabel" style={{ whiteSpace: 'nowrap' }}>
        {translate("LUMINA.tonemappingmodedropdowntooltip")}
      </label>

      <label className="title_SVH title_zQN LutLabel" style={{ whiteSpace: 'nowrap' }}>
        {translate("LUMINA.tonemappingtitle")}
      </label>

{ExternalModeActivated && (
  <div>
    <div>
      <button
        onClick={UpdateLUT}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LoadLUTButton">
        {translate("LUMINA.loadlutbutton")}
      </button>

      <button
        onClick={OpenLUTFolder}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS OpenLUTButton">
        {translate("LUMINA.openlutbutton")}
      </button>




      <label className="title_SVH title_zQN LutLabelInUse" style={{ whiteSpace: 'nowrap' }}>
      {translate("LUMINA.luttexture")}
      </label>
    </div>

    <div className="LUTSDropdown">

                <LUTSDropdown />
                
                <LUTContributionSlider />
            <label className="title_SVH title_zQN lut-contribution-label">{translate("LUMINA.lutcontribution")}</label>

                <Tooltip tooltip={translate("LUMINA.uploadlutbutton")}>
    <OpenFileDialogButton />
</Tooltip>

    </div>
  </div>
)}

{CustomModeActivated && (
  <div>


<div className="toe-strength-container">
  <input
    value={ToeStrengthValue}
    type="range"
    className="toggle_cca item-mouse-states_Fmi toggle_th_ toe-strength-input"
    onChange={(event) => handleToeStrength(Number(event.target.value))}
  />
  <label className="title_SVH title_zQN toe-strength-label" style={{ whiteSpace: 'nowrap' }}>
    {translate("LUMINA.ToeStrength")}
  </label>


  <Slider
  value={ToeStrengthValue}
  start={0}       // Minimum value of the slider
  end={1}          // Maximum value of the slider
  step={0.0001}    // Step size for precision
  className="toe-strength-slider"
  gamepadStep={stepSize} // Step size for gamepad interaction
  valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
  disabled={false}
  noFill={false}
  onChange={(number) => handleToeStrength(number)} // Callback for value change
/>

  
  <ToeStrengthCheckbox
  />
</div>

<div className="toe-length-container">
  <input
    value={ToeLengthValue}
    type="range"
    className="toggle_cca item-mouse-states_Fmi toggle_th_ toe-length-input"
    onChange={(event) => handleToeLength(Number(event.target.value))}
  />
  <label className="title_SVH title_zQN toe-length-label" style={{ whiteSpace: 'nowrap' }}>
    {translate("LUMINA.ToeLength")}
  </label>

  <Slider
    value={ToeLengthValue}
    start={0}       // Minimum value of the slider
    end={1}          // Maximum value of the slider
    step={0.0001}    // Step size for precision
    className="toe-length-slider"
    gamepadStep={stepSize} // Step size for gamepad interaction
    valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
    disabled={false}
    noFill={false}
    onChange={(number) => handleToeLength(number)} // Callback for value change
  />

  <ToeLengthCheckbox
  />
</div>

<div className="shoulder-strength-container-box">
  <input
    value={ShoulderStrengthValue}
    type="range"
    className="toggle_cca item-mouse-states_Fmi toggle_th_ shoulder-strength-input"
    onChange={(event) => handleShoulderStrength(Number(event.target.value))}
  />
  <label className="title_SVH title_zQN shoulder-strength-label" style={{ whiteSpace: 'nowrap' }}>
    {translate("LUMINA.ShoulderStrength")}
  </label>

  <Slider
    value={ShoulderStrengthValue}
    start={0}       // Minimum value of the slider
    end={1}         // Maximum value of the slider
    step={0.0001}   // Step size for precision
    className="shoulder-strength-slider"
    gamepadStep={stepSize} // Step size for gamepad interaction
    valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
    disabled={false}
    noFill={false}
    onChange={(number) => handleShoulderStrength(number)} // Callback for value change
  />

  <ShoulderStrengthCheckbox
  />
</div>






    </div>
)}



</div>

}

{SkyAndFogEnabled$ &&
<div className="SkyAndFogPanel"> 
<h1 className="CubemapName">{translate("LUMINA.cubemapname")}</h1>
<div className="CubemapsDropdown">

      <CubemapsDropdown />
    </div>
    
  <label className="space-emission-texture-label">{translate("LUMINA.environmenthdrisky")}</label>
  <SpaceEmissionCheckbox
  />

<CustomSunCheckbox

/>

<label className="custom-sun-label">{translate("LUMINA.usecustomsunproperties")}</label>
<label className="sun-diameter-label">{translate("LUMINA.sundiameter")}</label>
<Slider
    value={SunDiameter}
    start={0}       // Minimum value of the slider
    end={100}         // Maximum value of the slider
    step={0.000001}   // Step size for precision
    className="sun-adjust-diameter-slider"
    gamepadStep={stepSize} // Step size for gamepad interaction
    valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
    disabled={false}
    noFill={false}
    onChange={(number) => handleAngularDiameter(number)} // Callback for value change
  />



<label className="sun-intensity-label">{translate("LUMINA.sunintensity")}</label>
<Slider
    value={SunIntensity}
    start={0}       // Minimum value of the slider
    end={100}         // Maximum value of the slider
    step={0.000001}   // Step size for precision
    className="sun-adjust-intensity-slider"
    gamepadStep={stepSize} // Step size for gamepad interaction
    valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
    disabled={false}
    noFill={false}
    onChange={(number) => handleSunIntensity(number)} // Callback for value change
  />

<label className="sun-flare-size-label">{translate("LUMINA.sunflaresize")}</label>
<Slider
    value={SunFlareSize}
    start={0}       // Minimum value of the slider
    end={100}         // Maximum value of the slider
    step={0.000001}  
    className="sun-adjust-flare-size-slider"
    gamepadStep={stepSize} // Step size for gamepad interaction
    valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
    disabled={false}
    noFill={false}
    onChange={(number) => handleSunFlareSize(number)} // Callback for value change
  />

  </div>

}
{RoadPanel && <div className="RoadPanelBase"> 

<RoadPanelBase />
  </div>
  }
  
</div>




</div>

)}

