import React, { useEffect, useRef, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { Slider, SliderValueTransformer } from "../slider";
import { createPortal } from "react-dom";
import { Box, Layers, Search, Terminal, Check, ChevronRight, SlidersHorizontal } from "lucide-react";
import LuminaLogo from "../../img/Lumina.svg";
import NGXConsole from "./Console/NGXConsole";
import NGXSaveManager from "./NGXSaveManager/NGXSaveManager";
import "./NGXWorkspace.scss";
import "./NGXWorkspace.4k.scss";
import "./NGXConfirmation.scss";

export const volumes = bindValue<string[]>("Lumina", "GetVolumes");
export const selectedVolume = bindValue<string>("Lumina", "SelectedVolume");
export const volumeComponents = bindValue<string[]>("Lumina", "GetVolumeComponents");
export const availableComponents = bindValue<string[]>("Lumina", "GetAvailableComponents");
export const componentProperties = bindValue<string[]>("Lumina", "GetComponentProperties");
export const componentStates = bindValue<string[]>("Lumina", "GetComponentStates");

const WorkspaceIcon = ({ kind }: { kind: "close" | "plus" | "minus" }) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" aria-hidden="true">
        {kind === "close" ? <path d="M6 6l12 12M18 6L6 18" /> : <path d={kind === "plus" ? "M12 5v14M5 12h14" : "M5 12h14"} />}
    </svg>
);

type Property = {
    name: string;
    type: string;
    value: string;
    kind: string;
    min: string;
    max: string;
    options: string[];
};

const friendly = (name: string) => name.replace(/_/g, " ").replace(/([a-z0-9])([A-Z])/g, "$1 $2").replace(/^./, c => c.toUpperCase());
const parseProperty = (raw: string): Property | null => {
    try {
        const p = JSON.parse(raw);
        if (typeof p.name !== "string" || typeof p.value !== "string") return null;
        return p as Property;
    } catch {
        const [name, type, ...rest] = raw.split("|");
        if (!name) return null;
        const kind = /BoolParameter/i.test(type) ? "bool" : /IntParameter/i.test(type) ? "int" : /FloatParameter/i.test(type) ? "float" : "readonly";
        return { name, type, value: rest.join("|"), kind, min: "", max: "", options: [] };
    }
};

const finite = (s: string) => s.trim() !== "" && Number.isFinite(Number(s));
const matches = (name: string, query: string) => (name + " " + friendly(name)).toLowerCase().includes(query.trim().toLowerCase());

