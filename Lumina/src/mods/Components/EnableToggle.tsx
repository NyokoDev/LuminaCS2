import React from "react";
import "../../styles/lumina-controls.scss";

export interface EnableToggleProps {
    active: boolean;
    onToggle: () => void;
    className?: string;
}

export const EnableToggle: React.FC<EnableToggleProps> = ({
    active,
    onToggle,
    className = "",
}) => (
    <div className={`lumina-enable-toggle ${className}`} onClick={onToggle}>
        <div
            className={
                "lumina-enable-toggle__check" +
                (active ? " lumina-enable-toggle__check--on" : "")
            }
        />
        <button
            type="button"
            className="lumina-enable-toggle__hit toggle_cca item-mouse-states_Fmi toggle_th_"
            onClick={(e) => {
                e.stopPropagation();
                onToggle();
            }}
        />
    </div>
);
