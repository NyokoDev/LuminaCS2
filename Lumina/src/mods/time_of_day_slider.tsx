import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../mod.json";
import { Slider, SliderValueTransformer } from "./slider";

export const TimeFloatValue$ = bindValue<number>(mod.id, 'TimeFloatValue');
export const Active$ = bindValue<boolean>(mod.id, 'TimeFloatIsActive')

export const TimeOfDaySliderModuleRegistryExtend = (Component: React.ComponentType) => {
    return (props: any) => {
      const TimeFloatValue = useValue(TimeFloatValue$) // Example value, replace with actual logic
      const TimeFloatIsActive = useValue(Active$)
  
      const handleTimeOfDay = (number: number) => {
        trigger(mod.id, 'HandleTimeFloatValue', number);
      };
  
      return (
        <>
          {TimeFloatIsActive && (
            <>
              <h1 className="TimeOfDaySlider">TIME OF DAY</h1>
              <Slider
                value={TimeFloatValue}
                start={0}
                end={100}
                className="TintSlider TimeOfDaySlider"
                gamepadStep={0.001}
                valueTransformer={SliderValueTransformer.intTransformer}
                disabled={false}
                noFill={false}
                onChange={(number) => handleTimeOfDay(number)}
              />
            </>
          )}
          <Component {...props} />
        </>
      );
    };
  };