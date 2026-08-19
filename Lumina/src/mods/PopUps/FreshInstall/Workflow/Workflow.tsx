import React, { useState } from "react";
import "./Workflow.scss";

import LuminaSVG from "../../../../img/Lumina.svg";
import { trigger } from "cs2/api";
import mod from "../../../../../mod.json";

import {
  ArrowLeft,
  ArrowRight,
  Boxes,
  CheckCircle2,
  Gauge,
  Layers3,
  Save,
  SlidersHorizontal,
  Sparkles,
  WandSparkles,
} from "lucide-react";

export type WorkflowType = "classic" | "ngx";

interface WorkflowProps {
  onBack: () => void;
  onComplete: (workflow: WorkflowType) => void;
}

export const Workflow: React.FC<WorkflowProps> = ({
  onBack,
  onComplete,
}) => {
  const [selected, setSelected] =
    useState<WorkflowType>("classic");


  const handleComplete = () => {
    if (selected === "ngx") {
      trigger(mod.id, "UserSelectNGXMode");
    }

    onComplete(selected);

    trigger(mod.id, "StopFreshInstall");
  };

  return (
    <div className="workflow">
      {/* HEADER */}

      <header className="workflow__header">
        <h1 className="workflow__title">
          Choose your workflow
        </h1>

        <p className="workflow__subtitle">
          You can change this at any time.
        </p>
      </header>

      {/* OPTIONS */}

      <div className="workflow__options">
        {/* =================================
            LUMINA CLASSIC
        ================================= */}

        <button
          type="button"
          className={`workflow-card ${
            selected === "classic"
              ? "workflow-card--selected"
              : ""
          }`}
          onClick={() => setSelected("classic")}
        >
          <span className="workflow-card__badge">
            Recommended
          </span>

          <img
            className="workflow-card__logo"
            src={LuminaSVG}
            alt=""
          />

          <h2 className="workflow-card__title">
            Lumina
          </h2>

          <p className="workflow-card__description">
            The easiest way to enhance your game's visuals.
          </p>

          <div className="workflow-card__divider" />

          <ul className="workflow-card__features">
            <li>
              <CheckCircle2 className="workflow-card__feature-icon" />
              <span>Easy to use</span>
            </li>

            <li>
              <SlidersHorizontal className="workflow-card__feature-icon" />
              <span>Friendly interface</span>
            </li>

            <li>
              <Sparkles className="workflow-card__feature-icon" />
              <span>Quick Presets</span>
            </li>
          </ul>
        </button>

        {/* =================================
            LUMINA NGX
        ================================= */}

        <button
          type="button"
          className={`workflow-card ${
            selected === "ngx"
              ? "workflow-card--selected"
              : ""
          }`}
          onClick={() => setSelected("ngx")}
        >
          <span
            className="
              workflow-card__badge
              workflow-card__badge--advanced
            "
          >
            Advanced
          </span>

          <div className="workflow-card__ngx-logo">
            <Gauge />
          </div>

          <h2 className="workflow-card__title">
            Lumina NGX
          </h2>

          <p className="workflow-card__description">
            Direct access to Cities: Skylines II's HDRP
            Volume system.
          </p>

          <div className="workflow-card__divider" />

          <ul className="workflow-card__features">
            <li>
              <Layers3 className="workflow-card__feature-icon" />
              <span>Volume Inspector</span>
            </li>

            <li>
              <Boxes className="workflow-card__feature-icon" />
              <span>Any HDRP component</span>
            </li>

            <li>
              <Save className="workflow-card__feature-icon" />
              <span>NGX Presets</span>
            </li>

            <li>
              <WandSparkles className="workflow-card__feature-icon" />
              <span>Maximum customization</span>
            </li>
          </ul>
        </button>
      </div>

      {/* FOOTER */}

      <footer className="workflow__footer">
        <button
          type="button"
          className="workflow__back"
          onClick={onBack}
        >
          <ArrowLeft className="workflow__nav-icon" />
          Back
        </button>

        <div className="workflow__steps">
          <span />
          <span className="workflow__step--active" />
        </div>

        <button
          type="button"
          className="workflow__continue"
          onClick={handleComplete}
        >
          Continue
          <ArrowRight className="workflow__nav-icon" />
        </button>
      </footer>
    </div>
  );
};