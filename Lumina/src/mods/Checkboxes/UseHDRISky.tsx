import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';
import { useRef } from 'react';
import './CheckboxesStyle.scss'

export const Active$ = bindValue<boolean>(mod.id, "IsHDRISkyEnabled");

export const SpaceEmissionCheckbox = () => {
  const isActive = useValue(Active$);
  const containerRef = useRef<HTMLDivElement>(null);

  const toggle = () => {
    trigger(mod.id, "SetHDRISkyEnabled");
  };

  return (
    <>
      <div className="hdri-strength-container" ref={containerRef}>
        {isActive && (
          <div 
            className="hdri-checkbox-image" 
            onClick={toggle}
          />
        )}
        <button
          className="toggle_cca item-mouse-states_Fmi toggle_th_ hdristrength-checkbox-button"
          onClick={toggle}
        />
      </div>
    </>
  );
};

export default SpaceEmissionCheckbox;
