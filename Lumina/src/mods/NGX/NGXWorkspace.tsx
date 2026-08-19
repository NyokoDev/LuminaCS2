import React, { useRef, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import "./NGXWorkspace.scss";
import "./NGXNotice.scss";
import "./NGXConfirmation.scss";
import "./AddComponent.scss";
import LuminaLogo from "../../img/Lumina.svg"
import { createPortal } from "react-dom";
import NGXConsole from "./Console/NGXConsole";
import NGXSaveManager from "./NGXSaveManager/NGXSaveManager";
import "./NGXSaveManager/NGXSaveManager.scss";


// ==============================
// UNITY VALUES (READ)
// ==============================

export const volumes = bindValue<string[]>(
    "Lumina",
    "GetVolumes"
);

export const selectedVolume = bindValue<string>(
    "Lumina",
    "SelectedVolume"
);

export const volumeComponents = bindValue<string[]>(
    "Lumina",
    "GetVolumeComponents"
);

export const availableComponents = bindValue<string[]>(
    "Lumina",
    "GetAvailableComponents"
);

export const componentProperties = bindValue<any[]>(
    "Lumina",
    "GetComponentProperties"
);


export const componentStates = bindValue<string[]>(
    "Lumina",
    "GetComponentStates"
);

// ==============================
// UNITY ACTIONS (WRITE)
// ==============================

const SelectVolume = "SelectVolume";
const SelectComponent = "SelectComponent";
const AddComponent = "AddComponent";
const RemoveComponent = "RemoveComponent";
const ToggleComponent = "ToggleComponent";


// ==============================
// NGX WORKSPACE
// ==============================

export default function NGXWorkspace() {

const [position, setPosition] = useState({
    x: 350,
    y: 120
});

const [consoleCommand, setConsoleCommand] = useState("");
    const panelRef = useRef<HTMLDivElement>(null);

const dragging = useRef(false);

const offset = useRef({ x: 0, y: 0 });

const beginDrag = (e: React.MouseEvent) => {

    if (!panelRef.current)
        return;

    dragging.current = true;

    const rect = panelRef.current.getBoundingClientRect();

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

        document.removeEventListener("mousemove", move);
        document.removeEventListener("mouseup", up);
    };

    document.addEventListener("mousemove", move);
    document.addEventListener("mouseup", up);
};

    const [volumeSearch, setVolumeSearch] = useState("");
    const [componentSearch, setComponentSearch] = useState("");
    const [selectedComponent, setSelectedComponent] = useState<string | null>(null);
    const [removeTarget, setRemoveTarget] = useState<string | null>(null);
    const [showAddComponents, setShowAddComponents] = useState(false);
    const [showConsole, setShowConsole] = useState(false);
    const [showSaveManager, setShowSaveManager] = useState(false);

    const volumeList = useValue(volumes) ?? [];
    const currentVolume = useValue(selectedVolume) ?? "";
    const components = useValue(volumeComponents) ?? [];
    const available = useValue(availableComponents) ?? [];
    const properties = useValue(componentProperties) ?? [];
    const states = useValue(componentStates) ?? [];

    const isComponentEnabled = (name:string) =>
{
    const state = states.find(x => x.startsWith(name));

    if(!state)
        return false;

    return state.split("|")[1] === "True";
};

    const filteredVolumes = volumeList.filter(volume =>
        volume
            .toLowerCase()
            .includes(volumeSearch.toLowerCase())
    );


    const filteredComponents = available.filter(component =>
        component
            .toLowerCase()
            .includes(componentSearch.toLowerCase())
    );

    const addableComponents = filteredComponents.filter(component =>
    !components.includes(component)
);



    return (
        <div
    ref={panelRef}
    className="NGXWorkspace"
    style={{
    transform: `translate(${position.x}px, ${position.y}px)`
}}
    
    
>


            
        <div
    className="NGXHeader"
    onMouseDown={beginDrag}
>
    <div className="NGXTitle">

        <img
            src={LuminaLogo}
            className="NGXLogo"
            alt="Lumina"
        />

        <h2>
            Lumina NGX - Workspace
        </h2>

    </div>

<div
    className="NGXHeaderButtons"
    onMouseDown={e => e.stopPropagation()}
>
    <button
        className="NGXPresetsButton"
        onClick={() => setShowSaveManager(true)}
        title="NGX Presets"
    >
        Presets
    </button>

    <button
        className="ConsoleButton"
        onClick={() => setShowConsole(true)}
        title="Open Console"
    >
        &gt;_
    </button>
</div>

</div>

{
    showSaveManager && (
        <NGXSaveManager
            onClose={() => setShowSaveManager(false)}
        />
    )
}


{/* NGX BETA NOTICE */}
<div className="NGXNotice">

        <p>
            Lumina NGX is running in inspector mode. Explore volumes, modify component properties, add or remove components, and save or load your changes using NGX Presets.
        </p>



</div>

{/* VOLUME SELECTION */}

<section className="NGXPanel">

    <h3>
        Select Volume
    </h3>


    {
        currentVolume && (

            <button
                className="ChangeVolume"
                onClick={() =>
                    trigger(
                        "Lumina",
                        SelectVolume,
                        ""
                    )
                }
            >
                Change Volume
            </button>

        )
    }


    {
        !currentVolume && (

            <>
                <div className="SearchBox">

                    <span className="SearchIcon">
                        <svg
                            viewBox="0 0 24 24"
                            fill="none"
                        >
                            <circle
                                cx="11"
                                cy="11"
                                r="7"
                                stroke="currentColor"
                                strokeWidth="2"
                            />

                            <path
                                d="M20 20L16.5 16.5"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                            />

                        </svg>
                    </span>


                    <input
                        placeholder="Search volumes..."
                        value={volumeSearch}
                        onChange={(e) =>
                            setVolumeSearch(e.target.value)
                        }
                    />

                </div>


                <div className="VolumeList">

                    {
                        filteredVolumes.map(volume => (

                            <button
                                key={volume}
                                onClick={() =>
                                    trigger(
                                        "Lumina",
                                        SelectVolume,
                                        volume
                                    )
                                }
                            >
                                {volume}
                            </button>

                        ))
                    }

                </div>
            </>

        )
    }

</section>


            {/* SELECTED VOLUME */}

            {
                currentVolume && (

                    <section className="NGXPanel">


                        <h3>
    Editing: {currentVolume}
</h3>




                        <h4>
                            Current Components
                        </h4>



                        {
                            components.length === 0 ? (

                                <p>
                                    No components attached
                                </p>

                            ) : (

components.map(component => (

<div
    key={component}
    className={`Component ${
        selectedComponent === component ? "Selected" : ""
    }`}
>


    <button
        className={`ComponentToggle ${
            isComponentEnabled(component)
            ? "Enabled"
            : ""
        }`}
        onClick={() =>
            trigger(
                "Lumina",
                ToggleComponent,
                component
            )
        }
    >
        {isComponentEnabled(component) ? "✓" : ""}
    </button>



    <div
        className="ComponentName"
        onClick={() => {

            setSelectedComponent(component);

            trigger(
                "Lumina",
                SelectComponent,
                component
            );

        }}
    >
        {component}
    </div>


    <button
        className="RemoveComponent"
        onClick={() =>
            setRemoveTarget(component)
        }
    >
        ×
    </button>


</div>

))

                            )
                        }






                        <h4
    className="AddComponentHeader"
    onClick={() =>
        setShowAddComponents(!showAddComponents)
    }
>
    Add Component {showAddComponents ? "" : ""}
</h4>



{
showAddComponents && (

    <>
        <div className="SearchBox">

            <span className="SearchIcon">
                <svg
                    viewBox="0 0 24 24"
                    fill="none"
                >
                    <circle
                        cx="11"
                        cy="11"
                        r="7"
                        stroke="currentColor"
                        strokeWidth="2"
                    />

                    <path
                        d="M20 20L16.5 16.5"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                    />
                </svg>
            </span>

            <input
                placeholder="Search components..."
                value={componentSearch}
                onChange={(e) =>
                    setComponentSearch(e.target.value)
                }
            />

        </div>


        <div className="ComponentList">

        {
            addableComponents.length === 0 ? (

                <p>
                    No additional components available.
                </p>

            ) : (

                addableComponents.map(component => (

                    <button
                        key={component}
                        onClick={() =>
                            trigger(
                                "Lumina",
                                AddComponent,
                                component
                            )
                        }
                    >
                        + {component}
                    </button>

                ))

            )
        }

        </div>

    </>

)
}



                    </section>

                )
            }

            {
    selectedComponent && (

        <section className="NGXPanel">

            <h3>
                {selectedComponent}
            </h3>

            <div className="PropertyList">
    {properties.map((property, i) => {

        const [name, type, value] = property.split("|");

        return (
           <div className="PropertyRow">

    <div className="PropertyName">
        {name}
    </div>

    <div className="PropertyType">
        {type}
    </div>

    <div className="PropertyValue">
        {value}
    </div>

    <button
        className="ConsolePropertyButton"
        onClick={() => {

            setConsoleCommand(
                `edit "${currentVolume}" ${selectedComponent}.${name} ${value}`
            );

            setShowConsole(true);

        }}
    >
        &gt;_
    </button>

</div>
        );
    })}
</div>
            

        </section>

    )
}

<div>
{
    removeTarget &&
    createPortal(

        <div className="NGXConfirmOverlay">

            <div className="NGXConfirm">

                <h3>
                    Remove Component?
                </h3>

                <p>
                    Are you sure you want to remove the component?

                    This action cannot be undone.
                    This will remove the component from the selected volume.

                    To restore the component if unsaved, restart the game.
                    Save your game beforehand.
                </p>


                <div className="NGXConfirmButtons">

                    <button
                        className="Cancel"
                        onClick={() =>
                            setRemoveTarget(null)
                        }
                    >
                        Cancel
                    </button>


                    <button
                        className="Danger"
                        onClick={() => {

                            trigger(
                                "Lumina",
                                RemoveComponent,
                                removeTarget
                            );

                            setRemoveTarget(null);

                        }}
                    >
                        Remove
                    </button>

                </div>

            </div>

        </div>,

        document.body
    )
}
</div>


{
    showConsole && (
        <NGXConsole
            onClose={() => setShowConsole(false)}
            command={consoleCommand}
            clearCommand={() => setConsoleCommand("")}
        />
    )
}

    </div>

        
        

        

        
    );
}