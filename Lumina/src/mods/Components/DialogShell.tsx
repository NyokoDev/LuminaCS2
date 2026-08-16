import React from "react";
import "../../styles/lumina-controls.scss";
import { SettingButton } from "./SettingButton";

export interface DialogShellProps {
    title: string;
    onClose?: () => void;
    children: React.ReactNode;
    footer?: React.ReactNode;
}

export const DialogShell: React.FC<DialogShellProps> = ({
    title,
    onClose,
    children,
    footer,
}) => (
    <div className="lumina-dialog-overlay">
        <div className="lumina-dialog">
            <div className="lumina-dialog__header">
                <span>{title}</span>
                {onClose && (
                    <span className="lumina-dialog__close" onClick={onClose}>
                        ×
                    </span>
                )}
            </div>
            <div className="lumina-dialog__body">{children}</div>
            {footer && (
                <div className="lumina-dialog__footer">{footer}</div>
            )}
        </div>
    </div>
);

export interface DialogActionsProps {
    onCancel: () => void;
    onConfirm?: () => void;
    confirmLabel?: string;
    cancelLabel?: string;
    confirmDisabled?: boolean;
}

export const DialogActions: React.FC<DialogActionsProps> = ({
    onCancel,
    onConfirm,
    confirmLabel = "Confirm",
    cancelLabel = "Cancel",
    confirmDisabled = false,
}) => (
    <>
        <SettingButton onClick={onCancel}>{cancelLabel}</SettingButton>
        {onConfirm && (
            <SettingButton
                variant="primary"
                onClick={onConfirm}
                disabled={confirmDisabled}
            >
                {confirmLabel}
            </SettingButton>
        )}
    </>
);
