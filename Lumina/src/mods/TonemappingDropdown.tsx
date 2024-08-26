import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";

const TonemappingModes = ["None", "External", "Custom", "Neutral", "ACES"];

// This functions trigger an event on C# side and C# designates the method to implement.
const TonemappingMode$ = bindValue<number>(mod.id, "TonemappingMode", 0);
const TonemappingModeValue = bindValue<string>(mod.id, "TonemappingMode","");


const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");


console.log(UISound);

const handleToggleSelected = (index: number) => {
    console.log(`[LUMINA] Selected mode index: ${index}`);
    console.log(`[LUMINA] Selected mode value: ${TonemappingModes[index]}`);
    trigger(mod.id, "SetTonemappingMode", index); 
  };

export const TonemappingDropdown = () => {
  const ColorMode = useValue(TonemappingMode$);
  const TonemappingMode = useValue(TonemappingModeValue);
  const TonemmapingModeValueLabel = TonemappingMode ? TonemappingMode : "TonemappingMode";
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
        <DropdownToggle>{TonemmapingModeValueLabel}</DropdownToggle>
      </Dropdown>
    </div>
  );
};