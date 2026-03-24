import { useEffect, useState } from "react";
import { Button, ConfirmationDialog, Panel, Portal, FloatingButton, PanelSection, PanelSectionRow, FormattedParagraphs } from "cs2/ui";
import { bindValue, trigger, useValue } from "cs2/api";
import { game, tool } from "cs2/bindings";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";
//import { LocalizedString, useLocalization } from "cs2/l10n";
import mod from "../../mod.json";
import { isInstalled$ as originalIsInstalled$ } from './panel';

import { YourPanelComponent } from "./panel";
import './luminaButton.scss';

let showModeRow$: boolean; // Assuming this is initialized somewhere

// Getting the vanilla theme css for compatibility
const ToolBarButtonTheme: any = getModule(
    "game-ui/game/components/toolbar/components/feature-button/toolbar-feature-button.module.scss",
    "classes"
);
const ToolBarTheme: any = getModule(
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

        const [isInstalled, setIsInstalled] = useState(false);

        let a = iconOff;
        a = iconActive;

        return (
            <>
                <div className={styles.LuminaButtonWrapper}>
                    <Button
  className={
    ToolBarButtonTheme.button +
    " " +
    styles.LuminaIcon +
    (isInstalled ? " " + styles.active : "")
  }
  variant="icon"
  focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}
  onClick={() => {
    setIsInstalled(!isInstalled);
    trigger(mod.id, "SaveAutomatically");
    trigger(mod.id, "UpdateUIElements");
  }}
>
  <img src="coui://ui-mods/images/Lumina.svg" className={styles.IconInner} />
</Button>
                </div>

                <div className={ToolBarTheme.divider + " " + styles.LuminaDivider}></div>

                <Component {...otherProps} />
                {isInstalled && <YourPanelComponent />}
            </>
        );
    };
};