import { Slider, SliderValueTransformer } from "mods/slider";
import './LutContributionSlider.scss';
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "./../../../mod.json";

// Bind value for LUT contribution and export it
export const GetLutContributionValue$ = bindValue<number>(mod.id, 'GetLutContributionValue');
// Hook to get the current value of LUT contribution


// Handler for when the slider value changes
const handleLUTContribution = (value: number) => {
    trigger(mod.id, 'HandleLUTContribution', value);
};

export const LUTContributionSlider = () => {
    // Initialize the slider value

    const LutContributionValue = useValue(GetLutContributionValue$)
    return (
        <div>
            {/* Slider Component */}
            <Slider
                value={LutContributionValue}
                start={0}       // Minimum value of the slider
                end={1}         // Maximum value of the slider
                step={0.0001}   // Step size for precision    
                className="lut-contribution-slider"
                gamepadStep={0} // Step size for gamepad interaction
                valueTransformer={SliderValueTransformer.floatTransformer} // Value transformation logic
                disabled={false}
                noFill={false}
                onChange={(number: number) => handleLUTContribution(number)} // Callback for value change
            />

            {/* Range Input Component */}
            <input
                type="range"
                className="toggle_cca item-mouse-states_Fmi toggle_th_ lut-contribution-input"
                min={0}
                max={1}
                step={0.0001}
                value={LutContributionValue}
                onChange={(event) => handleLUTContribution(parseFloat(event.target.value))}
            />
            
            {/* Label for the slider */}

        </div>
    );
};
