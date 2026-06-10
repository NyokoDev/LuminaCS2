import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { Tooltip } from "cs2/ui";
import mod from "../../../mod.json";
import "./TonemappingPanelNew.scss";
import { TonemappingDropdown } from "mods/TonemappingDropdown";
import { LUTSDropdown } from "mods/LUTSDropdown";
import { LUTContributionSlider } from "mods/Sliders/LutContributionSlider";
import { OpenFileDialogButton } from "mods/Buttons/UploadFileButton";
import { Slider, SliderValueTransformer } from "mods/slider";

import ToeStrengthCheckbox from "mods/Checkboxes/ToeStrengthCheckbox";
import ToeLengthCheckbox from "mods/Checkboxes/ToeLengthCheckbox";
import ShoulderStrengthCheckbox from "mods/Checkboxes/ShoulderStrengthCheckbox";

// Tonemapping bindings
const TonemappingMode$ = bindValue<string>(mod.id, "TonemappingMode");
const TextureFormatMode$ = bindValue<string>(mod.id, "TextureFormat");
const ExternalModeActivated$ = bindValue<boolean>(mod.id, "IsExternal");
const CustomModeActivated$ = bindValue<boolean>(mod.id, "IsCustom");

export const LUTName$ = bindValue<string>(mod.id, "LUTName");
export const ToeStrengthValue$ = bindValue<number>(mod.id, "ToeStrengthValue");
export const ToeLengthValue$ = bindValue<number>(mod.id, "ToeLengthValue");
export const ShoulderStrengthValue$ = bindValue<number>(mod.id, "ShoulderStrengthValue");

const planetstart = -100;
const planetstepSize = 0.0001;

const handleLUTContribution = (value: number) => {
    const id = planetstart + value * planetstepSize;
    trigger(mod.id, "HandleLUTContribution", id);
};

const handleToeStrength = (value: number) => {
    trigger(mod.id, "HandleToeStrengthActive", value);
};

const handleEmissionMultiplier = (value: number) => {
    trigger(mod.id, "handleEmissionMultiplier", value);
};

const handleToeLength = (value: number) => {
    trigger(mod.id, "HandleToeLengthActive", value);
};

const handleShoulderStrength = (value: number) => {
    trigger(mod.id, "handleShoulderStrength", value);
};

const UpdateLUT = () => {
    trigger(mod.id, "UpdateLUT");
};

const OpenLUTFolder = () => {
    trigger(mod.id, "OpenLUTFolder");
};

function UpdateLUTName(value: string) {
    trigger(mod.id, "UpdateLUTName", value);
}

