import React, { useEffect, useRef, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import mod from "../../../mod.json";
import "../Cubemaps/Cubemaps.scss";

export const EmissionMultiplier$ = bindValue<number>(mod.id, "EmissionMultiplier", 1000);
export const SunDiameter$ = bindValue<number>(mod.id, "SunDiameter", 0);
export const SunIntensity$ = bindValue<number>(mod.id, "SunIntensity", 0);
export const SunFlareSize$ = bindValue<number>(mod.id, "SunFlareSize", 0);
const cubes$ = bindValue<string[]>(mod.id, "CubemapArrayExtended", []);
const name$ = bindValue<string>(mod.id, "CubemapName", "None");
const loaded$ = bindValue<boolean>(mod.id, "HasLoadedCubemap", false);
const preview$ = bindValue<string>(mod.id, "CubemapPreview", "");
const hdri$ = bindValue<boolean>(mod.id, "IsHDRISkyEnabled", false);
const customSun$ = bindValue<boolean>(mod.id, "IsCustomSunEnabled", false);
const skyStatus$ = bindValue<string>(mod.id, "SkyStatus", "");
const sunStatus$ = bindValue<string>(mod.id, "SunStatus", "");
const lux$ = bindValue<number>(mod.id, "HDRISkyLux", 7000);
const rotation$ = bindValue<number>(mod.id, "HDRISkyRotation", 0);
const fog$ = bindValue<boolean>(mod.id, "CustomFogEnabled", false);
const fogDistance$ = bindValue<number>(mod.id, "FogDistance", 1000);
const fogBase$ = bindValue<number>(mod.id, "FogBaseHeight", 0);
const fogMax$ = bindValue<number>(mod.id, "FogMaximumHeight", 100);

export const SkyCheck = ({ label, checked, onClick, disabled = false }: { label: string; checked: boolean; onClick: () => void; disabled?: boolean }) =>
    <button type="button" className="sky-check" role="checkbox" aria-checked={checked} disabled={disabled} onClick={onClick}><span className={"sky-check-box" + (checked ? " checked" : "")}>{checked ? "✓" : ""}</span><span>{label}</span></button>;

export const SkySlider = ({ label, value, min, max, step, disabled, onChange, unit = "" }: { label: string; value: number; min: number; max: number; step: number; disabled: boolean; onChange: (n: number) => void; unit?: string }) => {
    const ref = useRef<HTMLDivElement>(null);
    const cleanup = useRef<(() => void) | null>(null);
    const callback = useRef(onChange);
    callback.current = onChange;
    const safe = Number.isFinite(value) ? Math.max(min, Math.min(max, value)) : min;
    const [drag, setDrag] = useState<number | null>(null);
    const [draft, setDraft] = useState(String(safe));
    const editing = useRef(false);
    useEffect(() => { if (!editing.current) setDraft(String(safe)); }, [safe]);
    useEffect(() => () => cleanup.current?.(), []);
    const clamp = (n: number) => Math.max(min, Math.min(max, n));
    const send = (n: number) => { const v = clamp(n); setDraft(String(v)); callback.current(v); };
    const begin = (e: React.MouseEvent<HTMLDivElement>) => {
        if (disabled || e.button !== 0 || !ref.current) return;
        e.preventDefault(); e.stopPropagation(); cleanup.current?.();
        const rect = ref.current.getBoundingClientRect();
        if (rect.width <= 0 || max <= min) return;
        const update = (x: number) => { const n = clamp(Number((min + Math.round((x - rect.left) / rect.width * (max - min) / step) * step).toPrecision(10))); setDrag(n); send(n); };
        const move = (ev: MouseEvent) => { ev.preventDefault(); update(ev.clientX); };
        const stop = () => { cleanup.current?.(); setDrag(null); };
        const up = (ev: MouseEvent) => { update(ev.clientX); stop(); };
        cleanup.current = () => { document.removeEventListener("mousemove", move, true); document.removeEventListener("mouseup", up, true); window.removeEventListener("blur", stop); cleanup.current = null; };
        document.addEventListener("mousemove", move, true); document.addEventListener("mouseup", up, true); window.addEventListener("blur", stop); update(e.clientX);
    };
    const percent = max > min ? (clamp(drag ?? safe) - min) / (max - min) * 100 : 0;
    return <div className={"sky-slider-row" + (disabled ? " disabled" : "")}>
        <div className="sky-slider-label"><span>{label}</span><div><input aria-label={label} type="text" disabled={disabled} value={draft} onFocus={() => { editing.current = true; }} onChange={e => setDraft(e.target.value)} onBlur={() => { editing.current = false; if (draft.trim() && Number.isFinite(Number(draft))) send(Number(draft)); else setDraft(String(safe)); }} onKeyDown={e => { e.stopPropagation(); if (e.key === "Enter") e.currentTarget.blur(); }} />{unit && <span className="sky-unit">{unit}</span>}</div></div>
        <div ref={ref} className="sky-slider" tabIndex={disabled ? -1 : 0} role="slider" aria-label={label} aria-valuemin={min} aria-valuemax={max} aria-valuenow={safe} aria-disabled={disabled} onMouseDown={begin} onKeyDown={e => { if (disabled) return; const delta = e.key === "ArrowLeft" || e.key === "ArrowDown" ? -step : e.key === "ArrowRight" || e.key === "ArrowUp" ? step : 0; if (delta) { e.preventDefault(); e.stopPropagation(); send(Number((safe + delta).toPrecision(10))); } }}>
            <div className="sky-slider-track"/><div className="sky-slider-fill" style={{width: percent + "%"}}/><div className="sky-slider-thumb" style={{left: percent + "%"}}/>
        </div>
    </div>;
};

const Preview = ({ pixels }: { pixels: string }) => {
    const ref = useRef<HTMLCanvasElement>(null);
    useEffect(() => {
        if (!ref.current || typeof pixels !== "string" || pixels.length !== 48 * 28 * 6) return;
        const ctx = ref.current.getContext("2d");
        if (!ctx) return;
        for (let y = 0; y < 28; y++) for (let x = 0; x < 48; x++) {
            const i = (y * 48 + x) * 6;
            ctx.fillStyle = "#" + pixels.slice(i, i + 6);
            ctx.fillRect(x * 4, y * 4, 4, 4);
        }
    }, [pixels]);
    return <div className="sky-preview">{pixels && pixels.length === 48 * 28 * 6 ? <canvas ref={ref} width={192} height={112} /> : <span>No preview</span>}</div>;
};

export const SkyAndFogPanel: React.FC = () => {
    const { translate } = useLocalization();
    const t = (key: string, fallback: string) => translate(key) || fallback;
    const raw = useValue(cubes$);
    const cubes = Array.isArray(raw) ? raw.filter((v): v is string => typeof v === "string" && v !== "None").sort() : [];
    const name = useValue(name$) || "None";
    const preview = useValue(preview$) || "";
    const hdri = !!useValue(hdri$);
    const customSun = !!useValue(customSun$);
    const fog = !!useValue(fog$);
    const skyStatus = useValue(skyStatus$);
    const sunStatus = useValue(sunStatus$);
    const diameter = useValue(SunDiameter$), intensity = useValue(SunIntensity$), flare = useValue(SunFlareSize$);
    const lux = useValue(lux$), rotation = useValue(rotation$);
    const fogDistance = useValue(fogDistance$), fogBase = useValue(fogBase$), fogMax = useValue(fogMax$);
    const loaded = !!useValue(loaded$);
    const [picker, setPicker] = useState(false);
    const [pathOpen, setPathOpen] = useState(false);
    const [path, setPath] = useState("");
    useEffect(() => { trigger(mod.id, "ReloadCubemaps"); }, []);
    const importPath = () => { if (path.trim()) trigger(mod.id, "ImportCubemapPath", path.trim()); };
    const send = (key: string) => (value: number) => trigger(mod.id, key, value);
    const choose = (value: string) => { trigger(mod.id, "UpdateCubemapName", value); setPicker(false); };
    return <div className="SkyAndFogPanel sky-panel-modern sky-import-panel">
        <div className="sky-panel-title">SKY &amp; FOG</div>
        <button type="button" className="sky-button sky-import-primary" onClick={() => trigger(mod.id, "ImportCubemap")}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round"><path d="M3 7h6l2 3h10v10H3zM12 13v4M10 15h4"/></svg>
            <span>Load Cubemap</span>
        </button>
        <div className="sky-import-hint">Cubemaps must be PNG images · 4:3 cross or 2:1 panorama</div>
        <button type="button" className="sky-button" onClick={() => trigger(mod.id, "ObtainCubemaps")}>Obtain cubemaps · Poly Haven ↗</button>
        <div className="sky-import-hint">Convert downloaded HDR or EXR panoramas to PNG before importing.</div>
        <div className="sky-file-actions sky-import-tools">
            <button type="button" className="sky-button" onClick={() => setPicker(!picker)}>Library ({cubes.length})</button>
            <button type="button" className="sky-button" onClick={() => trigger(mod.id, "OpenCubemapFolder")}>Folder</button>
            <button type="button" className="sky-button" onClick={() => setPathOpen(!pathOpen)}>File path</button>
        </div>
        {pathOpen && <div className="sky-path-row"><input type="text" value={path} placeholder="C:\\...\\sky.png" onChange={e => setPath(e.target.value)} onKeyDown={e => { e.stopPropagation(); if (e.key === "Enter") importPath(); }}/><button type="button" className="sky-button" onClick={importPath}>Load</button></div>}
        {picker && <div className="sky-library"><div className="sky-library-heading"><span>Installed cubemaps</span><button className="sky-button" type="button" onClick={() => trigger(mod.id, "ReloadCubemaps")}>Refresh</button></div><div className="sky-cubemap-list">{cubes.map(c => <button type="button" key={c} className={"sky-button" + (c === name && loaded ? " selected" : "")} onClick={() => choose(c)}>{c}</button>)}{cubes.length === 0 && <span className="sky-status">No installed cubemaps. Load a PNG to get started.</span>}</div></div>}
        <div className="sky-cubemap sky-current">
            <div className="sky-cubemap-select"><span className="sky-label">Current sky</span><strong>{loaded ? name : "Game sky"}</strong><span className="sky-status">{hdri && loaded ? "HDRI enabled" : loaded ? "Loaded · inactive" : "Vanilla environment"}</span>
                {loaded && <button type="button" className="sky-button" onClick={() => choose("None")}>Restore game sky</button>}
            </div>
            <Preview pixels={loaded ? preview : ""}/>
        </div>
        <SkyCheck label={t("LUMINA.environmenthdrisky", "Enable environment HDRI")} checked={hdri} onClick={() => trigger(mod.id, "SetHDRISkyEnabled")} disabled={!loaded && !hdri}/>
        {skyStatus && <div className={"sky-status sky-load-status" + (/rejected|failed|unavailable|invalid|not found|before enabling/i.test(skyStatus) ? " sky-load-error" : "")}>
            {/rejected|failed|unavailable|invalid|not found|before enabling/i.test(skyStatus) && <strong>Unable to load cubemap</strong>}
            <div>{skyStatus}</div>
            {/rejected|failed|unavailable|invalid|not found|before enabling/i.test(skyStatus) && <div className="sky-error-actions"><button type="button" className="sky-button" onClick={() => trigger(mod.id, "ImportCubemap")}>Try another image</button><button type="button" className="sky-button" onClick={() => setPathOpen(true)}>Enter file path</button></div>}
        </div>}
        {hdri && <div className="sky-group">
            <SkySlider label="Environment brightness" unit="lx" value={lux} min={0} max={100000} step={100} disabled={!hdri} onChange={send("SetHDRISkyLux")}/>
            <SkySlider label="Sky rotation" unit="°" value={rotation} min={0} max={360} step={1} disabled={!hdri} onChange={send("SetHDRISkyRotation")}/>
        </div>}
        <div className="sky-section-line"/>
        <SkyCheck label={t("LUMINA.usecustomsunproperties", "Use custom sun properties")} checked={customSun} onClick={() => trigger(mod.id, "SetCustomSunEnabled")}/>
        {sunStatus && <div className="sky-status">{sunStatus}</div>}
        <div className="sky-group">
            <SkySlider label={t("LUMINA.sundiameter", "Sun diameter")} unit="°" value={diameter} min={0} max={90} step={0.01} disabled={!customSun} onChange={send("handleAngularDiameter")}/>
            <SkySlider label={t("LUMINA.sunintensity", "Sun intensity")} unit="lx" value={intensity} min={0} max={200000} step={100} disabled={!customSun} onChange={send("handleSunIntensity")}/>
            <SkySlider label={t("LUMINA.sunflaresize", "Sun flare size")} unit="°" value={flare} min={0} max={90} step={0.01} disabled={!customSun} onChange={send("handleSunFlareSize")}/>
            {customSun && <button type="button" className="sky-button sky-reset" onClick={() => trigger(mod.id, "ResetCustomSun")}>Restore scene sun values</button>}
        </div>
        <div className="sky-section-line"/>
        <SkyCheck label="Override global fog" checked={fog} onClick={() => trigger(mod.id, "ToggleCustomFog")}/>
        {fog && <div className="sky-group">
            <SkySlider label="Fog attenuation distance" unit="m" value={fogDistance} min={1} max={10000} step={10} disabled={!fog} onChange={send("SetFogDistance")}/>
            <SkySlider label="Fog base height" unit="m" value={fogBase} min={-500} max={999} step={1} disabled={!fog} onChange={send("SetFogBaseHeight")}/>
            <SkySlider label="Fog maximum height" unit="m" value={fogMax} min={Math.min(999, (Number.isFinite(fogBase) ? fogBase : 0) + 1)} max={1000} step={1} disabled={!fog} onChange={send("SetFogMaximumHeight")}/>
        </div>}
    </div>;
};
