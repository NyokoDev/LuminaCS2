import React, { useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import "./NGXWorkspace.scss";


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


// ==============================
// UNITY ACTIONS (WRITE)
// ==============================

const SelectVolume = "SelectVolume";

const AddComponent = "AddComponent";



// ==============================
// NGX WORKSPACE
// ==============================

export default function NGXWorkspace() {

    const [volumeSearch, setVolumeSearch] = useState("");
    const [componentSearch, setComponentSearch] = useState("");

    const volumeList = useValue(volumes) ?? [];
    const currentVolume = useValue(selectedVolume) ?? "";
    const components = useValue(volumeComponents) ?? [];
    const available = useValue(availableComponents) ?? [];

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



    return (
        <div className="NGXWorkspace">


            <div className="NGXHeader">
                <h2>
                   Lumina NGX - Workspace
                </h2>
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
                                        className="Component"
                                        key={component}
                                    >
                                        {component}
                                    </div>

                                ))

                            )
                        }






                        <h4>
                            Add Component
                        </h4>



                        <div className="SearchBox">
<span className="SearchIcon">
    <svg
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
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
                                filteredComponents.map(component => (

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
                            }

                        </div>



                    </section>

                )
            }


        </div>
    );
}