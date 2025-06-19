import React from "react";
import mod from "../../../mod.json";
import "./locktimebutton.scss";
import { bindValue, trigger, useValue } from "cs2/api";
import { Tooltip } from "cs2/ui";

export const Locked$ = bindValue<boolean>(mod.id, 'TimeLockStatus');

const LockTimeButton: React.FC = () => {
  const handleClick = () => {
    console.log("Time locked!");
    trigger(mod.id, 'LockTime');
  };

    const TimeLockStatus = useValue(Locked$);
  return (
   <Tooltip tooltip="This will lock Time of day however time will return to game time if disabled due to a limitation.">
  <button
    className={`lock-time-button ${TimeLockStatus ? "enabled" : ""}`}
    onClick={handleClick}
  >
    {/* Inline SVG lock icon */}
    <svg
      className="icon"
      xmlns="http://www.w3.org/2000/svg"
      width="18"
      height="18"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M12 17a1.5 1.5 0 0 0 1.5-1.5v-1a1.5 1.5 0 0 0-3 0v1A1.5 1.5 0 0 0 12 17zm6-5V9a6 6 0 1 0-12 0v3H6a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-6a2 2 0 0 0-2-2h-1zm-8 0V9a4 4 0 1 1 8 0v3"
      />
    </svg>
    <span>Lock Time</span>
  </button>
</Tooltip>

  );
};

export default LockTimeButton;
