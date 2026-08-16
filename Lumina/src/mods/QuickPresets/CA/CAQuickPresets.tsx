import React from "react";
import {
  Palette,
  Sparkles,
  Aperture,
  Waves,
  type LucideIcon,
} from "lucide-react";

import "./CAQuickPresets.scss";

type QuickPresetName =
  | "colorful"
  | "vibe"
  | "non-contemporary"
  | "flow";

interface QuickPresetsProps {
  onPreset?: (preset: QuickPresetName) => void;
}

interface QuickPreset {
  id: QuickPresetName;
  label: string;
  icon: LucideIcon;
  accentClass: string;
}

const presets: QuickPreset[] = [
  {
    id: "colorful",
    label: "Colorful",
    icon: Palette,
    accentClass: "preset-colorful",
  },
  {
    id: "vibe",
    label: "Vibe",
    icon: Sparkles,
    accentClass: "preset-vibe",
  },
  {
    id: "non-contemporary",
    label: "Non-Contemporary",
    icon: Aperture,
    accentClass: "preset-non-contemporary",
  },
  {
    id: "flow",
    label: "Flow",
    icon: Waves,
    accentClass: "preset-flow",
  },
];

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
        {presets.map((preset) => {
          const Icon = preset.icon;

          return (
            <button
              key={preset.id}
              type="button"
              className={`quick-preset ${preset.accentClass}`}
              onClick={() => onPreset?.(preset.id)}
            >
              <div className="quick-preset__content">
                <span className="quick-preset__icon">
                  <Icon
                    className="quick-preset__lucide"
                    strokeWidth={1.7}
                  />
                </span>

                <span className="quick-preset__label">
                  {preset.label}
                </span>
              </div>

              <span
                className="quick-preset__accent"
                aria-hidden="true"
              />
            </button>
          );
        })}
      </div>
    </section>
  );
};