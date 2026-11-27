import { bindValue, trigger, useValue } from "cs2/api";
import { Slider, SliderValueTransformer } from "mods/slider";
import React, { useEffect, useRef, useState } from "react";
import mod from "../../../mod.json";
import "./roadpanel.scss";
import { useLocalization } from "cs2/l10n";

export const GetOpacity$ = bindValue<number>(mod.id, "GetOpacity");
export const GetBrightness$ = bindValue<number>(mod.id, "GetBrightness");
export const GetSmoothness$ = bindValue<number>(mod.id, "GetSmoothness");
export const primaryRoadColor$ = bindValue<string>(mod.id, "PrimaryRoadColor");
export const secondaryRoadColor$ = bindValue<string>(mod.id, "SecondaryRoadColor");

type RoadPanelBaseProps = {
  title?: string;
  children?: React.ReactNode;
  className?: string;
};

const RoadIcon = ({ name }: { name: "reset" | "folder" | "reload" | "dice" | "palette" }) => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    {name === "reset" && <path d="M4 11a8 8 0 1 1 2 6M4 4v7h7" />}
    {name === "folder" && <path d="M3 7V5h6l2 3h10v3M3 7v13h15l4-10H7L3 20" />}
    {name === "reload" && <path d="M20 10a8 8 0 0 0-14-4L3 9m0-6v6h6M4 14a8 8 0 0 0 14 4l3-3m0 6v-6h-6" />}
    {name === "dice" && <><rect x="3" y="3" width="18" height="18" rx="3"/><circle cx="8" cy="8" r="1"/><circle cx="16" cy="8" r="1"/><circle cx="12" cy="12" r="1"/><circle cx="8" cy="16" r="1"/><circle cx="16" cy="16" r="1"/></>}
    {name === "palette" && <><path d="M12 3a9 9 0 1 0 0 18h2a2 2 0 0 0 1-4 2 2 0 0 1 1-4h3a2 2 0 0 0 2-2c0-5-5-8-9-8z"/><circle cx="7" cy="11" r="1"/><circle cx="10" cy="7" r="1"/><circle cx="15" cy="7" r="1"/></>}
  </svg>
);

const formatNumber = (value: number) => Number.isFinite(value) ? String(Number(value.toFixed(3))) : "";
const normalizeHex = (value: string) => /^#?[0-9a-f]{6}$/i.test(value) ? "#" + value.replace(/^#/, "").toLowerCase() : null;

const RoadNumber = ({ label, value, onChange, initial, gamepadStep }: { label: string; value: number; onChange: (v: number) => void; initial: number; gamepadStep: number }) => {
  const { translate } = useLocalization();
  const [draft, setDraft] = useState(formatNumber(value));
  const editing = useRef(false);
  const cancelled = useRef(false);

  useEffect(() => {
    if (!editing.current) setDraft(formatNumber(value));
  }, [value]);

  return (
    <div className="road-material-row">
      <span className="road-material-label">{label}</span>
      <div className="road-material-slider-wrap">
        <Slider value={value} start={-1} end={5} step={0.00001} gamepadStep={gamepadStep} valueTransformer={SliderValueTransformer.floatTransformer} disabled={false} noFill={false} onChange={onChange} className="road-material-slider" />
      </div>
      <input className="road-material-number" type="text" inputMode="decimal" aria-label={label} value={draft}
        onFocus={() => { editing.current = true; cancelled.current = false; }}
        onChange={event => setDraft(event.target.value)}
        onBlur={() => {
          editing.current = false;
          const parsed = Number(draft);
          if (cancelled.current || !draft.trim() || !Number.isFinite(parsed) || draft === formatNumber(value)) {
            setDraft(formatNumber(value));
            return;
          }
          const next = Math.max(-1, Math.min(5, parsed));
          setDraft(formatNumber(next));
          if (next !== value) onChange(next);
        }}
        onKeyDown={event => {
          event.stopPropagation();
          if (event.key === "Enter" || event.key === "Escape") {
            event.preventDefault();
            cancelled.current = event.key === "Escape";
            event.currentTarget.blur();
          }
        }} />
      <button type="button" className="road-material-icon-button" title={(translate("LUMINA.restoreopening") ?? "Restore value from when this panel opened")} aria-label={(translate("LUMINA.restoreopening") ?? "Restore value from when this panel opened") + " · " + label} disabled={!Number.isFinite(initial)} onClick={() => onChange(initial)}><RoadIcon name="reset" /></button>
    </div>
  );
};

const RoadColor = ({ label, value, onCommit }: { label: string; value: string; onCommit: (v: string) => void }) => {
  const color = normalizeHex(value || "") || "#ffffff";
  const [draft, setDraft] = useState(color);
  const editing = useRef(false);
  const cancelled = useRef(false);
  useEffect(() => {
    if (!editing.current) setDraft(color);
  }, [color]);

  return (
    <div className="road-material-color">
      <span className="road-material-color-label">{label}</span>
      <div className="road-material-color-preview" style={{ backgroundColor: color }}>
        <input type="color" className="road-material-picker" value={color} aria-label={"Pick " + label} onChange={event => onCommit(event.target.value)} />
      </div>
      <input className="road-material-hex" type="text" maxLength={7} aria-label={label} value={draft}
        onFocus={() => { editing.current = true; cancelled.current = false; }}
        onChange={event => setDraft(event.target.value)}
        onBlur={() => {
          editing.current = false;
          const next = cancelled.current ? null : normalizeHex(draft.trim());
          setDraft(next || color);
          if (next && next !== color) onCommit(next);
        }}
        onKeyDown={event => {
          event.stopPropagation();
          if (event.key === "Enter" || event.key === "Escape") {
            event.preventDefault();
            cancelled.current = event.key === "Escape";
            event.currentTarget.blur();
          }
        }} />
    </div>
  );
};

