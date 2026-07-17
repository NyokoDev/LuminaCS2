import { useState, useCallback, useMemo } from "react";

import { Button } from "cs2/ui";
import { bindValue, useValue } from "cs2/api";
import { getModule, ModuleRegistryExtend } from "cs2/modding";

import { VanillaComponentResolver } from "classes/VanillaComponentResolver";

import mod from "../../mod.json";

import { YourPanelComponent } from "./panel";
import NGXWorkspace from "./NGX/NGXWorkspace";

import "./luminaButton.scss";
import styles from "../lumina.module.scss";


export const $ngxMode = bindValue<boolean>(
    mod.id,
    "NGXMode"
);


const ToolBarButtonTheme: any = getModule(
    "game-ui/game/components/toolbar/components/feature-button/toolbar-feature-button.module.scss",
    "classes"
);

const ToolBarTheme: any = getModule(
    "game-ui/game/components/toolbar/toolbar.module.scss",
    "classes"
);


export const LuminaButton: ModuleRegistryExtend = (Component) => {
    return (props) => {

        const {
            children,
            ...otherProps
        } = props || {};

        const ngxMode = useValue($ngxMode);

        const [panelOpen, setPanelOpen] = useState(false);
        const [useNGXPanel, setUseNGXPanel] = useState(false);


        const handleClick = useCallback(() => {

            if (panelOpen) {
                setPanelOpen(false);
                return;
            }

            setUseNGXPanel(Boolean(ngxMode));
            setPanelOpen(true);

        }, [
            panelOpen,
            ngxMode
        ]);


        const buttonClassName = useMemo(() => {

            return (
                ToolBarButtonTheme.button +
                " " +
                styles.LuminaIcon +
                (panelOpen ? " " + styles.active : "")
            );

        }, [
            panelOpen
        ]);


        return (
            <>
                <div className={styles.LuminaButtonWrapper}>

                    <Button
                        className={buttonClassName}
                        variant="icon"
                        focusKey={
                            VanillaComponentResolver.instance.FOCUS_DISABLED
                        }
                        onClick={handleClick}
                    >
                        <div
    className={
        styles.IconInner +
        " " +
        (ngxMode ? styles.NGXIcon : styles.ClassicIcon)
    }
/>
                    </Button>

                </div>


                <div
                    className={
                        ToolBarTheme.divider +
                        " " +
                        styles.LuminaDivider
                    }
                />


                {/* Always keep original toolbar props */}
                <Component {...otherProps}>
                    {children}
                </Component>


                {/* Replace the panel when opened */}
                {
                    panelOpen &&
                    useNGXPanel &&
                    <NGXWorkspace />
                }


                {
                    panelOpen &&
                    !useNGXPanel &&
                    <YourPanelComponent />
                }

            </>
        );
    };
};