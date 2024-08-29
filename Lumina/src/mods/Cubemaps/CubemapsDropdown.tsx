import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";

// Bind the CubemapArray to a variable and initialize it with an empty array
export const CubemapArray = bindValue<string[]>(mod.id, "CubemapArrayExtended", []);
const SelectedCubemap$ = bindValue<string>(mod.id, "CubemapName", "");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

const handleSelect = (selectedCubemap: string) => {
  const modeName = selectedCubemap; // Name the selected mode
  console.log(`[LUMINA] Selected Cubemap value: ${modeName}`);

  // Debug before triggering
  console.log(`[LUMINA] About to trigger UpdateCubemapName with modeName: ${modeName}`);

  try {
    // Call the trigger function
    trigger(mod.id, "UpdateCubemapName", modeName);
    console.log(`[LUMINA] Successfully triggered UpdateCubemapName with modeName: ${modeName}`);
  } catch (error) {
    // Log any errors that occur during triggering
    console.error(`[LUMINA] Error triggering UpdateCubemapName:`, error);
  }
};


export const CubemapsDropdown = () => {
  const Cubemaps = useValue(CubemapArray); // Get the array of Cubemaps
  const selectedCubemap = useValue(SelectedCubemap$); // Get the currently selected Cubemap
  console.log("Cubemaps:", Cubemaps); // Debug log
  console.log("Selected Cubemap:", selectedCubemap); // Debug log
  
  // Find the label for the selected Cubemap
  const selectedCubemapLabel = selectedCubemap ? selectedCubemap : "Select Cubemap";

  const dropDownItems = Cubemaps.map((mode) => (
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
        <DropdownToggle>{selectedCubemapLabel}</DropdownToggle>
      </Dropdown>
    </div>
  );
};
