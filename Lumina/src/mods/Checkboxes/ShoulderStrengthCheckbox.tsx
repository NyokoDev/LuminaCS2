import './Checkboxes.scss';
import './CheckboxesStyle.scss';
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';

export const Active$ = bindValue<boolean>(mod.id, "IsShoulderStrengthActive");

export const ShoulderStrengthCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetShoulderStrengthActive");
  };

  return (
    <div className="checkbox-container shoulder-strength-container">
      {isActive && (
        <div 
          className="checkbox-image shoulderStrength-checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ shoulderstrength-checkbox-button"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default ShoulderStrengthCheckbox;
