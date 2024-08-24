import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";

const TonemappingModes = ["RGBA64","RGBAHalf"];

// This functions trigger an event on C# side and C# designates the method to implement.
const TonemappingMode$ = bindValue<number>(mod.id, "TextureFormat", 0);


const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

const handleToggleSelected = (index: number) => {
    console.log(`[LUMINA] Texture format index: ${index}`);
    console.log(`[LUMINA] Selected texture format value: ${TonemappingModes[index]}`);
    trigger(mod.id, "SetTextureFormat", index); 
  };

export const TextureFormatDropdown = () => {
  const ColorMode = useValue(TonemappingMode$);
  const dropDownItems = TonemappingModes.map((mode, index) => (
    <DropdownItem<Number>
      theme={DropdownStyle}
      focusKey={FOCUS_AUTO}
      value={index}
      closeOnSelect={true}
      onToggleSelected={() => handleToggleSelected(index)}
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