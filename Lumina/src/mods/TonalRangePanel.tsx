import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { Slider } from "./slider";
import mod from "../../mod.json";

const ShadowsStart$ = bindValue<number>(mod.id, "GetShadowsStart");
const ShadowsEnd$ = bindValue<number>(mod.id, "GetShadowsEnd");
const HighlightsStart$ = bindValue<number>(mod.id, "GetHighlightsStart");
const HighlightsEnd$ = bindValue<number>(mod.id, "GetHighlightsEnd");

const smooth = (a: number, b: number, x: number) => {
    const t = Math.max(0, Math.min(1, (x - a) / Math.max(0.0001, b - a)));
    return t * t * (3 - 2 * t);
};

const finite = (value: number, fallback: number) => Number.isFinite(value) ? value : fallback;
const clamp = (value: number, min: number, max: number) => Math.max(min, Math.min(max, value));

export const TonalRangePanel = () => {
    const ss = clamp(finite(useValue(ShadowsStart$), 0.02), 0, 0.98);
    const se = clamp(finite(useValue(ShadowsEnd$), 0.22), ss + 0.01, 0.99);
    const hs = clamp(finite(useValue(HighlightsStart$), 0.55), se, 0.99);
    const he = clamp(finite(useValue(HighlightsEnd$), 0.85), hs + 0.01, 1);
    const samples = Array.from({ length: 101 }, (_, i) => {
        const x = i / 100;
        const shadow = 1 - smooth(ss, se, x);
        const highlight = smooth(hs, he, x);
        return { x, shadow, midtone: Math.max(0, 1 - shadow - highlight), highlight };
    });
    const series = [
        { key: "shadow" as const, color: "#77a8ef", label: "Shadows" },
        { key: "midtone" as const, color: "#73cbb0", label: "Midtones" },
        { key: "highlight" as const, color: "#e9c172", label: "Highlights" },
    ];
    const controls = [
        { label: "Shadow Start", key: "SetShadowsStart", value: ss, min: 0, max: se - 0.01 },
        { label: "Shadow End", key: "SetShadowsEnd", value: se, min: ss + 0.01, max: hs },
        { label: "Highlight Start", key: "SetHighlightsStart", value: hs, min: se, max: he - 0.01 },
        { label: "Highlight End", key: "SetHighlightsEnd", value: he, min: hs + 0.01, max: 1 },
    ];
    return (
        <section className="lumina-tonal-range">
            <div className="lumina-tonal-header">
                <span>Tonal Range</span>
                <button className="lumina-tonal-reset" onClick={() => trigger(mod.id, "ResetTonalRanges")}>Reset ranges</button>
            </div>
            <div className="lumina-tonal-description">Adjust which HDR luminance values receive shadow, midtone and highlight grading. Enable a brightness adjustment above to see the effect.</div>
            <svg className="lumina-tonal-graph" viewBox="0 0 700 126" preserveAspectRatio="none">
                {[0, .25, .5, .75, 1].map(x => <line key={x} x1={10+x*680} y1="6" x2={10+x*680} y2="103" stroke="#ffffff" opacity=".12" />)}
                {Array.from({length: 50}, (_, i) => <rect key={i} x={10+i*13.6} y="108" width="13.7" height="13" fill={"rgb("+Math.round(i*255/49)+","+Math.round(i*255/49)+","+Math.round(i*255/49)+")"} />)}
                {series.map(line => <polyline key={line.key} points={samples.map(point => (10+point.x*680)+","+(102-point[line.key]*94)).join(" ")} fill="none" stroke={line.color} strokeWidth="2.5" />)}
            </svg>
            <div className="lumina-tonal-legend">
                {series.map(line => <span key={line.key} style={{color: line.color}}>{line.label}</span>)}
            </div>
            {controls.map(control => (
                <div key={control.key} className="lumina-tonal-row">
                    <span className="lumina-tonal-row-label">{control.label}</span>
                    <Slider className="lumina-tonal-slider" value={control.value} start={control.min} end={Math.max(control.min + 0.0001, control.max)} step={0.01} gamepadStep={0.01} disabled={control.max - control.min < 0.0001} noFill={false}
                        onChange={value => trigger(mod.id, control.key, Math.round(clamp(value, control.min, control.max)*100)/100)} />
                    <span className="lumina-tonal-value">{control.value.toFixed(2)}</span>
                </div>
            ))}
        </section>
    );
};
