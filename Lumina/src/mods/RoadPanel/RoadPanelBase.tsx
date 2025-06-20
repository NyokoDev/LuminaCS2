import { bindValue, trigger, useValue } from "cs2/api";
import { Slider, SliderValueTransformer } from "mods/slider";
import React from "react";
import mod from "../../../mod.json";
import './roadpanel.scss';
import { useLocalization } from "cs2/l10n";

// Imports
export const GetOpacity$ = bindValue<number>(mod.id, "GetOpacity");
export const GetBrightness$ = bindValue<number>(mod.id, "GetBrightness");
export const GetSmoothness$ = bindValue<number>(mod.id, "GetSmoothness");

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
  const HandleOpacity = (value: number) => {
    trigger(mod.id, "SetOpacity", value);
  };

  const HandleBrightness = (value: number) => {
    trigger(mod.id, "SetBrightness", value);
  };

  const HandleSmoothness = (value: number) => {
    trigger(mod.id, "SetSmoothness", value);
  };

  const GetOpacity = useValue(GetOpacity$);
  const GetBrightness = useValue(GetBrightness$);
  const GetSmoothness = useValue(GetSmoothness$);

    //Use localization
    const { translate } = useLocalization();

 const OpenLUTFolder= () => {
      trigger(mod.id, 'OpenTexturesFolder'); 
    }

 const ApplyTextures= () => {
      trigger(mod.id, 'ApplyRoadTextures'); 
    }

  return (
    <div className={className}>

<h1 className="title_SVH title_zQN roadconfiglabel"> Road Configuration</h1>
      
<div className="road-panel-base">



  <div className="opacity-container">
    <div className="slider-label-1">Opacity</div>
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
    <div className="slider-label-2">Brightness</div>
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
    <div className="slider-label-3">Smoothness</div>
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

      <button
        onClick={OpenLUTFolder}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS OpenRoadTexturesButton">
        {translate("LUMINA.opentexturesbutton")}
      </button>

            <button
        onClick={ApplyTextures}
        className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS ApplyTextures">
        {translate("LUMINA.applytexturesbutton")}
      </button>

</div>



      <h2 className="text-xl font-bold mb-4">{title}</h2>
      <div>{children}</div>
    </div>
  );
};
