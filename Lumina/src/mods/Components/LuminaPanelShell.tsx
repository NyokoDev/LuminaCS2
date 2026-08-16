import React from "react";
import { LuminaHeader } from "./LuminaHeader";
import { LuminaNav } from "./LuminaNav";
import { LuminaTabId } from "./types";
import "../../styles/lumina-shell.scss";

export interface LuminaPanelShellProps {
    activeTab: LuminaTabId;
    onTabChange: (tab: LuminaTabId) => void;
    translate: (key: string) => string;
    children: React.ReactNode;
    dialogs?: React.ReactNode;
    version?: string;
}

export const LuminaPanelShell: React.FC<LuminaPanelShellProps> = ({
    activeTab,
    onTabChange,
    translate,
    children,
    dialogs,
    version,
}) => (
    <div className="lumina-panel-shell" id="Global">
        <LuminaHeader version={version} />
        <div className="lumina-body">
            <LuminaNav
                activeTab={activeTab}
                onTabChange={onTabChange}
                translate={translate}
            />
            <main className="lumina-content">{children}</main>
        </div>
        {dialogs}
    </div>
);
