import React from "react";
import { Tooltip } from "cs2/ui";
import { LuminaTabId, NavItemConfig } from "./types";
import "../../styles/lumina-shell.scss";

export const NAV_ITEMS: NavItemConfig[] = [
    { id: "color", tooltipKey: "LUMINA.colortooltip", iconClass: "lumina-nav-item--color" },
    { id: "settings", tooltipKey: "LUMINA.settingstooltip", iconClass: "lumina-nav-item--settings" },
    { id: "planetary", tooltipKey: "LUMINA.planetarytooltip", iconClass: "lumina-nav-item--planetary" },
    { id: "tonemapping", tooltipKey: "LUMINA.tonemappingtooltip", iconClass: "lumina-nav-item--tonemapping" },
    { id: "sky-fog", tooltipKey: "LUMINA.skyandfogtooltip", iconClass: "lumina-nav-item--sky-fog" },
    { id: "road", tooltipKey: "LUMINA.roadconfig", iconClass: "lumina-nav-item--road" },
    { id: "ssao", tooltipKey: "Screen Space Ambient Occlusion", iconClass: "lumina-nav-item--ssao" },
    { id: "ssr", tooltipKey: "Screen Space Refraction", iconClass: "lumina-nav-item--ssr" },
];

export interface NavItemProps {
    iconClass: string;
    active: boolean;
    tooltip: string;
    onClick: () => void;
}

export const NavItem: React.FC<NavItemProps> = ({
    iconClass,
    active,
    tooltip,
    onClick,
}) => (
    <Tooltip tooltip={tooltip} alignment="center" className="custom-tooltip">
        <button
            type="button"
            className={
                "lumina-nav-item " +
                iconClass +
                (active ? " lumina-nav-item--active" : "")
            }
            onClick={onClick}
        />
    </Tooltip>
);

export interface LuminaNavProps {
    activeTab: LuminaTabId;
    onTabChange: (tab: LuminaTabId) => void;
    translate: (key: string) => string;
}

export const LuminaNav: React.FC<LuminaNavProps> = ({
    activeTab,
    onTabChange,
    translate,
}) => (
    <nav className="lumina-nav">
        {NAV_ITEMS.map((item) => (
            <NavItem
                key={item.id}
                iconClass={item.iconClass}
                active={activeTab === item.id}
                tooltip={
                    item.tooltipKey.startsWith("LUMINA.")
                        ? translate(item.tooltipKey)
                        : item.tooltipKey
                }
                onClick={() => onTabChange(item.id)}
            />
        ))}
    </nav>
);
