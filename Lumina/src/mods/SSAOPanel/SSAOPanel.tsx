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
/* =========================
   Component
========================= */

export const SSAOPanelBase: React.FC<SSAOPanelBaseProps> = ({
  title = "",
  children,
  className = "",
}) => {
  /* =========================
     Values
  ========================= */

  const ssaoEnabled = useValue(ssaoEnabled$);
  const intensity = useValue(ssaoIntensity$);
  const maxRadius = useValue(ssaoMaxRadius$);
  const radius = useValue(ssaoRadius$);
  const stepCount = useValue(ssaoStepCount$);
  const bilateral = useValue(ssaoBilateral$);
  const ghosting = useValue(ssaoGhosting$);
  const directLighting = useValue(ssaoDirectLighting$);

  /* =========================
     Handlers
  ========================= */

const handleSSAOEnabled = (value: boolean) => {
  trigger(mod.id, "HandleSSAOEnabled", value);
};

const handleSSAOIntensity = (value: number) => {
  trigger(mod.id, "HandleSSAOIntensity", value);
};

const handleSSAOMaxRadius = (value: number) => {
  trigger(mod.id, "HandleSSAOMaxRadius", value);
};

const handleSSAORadius = (value: number) => {
  trigger(mod.id, "HandleSSAORadius", value);
};

const handleSSAOStepCount = (value: number) => {
  trigger(mod.id, "HandleSSAOStepCount", value);
};

const handleSSAOBilateral = (value: number) => {
  trigger(mod.id, "HandleSSAOBilateral", value);
};

const handleSSAOGhosting = (value: number) => {
  trigger(mod.id, "HandleSSAOGhosting", value);
};

const handleSSAODirectLighting = (value: number) => {
  trigger(mod.id, "HandleSSAODirectLighting", value);
};
  /* =========================
     UI
  ========================= */

  return (
    <div className={`ssao-panel-base ${className}`}>
      {title && <div className="ssaoconfiglabel">{title}</div>}
      {children}

      {/* Enable / Disable */}
      <button
        className={`SSAOToggleButton ${ssaoEnabled ? "enabled" : "disabled"}`}
        onClick={() => handleSSAOEnabled(!ssaoEnabled)}
      >
        {ssaoEnabled ? "SSAO Enabled" : "SSAO Disabled"}
      </button>

      {/* Intensity */}
      <label className="title_SVH title_zQN">SSAO Intensity</label>
      <Slider
        value={intensity}
        start={0}
        end={5}
        step={0.01}
        className="SSAOIntensitySlider"
        onChange={(number: number) => handleSSAOIntensity(number)}
        gamepadStep={0.01}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Max Radius */}
      <label className="title_SVH title_zQN">Max Radius (Pixels)</label>
      <Slider
        value={maxRadius}
        start={0}
        end={256}
        step={1}
        className="SSAOMaxRadiusSlider"
        onChange={(number: number) => handleSSAOMaxRadius(number)}
        gamepadStep={1}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Radius */}
      <label className="title_SVH title_zQN">Radius</label>
      <Slider
        value={radius}
        start={0}
        end={5}
        step={0.01}
        className="SSAORadiusSlider"
        onChange={(number: number) => handleSSAORadius(number)}
        gamepadStep={0.01}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Step Count */}
      <label className="title_SVH title_zQN">Step Count</label>
      <Slider
        value={stepCount}
        start={1}
        end={32}
        step={1}
        className="SSAOStepCountSlider"
        onChange={(number: number) => handleSSAOStepCount(number)}
        gamepadStep={1}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Bilateral */}
      <label className="title_SVH title_zQN">
        Bilateral Aggressiveness
      </label>
      <Slider
        value={bilateral}
        start={0}
        end={1}
        step={0.01}
        className="SSAOBilateralSlider"
        onChange={(number: number) => handleSSAOBilateral(number)}
        gamepadStep={0.01}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Ghosting */}
      <label className="title_SVH title_zQN">
        Ghosting Reduction
      </label>
      <Slider
        value={ghosting}
        start={0}
        end={1}
        step={0.01}
        className="SSAOGhostingSlider"
        onChange={(number: number) => handleSSAOGhosting(number)}
        gamepadStep={0.01}
        disabled={!ssaoEnabled}
        noFill={false}
      />

      {/* Direct Lighting */}
      <label className="title_SVH title_zQN">
        Direct Lighting Strength
      </label>
      <Slider
        value={directLighting}
        start={0}
        end={1}
        step={0.01}
        className="SSAODirectLightingSlider"
        onChange={(number: number) => handleSSAODirectLighting(number)}
        gamepadStep={0.01}
        disabled={!ssaoEnabled}
        noFill={false}
      />
    </div>
  );
};