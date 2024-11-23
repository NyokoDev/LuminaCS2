import './Checkboxes.scss';
import './CheckboxesStyle.scss';
import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';

export const Active$ = bindValue<boolean>(mod.id, "UseLuminaVolume");

export const LuminaVolumeCheckbox = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "RestartLuminaVolume");
  };

  return (
    <div className="checkbox-container lumina-volume-container">
      {isActive && (
        <div 
          className="checkbox-image lumina-volume-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_ lumina-volume-toggle"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default LuminaVolumeCheckbox;
