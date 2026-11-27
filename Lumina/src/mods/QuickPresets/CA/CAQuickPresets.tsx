import React, { useRef, useState } from "react";
import ViceCityRacer from "./ViceCityRacer";
import ViceCityLogo from "../../../img/ViceCity.svg";
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
  | "flow"
  | "vice-city";

interface QuickPreset {
  id: QuickPresetName;
  label: string;
  icon: LucideIcon;
  accentClass: string;
}

export const CAQuickPresets: React.FC = () => {
  const { translate } = useLocalization();
  const [showRacer, setShowRacer] = useState(false);
  const viceClicks = useRef({ count: 0, last: 0 });

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

  const handleViceCity = () => {
    const now = Date.now();
    const state = viceClicks.current;
    state.count = now - state.last > 1500 ? 1 : state.count + 1;
    state.last = now;
    if (state.count === 1) applyPreset("vice-city");
    if (state.count >= 5) {
      state.count = 0;
      state.last = 0;
      setShowRacer(true);
    }
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
        <button
          type="button"
          className="quick-preset preset-vice-city"
          title="Vice City '86"
          aria-label="Vice City '86"
          onClick={handleViceCity}
        >
          <img className="quick-preset__vice-logo" src={ViceCityLogo} alt="Vice City" />
          <span className="quick-preset__accent" aria-hidden="true" />
        </button>
      </div>
      {showRacer && <ViceCityRacer onClose={() => setShowRacer(false)} />}
    </section>
  );
};