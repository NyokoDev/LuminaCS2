import { ModRegistrar } from "cs2/modding";
import { Editor_Button} from "mods/editor_button";
import { Editor_Panel } from "mods/editor_panel";
import { LuminaButton } from "mods/hello-world";
import { YourPanelComponent } from "mods/panel";
import { LuminaButton2 } from "mods/set-to-lumina-button";


const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.extend("game-ui/game/components/photo-mode/photo-widget-renderer.tsx","PhotoWidgetListRenderer", LuminaButton2);
    moduleRegistry.extend("game-ui/game/components/toolbar/top/toggles.tsx", "PhotoModeToggle", LuminaButton);
    moduleRegistry.extend('game-ui/editor/components/toolbar/toolbar.tsx','Toolbar', Editor_Button);
    


}
export default register;