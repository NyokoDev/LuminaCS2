import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";

// Define the available tonemapping modes
const TonemappingModes = ["RGBA64", "RGBAHalf"];

// Bind the texture format value to a number, starting with 0
const TonemappingMode$ = bindValue<number>(mod.id, "TextureFormat", 0);

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

// Handle selection change with string value
const handleToggleSelected = (mode: string) => {
    console.log(`[LUMINA] Selected texture format value: ${mode}`);
    trigger(mod.id, "SetTextureFormat", mode); 
};

export const TextureFormatDropdown = () => {
    // Get the current color mode value
    const ColorMode = useValue(TonemappingMode$);

    // Map dropdown items to tonemapping modes
    const dropDownItems = TonemappingModes.map((mode, index) => (
        <DropdownItem<string>
            theme={DropdownStyle}
            focusKey={FOCUS_AUTO}
            value={mode} // Pass mode as string
            closeOnSelect={true}
            onToggleSelected={() => handleToggleSelected(mode)} // Pass mode as string
            selected={true}
            sounds={{ select: "select-item" }}
        >
            {mode}
        </DropdownItem>
    ));

    return (
        <div style={{ padding: "5rem" }}>
            <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
                <DropdownToggle>{TonemappingModes[ColorMode]}</DropdownToggle>
            </Dropdown>
        </div>
    );
};
