import React, { useEffect, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "../../../mod.json";
import { SkyCheck, SkySlider } from "../SkyAndFogPanel/SkyAndFogPanel";
import "./TonemappingPanelNew.scss";

const mode$ = bindValue<string>(mod.id, "TonemappingMode", "None");
const lutArray$ = bindValue<string[]>(mod.id, "LUTArray", []);
const contribution$ = bindValue<number>(mod.id, "GetLutContributionValue", 1);
const toeActive$ = bindValue<boolean>(mod.id, "IsToeStrengthActive", false);
const lengthActive$ = bindValue<boolean>(mod.id, "IsToeLengthActive", false);
const shoulderActive$ = bindValue<boolean>(mod.id, "IsShoulderStrengthActive", false);
export const LUTName$ = bindValue<string>(mod.id, "LUTName", "");
export const ToeStrengthValue$ = bindValue<number>(mod.id, "ToeStrengthValue", 0);
export const ToeLengthValue$ = bindValue<number>(mod.id, "ToeLengthValue", 0);
export const ShoulderStrengthValue$ = bindValue<number>(mod.id, "ShoulderStrengthValue", 0);

const modes = [
    { name: "None", id: 0, description: "Disable the tonemapping operator." },
    { name: "Neutral", id: 3, description: "Balanced tonal response and restrained color shifts." },
    { name: "ACES", id: 4, description: "Filmic contrast and highlight roll-off." },
    { name: "Custom", id: 2, description: "Adjust the shadow and highlight response manually." },
    { name: "External", id: 1, description: "Apply a custom 3D color lookup table." },
];

const featuredPacks = [
    { name: "Filmstock LUT Pack", action: "DownloadFilmstockLUTPack" },
    { name: "Candy Commercial LUT Pack", action: "DownloadCandyCommercialLUTPack" },
    { name: "AgX LUT Pack", action: "DownloadAgXLUTPack" },
];

export const TonemappingPanel: React.FC = () => {
    const { translate } = useLocalization();
    const t = (key: string, fallback: string) => translate(key) || fallback;
    const mode = useValue(mode$) || "None";
    const name = useValue(LUTName$) || "";
    const raw = useValue(lutArray$);
    const luts = Array.isArray(raw) ? raw.filter(v => typeof v === "string" && v !== "None").sort() : [];
    const contribution = useValue(contribution$);
    const toe = useValue(ToeStrengthValue$);
    const length = useValue(ToeLengthValue$);
    const shoulder = useValue(ShoulderStrengthValue$);
    const toeActive = !!useValue(toeActive$);
    const lengthActive = !!useValue(lengthActive$);
    const shoulderActive = !!useValue(shoulderActive$);
    const [picker, setPicker] = useState(false);
    const [selected, setSelected] = useState("");
    const send = (key: string) => (value: number) => trigger(mod.id, key, value);
    useEffect(() => { trigger(mod.id, "UpdateUIElements"); }, []);
    const load = () => {
        if (!selected) return;
        trigger(mod.id, "UpdateLUTName", selected);
        trigger(mod.id, "UpdateLUT");
    };
    return <div className="SkyAndFogPanel sky-panel-modern sky-import-panel tonemapping-modern">
        <div className="sky-panel-title">TONEMAPPING</div>
        <div className="sky-label">Rendering mode</div>
        <div className="tone-mode-grid">
            {modes.map(m => <button type="button" key={m.name} className={"sky-button" + (mode === m.name ? " selected" : "")} aria-pressed={mode === m.name} onClick={() => trigger(mod.id, "SetTonemappingMode", m.id)}>{m.name}</button>)}
        </div>
        <div className="sky-status">{modes.find(m => m.name === mode)?.description || "Select a rendering mode."}</div>
        {mode === "External" && <>
            <div className="sky-section-line"/>
            <button type="button" className="sky-button sky-import-primary" onClick={() => trigger(mod.id, "UploadLUTFileDialog")}>
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round"><path d="M3 7h6l2 3h10v10H3zM12 13v4M10 15h4"/></svg>
                <span>Load LUT</span>
            </button>
            <div className="sky-import-hint">.cube · 3D color lookup table</div>
            <div className="sky-file-actions sky-import-tools">
                <button type="button" className="sky-button" onClick={() => setPicker(!picker)}>Library ({luts.length})</button>
                <button type="button" className="sky-button" onClick={() => trigger(mod.id, "OpenLUTFolder")}>Folder</button>
                <button type="button" className="sky-button" onClick={() => trigger(mod.id, "UpdateUIElements")}>Refresh</button>
            </div>
            {picker && <div className="sky-library">
                <div className="sky-library-heading">Installed LUTs</div>
                <div className="sky-cubemap-list">{luts.map(lut => <button type="button" key={lut} className={"sky-button" + (selected === lut ? " selected" : "")} onClick={() => setSelected(lut)}>{lut}</button>)}
                {!luts.length && <div className="sky-status">No installed LUTs. Load a .cube file to get started.</div>}</div>
                <button type="button" className="sky-button" disabled={!selected} onClick={load}>Apply selected LUT</button>
            </div>}
            <div className="sky-cubemap sky-current"><div className="sky-cubemap-select">
                <span className="sky-label">Selected LUT</span><strong>{name && name !== "None" ? name : "No LUT selected"}</strong>
            </div></div>
            <SkySlider label={t("LUMINA.lutcontribution", "LUT contribution")} value={contribution} min={0} max={1} step={0.001} disabled={false} onChange={send("HandleLUTContribution")}/>
            <div className="sky-section-line"/>
            <div className="sky-label">Featured LUT Packs</div>
            <div className="sky-status">Discover more color grading looks on Paradox Mods.</div>
            <div className="tone-featured-packs">
                {featuredPacks.map(pack => <div className="tone-featured-pack" key={pack.action}>
                    <span className="tone-featured-name">{pack.name}</span>
                    <button type="button" className="sky-button tone-download" aria-label={"Download " + pack.name} onClick={() => trigger(mod.id, pack.action)}>Download</button>
                </div>)}
            </div>
            <div className="sky-import-hint">Download opens the pack's Paradox Mods page in your browser.</div>
        </>}
        {mode === "Custom" && <>
            <div className="sky-section-line"/>
            <div className="sky-label">Custom tone curve</div>
            <div className="sky-group">
                <SkyCheck label={t("LUMINA.ToeStrength", "Toe strength")} checked={toeActive} onClick={() => trigger(mod.id, "SetToeStrengthActive")}/>
                <SkySlider label="Shadow response" value={toe} min={0} max={1} step={0.001} disabled={!toeActive} onChange={send("HandleToeStrengthActive")}/>
                <SkyCheck label={t("LUMINA.ToeLength", "Toe length")} checked={lengthActive} onClick={() => trigger(mod.id, "SetToeLengthActive")}/>
                <SkySlider label="Shadow transition" value={length} min={0} max={1} step={0.001} disabled={!lengthActive} onChange={send("HandleToeLengthActive")}/>
                <SkyCheck label={t("LUMINA.ShoulderStrength", "Shoulder strength")} checked={shoulderActive} onClick={() => trigger(mod.id, "SetShoulderStrengthActive")}/>
                <SkySlider label="Highlight response" value={shoulder} min={0} max={1} step={0.001} disabled={!shoulderActive} onChange={send("handleShoulderStrength")}/>
            </div>
        </>}
    </div>;
};
