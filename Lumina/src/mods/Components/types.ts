export type LuminaTabId =
    | "color"
    | "settings"
    | "planetary"
    | "tonemapping"
    | "sky-fog"
    | "road"
    | "ssao"
    | "ssr";

export interface NavItemConfig {
    id: LuminaTabId;
    tooltipKey: string;
    iconClass: string;
}
