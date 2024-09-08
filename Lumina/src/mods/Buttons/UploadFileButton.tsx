import React from 'react';
import UploadSVG from './../../img/upload-file-svgrepo-com.svg';
import { trigger } from 'cs2/api';
import mod from "./../../../mod.json";
import './UploadFileButton.scss'

export const OpenFileDialogButton = () => {
    const handleClick = () => {
        // Logic to open file dialog or handle the file upload
        console.log('Open file dialog');
        trigger(mod.id,'UploadLUTFileDialog')
    };

    return (
        <button onClick={handleClick}
            className='UploadFileButton'
            style={{ border: 'none', background: 'none', cursor: 'pointer' }}>
            <img src={UploadSVG} alt="Upload" />
        </button>
    );
};
