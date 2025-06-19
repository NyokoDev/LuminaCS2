import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../mod.json";
import { Slider, SliderValueTransformer } from "./slider";
import "./time-of-day-css.scss";
import LockTimeButton from "./LockTimeButton/locktimebutton";

// Reactive bindings
export const TimeFloatValue$ = bindValue<number>(mod.id, 'TimeFloatValue');
export const Active$ = bindValue<boolean>(mod.id, 'TimeFloatIsActive');

// HOC to inject the Time of Day slider at the top of the settings panel
export const TimeOfDaySliderModuleRegistryExtend = (Component: React.ComponentType) => {
  return (props: any) => {
    const TimeFloatValue = useValue(TimeFloatValue$);
    const TimeFloatIsActive = useValue(Active$);

    const handleTimeOfDay = (number: number) => {
      trigger(mod.id, 'HandleTimeFloatValue', number);
    };

    return (
      <>
        {/* Time slider goes at the very top */}
        {TimeFloatIsActive && (
          <div
          
          className="time-of-day-wrapper">
            <label className="time-of-day-label">Time of Day</label>
            <Slider
              value={TimeFloatValue}
              start={0}
              end={50}
              step={0.01}
              gamepadStep={0.001}
              className="time-of-day-slider"
              valueTransformer={SliderValueTransformer.floatTransformer}
              onChange={(number) => handleTimeOfDay(number)}
              disabled={false}
              noFill={false}

            />
            <img id="lumina-time-of-day-image"
            src="coui://ui-mods/Icons/Lumina.svg"></img>
            <LockTimeButton />
          </div>
        )}

        {/* Then render the rest of the settings */}
        <Component {...props} />
      </>
    );
  };
};
