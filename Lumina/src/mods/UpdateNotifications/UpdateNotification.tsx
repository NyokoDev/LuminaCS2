import React, { useState } from "react";
import "./UpdateNotification.scss";
import LuminaSVG from "../../img/Lumina.svg";

export const UpdateNotification: React.FC = () => {
  const [visible, setVisible] = useState(true);

  if (!visible) {
    return null;
  }

  return (
    <div
      className="update-notification"
      role="dialog"
      aria-labelledby="update-title"
    >
      <button
        className="update-notification__close"
        type="button"
        aria-label="Close update notification"
        onClick={() => setVisible(false)}
      >
        ×
      </button>

      <div className="update-notification__header">
        <h2 id="update-title">
          New version
          <br />
          available
        </h2>
      </div>

      <div className="update-notification__graphic">
        <img
          src={LuminaSVG}
          alt="Lumina"
          className="update-notification__logo"
        />
      </div>

      <div className="update-notification__version">
        v3.1.6
      </div>

      <button
        className="update-notification__download"
        type="button"
        aria-label="View update"
      >
        <svg viewBox="0 0 24 24" aria-hidden="true">
          <path
            d="M12 3v12m0 0 5-5m-5 5-5-5M5 17v3h14v-3"
            fill="none"
            stroke="currentColor"
            strokeWidth="2.4"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>
    </div>
  );
};

export default UpdateNotification;