import React from "react";
import "../../styles/lumina-controls.scss";

export interface SettingButtonProps
    extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: "default" | "primary";
}

export const SettingButton: React.FC<SettingButtonProps> = ({
    variant = "default",
    className = "",
    children,
    ...props
}) => (
    <button
        type="button"
        className={
            "lumina-btn" +
            (variant === "primary" ? " lumina-btn--primary" : "") +
            (className ? " " + className : "")
        }
        {...props}
    >
        {children}
    </button>
);

export const SettingButtonRow: React.FC<{ children: React.ReactNode }> = ({
    children,
}) => <div className="lumina-btn-row">{children}</div>;