const PropertyEditor = ({ property: p, onCommit, onConsole }: { property: Property; onCommit: (value: string) => void; onConsole: () => void }) => {
    const [draft, setDraft] = useState(p.value);
    const editing = useRef(false);
    const cancelled = useRef(false);
    const numeric = p.kind === "float" || p.kind === "int" || p.kind === "mask";
    const bounded = numeric && finite(p.min) && finite(p.max) && Number(p.min) < Number(p.max) && Number(p.max) - Number(p.min) <= 1000000;
    const sliderMin = Number(p.min);
    const sliderMax = Number(p.max);
    const step = p.kind === "int" ? 1 : Math.min(0.01, (sliderMax - sliderMin) / 1000);
    useEffect(() => {
        if (!editing.current) setDraft(p.value);
    }, [p.value]);
    const commit = (raw: string) => {
        let value = raw.trim();
        if (numeric) {
            if (!finite(value)) { setDraft(p.value); return; }
            let n = Number(value);
            if (p.kind !== "float") n = Math.round(n);
            if (finite(p.min)) n = Math.max(Number(p.min), n);
            if (finite(p.max)) n = Math.min(Number(p.max), n);
            value = String(n);
        } else if (/^vector[234]$/.test(p.kind)) {
            const values = value.split(",").map(x => x.trim());
            if (values.length !== Number(p.kind.slice(-1)) || !values.every(finite)) { setDraft(p.value); return; }
            value = values.join(",");
        } else if (p.kind === "color") {
            const values = value.split(",").map(x => x.trim());
            if (!/^#[0-9a-f]{6}([0-9a-f]{2})?$/i.test(value) && !([3, 4].includes(values.length) && values.every(finite))) {
                setDraft(p.value);
                return;
            }
        }
        if (!value) { setDraft(p.value); return; }
        setDraft(value);
        if (value !== p.value) onCommit(value);
    };
    const textInput = <input className={"ngx-value-input" + (numeric ? " numeric" : "")} type="text" value={draft} aria-label={friendly(p.name)}
        onFocus={() => { editing.current = true; cancelled.current = false; }}
        onChange={e => setDraft(e.target.value)}
        onBlur={() => { editing.current = false; if (cancelled.current) setDraft(p.value); else commit(draft); }}
        onKeyDown={e => { e.stopPropagation(); if (e.key === "Enter" || e.key === "Escape") { e.preventDefault(); cancelled.current = e.key === "Escape"; e.currentTarget.blur(); } }} />;
    const colorValues = p.value.split(",").map(Number);
    const swatch = colorValues.length >= 3 && colorValues.slice(0, 3).every(Number.isFinite) ? "#" + colorValues.slice(0, 3).map(v => Math.round(Math.max(0, Math.min(1, v)) * 255).toString(16).padStart(2, "0")).join("") : "#ffffff";
    return <div className="ngx-property-row">
        <div className="ngx-property-label" title={p.name + " · " + p.type}><span>{friendly(p.name)}</span><small>{p.type}</small></div>
        <div className="ngx-property-editor">
            {p.kind === "bool" ? <button type="button" role="switch" aria-checked={p.value.toLowerCase() === "true"} className={"ngx-switch" + (p.value.toLowerCase() === "true" ? " active" : "")} onClick={() => onCommit(p.value.toLowerCase() === "true" ? "false" : "true")}><span /></button> :
            p.kind === "enum" && p.options && p.options.length ? <select className="ngx-value-input" value={p.value} onChange={e => onCommit(e.target.value)}>{!p.options.includes(p.value) && <option value={p.value}>{p.value}</option>}{p.options.map(option => <option key={option} value={option}>{friendly(option)}</option>)}</select> :
            p.kind === "readonly" ? <span className="ngx-readonly" title={p.value}>{p.value || "—"}</span> :
            bounded && finite(p.value) ? <><div className="ngx-slider"><Slider value={Math.max(sliderMin, Math.min(sliderMax, Number(p.value)))} start={sliderMin} end={sliderMax} step={step} gamepadStep={step} valueTransformer={SliderValueTransformer.floatTransformer} disabled={false} noFill={false} onChange={v => commit(String(v))} /></div>{textInput}</> :
            p.kind === "color" ? <><span className="ngx-swatch" style={{ backgroundColor: swatch }} title="Current color" />{textInput}</> : textInput}
        </div>
        <button className="ngx-icon-button ngx-property-console" type="button" title="Edit in optional console" aria-label={"Console: " + friendly(p.name)} onClick={onConsole}><Terminal /></button>
    </div>;
};

