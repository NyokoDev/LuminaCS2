import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, FOCUS_AUTO } from "cs2/ui";
import "./dropdown_module.scss";

const TonemappingModes = ["None", "External", "Custom", "Neutral", "ACES"];

const TonemappingMode$ = bindValue<number>(mod.id, "TonemappingMode", 0);
const TonemappingModeValue = bindValue<string>(mod.id, "TonemappingMode", "");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

const handleToggleSelected = (index: number) => {
  trigger(mod.id, "SetTonemappingMode", index);
};

export const TonemappingDropdown = () => {
  const colorMode = useValue(TonemappingMode$);
  const tonemappingMode = useValue(TonemappingModeValue);
  const tonemappingModeLabel = tonemappingMode || "Tonemapping";

  const dropDownItems = TonemappingModes.map((mode, index) => (
    <DropdownItem<number>
      theme={DropdownStyle}
      className="dropdownItem"
      focusKey={FOCUS_AUTO}
      value={index}
      closeOnSelect={true}
      onToggleSelected={() => handleToggleSelected(index)}
      selected={colorMode === index}
      sounds={{ select: "select-item" }}
    >
      {mode}
    </DropdownItem>
  ));

  return (
    <div className="luminaDropdownShell">
      <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
        <DropdownToggle>{tonemappingModeLabel}</DropdownToggle>
      </Dropdown>
    </div>
  );
};
