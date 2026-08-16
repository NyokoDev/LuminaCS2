import React, { useState } from "react";
import { trigger } from "cs2/api";

import {
  Zap,
  CircleDot,
  ChartNoAxesColumnIncreasing,
  Sun,
  CircleHelp,
  ChevronRight,
} from "lucide-react";

import mod from "../../../../mod.json";
import "./SSAOQuickPresets.scss";

type SSAOPreset = "subtle" | "heavy" | "dark" | "light";

export const SSAOQuickPresets: React.FC = () => {
  const [activePreset, setActivePreset] =
    useState<SSAOPreset | null>(null);

  const applyPreset = (preset: SSAOPreset) => {
    setActivePreset(preset);

    trigger(
      mod.id,
      "ApplySSAOPreset",
      preset
    );
  };

  const openHelp = () => {
    trigger(
      mod.id,
      "OpenSSAOHelp"
    );
  };

  return (
    <div className="ssao-quick-presets">

      {/* INTRO */}

      <div className="ssao-quick-presets__intro">
        <div className="ssao-quick-presets__title-row">
          <Zap className="ssao-quick-presets__bolt" />

          <span className="ssao-quick-presets__title">
            QUICK PRESETS
          </span>
        </div>

        <div className="ssao-quick-presets__subtitle">
          One-click ambient occlusion setups
        </div>
      </div>

      {/* PRESETS */}

      <div className="ssao-quick-presets__items">

        <div className="ssao-preset-row">

          {/* SUBTLE */}

          <button
            className={`ssao-preset-card ${
              activePreset === "subtle"
                ? "active"
                : ""
            }`}
            type="button"
            onClick={() => applyPreset("subtle")}
          >
            <div className="ssao-preset-card__icon subtle">
              <CircleDot />
            </div>

            <div className="ssao-preset-card__text">
              <span className="ssao-preset-card__name">
                SUBTLE
              </span>

              <span className="ssao-preset-card__description">
                Soft + natural
              </span>
            </div>
          </button>

          {/* HEAVY */}

          <button
            className={`ssao-preset-card ${
              activePreset === "heavy"
                ? "active"
                : ""
            }`}
            type="button"
            onClick={() => applyPreset("heavy")}
          >
            <div className="ssao-preset-card__icon heavy">
              <ChartNoAxesColumnIncreasing />
            </div>

            <div className="ssao-preset-card__text">
              <span className="ssao-preset-card__name">
                HEAVY
              </span>

              <span className="ssao-preset-card__description">
                Strong definition
              </span>
            </div>
          </button>

        </div>

        <div className="ssao-preset-row">

          {/* DARK */}

          <button
            className={`ssao-preset-card ${
              activePreset === "dark"
                ? "active"
                : ""
            }`}
            type="button"
            onClick={() => applyPreset("dark")}
          >
            <div className="ssao-preset-card__icon dark">
              <CircleDot />
            </div>

            <div className="ssao-preset-card__text">
              <span className="ssao-preset-card__name">
                DARK
              </span>

              <span className="ssao-preset-card__description">
                Deep contact shadows
              </span>
            </div>
          </button>

          {/* LIGHT */}

          <button
            className={`ssao-preset-card ${
              activePreset === "light"
                ? "active"
                : ""
            }`}
            type="button"
            onClick={() => applyPreset("light")}
          >
            <div className="ssao-preset-card__icon light">
              <Sun />
            </div>

            <div className="ssao-preset-card__text">
              <span className="ssao-preset-card__name">
                LIGHT
              </span>

              <span className="ssao-preset-card__description">
                Minimal AO
              </span>
            </div>
          </button>

        </div>
      </div>

      {/* HELP */}

      <button
        className="ssao-quick-presets__help"
        type="button"
        onClick={openHelp}
      >
        <div className="ssao-quick-presets__help-icon">
          <CircleHelp />
        </div>

        <div className="ssao-quick-presets__help-text">
          <span className="ssao-quick-presets__help-title">
            SSAO HELP
          </span>

          <span className="ssao-quick-presets__help-description">
            Learn how ambient occlusion works
          </span>
        </div>

        <ChevronRight className="ssao-quick-presets__help-arrow" />
      </button>

    </div>
  );
};