const quickColors = [
  { name: "Natural", primary: "#ffffff", secondary: "#ffffff" },
  { name: "Asphalt", primary: "#505256", secondary: "#737477" },
  { name: "Dark", primary: "#303236", secondary: "#525457" },
  { name: "Warm", primary: "#777168", secondary: "#a09a90" }
];

export const RoadPanelBase: React.FC<RoadPanelBaseProps> = ({ title = "", children, className = "" }) => {
  const opacity = useValue(GetOpacity$);
  const brightness = useValue(GetBrightness$);
  const smoothness = useValue(GetSmoothness$);
  const primary = useValue(primaryRoadColor$);
  const secondary = useValue(secondaryRoadColor$);
  const { translate } = useLocalization();
  const initial = useRef({ opacity, brightness, smoothness, primary, secondary });

  const setOpacity = (v: number) => trigger(mod.id, "SetOpacity", v);
  const setBrightness = (v: number) => trigger(mod.id, "SetBrightness", v);
  const setSmoothness = (v: number) => trigger(mod.id, "SetSmoothness", v);
  const setPrimary = (v: string) => trigger(mod.id, "HandlePrimaryRoadColorHex", v);
  const setSecondary = (v: string) => trigger(mod.id, "HandleSecondaryRoadColorHex", v);
  const reset = () => {
    if (Number.isFinite(initial.current.opacity)) setOpacity(initial.current.opacity);
    if (Number.isFinite(initial.current.brightness)) setBrightness(initial.current.brightness);
    if (Number.isFinite(initial.current.smoothness)) setSmoothness(initial.current.smoothness);
    const p = normalizeHex(initial.current.primary || "");
    const s = normalizeHex(initial.current.secondary || "");
    if (p) setPrimary(p);
    if (s) setSecondary(s);
  };

  return (
    <div className={`road-material-panel ${className}`}>
      <div className="road-material-brand">
        <div><div className="road-material-brand-name">LUMINA</div><div className="road-material-subtitle">{title || (translate("LUMINA.roadmaterials") ?? "Road Materials")}</div></div>
        <div className="road-material-logo" aria-hidden="true" />
      </div>
      {children}
      <div className="road-material-section">
        <div className="road-material-heading">{(translate("LUMINA.surfaceproperties") ?? "Surface properties")}</div>
        <RoadNumber label={translate("LUMINA.opacity") ?? "Opacity"} value={opacity} initial={initial.current.opacity} gamepadStep={0.001} onChange={setOpacity} />
        <RoadNumber label={translate("LUMINA.brightness") ?? "Brightness"} value={brightness} initial={initial.current.brightness} gamepadStep={0.001} onChange={setBrightness} />
        <RoadNumber label={translate("LUMINA.smoothness") ?? "Smoothness"} value={smoothness} initial={initial.current.smoothness} gamepadStep={0.00001} onChange={setSmoothness} />
      </div>
      <div className="road-material-section">
        <div className="road-material-heading road-material-heading-actions">
          <span>{(translate("LUMINA.roadcolor") ?? "Road color")}</span>
          <div className="road-material-actions">
            <button type="button" className="road-material-icon-button" title={translate("LUMINA.colorpicker") ?? "Open color picker website"} onClick={() => trigger(mod.id, "OpenColorPickerSite")}><RoadIcon name="palette" /></button>
            <button type="button" className="road-material-icon-button" title={translate("LUMINA.randomize") ?? "Randomize primary color"} onClick={() => trigger(mod.id, "HandleRandomizer")}><RoadIcon name="dice" /></button>
          </div>
        </div>
        <div className="road-material-color-row">
          <RoadColor label={translate("LUMINA.hexprimarycolor") ?? "Primary color"} value={primary} onCommit={setPrimary} />
          <RoadColor label={translate("LUMINA.hexsecondcolor") ?? "Secondary color"} value={secondary} onCommit={setSecondary} />
        </div>
        <div className="road-material-quick-label">{(translate("LUMINA.quickcolors") ?? "Quick colors")}</div>
        <div className="road-material-presets">
          {quickColors.map(p => (
            <button key={p.name} type="button" className={"road-material-preset" + (normalizeHex(primary || "") === p.primary && normalizeHex(secondary || "") === p.secondary ? " active" : "")} onClick={() => { setPrimary(p.primary); setSecondary(p.secondary); }}>
              <span className="road-material-preset-swatch"><span style={{ backgroundColor: p.primary }} /><span style={{ backgroundColor: p.secondary }} /></span>
              <span>{translate("LUMINA." + p.name.toLowerCase()) ?? p.name}</span>
            </button>
          ))}
        </div>
      </div>
      <div className="road-material-section">
        <div className="road-material-heading">{(translate("LUMINA.customtextures") ?? "Custom textures")}</div>
        <div className="road-material-texture-actions">
          <button type="button" className="road-material-button" onClick={() => trigger(mod.id, "OpenTexturesFolder")}><RoadIcon name="folder" />{translate("LUMINA.opentexturesbutton") ?? "Open textures folder"}</button>
          <button type="button" className="road-material-button" onClick={() => trigger(mod.id, "ApplyRoadTextures")}><RoadIcon name="reload" />{translate("LUMINA.applytexturesbutton") ?? "Reload textures"}</button>
        </div>
      </div>
      <div className="road-material-footer"><span>{(translate("LUMINA.roadmaterials") ?? "Road Materials")} · LUMINA</span><button type="button" className="road-material-button" onClick={reset}><RoadIcon name="reset" />{(translate("LUMINA.restoreallopening") ?? "Restore opening values")}</button></div>
    </div>
  );
};
