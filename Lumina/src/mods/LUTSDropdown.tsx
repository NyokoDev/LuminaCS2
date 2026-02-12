import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, FOCUS_AUTO } from "cs2/ui";

// Bind the LUTArray to a variable and initialize it with an empty array
export const LUTSArray = bindValue<string[]>(mod.id, "LUTArray", []);
const SelectedLUT$ = bindValue<string>(mod.id, "LUTName", "");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

const handleSelect = (selectedLUT: string) => {
  trigger(mod.id, "UpdateLUTName", selectedLUT);
};


export const LUTSDropdown = () => {
  const LUTS = useValue(LUTSArray);
  const selectedLUT = useValue(SelectedLUT$);
  
  // Find the label for the selected LUT
  const selectedLUTLabel = selectedLUT ? selectedLUT : "Select LUT";

  const dropDownItems = LUTS.map((mode) => (
    <DropdownItem<string>
      theme={DropdownStyle}
      focusKey={FOCUS_AUTO}
      value={mode}
      closeOnSelect={true}
      onToggleSelected={() => handleSelect(mode)}
      selected={selectedLUT === mode}
      sounds={{ select: "select-item" }}
    >
      {mode}
    </DropdownItem>
  ));

  return (
    <div style={{ padding: "5rem" }}>
      <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
        <DropdownToggle>{selectedLUTLabel}</DropdownToggle>
      </Dropdown>
    </div>
  );
};
