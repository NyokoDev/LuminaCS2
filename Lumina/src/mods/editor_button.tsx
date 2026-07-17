import { useState } from "react";
import { Button } from "cs2/ui";
import { bindValue, trigger, useValue } from "cs2/api";
import { getModule, ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "classes/VanillaComponentResolver";

import mod from "../../mod.json";
import { YourPanelComponent } from "./panel";
import NGXWorkspace from "./NGX/NGXWorkspace";

import "../editor_lumina.scss";


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



let PanelVisible = false;

let OpenNGX = false;



export const EditorButton: ModuleRegistryExtend = (Component) =>
{
    return (props) =>
    {
        const { children, ...otherProps } = props || {};

        const [, forceUpdate] = useState(0);


        const ngxMode = useValue($ngxMode);



        const TogglePanel = () =>
        {
            PanelVisible = !PanelVisible;


            if (PanelVisible)
            {

                trigger("Lumina","SaveAutomatically");
                // Decide what panel to open when clicked
                OpenNGX = Boolean(ngxMode);
            }
            else
            { 
                trigger("Lumina","SaveAutomatically");
                OpenNGX = false;
            }


            forceUpdate(v => v + 1);


            console.log(
                "[LUMINA] Editor Button Click",
                {
                    PanelVisible,
                    NGXMode: ngxMode,
                    Opening:
                        OpenNGX
                            ? "NGXWorkspace"
                            : "YourPanelComponent"
                }
            );
        };



        return (
            <>

                <div className="LuminaButtonWrapper">

                    <Button
                        className={
                            ToolBarButtonTheme.button +
                            " LuminaToolbarButton" +
                            (
                                PanelVisible
                                    ? " LuminaToolbarButtonActive"
                                    : ""
                            )
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




                {
                    PanelVisible && (

                        <div
                            style={{
                                position: "absolute",
                                top: "1000rem",
                                left: "200rem",
                            }}
                        >

                            {
                                OpenNGX
                                    ? <NGXWorkspace />
                                    : <YourPanelComponent />
                            }

                        </div>

                    )
                }


            </>
        );
    };
};