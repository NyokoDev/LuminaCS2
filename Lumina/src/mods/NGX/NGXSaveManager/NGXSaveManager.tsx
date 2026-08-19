import React, { useMemo, useState } from "react";
import { bindValue, trigger } from "cs2/api";

import "./NGXSaveManager.scss";
import { X } from "lucide-react";

import mod from "../../../../mod.json";

const MOD_ID = mod.id;

// C#:
// GetterValueBinding<string[]>("Lumina", "GetNGXSaves", ...)
const ngxSaves$ = bindValue<string[]>(MOD_ID, "GetNGXSaves", []);

export const NGXSaveManager: React.FC<{ onClose: () => void }> = ({
    onClose,
}) => {
    const saves = ngxSaves$.value ?? [];

    const [saveName, setSaveName] = useState("");
    const [selectedSave, setSelectedSave] = useState<string | null>(null);

    const normalizedSaveName = useMemo(() => {
        return saveName.trim();
    }, [saveName]);

    const handleSave = () => {
        if (!normalizedSaveName) {
            return;
        }

        trigger(MOD_ID, "SaveNGX", normalizedSaveName);

        setSelectedSave(normalizedSaveName);
        setSaveName("");
    };

    const handleLoad = (name: string) => {
        if (!name) {
            return;
        }

        trigger(MOD_ID, "LoadNGX", name);
        setSelectedSave(name);
    };

    const handleDelete = (name: string) => {
        if (!name) {
            return;
        }

        trigger(MOD_ID, "DeleteNGX", name);

        if (selectedSave === name) {
            setSelectedSave(null);
        }
    };

    const handleKeyDown = (
        event: React.KeyboardEvent<HTMLInputElement>
    ) => {
        if (event.key === "Enter") {
            handleSave();
        }
    };

    return (
        <div className="ngx-save-manager">

            {/* HEADER */}
            <div className="ngx-save-manager__header">
                <div>
                    <div className="ngx-save-manager__title">
                        NGX Presets
                    </div>

                    <div className="ngx-save-manager__description">
                        Save and restore NGX volume modifications.
                    </div>
                </div>

           <button
    className="ngx-save-manager__close"
    type="button"
    onClick={onClose}
    aria-label="Close NGX Presets"
>
    <X
        className="ngx-save-manager__close-icon"
        strokeWidth={2}
    />
</button>
            </div>


            {/* SAVE PRESET */}
            <div className="ngx-save-manager__save">

                <input
                    className="ngx-save-manager__input"
                    type="text"
                    placeholder="Enter preset name..."
                    value={saveName}
                    maxLength={80}
                    onChange={(event) =>
                        setSaveName(event.target.value)
                    }
                    onKeyDown={handleKeyDown}
                />

                <button
                    className="ngx-save-manager__save-button"
                    type="button"
                    disabled={!normalizedSaveName}
                    onClick={handleSave}
                >
                    SAVE
                </button>

            </div>


            {/* SAVED PRESETS HEADER */}
            <div className="ngx-save-manager__list-header">
                <span>
                    Saved Presets
                </span>

                <span className="ngx-save-manager__list-count">
                    {saves.length}{" "}
                    {saves.length === 1 ? "preset" : "presets"}
                </span>
            </div>


            {/* EXISTING PRESETS */}
            <div className="ngx-save-manager__list">

                {saves.length === 0 && (
                    <div className="ngx-save-manager__empty">
                        No NGX presets saved.
                    </div>
                )}

                {saves.map((save) => {
                    const selected = selectedSave === save;

                    return (
                        <div
                            key={save}
                            className={[
                                "ngx-save-manager__preset",
                                selected
                                    ? "ngx-save-manager__preset--selected"
                                    : "",
                            ]
                                .filter(Boolean)
                                .join(" ")}
                        >
                            <button
                                className="ngx-save-manager__preset-name"
                                type="button"
                                onClick={() =>
                                    setSelectedSave(save)
                                }
                            >
                                {save}
                            </button>

                            <div className="ngx-save-manager__actions">

                                <button
                                    className="
                                        ngx-save-manager__action
                                        ngx-save-manager__action--load
                                    "
                                    type="button"
                                    onClick={() =>
                                        handleLoad(save)
                                    }
                                >
                                    LOAD
                                </button>

                                <button
                                    className="
                                        ngx-save-manager__action
                                        ngx-save-manager__action--delete
                                    "
                                    type="button"
                                    onClick={() =>
                                        handleDelete(save)
                                    }
                                >
                                    DELETE
                                </button>

                            </div>
                        </div>
                    );
                })}

            </div>


            {/* INFO FOOTER */}
            <div className="ngx-save-manager__info">
                <span className="ngx-save-manager__info-icon">
                    i
                </span>

                <span>
                    Presets save the current state of volumes,
                    components and their properties.
                </span>
            </div>

        </div>
    );
};

export default NGXSaveManager;