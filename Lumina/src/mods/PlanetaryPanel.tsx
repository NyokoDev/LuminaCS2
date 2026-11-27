import React, { useEffect, useRef, useState } from "react";
import { Slider, SliderValueTransformer } from "mods/slider";
import { useLocalization } from "cs2/l10n";
import "./PlanetaryPanel.scss";

type Props = {
  latitude: number;
  longitude: number;
  onLatitude: (value: number) => void;
  onLongitude: (value: number) => void;
};

const locations = [
  { name: "San José", lat: 9.93, lon: -84.08 },
  { name: "London", lat: 51.51, lon: -0.13 },
  { name: "New York", lat: 40.71, lon: -74.01 },
  { name: "Tokyo", lat: 35.68, lon: 139.69 },
  { name: "Sydney", lat: -33.87, lon: 151.21 },
  { name: "Reykjavík", lat: 64.15, lon: -21.94 }
];

const land = [
  [[-168,66],[-157,72],[-139,70],[-128,56],[-125,49],[-124,40],[-117,32],[-106,23],[-97,20],[-87,21],[-83,10],[-77,8],[-80,20],[-90,29],[-82,25],[-80,32],[-74,40],[-66,45],[-60,53],[-76,61],[-83,68],[-104,75],[-130,74],[-151,70],[-168,66]],
  [[-81,12],[-70,12],[-60,6],[-49,2],[-35,-6],[-39,-17],[-49,-29],[-57,-38],[-65,-55],[-73,-49],[-75,-32],[-70,-18],[-80,-5],[-81,12]],
  [[-73,60],[-53,59],[-20,70],[-23,79],[-43,83],[-61,79],[-73,60]],
  [[-10,36],[-9,44],[-5,49],[3,51],[7,55],[5,59],[15,71],[30,70],[40,66],[47,60],[41,47],[30,41],[24,36],[12,38],[2,43],[-10,36]],
  [[-17,37],[10,37],[32,31],[43,12],[51,11],[42,-12],[35,-24],[19,-35],[12,-29],[9,-13],[-3,5],[-16,14],[-17,37]],
  [[30,71],[60,74],[90,76],[120,74],[149,70],[179,66],[168,54],[143,49],[135,35],[122,23],[108,20],[106,6],[99,8],[92,23],[80,8],[73,21],[60,25],[50,30],[40,42],[30,50],[30,71]],
  [[113,-22],[115,-34],[136,-35],[151,-39],[154,-27],[145,-14],[130,-12],[113,-22]],
  [[130,31],[136,34],[141,42],[145,44],[141,36],[135,32],[130,31]],
  [[95,5],[104,-5],[115,-8],[120,-4],[111,1],[95,5]],
  [[125,1],[142,-5],[151,-6],[143,-10],[131,-5],[125,1]],
  [[47,-13],[50,-17],[47,-26],[43,-23],[47,-13]],
  [[166,-34],[178,-38],[174,-47],[168,-45],[166,-34]],
  [[-180,-72],[-140,-74],[-100,-71],[-65,-64],[-30,-73],[10,-70],[50,-66],[100,-68],[140,-67],[180,-73],[180,-90],[-180,-90]]
];

const clamp = (value: number, min: number, max: number) => Math.max(min, Math.min(max, Number.isFinite(value) ? value : 0));
const format = (value: number) => String(Number(value.toFixed(4)));
const points = (coordinates: number[][]) => coordinates.map(([lon, lat]) => (lon + 180) + "," + (90 - lat)).join(" ");

const CoordinateInput = ({ label, value, limit, onChange }: { label: string; value: number; limit: number; onChange: (value: number) => void }) => {
  const [draft, setDraft] = useState(format(value));
  const editing = useRef(false);
  const cancelled = useRef(false);
  useEffect(() => {
    if (!editing.current) setDraft(format(value));
  }, [value]);

  return (
    <div className="planetary-coordinate-row">
      <span className="planetary-coordinate-label">{label}</span>
      <div className="planetary-slider-wrap">
        <Slider value={value} start={-limit} end={limit} step={0.0001} gamepadStep={0.01} valueTransformer={SliderValueTransformer.floatTransformer} disabled={false} noFill={false} onChange={onChange} className="planetary-coordinate-slider" />
      </div>
      <input type="text" inputMode="decimal" className="planetary-coordinate-input" aria-label={label} value={draft}
        onFocus={() => { editing.current = true; cancelled.current = false; }}
        onChange={event => setDraft(event.target.value)}
        onBlur={() => {
          editing.current = false;
          const parsed = Number(draft);
          if (cancelled.current || !draft.trim() || !Number.isFinite(parsed) || draft === format(value)) {
            setDraft(format(value));
            return;
          }
          const next = clamp(parsed, -limit, limit);
          setDraft(format(next));
          if (next !== value) onChange(next);
        }}
        onKeyDown={event => {
          event.stopPropagation();
          if (event.key === "Enter" || event.key === "Escape") {
            event.preventDefault();
            cancelled.current = event.key === "Escape";
            event.currentTarget.blur();
          }
        }} />
      <button type="button" className="planetary-reset-icon" title={"Reset " + label + " to zero"} aria-label={"Reset " + label} onClick={() => onChange(0)}>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8"><path d="M4 11a8 8 0 1 1 2 6M4 4v7h7" /></svg>
      </button>
    </div>
  );
};

