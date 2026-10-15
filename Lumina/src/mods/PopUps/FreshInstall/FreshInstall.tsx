import React, { useState } from "react";
import "./FreshInstall.scss";

import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";

import mod from "../../../../mod.json";
import LuminaSVG from "../../../img/Lumina.svg";

import { Workflow, WorkflowType } from "./Workflow/Workflow";
import { Ready } from "./Workflow/Ready";

const freshInstall$ = bindValue<boolean>(
  mod.id,
  "FreshInstall"
);

type OnboardingStep = "welcome" | "workflow" | "ready";

export const FreshInstall: React.FC = () => {
  const { translate } = useLocalization();
  const isFreshInstall = useValue(freshInstall$);

  const [step, setStep] =
    useState<OnboardingStep>("welcome");

const handleWorkflowComplete = (
  workflow: WorkflowType
) => {
  console.log("Selected workflow:", workflow);
  setStep("ready");
};

  const handleSkip = () => {
    trigger(mod.id, "StopFreshInstall");
  };

  if (!isFreshInstall) {
    return null;
  }

  /* ========================================
     WORKFLOW
  ======================================== */

  if (step === "workflow") {
    return (
      <Workflow
        onBack={() => setStep("welcome")}
        onComplete={handleWorkflowComplete}
      />
    );
  }


if (step === "ready") {
  return (
    <Ready
      onBack={() => setStep("workflow")}
      onComplete={() => trigger(mod.id, "StopFreshInstall")}
    />
  );
}

  /* ========================================
     WELCOME
  ======================================== */

  return (
    <div className="fresh-install fresh-install--visible">
      <div
        className="fresh-install__logo"
        aria-hidden="true"
      >
        <img
          className="fresh-install__logo-svg"
          src={LuminaSVG}
          alt=""
        />
      </div>

      <div className="fresh-install__content">
        <h1 className="fresh-install__title">
          Welcome to Lumina
        </h1>

        <p className="fresh-install__message">
          {translate("LUMINA.thankyou") ??
            "Thank you for installing Lumina."}
        </p>

          <div className="fresh-install__steps" aria-label="Setup progress">
    <span className="fresh-install__step fresh-install__step--active" />
    <span className="fresh-install__step" />
  </div>

        <p className="fresh-install__description">
          This onboarding process will make things easier for you.
        </p>
      </div>

      <div className="fresh-install__actions">
        <button
          className="fresh-install__button"
          type="button"
          onClick={() => setStep("workflow")}
        >
          {translate("LUMINA.getstarted") ??
            "Get Started"}

          <span aria-hidden="true">›</span>
        </button>

        <button
          className="fresh-install__skip"
          type="button"
          onClick={handleSkip}
        >
          Already know Lumina? Skip ›
        </button>
      </div>
    </div>
  );
};