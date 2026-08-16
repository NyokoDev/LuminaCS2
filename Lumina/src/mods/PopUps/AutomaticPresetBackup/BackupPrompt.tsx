import React from "react";
import "./BackupPrompt.scss";
import LuminaSVG from "../../img/Lumina.svg";

interface BackupPromptProps {
  onAccept?: () => void;
  onDecline?: () => void;
}

export const BackupPrompt: React.FC<BackupPromptProps> = ({
  onAccept,
  onDecline,
}) => {
  return (
    <div className="backup-prompt" role="dialog" aria-modal="true">
      <div className="backup-prompt__content">
        <img
          src={LuminaSVG}
          className="backup-prompt__logo"
          alt=""
          aria-hidden="true"
        />

        <h1 className="backup-prompt__title">
          Do automatic backups of your presets?
        </h1>

        <div className="backup-prompt__actions">
          <button
            className="backup-prompt__button backup-prompt__button--accept"
            type="button"
            onClick={onAccept}
          >
            SURE
          </button>

          <button
            className="backup-prompt__button backup-prompt__button--decline"
            type="button"
            onClick={onDecline}
          >
            GTFO
          </button>
        </div>
      </div>
    </div>
  );
};

export default BackupPrompt;