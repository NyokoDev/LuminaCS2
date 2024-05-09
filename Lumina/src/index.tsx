import { ModRegistrar } from "cs2/modding";
import { LuminaButton } from "mods/hello-world";
import { YourPanelComponent } from "mods/panel";
import { LuminaButton2 } from "mods/set-to-lumina-button";


const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.extend("game-ui/game/components/photo-mode/photo-widget-renderer.tsx","PhotoWidgetListRenderer", LuminaButton2);
    moduleRegistry.extend("game-ui/game/components/toolbar/top/toggles.tsx", "PhotoModeToggle", LuminaButton);

}
export default register;