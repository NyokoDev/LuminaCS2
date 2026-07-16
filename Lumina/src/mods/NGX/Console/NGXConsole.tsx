import React, { useRef, useState } from "react";
import { createPortal } from "react-dom";
import "./NGXConsole.scss";
import { trigger } from "cs2/api";

interface NGXConsoleProps {
    onClose: () => void;
}

type EntryType =
    | "info"
    | "warning"
    | "error"
    | "success";

interface ConsoleEntry {
    type: EntryType;
    text: string;
}

export default function NGXConsole({ onClose }: NGXConsoleProps) {

    const [position, setPosition] = useState({
        x: 180,
        y: 120
    });
const SelectVolume = "SelectVolume";
const SelectComponent = "SelectComponent";
const FocusProperty = "FocusProperty";
    const [input, setInput] = useState("");
    const [commandHistory, setCommandHistory] = useState<string[]>([]);
const [historyIndex, setHistoryIndex] = useState(-1);

    const [history, setHistory] = useState<ConsoleEntry[]>([
    {
        type: "success",
        text: "Lumina NGX Console initialized."
    },
    {
        type: "info",
        text: "Type 'help' to list available commands. Press TAB to recall previous commands."
    }
]);

    const windowRef = useRef<HTMLDivElement>(null);

    const dragging = useRef(false);

    const offset = useRef({
        x: 0,
        y: 0
    });

    function log(type: EntryType, text: string) {

        setHistory(prev => [
            ...prev,
            {
                type,
                text
            }
        ]);

    }


    function editCommand(args: string[]) {

    if (args.length < 3) {

        log(
            "error",
            "Usage: edit <volume> <component.property> <value>"
        );

        return;
    }


    const volume = args[0];

    const property = args[1];

    const value = args[2];


    trigger(
        "Lumina",
        "SetProperty",
        `${volume}|${property}|${value}`
    );


    log(
        "success",
        `Edited ${property} = ${value}`
    );
}

    function executeCommand(command: string) {

        const trimmed = command.trim();

        if (!trimmed)
            return;

        log("info", `> ${trimmed}`);

        const args = trimmed.match(/(?:[^\s"]+|"[^"]*")+/g)
    ?.map(arg => arg.replace(/^"|"$/g, "")) ?? [];

        const cmd = args.shift()?.toLowerCase();

        switch (cmd) {

            case "help":

                log("success",
                    "Commands: help, edit, list, select, set, get, toggle, enable, clear"
                );

                break;

            case "clear":

                setHistory([]);

                return;

            case "edit":

    editCommand(args);

    break;

            case "list":

                log("warning",
                    "list is not implemented yet."
                );

                break;

            case "select":

                log("warning",
                    "select is not implemented yet."
                );

                break;

            case "set":

                log("warning",
                    "set is not implemented yet."
                );

                break;

            case "get":

                log("warning",
                    "get is not implemented yet."
                );

                break;

            case "toggle":

                log("warning",
                    "toggle is not implemented yet."
                );

                break;

            case "enable":

                log("warning",
                    "enable is not implemented yet."
                );

                break;

            default:

                log(
                    "error",
                    `Unknown command '${cmd}'. Type 'help'.`
                );

                break;
        }

    }

    const beginDrag = (e: React.MouseEvent) => {

        if (!windowRef.current)
            return;

        dragging.current = true;

        const rect = windowRef.current.getBoundingClientRect();

        offset.current = {

            x: e.clientX - rect.left,

            y: e.clientY - rect.top

        };

        const move = (ev: MouseEvent) => {

            if (!dragging.current)
                return;

            setPosition({

                x: ev.clientX - offset.current.x,

                y: ev.clientY - offset.current.y

            });

        };

        const up = () => {

            dragging.current = false;

            document.removeEventListener(
                "mousemove",
                move
            );

            document.removeEventListener(
                "mouseup",
                up
            );

        };

        document.addEventListener(
            "mousemove",
            move
        );

        document.addEventListener(
            "mouseup",
            up
        );

    };

    return createPortal(

        <div className="NGXConsoleOverlay">

            <div
                ref={windowRef}
                className="NGXConsoleWindow"
                style={{
                    left: position.x,
                    top: position.y
                }}
            >

                <div
                    className="NGXConsoleHeader"
                    onMouseDown={beginDrag}
                >

                    <h3>
                        NGX Console
                    </h3>

                    <button
                        onMouseDown={e => e.stopPropagation()}
                        onClick={onClose}
                    >
                        ✕
                    </button>

                </div>

                <div className="NGXConsoleMessages">

                    {
                        history.map((entry, i) => (

                            <div
                                key={i}
                                className={`ConsoleMessage ${entry.type}`}
                            >

                                {entry.text}

                            </div>

                        ))
                    }

                </div>

                <div className="NGXConsoleInput">

                    <span>&gt;</span>

                    <input

                        value={input}

                        autoFocus

                        spellCheck={false}

                        placeholder="Enter command..."

                        onChange={e =>
                            setInput(e.target.value)
                        }

                        onKeyDown={e => {

    if (e.key === "Enter") {

        if (!input.trim())
            return;

        setCommandHistory(prev => [
            ...prev,
            input
        ]);

        setHistoryIndex(-1);

        executeCommand(input);

        setInput("");

        return;
    }


    if (e.key === "Tab") {

        e.preventDefault();

        if (commandHistory.length === 0)
            return;


        const index =
            historyIndex === -1
                ? commandHistory.length - 1
                : Math.max(
                    0,
                    historyIndex - 1
                );


        setHistoryIndex(index);

        setInput(
            commandHistory[index]
        );

    }

}}

                    />

                </div>

            </div>

        </div>,

        document.body

    );

}