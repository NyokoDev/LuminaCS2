import React, { useState } from "react";
import "./Update.scss";
import SSAOPreview from "./SSAOPreview.png";

export const SSAOPopupWrapper: React.FC = () => {
  const [showUpdate, setShowUpdate] = useState(true);

  return (
    <div className="SSAOPopupWrapper">
      {showUpdate && <SSAOPopup onClose={() => setShowUpdate(false)} />}
    </div>
  );
};

export const SSAOPopup: React.FC<{ onClose: () => void }> = ({ onClose }) => {
  const [visible, setVisible] = useState(true);

  const handleClose = () => {
    setVisible(false);
    onClose();
  };

  if (!visible) return null;

  return (
    <div className="SSAOPopup">
      <img src={SSAOPreview} className="ImagePreview" />

      <div className="FeatureTitle">
        SSAO Now Available
      </div>

      <div className="FeatureText">
        Lumina now supports Screen Space Ambient Occlusion (SSAO).
        This improves depth perception and shadow realism across your
        city, giving buildings and terrain more visual definition.
      </div>

      <button className="UnderstoodButton" onClick={handleClose}>
        Understood
      </button>
    </div>
  );
};