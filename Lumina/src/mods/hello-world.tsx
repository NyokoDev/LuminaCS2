import { useState } from "react";
import { ButtonTheme, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs } from "cs2/ui";
import { bindValue, trigger, useValue } from "cs2/api";
import { game, tool, Theme } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import { isInstalled$ as originalIsInstalled$ } from './panel';

import { YourPanelComponent } from "./panel";

let showModeRow$: boolean; // Assuming this is initialized somewhere


// Getting the vanilla theme css for compatibility
const ToolBarButtonTheme: Theme | any = getModule(
    "game-ui/game/components/toolbar/components/feature-button/toolbar-feature-button.module.scss",
    "classes"
);
const ToolBarTheme: Theme | any = getModule(
    "game-ui/game/components/toolbar/toolbar.module.scss",
    "classes"
);

import iconOff from "../img/Lumina.svg";
import iconActive from "../img/Lumina.svg";
import styles from "../lumina.module.scss";



let isInstalled$ = originalIsInstalled$; 


export const LuminaButton: ModuleRegistryExtend = (Component) => {
    return (props) => {
        const { children, ...otherProps } = props || {};
        const MIT_ToolEnabled = isInstalled$
        const moveItIconSrc = MIT_ToolEnabled ? "coui://ui-mods/images/Lumina.svg" : "coui://ui-mods/images/Lumina.svg";
        const [isInstalled, setIsInstalled] = useState(false); // assuming you meant to use useState to manage isInstalled state
        const [refreshKey, setRefreshKey] = useState(0); // Key for forcing re-render

        let a = iconOff;
        a = iconActive;

        return (
            <>    
            
                <Button
                    src={moveItIconSrc}
                    className={ToolBarButtonTheme.button + ' ' + styles.LuminaIcon}
                    variant="icon"
                    focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}
                    onClick={() => {
                        setIsInstalled(!isInstalled); // update isInstalled state
                        setRefreshKey((prev) => prev + 1); // Change key to force re-render
                        trigger(mod.id, 'SaveAutomatically');
                        trigger(mod.id, 'UpdateUIElements');
                    }}
                      
                    onSelect={() => {
          
                        console.log("[LUMINA] Toggled panel");
                    }}
                  

                />
                <div className={ToolBarTheme.divider}></div>
                <Component {...otherProps} />
                {isInstalled && <YourPanelComponent />} {/* Forces re-render */}
            </>
        );
    };
};
