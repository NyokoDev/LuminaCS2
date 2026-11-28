import { useLocalization } from "cs2/l10n";
import React, { useEffect, useRef, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { Slider } from "mods/slider";
import mod from "./../../../mod.json";
import { SSAOQuickPresets } from "./SSAOQuickPresets/QuickPresets";

export interface SSAOPanelBaseProps {
  title?: string;
  children?: React.ReactNode;
  className?: string;
}

/* =========================
   Bindings
========================= */

const ssaoEnabled$ = bindValue<boolean>(mod.id, "IsScreenSpaceAmbientOcclusion");
const ssaoIntensity$ = bindValue<number>(mod.id, "AmbientOcclusionIntensity");
const ssaoMaxRadius$ = bindValue<number>(mod.id, "AmbientOcclusionMaxRadiusInPixels");
const ssaoRadius$ = bindValue<number>(mod.id, "AmbientOcclusionRadius");
const ssaoStepCount$ = bindValue<number>(mod.id, "AmbientOcclusionStepCount");
const ssaoDirectionCount$ = bindValue<number>(mod.id, "AmbientOcclusionDirectionCount");
const ssaoBilateral$ = bindValue<number>(mod.id, "AmbientOcclusionBilateralAggressiveness");
const ssaoGhosting$ = bindValue<number>(mod.id, "AmbientOcclusionGhostingReduction");
const ssaoDirectLighting$ = bindValue<number>(mod.id, "AmbientOcclusionDirectLightingStrength");

/* NEW BINDINGS */

const ssaoTemporal$ = bindValue<boolean>(mod.id, "AmbientOcclusionTemporalAccumulation");
const ssaoBlurSharpness$ = bindValue<number>(mod.id, "AmbientOcclusionBlurSharpness");
const ssaoFullRes$ = bindValue<boolean>(mod.id, "AmbientOcclusionFullResolution");
const ssaoBilateralUpsample$ = bindValue<boolean>(mod.id, "AmbientOcclusionBilateralUpsample");
const ssaoSpecular$ = bindValue<number>(mod.id, "AmbientOcclusionSpecularOcclusion");
const ssaoOccluderMotion$ = bindValue<boolean>(mod.id, "AmbientOcclusionOccluderMotionRejection");
const ssaoReceiverMotion$ = bindValue<boolean>(mod.id, "AmbientOcclusionReceiverMotionRejection");

const ssaoRayTracing$ = bindValue<boolean>(mod.id, "AmbientOcclusionRayTracing");
const ssaoRayLength$ = bindValue<number>(mod.id, "AmbientOcclusionRayLength");
const ssaoSampleCountRT$ = bindValue<number>(mod.id, "AmbientOcclusionSampleCount");
const ssaoDenoise$ = bindValue<boolean>(mod.id, "AmbientOcclusionDenoise");
const ssaoDenoiserRadius$ = bindValue<number>(mod.id, "AmbientOcclusionDenoiserRadius");

/* =========================
   Component
========================= */

type NumericControlProps = {
  label: string;
  value: number;
  min: number;
  max: number;
  step: number;
  disabled: boolean;
  onChange: (value: number) => void;
};

const SSAONumber = ({ label, value, min, max, step, disabled, onChange }: NumericControlProps) => {
  const { translate } = useLocalization();
  const format = (v: number) => Number.isFinite(v) ? String(Number(v.toFixed(3))) : "";
  const [draft, setDraft] = useState(format(value));
  const editing = useRef(false);
  const cancelled = useRef(false);
  const initial = useRef(value);

  useEffect(() => {
    if (!editing.current) setDraft(format(value));
    if (Number.isFinite(value) && !Number.isFinite(initial.current)) initial.current = value;
  }, [value]);

  return (
    <div className="ssao-setting-row">
      <span className="ssao-setting-label">{label}</span>
      <div className="ssao-slider-wrap">
        <Slider value={value} start={min} end={max} step={step} gamepadStep={step} disabled={disabled} noFill={false} onChange={onChange} className="ssao-setting-slider" />
      </div>
      <input className="ssao-number" type="text" inputMode="decimal" aria-label={label} value={draft} disabled={disabled}
        onFocus={() => { editing.current = true; cancelled.current = false; }}
        onChange={event => setDraft(event.target.value)}
        onBlur={() => {
          editing.current = false;
          const parsed = Number(draft);
          if (disabled || cancelled.current || !draft.trim() || !Number.isFinite(parsed)) {
            setDraft(format(value));
            return;
          }
          const clamped = Math.max(min, Math.min(max, parsed));
          const next = step === 1 && Number.isInteger(min) ? Math.round(clamped) : clamped;
          setDraft(format(next));
          if (next !== value) onChange(next);
        }}
        onKeyDown={event => {
          event.stopPropagation();
          if (event.key === "Enter" || event.key === "Escape") {
            event.preventDefault();
            cancelled.current = event.key === "Escape";
            event.currentTarget.blur();
          }
        }}
      />
      <button type="button" className="ssao-reset" title={(translate("LUMINA.restoreopening") ?? "Restore value from when this panel opened")} aria-label={(translate("LUMINA.restoreopening") ?? "Restore value from when this panel opened") + " · " + label} disabled={disabled || !Number.isFinite(initial.current)} onClick={() => onChange(initial.current)}>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8"><path d="M4 11a8 8 0 1 1 2 6M4 4v7h7" /></svg>
      </button>
    </div>
  );
};

const SSAOCheck = ({ label, value, disabled, onChange }: { label: string; value: boolean; disabled?: boolean; onChange: (value: boolean) => void }) => (
  <button type="button" className="ssao-check" role="checkbox" aria-checked={value} disabled={disabled} onClick={() => onChange(!value)}>
    <span className={"ssao-check-box" + (value ? " checked" : "")}>{value ? "✓" : ""}</span>
    <span>{label}</span>
  </button>
);

const SSAOSection = ({ title, open, onToggle, children }: { title: string; open: boolean; onToggle: () => void; children: React.ReactNode }) => (
  <div className="ssao-section">
    <button type="button" className="ssao-section-heading" aria-expanded={open} onClick={onToggle}>
      <span className={"ssao-chevron" + (open ? " open" : "")} />
      <span>{title}</span>
    </button>
    <div className={open ? "ssao-section-content" : "ssao-section-content ssao-collapsed"}>{children}</div>
  </div>
);

export const SSAOPanelBase: React.FC<SSAOPanelBaseProps> = ({
  title = "",
  children,
  className = "",
}) => {

  const { translate } = useLocalization();

  /* VALUES */

  const ssaoEnabled = useValue(ssaoEnabled$);
  const intensity = useValue(ssaoIntensity$);
  const maxRadius = useValue(ssaoMaxRadius$);
  const radius = useValue(ssaoRadius$);
  const stepCount = useValue(ssaoStepCount$);
  const directionCount = useValue(ssaoDirectionCount$);
  const bilateral = useValue(ssaoBilateral$);
  const ghosting = useValue(ssaoGhosting$);
  const directLighting = useValue(ssaoDirectLighting$);

  /* NEW VALUES */

  const temporal = useValue(ssaoTemporal$);
  const blurSharpness = useValue(ssaoBlurSharpness$);
  const fullRes = useValue(ssaoFullRes$);
  const bilateralUpsample = useValue(ssaoBilateralUpsample$);
  const specular = useValue(ssaoSpecular$);
  const occluderMotion = useValue(ssaoOccluderMotion$);
  const receiverMotion = useValue(ssaoReceiverMotion$);

  const rayTracing = useValue(ssaoRayTracing$);
  const rayLength = useValue(ssaoRayLength$);
  const sampleCountRT = useValue(ssaoSampleCountRT$);
  const denoise = useValue(ssaoDenoise$);
  const denoiserRadius = useValue(ssaoDenoiserRadius$);

  /* HANDLERS */

  const handleSSAOEnabled = (v: boolean) => trigger(mod.id, "HandleSSAOEnabled", v);
  const handleSSAOIntensity = (v: number) => trigger(mod.id, "HandleSSAOIntensity", v);
  const handleSSAOMaxRadius = (v: number) => trigger(mod.id, "HandleSSAOMaxRadius", v);
  const handleSSAORadius = (v: number) => trigger(mod.id, "HandleSSAORadius", v);
  const handleSSAOStepCount = (v: number) => trigger(mod.id, "HandleSSAOStepCount", v);
  const handleSSAODirectionCount = (v: number) => trigger(mod.id, "HandleSSAODirectionCount", v);
  const handleSSAOBilateral = (v: number) => trigger(mod.id, "HandleSSAOBilateral", v);
  const handleSSAOGhosting = (v: number) => trigger(mod.id, "HandleSSAOGhosting", v);
  const handleSSAODirectLighting = (v: number) => trigger(mod.id, "HandleSSAODirectLighting", v);

  /* NEW HANDLERS */

  const handleSSAOTemporal = (v: boolean) => trigger(mod.id, "HandleSSAOTemporalAccumulation", v);
  const handleSSAOBlurSharpness = (v: number) => trigger(mod.id, "HandleSSAOBLurSharpness", v);
  const handleSSAOFullResolution = (v: boolean) => trigger(mod.id, "HandleSSAOFullResolution", v);
  const handleSSAOBilateralUpsample = (v: boolean) => trigger(mod.id, "HandleSSAOBilateralUpsample", v);
  const handleSSAOSpecular = (v: number) => trigger(mod.id, "HandleSSAOSpecularOcclusion", v);
  const handleSSAOOccluderMotion = (v: boolean) => trigger(mod.id, "HandleSSAOOccluderMotionRejection", v);
  const handleSSAOReceiverMotion = (v: boolean) => trigger(mod.id, "HandleSSAOReceiverMotionRejection", v);

  const handleSSAORayTracing = (v: boolean) => trigger(mod.id, "HandleSSAORayTracing", v);
  const handleSSAORayLength = (v: number) => trigger(mod.id, "HandleSSAORayLength", v);
  const handleSSAOSampleCountRT = (v: number) => trigger(mod.id, "HandleSSAOSampleCount", v);
  const handleSSAODenoise = (v: boolean) => trigger(mod.id, "HandleSSAODenoise", v);
  const handleSSAODenoiserRadius = (v: number) => trigger(mod.id, "HandleSSAODenoiserRadius", v);


  const [sections, setSections] = useState({ ambient: true, filtering: true, ray: false });
  const toggleSection = (key: keyof typeof sections) => setSections(old => ({ ...old, [key]: !old[key] }));
  const disabled = !ssaoEnabled;
  const rayDisabled = disabled || !rayTracing;

  return (
    <div className={`ssao-panel-base ${className}`}>
      <div className="ssao-brand">
        <div><div className="ssao-brand-name">LUMINA</div><div className="ssao-brand-subtitle">{title || (translate("LUMINA.ssao") ?? "Screen Space Ambient Occlusion")}</div></div>
        <div className="ssao-brand-logo" />
      </div>
      {children}
      <div className="ssao-master-row">
        <span>{(translate("LUMINA.ao") ?? "Ambient Occlusion")}</span>
        <SSAOCheck label={ssaoEnabled ? (translate("LUMINA.enabled") ?? "Enabled") : (translate("LUMINA.disabled") ?? "Disabled")} value={ssaoEnabled} onChange={handleSSAOEnabled} />
      </div>
      <SSAOSection title={(translate("LUMINA.ao") ?? "Ambient Occlusion")} open={sections.ambient} onToggle={() => toggleSection("ambient")}>
        <SSAONumber label={(translate("LUMINA.intensity") ?? "Intensity")} value={intensity} min={0} max={4} step={0.01} disabled={disabled} onChange={handleSSAOIntensity} />
        <SSAONumber label={(translate("LUMINA.maxradius") ?? "Maximum Radius in Pixels")} value={maxRadius} min={16} max={256} step={1} disabled={disabled} onChange={handleSSAOMaxRadius} />
        <SSAONumber label={(translate("LUMINA.radius") ?? "Radius")} value={radius} min={0.25} max={5} step={0.25} disabled={disabled} onChange={handleSSAORadius} />
        <SSAONumber label={(translate("LUMINA.stepcount") ?? "Step Count")} value={stepCount} min={2} max={32} step={1} disabled={disabled} onChange={handleSSAOStepCount} />
        <SSAONumber label={(translate("LUMINA.directlighting") ?? "Direct Lighting Strength")} value={directLighting} min={0} max={1} step={0.01} disabled={disabled} onChange={handleSSAODirectLighting} />
      </SSAOSection>
      <SSAOSection title={(translate("LUMINA.filterquality") ?? "Filtering & Quality")} open={sections.filtering} onToggle={() => toggleSection("filtering")}>
        <SSAONumber label={(translate("LUMINA.bilateral") ?? "Bilateral Aggressiveness")} value={bilateral} min={0} max={1} step={0.01} disabled={disabled} onChange={handleSSAOBilateral} />
        <SSAONumber label={(translate("LUMINA.ghosting") ?? "Ghosting Reduction")} value={ghosting} min={0} max={1} step={0.01} disabled={disabled} onChange={handleSSAOGhosting} />
        <SSAONumber label={(translate("LUMINA.blursharpness") ?? "Blur Sharpness")} value={blurSharpness} min={0} max={1} step={0.01} disabled={disabled} onChange={handleSSAOBlurSharpness} />
        <SSAONumber label={(translate("LUMINA.specular") ?? "Specular Occlusion")} value={specular} min={0} max={1} step={0.01} disabled={disabled} onChange={handleSSAOSpecular} />
        <SSAONumber label={(translate("LUMINA.directioncount") ?? "Direction Count")} value={directionCount} min={1} max={6} step={1} disabled={disabled || temporal} onChange={handleSSAODirectionCount} />
        <div className="ssao-check-grid">
          <SSAOCheck label={(translate("LUMINA.temporal") ?? "Temporal Accumulation")} value={temporal} disabled={disabled} onChange={handleSSAOTemporal} />
          <SSAOCheck label={(translate("LUMINA.fullresolution") ?? "Full Resolution")} value={fullRes} disabled={disabled} onChange={handleSSAOFullResolution} />
          <SSAOCheck label={(translate("LUMINA.bilateralupsample") ?? "Bilateral Upsample")} value={bilateralUpsample} disabled={disabled} onChange={handleSSAOBilateralUpsample} />
          <SSAOCheck label={(translate("LUMINA.occludermotion") ?? "Occluder Motion Rejection")} value={occluderMotion} disabled={disabled} onChange={handleSSAOOccluderMotion} />
          <SSAOCheck label={(translate("LUMINA.receivermotion") ?? "Receiver Motion Rejection")} value={receiverMotion} disabled={disabled} onChange={handleSSAOReceiverMotion} />
        </div>
      </SSAOSection>
      <SSAOSection title={(translate("LUMINA.raytracing") ?? "Ray Tracing")} open={sections.ray} onToggle={() => toggleSection("ray")}>
        <SSAOCheck label={(translate("LUMINA.enableraytracing") ?? "Enable Ray Tracing")} value={rayTracing} disabled={disabled} onChange={handleSSAORayTracing} />
        <SSAONumber label={(translate("LUMINA.raylength") ?? "Ray Length")} value={rayLength} min={0.01} max={50} step={1} disabled={rayDisabled} onChange={handleSSAORayLength} />
        <SSAONumber label={(translate("LUMINA.samplecount") ?? "Sample Count")} value={sampleCountRT} min={1} max={64} step={1} disabled={rayDisabled} onChange={handleSSAOSampleCountRT} />
        <SSAOCheck label={(translate("LUMINA.denoise") ?? "Denoise")} value={denoise} disabled={rayDisabled} onChange={handleSSAODenoise} />
        <SSAONumber label={(translate("LUMINA.denoiserradius") ?? "Denoiser Radius")} value={denoiserRadius} min={0.1} max={1} step={0.001} disabled={rayDisabled || !denoise} onChange={handleSSAODenoiserRadius} />
      </SSAOSection>
      <div className="ssao-footer">SSAO · HDRP</div>
      <SSAOQuickPresets />
    </div>
  );
};