export const TonemappingPanel: React.FC<any> = () => {
    const { translate } = useLocalization();

    const TonemappingMode = useValue(TonemappingMode$);
    const ExternalModeActivated = useValue(ExternalModeActivated$);
    const CustomModeActivated = useValue(CustomModeActivated$);
    const TextureFormatMode = useValue(TextureFormatMode$);

    const ToeStrengthValue = useValue(ToeStrengthValue$);
    const ToeLengthValue = useValue(ToeLengthValue$);
    const ShoulderStrengthValue = useValue(ShoulderStrengthValue$);
    const LUTName = useValue(LUTName$);

    const stepSize = 0.0001;

    return (
        <div className="TonemappingPanel">
            <Tooltip tooltip={translate("LUMINA.tonemappingmodedropdowntooltip")}>
                <div className="TonemappingDropdown">
                    <TonemappingDropdown />
                </div>
            </Tooltip>

            <label
                className="title_SVH title_zQN ModeLabel"
                style={{ whiteSpace: "nowrap" }}
            >
                {translate("LUMINA.tonemappingmodedropdowntooltip")}
            </label>

            <label
                className="title_SVH title_zQN LutLabel"
                style={{ whiteSpace: "nowrap" }}
            >
                {translate("LUMINA.tonemappingtitle")}
            </label>

            {ExternalModeActivated && (
                <div>
                    <div>
                        <button
                            onClick={UpdateLUT}
                            className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS LoadLUTButton"
                        >
                            {translate("LUMINA.loadlutbutton")}
                        </button>

                        <button
                            onClick={OpenLUTFolder}
                            className="button_uFa child-opacity-transition_nkS button_uFa child-opacity-transition_nkS OpenLUTButton"
                        >
                            {translate("LUMINA.openlutbutton")}
                        </button>

                        <label
                            className="title_SVH title_zQN LutLabelInUse"
                            style={{ whiteSpace: "nowrap" }}
                        >
                            {translate("LUMINA.luttexture")}
                        </label>
                    </div>

                    <div className="LUTSDropdown">
                        <LUTSDropdown />

                        <LUTContributionSlider />

                        <label className="title_SVH title_zQN lut-contribution-label">
                            {translate("LUMINA.lutcontribution")}
                        </label>

                        <Tooltip tooltip={translate("LUMINA.uploadlutbutton")}>
                            <OpenFileDialogButton />
                        </Tooltip>
                    </div>
                </div>
            )}

            {CustomModeActivated && (
                <div>
                    <div className="toe-strength-container">
                        <input
                            type="range"
                            min={0}
                            max={1}
                            step={0.0001}
                            value={ToeStrengthValue}
                            className="toggle_cca item-mouse-states_Fmi toggle_th_ toe-strength-input"
                            onChange={(event) =>
                                handleToeStrength(Number(event.target.value))
                            }
                        />

                        <label
                            className="title_SVH title_zQN toe-strength-label"
                            style={{ whiteSpace: "nowrap" }}
                        >
                            {translate("LUMINA.ToeStrength")}
                        </label>

                        <Slider
                            value={ToeStrengthValue}
                            start={0}
                            end={1}
                            step={0.0001}
                            className="toe-strength-slider"
                            gamepadStep={stepSize}
                            valueTransformer={
                                SliderValueTransformer.floatTransformer
                            }
                            disabled={false}
                            noFill={false}
                            onChange={handleToeStrength}
                        />

                        <ToeStrengthCheckbox />
                    </div>

                    <div className="toe-length-container">
                        <input
                            type="range"
                            min={0}
                            max={1}
                            step={0.0001}
                            value={ToeLengthValue}
                            className="toggle_cca item-mouse-states_Fmi toggle_th_ toe-length-input"
                            onChange={(event) =>
                                handleToeLength(Number(event.target.value))
                            }
                        />

                        <label
                            className="title_SVH title_zQN toe-length-label"
                            style={{ whiteSpace: "nowrap" }}
                        >
                            {translate("LUMINA.ToeLength")}
                        </label>

                        <Slider
                            value={ToeLengthValue}
                            start={0}
                            end={1}
                            step={0.0001}
                            className="toe-length-slider"
                            gamepadStep={stepSize}
                            valueTransformer={
                                SliderValueTransformer.floatTransformer
                            }
                            disabled={false}
                            noFill={false}
                            onChange={handleToeLength}
                        />

                        <ToeLengthCheckbox />
                    </div>

                    <div className="shoulder-strength-container-box">
                        <input
                            type="range"
                            min={0}
                            max={1}
                            step={0.0001}
                            value={ShoulderStrengthValue}
                            className="toggle_cca item-mouse-states_Fmi toggle_th_ shoulder-strength-input"
                            onChange={(event) =>
                                handleShoulderStrength(Number(event.target.value))
                            }
                        />

                        <label
                            className="title_SVH title_zQN shoulder-strength-label"
                            style={{ whiteSpace: "nowrap" }}
                        >
                            {translate("LUMINA.ShoulderStrength")}
                        </label>

                        <Slider
                            value={ShoulderStrengthValue}
                            start={0}
                            end={1}
                            step={0.0001}
                            className="shoulder-strength-slider"
                            gamepadStep={stepSize}
                            valueTransformer={
                                SliderValueTransformer.floatTransformer
                            }
                            disabled={false}
                            noFill={false}
                            onChange={handleShoulderStrength}
                        />

                        <ShoulderStrengthCheckbox />
                    </div>
                </div>
            )}
        </div>
    );
};