import React from "react";
import {
  Palette,
  Sparkles,
  Aperture,
  Waves,
  type LucideIcon,
} from "lucide-react";

import { trigger } from "cs2/api";
import "./CAQuickPresets.scss";
import mod from "../../../../mod.json";
import { useLocalization } from "cs2/l10n";

type QuickPresetName =
  | "colorful"
  | "vibe"
  | "non-contemporary"
  | "flow";

interface QuickPreset {
  id: QuickPresetName;
  label: string;
  icon: LucideIcon;
  accentClass: string;
}

export const CAQuickPresets: React.FC = () => {
  const { translate } = useLocalization();

  const presets: QuickPreset[] = [
    {
      id: "colorful",
      label: translate("LUMINA.presetcolorful") ?? "Colorful",
      icon: Palette,
      accentClass: "preset-colorful",
    },
    {
      id: "vibe",
      label: translate("LUMINA.presetvibe") ?? "Vibe",
      icon: Sparkles,
      accentClass: "preset-vibe",
    },
    {
      id: "non-contemporary",
      label:
        translate("LUMINA.presetnoncontemporary") ?? "Non-Contemporary",
      icon: Aperture,
      accentClass: "preset-non-contemporary",
    },
    {
      id: "flow",
      label: translate("LUMINA.presetflow") ?? "Flow",
      icon: Waves,
      accentClass: "preset-flow",
    },
  ];

  const applyPreset = (preset: QuickPresetName) => {
    trigger(
      mod.id,
      "ApplyColorAdjustmentQuickPreset",
      preset
    );
  };

  return (
    <section className="quick-presets">
      <div className="quick-presets__header">
        <span className="quick-presets__title">
          {translate("LUMINA.caquickpresets")}
        </span>

        <span className="quick-presets__subtitle">
          {translate("LUMINA.oneclickcoloradjustment")}
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
              onClick={() => applyPreset(preset.id)}
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