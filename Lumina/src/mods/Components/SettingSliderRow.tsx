import React from "react";
import { EnableToggle } from "./EnableToggle";
import { SettingRow } from "./SettingRow";

export interface SettingSliderRowProps {
    label: string;
    value: number | string;
    active?: boolean;
    onToggle?: () => void;
    slider: React.ReactNode;
    extraControl?: React.ReactNode;
    className?: string;
}

export const SettingSliderRow: React.FC<SettingSliderRowProps> = ({
    label,
    value,
    active,
    onToggle,
    slider,
    extraControl,
    className = "",
}) => (
    <SettingRow
        className={className}
        label={label}
        value={typeof value === "number" ? value.toString() : value}
        toggle={
            onToggle !== undefined && active !== undefined ? (
                <EnableToggle active={active} onToggle={onToggle} />
            ) : undefined
        }
        control={
            <>
                {extraControl}
                {slider}
            </>
        }
    />
);
