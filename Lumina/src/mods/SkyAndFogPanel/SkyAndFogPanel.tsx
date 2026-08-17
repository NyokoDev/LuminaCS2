import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "../../../mod.json";
import CustomSunCheckbox from "mods/Checkboxes/UseCustomSunCheckbox";
import SpaceEmissionCheckbox from "mods/Checkboxes/UseHDRISky";
import { Slider } from "mods/slider";

export const EmissionMultiplier$ = bindValue<number>(mod.id, "EmissionMultiplier");
export const SunDiameter$ = bindValue<number>(mod.id, "SunDiameter");
export const SunIntensity$ = bindValue<number>(mod.id, "SunIntensity");
export const SunFlareSize$ = bindValue<number>(mod.id, "SunFlareSize");


export const SkyAndFogPanel: React.FC = () => {
  const { translate } = useLocalization();

  const EmissionMultiplier = useValue(EmissionMultiplier$);
  const SunDiameter = useValue(SunDiameter$);
  const SunIntensity = useValue(SunIntensity$);
  const SunFlareSize = useValue(SunFlareSize$);

  const handleSunDiameter = (value: number) => {
    trigger(mod.id, "handleSunDiameter", value);
  };

  const handleSunIntensity = (value: number) => {
    trigger(mod.id, "handleSunIntensity", value);
  };

  const handleSunFlareSize = (value: number) => {
    trigger(mod.id, "handleSunFlareSize", value);
  };

  return (
    <div className="SkyAndFogPanel">
      <h1 className="CubemapName">
        {translate("LUMINA.cubemapname")}
      </h1>

      <label className="space-emission-texture-label">
        {translate("LUMINA.environmenthdrisky")}
      </label>

      <SpaceEmissionCheckbox />

      <CustomSunCheckbox />

      <label className="custom-sun-label">
        {translate("LUMINA.usecustomsunproperties")}
      </label>

      <label className="sun-diameter-label">
        {translate("LUMINA.sundiameter")}
      </label>

      <Slider
        value={SunDiameter}
        start={0}
        end={100}
        step={0.01}
        onChange={handleSunDiameter}
        className="sun-adjust-diameter-slider"
        gamepadStep={0.01}
        disabled={false}
        noFill={false}
      />

      <label className="sun-intensity-label">
        {translate("LUMINA.sunintensity")}
      </label>

      <Slider
        value={SunIntensity}
        start={0}
        end={100}
        step={0.01}
        onChange={handleSunIntensity}
        className="sun-adjust-intensity-slider"
        gamepadStep={0.01}
        disabled={false}
        noFill={false}
      />

      <label className="sun-flare-size-label">
        {translate("LUMINA.sunflaresize")}
      </label>

      <Slider
        value={SunFlareSize}
        start={0}
        end={100}
        step={0.01}
        onChange={handleSunFlareSize}
        className="sun-adjust-flare-size-slider"
        gamepadStep={0.01}
        disabled={false}
        noFill={false}
      />
    </div>
  );
};