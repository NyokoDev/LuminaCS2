import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { Slider } from "mods/slider";
import mod from "./../../../mod.json";
import { Tooltip } from "cs2/ui";
import "./SSRPanel.scss"

export interface SSRPanelBaseProps {
    title?: string;
    children?: React.ReactNode;
    className?: string;
}

/* =========================
   Bindings
========================= */



const ssrEnabled$ = bindValue<boolean>(
    mod.id,
    "IsScreenSpaceRefraction"
);

const ssrScreenFadeDistance$ = bindValue<number>(
    mod.id,
    "ScreenSpaceRefractionScreenFadeDistance"
);

/* =========================
   Component
========================= */

export const SSRPanelBase: React.FC<SSRPanelBaseProps> = ({
    title = "",
    children,
    className = "",
}) => {

    /* VALUES */

const ssrEnabled = useValue(ssrEnabled$);
const screenFadeDistance = useValue(ssrScreenFadeDistance$);

    /* HANDLERS */

    const handleSSREnabled = (v: boolean) =>
        trigger(mod.id, "HandleScreenSpaceRefraction", v);

    const handleSSRScreenFadeDistance = (v: number) =>
        trigger(
            mod.id,
            "HandleScreenSpaceRefractionScreenFadeDistance",
            v
        );

    return (
        <div className={`ssr-panel-base ${className}`}>
            {title && (
                <div className="ssaoconfiglabel">
                    {title}
                </div>
            )}

            {children}

            <Tooltip tooltip={"Screen Space Refraction"}>
                <button
                    className={`SSAOToggleButton ${
                        ssrEnabled ? "enabled" : "disabled"
                    }`}
                    onClick={() =>
                        handleSSREnabled(!ssrEnabled)
                    }
                >
                    {ssrEnabled
                        ? "SSR Enabled"
                        : "SSR Disabled"}
                </button>
            </Tooltip>

            <label className="ScreenFadeLabel title_SVH title_zQN">
                Screen Fade Distance
            </label>

            <Slider
                value={screenFadeDistance}
                start={0}
                end={2}
                step={0.01}
                onChange={handleSSRScreenFadeDistance}
                disabled={!ssrEnabled}
                gamepadStep={0.01}
                noFill={false}
            />
        </div>
    );
};