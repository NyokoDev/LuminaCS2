import React from "react";
import DragButton from "../DraggableButton/DragButton";
import "../../styles/lumina-shell.scss";

export interface LuminaHeaderProps {
    version?: string;
}

export const LuminaHeader: React.FC<LuminaHeaderProps> = ({
    version = "3.6.7",
}) => (
    <header className="lumina-header">
        <div className="lumina-header__drag">
            <DragButton />
        </div>
        <div className="lumina-header__brand">
            <div className="lumina-header__logo" />
            <span className="lumina-header__title">Lumina</span>
            <span className="lumina-header__version">v{version}</span>
        </div>
    </header>
);
