import mod from "mod.json";
import { bindValue, trigger, useValue } from 'cs2/api';

export const Active$ = bindValue<boolean>(mod.id, "IsToeStrengthActive");


const Checkbox: React.FC = () => {
  const isActive = useValue(Active$);

  const toggle = () => {
    trigger(mod.id, "SetToeStrengthActive");
  };

  return (
    <div className="checkbox-container">
      {isActive && (
        <div 
          className="checkbox-image" 
          onClick={toggle}
        ></div>
      )}
      <button
        className="toggle_cca item-mouse-states_Fmi toggle_th_"
        onClick={toggle}
      ></button>
    </div>
  );
};

export default Checkbox;