export default function NGXWorkspace() {
    const [position, setPosition] = useState({ x: 30, y: 30 });
    const panelRef = useRef<HTMLDivElement>(null);
    const [volumeSearch, setVolumeSearch] = useState("");
    const [componentSearch, setComponentSearch] = useState("");
    const [propertySearch, setPropertySearch] = useState("");
    const [addSearch, setAddSearch] = useState("");
    const [selectedComponent, setSelectedComponent] = useState<string | null>(null);
    const [removeTarget, setRemoveTarget] = useState<string | null>(null);
    const [showAdd, setShowAdd] = useState(false);
    const [showConsole, setShowConsole] = useState(false);
    const [consoleCommand, setConsoleCommand] = useState("");
    const [showPresets, setShowPresets] = useState(false);
    const [compact, setCompact] = useState(false);
    const volumeList = useValue(volumes) ?? [];
    const currentVolume = useValue(selectedVolume) ?? "";
    const components = useValue(volumeComponents) ?? [];
    const available = useValue(availableComponents) ?? [];
    const rawProperties = useValue(componentProperties) ?? [];
    const states = useValue(componentStates) ?? [];
    const activeComponent = selectedComponent && components.includes(selectedComponent) ? selectedComponent : null;
    const properties = rawProperties.map(parseProperty).filter((p): p is Property => p !== null);
    const filteredVolumes = volumeList.filter(v => matches(v, volumeSearch));
    const filteredComponents = components.filter(c => matches(c, componentSearch));
    const addable = available.filter(c => !components.includes(c) && matches(c, addSearch));
    const filteredProperties = properties.filter(p => matches(p.name, propertySearch));
    const enabled = (name: string) => states.some(s => { const [component, active] = s.split("|"); return component === name && active === "True"; });
    const beginDrag = (event: React.MouseEvent) => {
        if (event.button !== 0 || !panelRef.current) return;
        const rect = panelRef.current.getBoundingClientRect();
        const initial = { x: event.clientX, y: event.clientY };
        const old = position;
        const move = (e: MouseEvent) => setPosition({
            x: Math.max(old.x - rect.left, Math.min(old.x + window.innerWidth - rect.right, old.x + e.clientX - initial.x)),
            y: Math.max(old.y - rect.top, Math.min(old.y + window.innerHeight - rect.top - 40, old.y + e.clientY - initial.y))
        });
        const up = () => { document.removeEventListener("mousemove", move); document.removeEventListener("mouseup", up); };
        document.addEventListener("mousemove", move);
        document.addEventListener("mouseup", up);
    };
    const chooseVolume = (name: string) => {
        setSelectedComponent(null);
        setShowAdd(false);
        setPropertySearch("");
        setComponentSearch("");
        trigger("Lumina", "SelectVolume", name);
    };
    const chooseComponent = (name: string) => {
        setSelectedComponent(name);
        setPropertySearch("");
        trigger("Lumina", "SelectComponent", name);
    };
    const openConsole = (command = "") => { setConsoleCommand(command); setShowConsole(true); };
    const selectAll = (e: React.KeyboardEvent<HTMLInputElement>) => e.stopPropagation();

    return <div ref={panelRef} className={"NGXWorkspace ngx-modern" + (compact ? " ngx-compact" : "")} style={{ transform: "translate(" + position.x + "px, " + position.y + "px)" }}>
        <div className="ngx-header" onMouseDown={beginDrag}>
            <div className="ngx-brand"><img src={LuminaLogo} alt="" /><strong>Lumina NGX</strong><span className="ngx-header-divider" /><span>Workspace</span><small>BETA</small></div>
            <div className="ngx-header-actions" onMouseDown={e => e.stopPropagation()}>
                <button className="ngx-button" type="button" onClick={() => setShowPresets(!showPresets)}><Layers />Presets</button>
                <button className="ngx-button" type="button" onClick={() => openConsole()}><Terminal />Console</button>
                <button className="ngx-icon-button" type="button" title={compact ? "Expand workspace" : "Collapse workspace"} onClick={() => setCompact(!compact)}><WorkspaceIcon kind={compact ? "plus" : "minus"} /></button>
            </div>
        </div>
        {!compact && <>
            <div className="ngx-context"><span>Scene volumes</span><ChevronRight /><span>{currentVolume || "Select a volume"}</span>{activeComponent && <><ChevronRight /><strong>{friendly(activeComponent)}</strong></>}<span className="ngx-header-spacer" /><span className="ngx-context-end">HDRP Volume Inspector</span></div>
            <div className="ngx-columns">
                <div className="ngx-column ngx-volumes">
                    <div className="ngx-section-title"><Box /><strong>Scene volumes</strong><small>{volumeList.length}</small></div>
                    <div className="ngx-search"><Search /><input value={volumeSearch} placeholder="Search volumes..." onChange={e => setVolumeSearch(e.target.value)} onKeyDown={selectAll} /></div>
                    <div className="ngx-scroll">
                        {filteredVolumes.map(volume => <button key={volume} className={"ngx-list-item" + (volume === currentVolume ? " selected" : "")} title={volume} type="button" onClick={() => chooseVolume(volume)}><Box /><span>{volume}</span>{volume === currentVolume && <ChevronRight />}</button>)}
                        {!filteredVolumes.length && <div className="ngx-empty">No matching volumes.</div>}
                    </div>
                </div>
                <div className="ngx-column ngx-components">
                    <div className="ngx-section-title"><Layers /><strong>Components</strong><small>{currentVolume ? components.length : 0}</small><span className="ngx-header-spacer" /><button className="ngx-icon-button ngx-add-button" disabled={!currentVolume} title="Add component" type="button" onClick={() => { setShowAdd(!showAdd); setAddSearch(""); }}><WorkspaceIcon kind="plus" /></button></div>
                    <div className="ngx-search"><Search /><input value={componentSearch} disabled={!currentVolume} placeholder="Search components..." onChange={e => setComponentSearch(e.target.value)} onKeyDown={selectAll} /></div>
                    {showAdd && currentVolume && <div className="ngx-add-panel"><div className="ngx-section-title"><strong>Add component</strong><span className="ngx-header-spacer" /><button className="ngx-icon-button" type="button" onClick={() => setShowAdd(false)}><WorkspaceIcon kind="close" /></button></div><div className="ngx-search"><Search /><input value={addSearch} placeholder="Search available..." onChange={e => setAddSearch(e.target.value)} onKeyDown={selectAll} /></div><div className="ngx-add-list">{addable.map(name => <button key={name} type="button" className="ngx-list-item" onClick={() => { trigger("Lumina", "AddComponent", name); setShowAdd(false); }}><WorkspaceIcon kind="plus" /><span>{friendly(name)}</span></button>)}{!addable.length && <div className="ngx-empty">No additional components.</div>}</div></div>}
                    <div className="ngx-scroll">
                        {currentVolume ? filteredComponents.map(name => <div key={name} className={"ngx-component-row" + (name === activeComponent ? " selected" : "")}>
                            <button type="button" className={"ngx-component-toggle" + (enabled(name) ? " active" : "")} title={enabled(name) ? "Disable component" : "Enable component"} aria-pressed={enabled(name)} onClick={() => trigger("Lumina", "ToggleComponent", name)}>{enabled(name) && <Check />}</button>
                            <button type="button" className="ngx-component-name" title={name} onClick={() => chooseComponent(name)}>{friendly(name)}</button>
                            <button type="button" className="ngx-icon-button ngx-remove" title="Remove component" onClick={() => setRemoveTarget(name)}><WorkspaceIcon kind="close" /></button>
                        </div>) : <div className="ngx-empty">Select a scene volume to browse its components.</div>}
                        {currentVolume && !filteredComponents.length && <div className="ngx-empty">No matching components.</div>}
                    </div>
                </div>
                <div className="ngx-column ngx-inspector">
                    <div className="ngx-section-title"><SlidersHorizontal /><strong>Properties</strong>{activeComponent && <small>{properties.length}</small>}</div>
                    {currentVolume && activeComponent ? <>
                        <div className="ngx-inspector-heading"><strong>{friendly(activeComponent)}</strong><button className={"ngx-switch" + (enabled(activeComponent) ? " active" : "")} role="switch" aria-checked={enabled(activeComponent)} type="button" title="Toggle component" onClick={() => trigger("Lumina", "ToggleComponent", activeComponent)}><span /></button></div>
                        <div className="ngx-search"><Search /><input value={propertySearch} placeholder="Search properties..." onChange={e => setPropertySearch(e.target.value)} onKeyDown={selectAll} /></div>
                        <div className="ngx-scroll ngx-properties">{filteredProperties.map(p => <PropertyEditor key={currentVolume + "|" + activeComponent + "|" + p.name} property={p} onCommit={value => trigger("Lumina", "SetProperty", currentVolume + "|" + activeComponent + "." + p.name + "|" + value)} onConsole={() => openConsole('edit "' + currentVolume + '" ' + activeComponent + "." + p.name + " " + p.value)} />)}{!filteredProperties.length && <div className="ngx-empty">No matching properties.</div>}</div>
                    </> : <div className="ngx-empty ngx-inspector-empty"><SlidersHorizontal /><strong>Select a component</strong><span>Its editable HDRP parameters will appear here.</span></div>}
                </div>
            </div>
            <div className="ngx-footer"><span>Unity HDRP · Direct property editing</span><button className="ngx-button" type="button" onClick={() => openConsole()}><Terminal />Optional console</button></div>
        </>}
        {showPresets && !compact && <div className="ngx-preset-overlay"><NGXSaveManager onClose={() => setShowPresets(false)} /></div>}
        {removeTarget && createPortal(<div className="NGXConfirmOverlay"><div className="NGXConfirm"><h3>Remove component?</h3><p>Remove {friendly(removeTarget)} from the selected volume? This cannot be undone. Save your preset or game first.</p><div className="NGXConfirmButtons"><button className="Cancel" onClick={() => setRemoveTarget(null)}>Cancel</button><button className="Danger" onClick={() => { trigger("Lumina", "RemoveComponent", removeTarget); setSelectedComponent(null); setRemoveTarget(null); }}>Remove</button></div></div></div>, document.body)}
        {showConsole && <NGXConsole onClose={() => setShowConsole(false)} command={consoleCommand} clearCommand={() => setConsoleCommand("")} />}
    </div>;
}
