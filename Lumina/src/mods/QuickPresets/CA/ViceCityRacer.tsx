import React, { useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import "./ViceCityRacer.scss";
import ViceCityLogo from "../../../img/ViceCity.svg";

type Traffic = { x: number; y: number; color: string };
type Game = { x: number; distance: number; speed: number; spawn: number; traffic: Traffic[]; over: boolean };
const fresh = (): Game => ({ x: 180, distance: 0, speed: 120, spawn: 0.5, traffic: [], over: false });
const clamp = (v: number, a: number, b: number) => Math.max(a, Math.min(b, v));
const scoreKey = "Lumina.ViceCityRacer.HighScore";
let sessionBest = 0;
const loadBest = () => {
    try { return Math.max(sessionBest, Number(window.localStorage.getItem(scoreKey)) || 0); }
    catch { return sessionBest; }
};

export default function ViceCityRacer({ onClose }: { onClose: () => void }) {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const modalRef = useRef<HTMLDivElement>(null);
    const game = useRef<Game>(fresh());
    const keys = useRef<{ [key: string]: boolean }>({});
    const running = useRef(false);
    const [status, setStatus] = useState<"ready" | "playing" | "over">("ready");
    const [score, setScore] = useState(0);
    const [best, setBest] = useState(loadBest);
    const bestRef = useRef(best);
    const closeRef = useRef(onClose);
    closeRef.current = onClose;
    const start = () => {
        game.current = fresh();
        keys.current = {};
        running.current = true;
        setScore(0);
        setStatus("playing");
        modalRef.current?.focus();
    };

    useEffect(() => {
        modalRef.current?.focus();
        const control = (event: KeyboardEvent, down: boolean) => {
            const key = event.key.toLowerCase();
            if (key === "escape" && down) {
                event.preventDefault();
                event.stopImmediatePropagation();
                closeRef.current();
                return;
            }
            if (["arrowleft", "arrowright", "arrowup", "arrowdown", "a", "d", "w", "s", " "].includes(key)) {
                event.preventDefault();
                event.stopImmediatePropagation();
                keys.current[key] = down;
                if (key === " " && down && !event.repeat && !running.current) start();
            }
        };
        const down = (e: KeyboardEvent) => control(e, true);
        const up = (e: KeyboardEvent) => control(e, false);
        const blur = () => { keys.current = {}; };
        window.addEventListener("keydown", down, true);
        window.addEventListener("keyup", up, true);
        window.addEventListener("blur", blur);
        let raf = 0;
        let last = 0;
        let shown = -1;
        const canvas = canvasRef.current;
        const ctx = canvas?.getContext("2d");
        if (!ctx) return () => {
            window.removeEventListener("keydown", down, true);
            window.removeEventListener("keyup", up, true);
            window.removeEventListener("blur", blur);
        };

        const car = (x: number, y: number, color: string, player: boolean) => {
            ctx.fillStyle = "rgba(0,0,0,.5)";
            ctx.fillRect(x - 17, y - 25, 34, 54);
            ctx.fillStyle = color;
            ctx.fillRect(x - 13, y - 25, 26, 50);
            ctx.fillStyle = "#181528";
            ctx.fillRect(x - 10, y - 12, 20, 15);
            ctx.fillStyle = "#a9e6ff";
            ctx.fillRect(x - 10, y - 18, 20, 6);
            ctx.fillStyle = "#15151c";
            for (const side of [-1, 1]) {
                ctx.fillRect(x + side * 13 - 3, y - 18, 6, 10);
                ctx.fillRect(x + side * 13 - 3, y + 10, 6, 10);
            }
            ctx.fillStyle = player ? "#ff5ba7" : "#ffd7a3";
            ctx.fillRect(x - 11, y + 20, 7, 4);
            ctx.fillRect(x + 4, y + 20, 7, 4);
        };
        const palm = (x: number, y: number) => {
            ctx.strokeStyle = "#62505c";
            ctx.lineWidth = 5;
            ctx.beginPath(); ctx.moveTo(x, y + 25); ctx.lineTo(x + 3, y); ctx.stroke();
            ctx.strokeStyle = "#213d42";
            ctx.lineWidth = 6;
            for (let i = 0; i < 6; i++) {
                const a = i * Math.PI / 3;
                ctx.beginPath();
                ctx.moveTo(x + 3, y);
                ctx.quadraticCurveTo(x + Math.cos(a) * 13, y + Math.sin(a) * 13 - 6, x + Math.cos(a) * 21, y + Math.sin(a) * 21);
                ctx.stroke();
            }
        };
        const frame = (now: number) => {
            const dt = last ? Math.min(0.05, (now - last) / 1000) : 0;
            last = now;
            const g = game.current;
            if (running.current) {
                const k = keys.current;
                const steer = Number(!!(k.arrowright || k.d)) - Number(!!(k.arrowleft || k.a));
                g.x = clamp(g.x + steer * 205 * dt, 77, 283);
                const target = Math.min(390, 150 + g.distance / 32) + (k.arrowup || k.w ? 90 : 0) - (k.arrowdown || k.s ? 75 : 0);
                g.speed += (target - g.speed) * Math.min(1, dt * 3);
                g.distance += g.speed * dt;
                g.spawn -= dt;
                if (g.spawn <= 0) {
                    const lanes = [108, 180, 252];
                    g.traffic.push({ x: lanes[Math.floor(Math.random() * lanes.length)], y: -30, color: ["#e9c56f", "#70d9dc", "#bc8ce8", "#f1eee4"][Math.floor(Math.random() * 4)] });
                    g.spawn = Math.max(0.48, 1.5 - g.distance / 7000);
                }
                for (const t of g.traffic) t.y += g.speed * dt;
                g.traffic = g.traffic.filter(t => t.y < 555);
                const points = Math.floor(g.distance / 10);
                if (points !== shown) { shown = points; setScore(points); }
                if (g.traffic.some(t => Math.abs(t.x - g.x) < 27 && Math.abs(t.y - 440) < 46)) {
                    running.current = false;
                    g.over = true;
                    setStatus("over");
                    if (points > bestRef.current) {
                        bestRef.current = points;
                        sessionBest = points;
                        setBest(points);
                        try { window.localStorage.setItem(scoreKey, String(points)); } catch {}
                    }
                }
            }
            ctx.fillStyle = "#171825"; ctx.fillRect(0, 0, 360, 520);
            ctx.fillStyle = "#20383a"; ctx.fillRect(0, 0, 58, 520); ctx.fillRect(302, 0, 58, 520);
            ctx.fillStyle = "#5a3c51"; ctx.fillRect(42, 0, 18, 520); ctx.fillRect(300, 0, 18, 520);
            ctx.fillStyle = "#302b3c"; ctx.fillRect(60, 0, 240, 520);
            ctx.fillStyle = "#ef80ad"; ctx.fillRect(60, 0, 2, 520); ctx.fillRect(298, 0, 2, 520);
            ctx.fillStyle = "#b4a3a8";
            const offset = g.distance % 56;
            for (let y = -56 + offset; y < 520; y += 56) {
                ctx.fillRect(140, y, 2, 24);
                ctx.fillRect(220, y, 2, 24);
            }
            for (let y = -100 + g.distance % 100; y < 550; y += 100) {
                ctx.fillStyle = "#be8e9f"; ctx.fillRect(43, y, 13, 36); ctx.fillRect(304, y + 50, 13, 36);
                palm(20, y + 25); palm(335, y + 70);
                ctx.fillStyle = "#f6c17d"; ctx.fillRect(48, y + 70, 3, 11); ctx.fillRect(309, y + 20, 3, 11);
            }
            for (const t of g.traffic) car(t.x, t.y, t.color, false);
            car(g.x, 440, "#ec6bb0", true);
            raf = requestAnimationFrame(frame);
        };
        raf = requestAnimationFrame(frame);
        return () => {
            cancelAnimationFrame(raf);
            running.current = false;
            window.removeEventListener("keydown", down, true);
            window.removeEventListener("keyup", up, true);
            window.removeEventListener("blur", blur);
        };
    }, []);

    return createPortal(
        <div className="vc-racer-overlay" onMouseDown={e => e.stopPropagation()} onClick={e => e.stopPropagation()}>
            <div className="vc-racer" ref={modalRef} tabIndex={-1} role="dialog" aria-modal="true" aria-label="Vice City Racer">
                <div className="vc-racer-header"><div><span>LUMINA ARCADE</span><strong className="vc-racer-brand"><img src={ViceCityLogo} alt="Vice City" /><span>Racer</span></strong></div><button type="button" aria-label="Close arcade" onClick={onClose}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M6 6l12 12M18 6L6 18" /></svg></button></div>
                <div className="vc-racer-score"><div><span>SCORE</span><strong>{String(score).padStart(5, "0")}</strong></div><div><span>BEST</span><strong>{String(best).padStart(5, "0")}</strong></div></div>
                <div className="vc-racer-screen">
                    <canvas ref={canvasRef} width={360} height={520} />
                    {status !== "playing" && <div className="vc-racer-message"><span>OCEAN DRIVE · 1986</span><strong>{status === "over" ? "GAME OVER" : "WELCOME TO VICE CITY"}</strong><div>{status === "over" ? "Score: " + score : "Dodge traffic and chase the high score."}</div><button type="button" onClick={start}>{status === "over" ? "PLAY AGAIN" : "START ENGINE"}</button></div>}
                </div>
                <div className="vc-racer-controls"><span>← → / A D · Steer</span><span>↑ ↓ / W S · Speed</span><span>ESC · Exit</span></div>
            </div>
        </div>, document.body
    );
}
