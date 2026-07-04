import React from 'react';
import UploadSVG from './../../img/folder.png';
import { trigger } from 'cs2/api';
import mod from "./../../../mod.json";
import './UploadFileButton.scss';

type OpenFileDialogButtonProps = {
    onFileLoaded?: (text: string) => void;
};

export const OpenFileDialogButton: React.FC<OpenFileDialogButtonProps> = ({
    onFileLoaded
}) => {

    const handleClick = async () => {
        console.log('Open file dialog');

        // Your existing trigger (opens CS2 file picker)
        const result = trigger(mod.id, 'UploadLUTFileDialog');
        
        if (onFileLoaded && typeof result === "string") {
            onFileLoaded(result);
        }
    };

    return (
        <button
            onClick={handleClick}
            className="UploadFileButton"
            style={{ border: 'none', background: 'none', cursor: 'pointer' }}
        >
            <img src={UploadSVG} alt="Upload" />
        </button>
    );
};