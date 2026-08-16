import React, { useState } from "react";
import "./UpdateNotification.scss";

export const UpdateNotification: React.FC = () => {
  const [visible, setVisible] = useState(true);

  if (!visible) {
    return null;
  }

  return (
  <main
    className="UpdateNotification"
    aria-label="Application update notification"
  >
    <section
      className="update-modal"
      id="update-modal"
      role="dialog"
      aria-modal="true"
      aria-labelledby="update-title"
      aria-describedby="update-description"
    >
      <button
        className="update-modal__close"
        type="button"
        aria-label="Close update notification"
        onClick={() => setVisible(false)}
      >
        <svg
          viewBox="0 0 24 24"
          aria-hidden="true"
          focusable="false"
        >
          <path
            d="M18 6 6 18M6 6l12 12"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>

      <div className="update-modal__content">
        <div className="update-icon" aria-hidden="true">
          <svg viewBox="0 0 24 24" focusable="false">
            <circle
              cx="12"
              cy="12"
              r="10"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            />

            <path
              d="m16 12-4-4-4 4M12 16V8"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </div>

        <h1
          className="update-modal__title"
          id="update-title"
        >
          New version
          <br />
          is available
        </h1>

        <p
          className="update-modal__description"
          id="update-description"
        >
          A brand new version of Lumina
          <br />
          is available with improvements,
          <br />
          bug fixes and new features.
        </p>

        <button
          className="update-modal__action"
          type="button"
        >
          View update
        </button>
      </div>
    </section>
  </main>
);
};
export default UpdateNotification;