import './Checkboxes.scss';
import './CheckboxesStyle.scss';
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';
import { useState } from 'react';

export const Active$ = bindValue<boolean>(mod.id, "IsHDRISkyEnabled");

export const SpaceEmissionCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetHDRISkyEnabled");
  };

  return (
    <div className="hdri-strength-container">
      {isActive && (
        <div 
          className="hdri-checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ hdristrength-checkbox-button"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default SpaceEmissionCheckbox;
