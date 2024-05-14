import { useState } from "react";
import { ButtonTheme, Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs } from "cs2/ui";
import { bindValue, trigger, useValue } from "cs2/api";
import { game, tool, Theme } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import { EditorEnabled$ as isEditor$ } from './editor_panel';
import { Editor_Panel } from "./editor_panel";
import "editor_lumina.scss";

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


export let EditorPanel$ = isEditor$;

export const Editor_Button: ModuleRegistryExtend = (Component) => {
    return (props) => {
        const { children, ...otherProps } = props || {};
        const [isEditor$, setIsEditor] = useState(false); // assuming you meant to use useState to manage isInstalled state
        const moveItIconSrc = isEditor$ ? "https://svgshare.com/i/15rV.svg" : "https://svgshare.com/i/15rV.svg";

        return (
            <>    
            
                <Button
                    src={moveItIconSrc}
                    className={"button_iZC button_iZC button_i0V EditorIcon"}
                    variant="icon"
                    focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}
                    onClick={() => {
                        setIsEditor(!isEditor$); // update isInstalled state
                    }}
                      
                    onSelect={() => {
          
                        console.log("[LUMINA] Toggled editor panel");
                    }}
                  

                />
                <div className={ToolBarTheme.divider}></div>

                <Component {...otherProps}></Component>
                {isEditor$ && <Editor_Panel />}

            </>
        );
    };

}

