import { useState } from "react";
import { Button } from "cs2/ui";
import { trigger } from "cs2/api";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";

import mod from "../../mod.json";
import { YourPanelComponent } from "./panel";

import "../editor_lumina.scss";

const ToolBarButtonTheme: any = getModule(
    "game-ui/game/components/toolbar/components/feature-button/toolbar-feature-button.module.scss",
    "classes"
);

const ToolBarTheme: any = getModule(
    "game-ui/game/components/toolbar/toolbar.module.scss",
    "classes"
);

let PanelVisible = false;

export const EditorButton: ModuleRegistryExtend = (Component) =>
{
    return (props) =>
    {
        const { children, ...otherProps } = props || {};

        const [, forceUpdate] = useState(0);

        const TogglePanel = () =>
        {
            PanelVisible = !PanelVisible;

            forceUpdate(v => v + 1);

            console.log(
                `[LUMINA] Editor Panel ${PanelVisible ? "opened" : "closed"}`
            );
        };

        return (
            <>
                <div className="LuminaButtonWrapper">
              <Button
    className={
        ToolBarButtonTheme.button +
        " LuminaToolbarButton" +
        (PanelVisible
            ? " LuminaToolbarButtonActive"
            : "")
    }
    variant="icon"
    focusKey={
        VanillaComponentResolver.instance.FOCUS_DISABLED
    }
    onClick={TogglePanel}
>
    <img
        src="coui://ui-mods/images/Lumina.svg"
        className="LuminaToolbarIcon"
    />
</Button>
                </div>

                <div
                    className={
                        ToolBarTheme.divider +
                        " LuminaDivider"
                    }
                />

                <Component {...otherProps}>
                    {children}
                </Component>

{PanelVisible && (
    <div
        style={{
            position: "absolute",
            top: "1000rem",
            left: "200rem",
   
        }}
    >
        <YourPanelComponent />
    </div>
)}
            </>
        );
    };
};