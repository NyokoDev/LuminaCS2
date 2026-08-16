import React from "react";
import "./CAQuickPresets.scss";

type QuickPresetName =
  | "colorful"
  | "vibe"
  | "non-contemporary"
  | "flow";

interface QuickPresetsProps {
  onPreset?: (preset: QuickPresetName) => void;
}

const presets = [
  {
    id: "colorful",
    label: "Colorful",
    icon: "◉",
    accentClass: "preset-colorful",
  },
  {
    id: "vibe",
    label: "Vibe",
    icon: "⌁",
    accentClass: "preset-vibe",
  },
  {
    id: "non-contemporary",
    label: "Non-Contemporary",
    icon: "▥",
    accentClass: "preset-non-contemporary",
  },
  {
    id: "flow",
    label: "Flow",
    icon: "≈",
    accentClass: "preset-flow",
  },
] as const;

export const CAQuickPresets: React.FC<QuickPresetsProps> = ({
  onPreset,
}) => {
  return (
    <section className="quick-presets">
      <div className="quick-presets__header">
        <span className="quick-presets__title">
          Quick Presets
        </span>

        <span className="quick-presets__subtitle">
          One click. Different vibes.
        </span>
      </div>

      <div className="quick-presets__items">
        {presets.map((preset) => (
          <button
            key={preset.id}
            type="button"
            className={`quick-preset ${preset.accentClass}`}
            onClick={() => onPreset?.(preset.id)}
          >
            <div className="quick-preset__content">
              <span className="quick-preset__icon">
                {preset.icon}
              </span>

              <span className="quick-preset__label">
                {preset.label}
              </span>
            </div>

            <span className="quick-preset__accent" />
          </button>
        ))}
      </div>
    </section>
  );
};