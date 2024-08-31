import './Checkboxes.scss';
import './CheckboxesStyle.scss';
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';
import { useState } from 'react';

export const Active$ = bindValue<boolean>(mod.id, "IsCustomSunEnabled");

export const CustomSunCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetCustomSunEnabled");
  };

  return (
    <div className="customsun-strength-container">
      {isActive && (
        <div 
          className="customsun-checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ customsun-checkbox-button"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default CustomSunCheckbox;
