import { useState, useEffect } from "react";
import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, FOCUS_AUTO } from "cs2/ui";
import '../dropdown_module.scss';
import '../Cubemaps/Cubemaps.scss';

// Bind CubemapArray to hold cubemap file names
export const CubemapArray = bindValue<string[]>(mod.id, "CubemapArrayExtended", []);
const SelectedCubemap$ = bindValue<string>(mod.id, "CubemapName", "");

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

export const CubemapsDropdown = () => {
  const [files, setFiles] = useState<string[]>([]);
  const Cubemaps = useValue(CubemapArray); // Current file list
  const selectedCubemap = useValue(SelectedCubemap$) || "Select Cubemap";

  // Effect to check for updates
  useEffect(() => {
    if (JSON.stringify(Cubemaps) !== JSON.stringify(files)) {
      console.log("[LUMINA] Detected new cubemap files, updating dropdown...");
      setFiles([...Cubemaps]); // Ensure UI updates with new files
    }
  }, [Cubemaps]);

  const dropDownItems = files.map((mode) => (
    <DropdownItem<string>
      key={mode}
      theme={DropdownStyle}
      className="CubemapDropdown"
      focusKey={FOCUS_AUTO}
      value={mode}
      closeOnSelect={true}
      selected={mode === selectedCubemap}
    >
      {mode}
    </DropdownItem>
  ));

  return (
    <div style={{ padding: "5rem", overflowY: "scroll" }}>
      <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
        <DropdownToggle>{selectedCubemap}</DropdownToggle>
      </Dropdown>
    </div>
  );
};
