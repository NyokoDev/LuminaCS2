import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, FOCUS_AUTO } from "cs2/ui";
import '../dropdown_module.scss'
import '../Cubemaps/Cubemaps.scss'

// Bind the CubemapArray to a variable and initialize it with an empty array
export const CubemapArray = bindValue<string[]>(mod.id, "CubemapArrayExtended", []);
const SelectedCubemap$ = bindValue<string>(mod.id, "CubemapName", "None");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");


const handleSelect = (selectedCubemap: string) => {
  trigger(mod.id, "UpdateCubemapName", selectedCubemap);
};


export const CubemapsDropdown = () => {
  const Cubemaps = useValue(CubemapArray);
  const selectedCubemap = useValue(SelectedCubemap$);

  const selectedCubemapLabel = selectedCubemap;

  const dropDownItems = Cubemaps.map((mode) => (
    <DropdownItem<string>
      theme={DropdownStyle}
      className="CubemapDropdown"
      focusKey={FOCUS_AUTO}
      value={mode}
      closeOnSelect={true}
      onToggleSelected={() => handleSelect(mode)}
      selected={selectedCubemap === mode}
      sounds={{ select: "select-item" }}
    >
      {mode}
    </DropdownItem>
  ));

  return (
    <div style={{ padding: "5rem", overflowY: "scroll" }}>
      <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
        <DropdownToggle >{selectedCubemapLabel}</DropdownToggle>
      </Dropdown>
    </div>
  );
  
};