import React from "react";
import "../../styles/lumina-controls.scss";

export interface SettingRowProps {
    label: string;
    value?: React.ReactNode;
    toggle?: React.ReactNode;
    control?: React.ReactNode;
    className?: string;
}

export const SettingRow: React.FC<SettingRowProps> = ({
    label,
    value,
    toggle,
    control,
    className = "",
}) => (
    <div className={`lumina-setting-row ${className}`}>
        <span className="lumina-setting-row__label">{label}</span>
        {value !== undefined && (
            <span className="lumina-setting-row__value">{value}</span>
        )}
        {toggle && (
            <div className="lumina-setting-row__toggle">{toggle}</div>
        )}
        {control && (
            <div className="lumina-setting-row__control">{control}</div>
        )}
    </div>
);
