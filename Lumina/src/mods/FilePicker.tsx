import React, { useState, ChangeEvent } from 'react';

function FilePicker() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      setSelectedFile(file);
      // Do something with the selected file, like parsing it
    }
  };

  return (
    <div>
      <input 
        type="file" 
        onChange={handleFileChange} 
        accept="application/xml,text/xml" 
      />
      {selectedFile && <p>Selected file: {selectedFile.name}</p>}
    </div>
  );
}

export default FilePicker;
