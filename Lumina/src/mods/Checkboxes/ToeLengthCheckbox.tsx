import './Checkboxes.scss';
import './CheckboxesStyle.scss';
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';

export const Active$ = bindValue<boolean>(mod.id, "IsToeLengthActive");

export const ToeLengthCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetToeLengthActive");
  };

  return (
    <div className="checkbox-container toe-length-container">
      {isActive && (
        <div 
          className="checkbox-image toeLength-checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ toeLength-checkbox-button"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default ToeLengthCheckbox;
