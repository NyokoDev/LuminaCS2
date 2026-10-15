
import React from "react";
import "./Ready.scss";

import mod from "../../../../../mod.json";
import LuminaSVG from "../../../../img/Lumina.svg";

import {
  ArrowLeft,
  BookOpen,
  CheckCircle2,
  MessageCircle,
} from "lucide-react";

import { trigger } from "cs2/api";

interface ReadyProps {
  onBack: () => void;
  onComplete: () => void;
}

export const Ready: React.FC<ReadyProps> = ({
  onBack,
  onComplete,
}) => {
  const handleOpenDocs = () => {
    trigger(mod.id, "OpenLuminaDocs");
  };

  const handleOpenDiscord = () => {
    trigger(mod.id, "discordlink");
  };

  return (
    <div className="workflow">
      <header className="workflow__header">
        <div className="workflow__ready-logo">
          <img src={LuminaSVG} alt="" />
        </div>

        <h1 className="workflow__title">
          You're ready to go!
        </h1>

        <p className="workflow__subtitle">
          Lumina is ready. Here's where to find help
          and learn more.
        </p>
      </header>

      <div className="workflow__options">
        {/* Documentation card */}
        <button
          type="button"
          className="workflow-card workflow-card--resource"
          onClick={handleOpenDocs}
        >
          <span className="workflow-card__badge">
            Documentation
          </span>

          <div className="workflow-card__ngx-logo">
            <BookOpen />
          </div>

          <h2 className="workflow-card__title">
            Lumina Docs
          </h2>

          <p className="workflow-card__description">
            Learn how to use Lumina and explore its
            features, settings, and customization options.
          </p>

          <div className="workflow-card__divider" />

          <div className="workflow-card__resource-info">
            <BookOpen className="workflow-card__feature-icon" />
            <span>
              skylinx.gitbook.io/lumina
            </span>
          </div>
        </button>

        {/* Discord card */}
        <button
          type="button"
          className="workflow-card workflow-card--resource"
          onClick={handleOpenDiscord}
        >
          <span className="workflow-card__badge workflow-card__badge--advanced">
            Community &amp; Support
          </span>

          <div className="workflow-card__ngx-logo">
            <MessageCircle />
          </div>

          <h2 className="workflow-card__title">
            Join our Discord
          </h2>

          <p className="workflow-card__description">
            Connect with other players, ask questions,
            and get help from the Lumina community.
          </p>

          <div className="workflow-card__divider" />

          <div className="workflow-card__resource-info">
            <MessageCircle className="workflow-card__feature-icon" />
            <span>
              discord.gg/5gZgRNm29e
            </span>
          </div>
        </button>
      </div>

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
          <span />
          <span className="workflow__step--active" />
        </div>

        <button
          type="button"
          className="workflow__continue"
          onClick={onComplete}
        >
          Finish
          <CheckCircle2 className="workflow__nav-icon" />
        </button>
      </footer>
    </div>
  );
};
