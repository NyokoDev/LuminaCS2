import { bindValue } from "cs2/api";
import mod from "../../../mod.json";

export const PostExposure$ = bindValue<number>(mod.id, "PostExposure");
export const PostExposureActive$ = bindValue<boolean>(mod.id, "GetPostExposureCheckbox");
export const Contrast$ = bindValue<number>(mod.id, "GetContrast");
export const ContrastActive$ = bindValue<boolean>(mod.id, "GetcontrastCheckbox");
export const HueShift$ = bindValue<number>(mod.id, "GetHueShift");
export const hueshiftActive$ = bindValue<boolean>(mod.id, "GethueshiftCheckbox");
export const Saturation$ = bindValue<number>(mod.id, "GetSaturation");
export const saturationActive$ = bindValue<boolean>(mod.id, "GetsaturationCheckbox");
export const fps = bindValue<number>("Lumina", "GetFPS");

export const Temperature$ = bindValue<number>(mod.id, "GetTemperature");
export const TemperatureActive$ = bindValue<boolean>(mod.id, "GetTempCheckbox");
export const Tint$ = bindValue<number>(mod.id, "GetTint");
export const TintActive$ = bindValue<boolean>(mod.id, "GetTintCheckbox");

export const Shadows$ = bindValue<number>(mod.id, "GetShadows");
export const ShadowsActive$ = bindValue<boolean>(mod.id, "GetShadowsCheckbox");
export const Midtones$ = bindValue<number>(mod.id, "GetMidtones");
export const MidtonesActive$ = bindValue<boolean>(mod.id, "GetMidtonesCheckbox");
export const Highlights$ = bindValue<number>(mod.id, "GetHighlights");
export const HighlightsActive$ = bindValue<boolean>(mod.id, "GetHighlightsCheckbox");

export const LatitudeValue$ = bindValue<number>(mod.id, "LatitudeValue");
export const LongitudeValue$ = bindValue<number>(mod.id, "LongitudeValue");

export const EmissionMultiplier$ = bindValue<number>(mod.id, "EmissionMultiplier");
export const SunDiameter$ = bindValue<number>(mod.id, "SunDiameter");
export const SunIntensity$ = bindValue<number>(mod.id, "SunIntensity");
export const SunFlareSize$ = bindValue<number>(mod.id, "SunFlareSize");

export const UpdateNotification = bindValue<boolean>(mod.id, "UpdateNotification");
