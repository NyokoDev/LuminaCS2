import React from "react";
import { trigger } from "cs2/api";
import { Zap, CircleDot, ChartNoAxesColumnIncreasing, Sun, CircleHelp, ChevronRight } from "lucide-react";
import mod from "../../../../mod.json";
import "./SSAOQuickPresets.scss";

type SSAOPreset = "subtle" | "heavy" | "dark" | "light";

interface SSAOQuickPresetsProps {
  quality: string;
  qualityOptions: { id: string; label: string }[];
  onQualityChange: (id: string) => void;
}

const styles = [
  { id: "subtle" as SSAOPreset, label: "Subtle", description: "Soft + natural", icon: CircleDot },
  { id: "heavy" as SSAOPreset, label: "Heavy", description: "Strong definition", icon: ChartNoAxesColumnIncreasing },
  { id: "dark" as SSAOPreset, label: "Dark", description: "Deep contact shadows", icon: CircleDot },
  { id: "light" as SSAOPreset, label: "Light", description: "Minimal AO", icon: Sun },
];

export const SSAOQuickPresets: React.FC<SSAOQuickPresetsProps> = ({ quality, qualityOptions, onQualityChange }) => (
  <div className="ssao-quick-presets">
    <div className="ssao-quick-presets__title-row">
      <Zap className="ssao-quick-presets__bolt" />
      <span className="ssao-quick-presets__title">SSAO PRESETS</span>
      <button type="button" className="ssao-presets-help" onClick={() => trigger(mod.id, "OpenSSAOHelp")}>
        <CircleHelp />
        <span>SSAO Help</span>
        <ChevronRight />
      </button>
    </div>
    <div className="ssao-quality-row">
      <span className="ssao-quality-label">Rendering quality</span>
      <div className="ssao-quality-buttons">
        {qualityOptions.map(p => (
          <button key={p.id} type="button" className={"ssao-quality-button" + (quality === p.id ? " active" : "")}
            disabled={p.id === "custom" && quality !== "disabled"}
            onClick={() => onQualityChange(p.id)}>
            {p.label}
          </button>
        ))}
      </div>
    </div>
    <div className="ssao-style-row">
      <span className="ssao-quality-label">Visual style</span>
      <div className="ssao-style-buttons">
        {styles.map(p => {
          const Icon = p.icon;
          return (
            <button key={p.id} type="button" className="ssao-preset-card" onClick={() => trigger(mod.id, "ApplySSAOPreset", p.id)}>
              <div className={"ssao-preset-card__icon " + p.id}><Icon /></div>
              <div className="ssao-preset-card__text">
                <span className="ssao-preset-card__name">{p.label}</span>
                <span className="ssao-preset-card__description">{p.description}</span>
              </div>
            </button>
          );
        })}
      </div>
      <span className="ssao-presets-note">Style presets also adjust rendering quality. Choose a quality afterward to retain the look.</span>
    </div>
  </div>
);
