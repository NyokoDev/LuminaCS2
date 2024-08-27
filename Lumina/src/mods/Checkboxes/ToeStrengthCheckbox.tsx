import './Checkboxes.scss'
import './CheckboxesStyle.scss'
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';

export const Active$ = bindValue<boolean>(mod.id, "IsToeStrengthActive");


export const ToeStrengthCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetToeStrengthActive");
  };

  return (
    <div className="checkbox-container toe-strength-container">
      {isActive && (
        <div 
          className="checkbox-image toestrength-checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ toestrength-checkbox-button"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default ToeStrengthCheckbox;


