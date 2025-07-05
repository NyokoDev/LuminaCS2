import { bindValue, trigger, useValue } from "cs2/api";
import { Slider, SliderValueTransformer } from "mods/slider";
import React, { useEffect } from "react";
import mod from "../../../mod.json";
import './roadpanel.scss';
import { useLocalization } from "cs2/l10n";
import { ChromePicker } from "react-color";
import { useState } from "react";
import { Color } from "cs2/bindings";
import { getModule } from "cs2/modding";
import { Tooltip } from "cs2/ui";

// Imports
export const GetOpacity$ = bindValue<number>(mod.id, "GetOpacity");
export const GetBrightness$ = bindValue<number>(mod.id, "GetBrightness");
export const GetSmoothness$ = bindValue<number>(mod.id, "GetSmoothness");
export const primaryRoadColor$ = bindValue<string>(mod.id, "PrimaryRoadColor");

  // New secondary color bind
export const secondaryRoadColor$ = bindValue<string>(mod.id, "SecondaryRoadColor");

type RoadPanelBaseProps = {
  title?: string;
  children?: React.ReactNode;
  className?: string;
};

export const RoadPanelBase: React.FC<RoadPanelBaseProps> = ({
  title = "",
  children,
  className = "",
}) => {


const handleRandomizeClick = () => {
  // Generate random float 0 to 1 for your color hue or whatever logic
  const randomHue = Math.random();

  // Trigger your mod method
  trigger(mod.id, "HandleRandomizer");
};


 const handlePrimaryRoadHueChange = (value: number) => {
  trigger(mod.id, "HandlePrimaryRoadColor", value); // send float to C#
};
  const HandleOpacity = (value: number) => {
    trigger(mod.id, "SetOpacity", value);
  };

  const HandleBrightness = (value: number) => {
    trigger(mod.id, "SetBrightness", value);
  };

  const HandleSmoothness = (value: number) => {
    trigger(mod.id, "SetSmoothness", value);
  };


      const HandleSecondaryRoadColor = (value: number) => {
    trigger(mod.id, "HandleSecondaryRoadColor", value);
  };


  const GetOpacity = useValue(GetOpacity$);
  const GetBrightness = useValue(GetBrightness$);
  const GetSmoothness = useValue(GetSmoothness$);
  const PrimaryRoadColor = useValue(primaryRoadColor$);
  const SecondaryRoadColor = useValue(secondaryRoadColor$);

  const [primaryHex, setPrimaryHex] = useState("#ffffff");
  const [secondaryHex, setSecondaryHex] = useState("#ffffff");



    //Use localization
    const { translate } = useLocalization();

 const OpenLUTFolder= () => {
      trigger(mod.id, 'OpenTexturesFolder'); 
    }

 const ApplyTextures= () => {
      trigger(mod.id, 'ApplyRoadTextures'); 
    }

    function hueToHex(hue: number): string {
  const rgb = hsvToRgb(hue, 1, 1); // Full saturation/value
  return "#" + rgb.map(x => x.toString(16).padStart(2, "0")).join("");
}

function hsvToRgb(h: number, s: number, v: number): [number, number, number] {
  let r = 0, g = 0, b = 0;
  const i = Math.floor(h * 6);
  const f = h * 6 - i;
  const p = v * (1 - s);
  const q = v * (1 - f * s);
  const t = v * (1 - (1 - f) * s);

  switch (i % 6) {
    case 0: r = v; g = t; b = p; break;
    case 1: r = q; g = v; b = p; break;
    case 2: r = p; g = v; b = t; break;
    case 3: r = p; g = q; b = v; break;
    case 4: r = t; g = p; b = v; break;
    case 5: r = v; g = p; b = q; break;
  }

  return [
    Math.round(r * 255),
    Math.round(g * 255),
    Math.round(b * 255),
  ];
}


const [hexInput, setHexInput] = useState("#ffffff");


useEffect(() => {
    if (PrimaryRoadColor?.startsWith("#") && primaryHex.toLowerCase() !== PrimaryRoadColor.toLowerCase()) {
      setPrimaryHex(PrimaryRoadColor);
    }
  }, [PrimaryRoadColor]);

  useEffect(() => {
    if (SecondaryRoadColor?.startsWith("#") && secondaryHex.toLowerCase() !== SecondaryRoadColor.toLowerCase()) {
      setSecondaryHex(SecondaryRoadColor);
    }
  }, [SecondaryRoadColor]);


  const handlePrimaryHexChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setPrimaryHex(val);
    if (/^#?[0-9a-fA-F]{6}$/.test(val)) {
      const formatted = val.startsWith("#") ? val : `#${val}`;
      trigger(mod.id, "HandlePrimaryRoadColorHex", formatted);
    }
  };

 const handleSecondaryHueChange = (value: number) => {
    trigger(mod.id, "HandleSecondaryRoadColor", value);
  };
  const handleSecondaryHexChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setSecondaryHex(val);
    if (/^#?[0-9a-fA-F]{6}$/.test(val)) {
      const formatted = val.startsWith("#") ? val : `#${val}`;
      trigger(mod.id, "HandleSecondaryRoadColorHex", formatted);
    }
  };



  return (
    <div className={className}>

<h1 className="title_SVH title_zQN roadconfiglabel">{translate("LUMINA.roadconfig")}</h1>
      
<div className="road-panel-base">



  <div className="opacity-container">
    <div className="slider-label-1">{translate("LUMINA.opacity")}</div>
    <Slider
      value={GetOpacity}
      start={-1}
      end={5}
      step={0.00001}
      className="opacity-slider"
      gamepadStep={0.001}
      valueTransformer={SliderValueTransformer.floatTransformer}
      disabled={false}
      noFill={false}
      onChange={HandleOpacity}
    />
  </div>

  <div className="brightness-container">
    <div className="slider-label-2"> {translate("LUMINA.brightness")}</div>
    <Slider
      value={GetBrightness}
      start={-1}
      end={5}
      step={0.00001}
      className="brightness-slider"
      gamepadStep={0.001}
      valueTransformer={SliderValueTransformer.floatTransformer}
      disabled={false}
      noFill={false}
      onChange={HandleBrightness}
    />
  </div>

  <div className="smoothness-container">
    <div className="slider-label-3"> {translate("LUMINA.smoothness")}</div>
    <Slider
      value={GetSmoothness}
      start={-1}
      end={5}
      step={0.00001}
      className="smoothness-slider"
      gamepadStep={0.00001}
      valueTransformer={SliderValueTransformer.floatTransformer}
      disabled={false}
      noFill={false}
      onChange={HandleSmoothness}
    />
  </div>

<div className="color-container">
  <div className="color-label-1">{translate("LUMINA.hexprimarycolor")}</div>

<input
            type="text"
            value={primaryHex}
            onChange={handlePrimaryHexChange}
            maxLength={7}
            className="toggle_cca item-mouse-states_Fmi toggle_th_ hex-input"
          />


<Tooltip tooltip={translate("LUMINA.colorpicker")}> 
<button
  onClick={() => trigger(mod.id, "OpenColorPickerSite")}
  title="Open HTML Color Codes"
  className="ColorPickerButton1"
>
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="20"
    height="20"
    viewBox="0 0 24 24"
    fill="white"
  >
    <path d="M12 2C6.486 2 2 6.017 2 11c0 3.225 2.078 6.048 5.081 7.293-.001.002-.008.026-.011.07-.019.214-.081.828.137 1.352.144.345.381.703.754.964.374.263.855.395 1.386.395 1.132 0 2.01-.742 2.01-2v-1.037c0-.222.187-.403.421-.382C19.106 18.484 22 15.002 22 11c0-4.983-4.486-9-10-9zm-4 10c-.552 0-1-.447-1-1s.448-1 1-1 1 .447 1 1-.448 1-1 1zm3-4c-.552 0-1-.447-1-1s.448-1 1-1 1 .447 1 1-.448 1-1 1zm4 0c-.552 0-1-.447-1-1s.448-1 1-1 1 .447 1 1-.448 1-1 1zm2 4c-.552 0-1-.447-1-1s.448-1 1-1 1 .447 1 1-.448 1-1 1z" />
  </svg>
</button>
</Tooltip>

<Tooltip tooltip={translate("LUMINA.randomize")}> 
<button
  onClick={handleRandomizeClick}
  title="Randomize Color"
  className="ColorPickerButtonRandomize"
>
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="20"
    height="20"
    viewBox="0 0 24 24"
    fill="white"
  >
    {/* Simple dice icon for randomize */}
    <path d="M19 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V5a2 2 0 0 0-2-2zm-8 13a1 1 0 1 1 0-2 1 1 0 0 1 0 2zm-4-4a1 1 0 1 1 0-2 1 1 0 0 1 0 2zm8 0a1 1 0 1 1 0-2 1 1 0 0 1 0 2zm-4-4a1 1 0 1 1 0-2 1 1 0 0 1 0 2z"/>
  </svg>
</button>
</Tooltip>


</div>

{/* Secondary Color Input */}
        <div className="color-container-2" style={{ marginTop: "1rem" }}>
          <div className="color-label-2">{translate("LUMINA.hexsecondcolor")}</div>
          <input
            type="text"
            value={secondaryHex}
            onChange={handleSecondaryHexChange}
            maxLength={7}
            className="toggle_cca item-mouse-states_Fmi toggle_th_ hex-input-2"
          />
        </div>





      <button
        onClick={OpenLUTFolder}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS OpenRoadTexturesButton">
        {translate("LUMINA.opentexturesbutton")}
      </button>

            <button
        onClick={ApplyTextures}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS ApplyTexturesButton">
        {translate("LUMINA.applytexturesbutton")}
      </button>

</div>



      <h2 className="text-xl font-bold mb-4">{title}</h2>
      <div>{children}</div>
    </div>
  );
};
