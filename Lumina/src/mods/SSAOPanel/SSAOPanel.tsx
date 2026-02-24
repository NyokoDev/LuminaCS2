import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { Slider } from "mods/slider";
import mod from "./../../../mod.json";

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

export const SSAOPanelBase: React.FC<SSAOPanelBaseProps> = ({
  title = "",
  children,
  className = "",
}) => {

  /* VALUES */

  const ssaoEnabled = useValue(ssaoEnabled$);
  const intensity = useValue(ssaoIntensity$);
  const maxRadius = useValue(ssaoMaxRadius$);
  const radius = useValue(ssaoRadius$);
  const stepCount = useValue(ssaoStepCount$);
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
  const handleSSAOBilateral = (v: number) => trigger(mod.id, "HandleSSAOBilateral", v);
  const handleSSAOGhosting = (v: number) => trigger(mod.id, "HandleSSAOGhosting", v);
  const handleSSAODirectLighting = (v: number) => trigger(mod.id, "HandleSSAODirectLighting", v);

  /* NEW HANDLERS */

  const handleSSAOTemporal = (v: boolean) => trigger(mod.id, "HandleSSAOTemporalAccumulation", v);
  const handleSSAOBlurSharpness = (v: number) => trigger(mod.id, "HandleSSAOBLurSharpness", v);
  const handleSSAOFullResolution = (v: boolean) => trigger(mod.id, "HandleSSAOFullResolution", v);
  const handleSSAOBilateralUpsample = (v: boolean) => trigger(mod.id, "HandleSSAOBilateralUpsample", v);
  const handleSSAODirectionCount = (v: number) => trigger(mod.id, "HandleSSAODirectionCount", v);
  const handleSSAOSpecular = (v: number) => trigger(mod.id, "HandleSSAOSpecularOcclusion", v);
  const handleSSAOOccluderMotion = (v: boolean) => trigger(mod.id, "HandleSSAOOccluderMotionRejection", v);
  const handleSSAOReceiverMotion = (v: boolean) => trigger(mod.id, "HandleSSAOReceiverMotionRejection", v);

  const handleSSAORayTracing = (v: boolean) => trigger(mod.id, "HandleSSAORayTracing", v);
  const handleSSAORayLength = (v: number) => trigger(mod.id, "HandleSSAORayLength", v);
  const handleSSAOSampleCountRT = (v: number) => trigger(mod.id, "HandleSSAOSampleCount", v);
  const handleSSAODenoise = (v: boolean) => trigger(mod.id, "HandleSSAODenoise", v);
  const handleSSAODenoiserRadius = (v: number) => trigger(mod.id, "HandleSSAODenoiserRadius", v);

  /* UI */

  return (
    <div className={`ssao-panel-base ${className}`}>
      {title && <div className="ssaoconfiglabel">{title}</div>}
      {children}


      <button
        className={`SSAOToggleButton ${ssaoEnabled ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOEnabled(!ssaoEnabled)}
      >
        {ssaoEnabled ? "SSAO Enabled" : "SSAO Disabled"}
      </button>

      {/* EXISTING SLIDERS REMAIN HERE (unchanged) */}
{/* ===== EXISTING SSAO SLIDERS ===== */}

<label className="title_SVH title_zQN">Intensity</label>
<Slider
    value={intensity}
    start={0}
    end={5}
    step={0.01}
    onChange={handleSSAOIntensity}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />

<label className="title_SVH title_zQN">Radius</label>
<Slider
    value={radius}
    start={0}
    end={5}
    step={0.25}
    onChange={handleSSAORadius}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />

<label className="title_SVH title_zQN">Step Count</label>
<Slider
    value={stepCount}
    start={1}
    end={16}
    step={1}
    onChange={handleSSAOStepCount}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />

<label className="title_SVH title_zQN">Bilateral Aggressiveness</label>
<Slider
    value={bilateral}
    start={0}
    end={5}
    step={0.01}
    onChange={handleSSAOBilateral}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />

<label className="title_SVH title_zQN">Ghosting Reduction</label>
<Slider
    value={ghosting}
    start={0.5}
    end={1}
    step={0}
    onChange={handleSSAOGhosting}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />

<label className="title_SVH title_zQN">Direct Lighting Strength</label>
<Slider
    value={directLighting}
    start={0}
    end={1}
    step={0.01}
    onChange={handleSSAODirectLighting}
    disabled={!ssaoEnabled} gamepadStep={0} noFill={false} />


      {/* ===== NEW CONTROLS ===== */}

      <button
        className={`SSAOToggleButton ${temporal ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOTemporal(!temporal)}
        disabled={!ssaoEnabled}
      >
        Temporal Accumulation
      </button>

      <label className="title_SVH title_zQN">Blur Sharpness</label>
      <Slider
              value={blurSharpness}
              start={0.1}
              end={1}
              step={0.01}
              onChange={handleSSAOBlurSharpness}
              disabled={!ssaoEnabled} gamepadStep={0} noFill={false}      />

      <button
        className={`SSAOToggleButton ${fullRes ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOFullResolution(!fullRes)}
        disabled={!ssaoEnabled}
      >
        Full Resolution
      </button>

      <button
        className={`SSAOToggleButton ${bilateralUpsample ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOBilateralUpsample(!bilateralUpsample)}
        disabled={!ssaoEnabled}
      >
        Bilateral Upsample
      </button>

      <label className="title_SVH title_zQN">Specular Occlusion</label>
      <Slider
              value={specular}
              start={0}
              end={1}
              step={0.01}
              onChange={handleSSAOSpecular}
              disabled={!ssaoEnabled} gamepadStep={0} noFill={false}      />

      <button
        className={`SSAOToggleButton ${occluderMotion ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOOccluderMotion(!occluderMotion)}
        disabled={!ssaoEnabled}
      >
        Occluder Motion Rejection
      </button>

      <button
        className={`SSAOToggleButton ${receiverMotion ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOReceiverMotion(!receiverMotion)}
        disabled={!ssaoEnabled}
      >
        Receiver Motion Rejection
      </button>

      {/* ===== RAY TRACING SECTION ===== */}

      <button
        className={`SSAOToggleButton ${rayTracing ? "enabled" : "disabled"}`}
        onClick={() => handleSSAORayTracing(!rayTracing)}
        disabled={!ssaoEnabled}
      >
        Ray Tracing
      </button>

      <label className="title_SVH title_zQN">Ray Length</label>
      <Slider
              value={rayLength}
              start={0.01}
              end={50}
              step={1}
              onChange={handleSSAORayLength}
              disabled={!ssaoEnabled || !rayTracing} gamepadStep={0} noFill={false}      />

      <label className="title_SVH title_zQN">Sample Count</label>
      <Slider
              value={sampleCountRT}
              start={1}
              end={64}
              step={1}
              onChange={handleSSAOSampleCountRT}
              disabled={!ssaoEnabled || !rayTracing} gamepadStep={0} noFill={false}      />

      <button
        className={`SSAOToggleButton ${denoise ? "enabled" : "disabled"}`}
        onClick={() => handleSSAODenoise(!denoise)}
        disabled={!ssaoEnabled || !rayTracing}
      >
        Denoise
      </button>

      <label className="title_SVH title_zQN">Denoiser Radius</label>
      <Slider
              value={denoiserRadius}
              start={0.1}
              end={1}
              step={0.01}
              onChange={handleSSAODenoiserRadius}
              disabled={!ssaoEnabled || !rayTracing} gamepadStep={0} noFill={false}      />
    </div>
  );
};