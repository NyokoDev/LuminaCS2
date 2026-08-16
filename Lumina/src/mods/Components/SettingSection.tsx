import React from "react";
import "../../styles/lumina-controls.scss";

export interface SettingSectionProps {
    title: string;
    description?: string;
    children: React.ReactNode;
    className?: string;
}

export const SettingSection: React.FC<SettingSectionProps> = ({
    title,
    description,
    children,
    className = "",
}) => (
    <section className={`lumina-section ${className}`}>
        <h3 className="lumina-section__title">{title}</h3>
        {description && (
            <p className="lumina-section__desc">{description}</p>
        )}
        {children}
    </section>
);