export const PlanetarySettings = ({ latitude, longitude, onLatitude, onLongitude }: Props) => {
  const { translate } = useLocalization();
  const lat = clamp(latitude, -90, 90);
  const lon = clamp(longitude, -180, 180);
  const setPosition = (newLat: number, newLon: number) => {
    onLatitude(newLat);
    onLongitude(newLon);
  };
  const coordinateText = Math.abs(lat).toFixed(2) + "° " + (lat >= 0 ? "N" : "S") + " · " + Math.abs(lon).toFixed(2) + "° " + (lon >= 0 ? "E" : "W");

  return (
    <div className="planetary-settings">
      <div className="planetary-brand">
        <div><div className="planetary-brand-name">LUMINA</div><div className="planetary-subtitle">Planetary Settings</div></div>
        <div className="planetary-logo" aria-hidden="true" />
      </div>
      <div className="planetary-heading">Geographic position</div>
      <div className="planetary-map-frame">
        <svg className="planetary-world-map" viewBox="0 0 360 180" preserveAspectRatio="none" aria-label="Schematic world map. Click to select coordinates."
          onClick={event => {
            const rect = event.currentTarget.getBoundingClientRect();
            if (!rect.width || !rect.height) return;
            const nextLon = clamp((event.clientX - rect.left) / rect.width * 360 - 180, -180, 180);
            const nextLat = clamp(90 - (event.clientY - rect.top) / rect.height * 180, -90, 90);
            setPosition(Number(nextLat.toFixed(4)), Number(nextLon.toFixed(4)));
          }}>
          <rect width="360" height="180" fill="#20292d" />
          {[-60, -30, 0, 30, 60].map(l => <line key={"lat" + l} x1="0" y1={90-l} x2="360" y2={90-l} stroke="#ffffff" strokeOpacity=".08" strokeWidth=".5" />)}
          {[-120, -60, 0, 60, 120].map(l => <line key={"lon" + l} x1={180+l} y1="0" x2={180+l} y2="180" stroke="#ffffff" strokeOpacity=".08" strokeWidth=".5" />)}
          <g fill="#59615d" stroke="#79817b" strokeWidth=".55">
            {land.map((polygon, index) => <polygon key={index} points={points(polygon)} />)}
          </g>
          <line x1={180+lon} x2={180+lon} y1="0" y2="180" stroke="#d3a052" strokeDasharray="3 3" opacity=".8" />
          <line x1="0" x2="360" y1={90-lat} y2={90-lat} stroke="#d3a052" strokeDasharray="3 3" opacity=".8" />
          <circle cx={180+lon} cy={90-lat} r="6" fill="#d3a052" opacity=".3" />
          <circle cx={180+lon} cy={90-lat} r="2.6" fill="#efbb6b" stroke="white" strokeWidth=".8" />
        </svg>
        <div className="planetary-map-caption"><span>Selected coordinates</span><span className="planetary-map-value">{coordinateText}</span></div>
      </div>
      <CoordinateInput label={translate("LUMINA.latitude") ?? "Latitude"} value={lat} limit={90} onChange={onLatitude} />
      <CoordinateInput label={translate("LUMINA.longitude") ?? "Longitude"} value={lon} limit={180} onChange={onLongitude} />
      <div className="planetary-heading">Location presets</div>
      <div className="planetary-locations">
        {locations.map(location => (
          <button key={location.name} type="button" className={"planetary-location" + (Math.abs(lat-location.lat) < 0.001 && Math.abs(lon-location.lon) < 0.001 ? " active" : "")} onClick={() => setPosition(location.lat, location.lon)}>
            <span>{location.name}</span><span className="planetary-location-coordinates">{location.lat.toFixed(2)}°, {location.lon.toFixed(2)}°</span>
          </button>
        ))}
      </div>
      <div className="planetary-notice"><span className="planetary-notice-symbol">i</span><span>Enable latitude and longitude adjustments in the game settings to use planetary overrides.</span></div>
      <div className="planetary-footer"><span>PLANETARY · LUMINA</span><button type="button" className="planetary-reset-button" onClick={() => setPosition(0, 0)}>Reset coordinates</button></div>
    </div>
  );
};
