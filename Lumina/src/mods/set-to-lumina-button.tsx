import { useState } from "react";
import { ButtonTheme, Tooltip, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs } from "cs2/ui";
import { bindValue, trigger, useValue,  } from "cs2/api";
import { game, tool, Theme, } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import mod from "../../mod.json";

export const MIT_ShowUI$ = bindValue<boolean>(mod.id, 'MIT_ShowUI', false);
export const MIT_ToolEnabled$ = bindValue<boolean>(mod.id, 'MIT_ToolEnabled', false);

// Getting the vanilla theme css for compatibility
const ToolBarButtonTheme: Theme | any = getModule(
    "game-ui/game/components/toolbar/components/feature-button/toolbar-feature-button.module.scss",
    "classes"
);





import iconOff from "../img/SendToLumina.svg";
import iconActive from "../img/SendToLumina.svg";
import "../luminasendto.scss"; 


function toggle_ToolEnabled() {
    console.log("Sent to Lumina button clicked.");
    trigger(mod.id, 'LUM_SendToLumina');
}

export const LuminaButton2: ModuleRegistryExtend = (Component) => {
    return (props) => {
        const { children, ...otherProps } = props || {};
        const MIT_ToolEnabled = useValue(MIT_ToolEnabled$);

        let a = iconOff;
        a = iconActive;

        const moveItIconSrc = MIT_ToolEnabled ? "coui://ui-mods/images/SendToLumina.svg" : "coui://ui-mods/images/SendToLumina.svg";

        return (
            <>
           <Tooltip
    tooltip="Send To Lumina" // Specify the content of the tooltip
    disabled={false} // Specify whether the tooltip is disabled (default: false)
    alignment="center" // Specify the alignment of the tooltip (e.g., "start", "center", "end")
    className="custom-tooltip" // Specify additional class names for styling purposes
>
    {/* Content inside the Tooltip component */}
    <Button
        src={moveItIconSrc}
        className="LuminaIcon2" // Use the class name directly
        variant="icon"
        focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}
        selected={MIT_ToolEnabled}
        onSelect={toggle_ToolEnabled}
    >
    </Button>
</Tooltip>

        <div></div>

                <Component {...otherProps}></Component>
            </>

        );
    }
}