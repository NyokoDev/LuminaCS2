import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";
import { LUTName$ } from "./panel";

// Bind the LUTArray to a variable and initialize it with an empty array
export const LUTSArray = bindValue<string[]>(mod.id, "LUTArray", []);
const SelectedLUT$ = bindValue<string>(mod.id, "LUTName", "");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

const handleSelect = (selectedLUT: string) => {
  const modeName = selectedLUT; // Name the selected mode
  console.log(`[LUMINA] Selected LUT value: ${modeName}`);

  // Debug before triggering
  console.log(`[LUMINA] About to trigger UpdateLUTName with modeName: ${modeName}`);

  try {
    // Call the trigger function
    trigger(mod.id, "UpdateLUTName", modeName);
    console.log(`[LUMINA] Successfully triggered UpdateLUTName with modeName: ${modeName}`);
  } catch (error) {
    // Log any errors that occur during triggering
    console.error(`[LUMINA] Error triggering UpdateLUTName:`, error);
  }
};


export const LUTSDropdown = () => {
  const LUTS = useValue(LUTSArray); // Get the array of LUTs
  const selectedLUT = useValue(SelectedLUT$); // Get the currently selected LUT
  console.log("LUTS:", LUTS); // Debug log
  console.log("Selected LUT:", selectedLUT); // Debug log
  
  // Find the label for the selected LUT
  const selectedLUTLabel = selectedLUT ? selectedLUT : "Select LUT";

  const dropDownItems = LUTS.map((mode) => (
    <DropdownItem<string>
      theme={DropdownStyle}
      focusKey={FOCUS_AUTO}
      value={mode}
      closeOnSelect={true}
      onToggleSelected={() => handleSelect(mode)}   // Highlight the selected item
      selected={true}
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
