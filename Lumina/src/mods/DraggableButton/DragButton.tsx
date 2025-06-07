import React, { useRef, useState } from 'react';

const DragButton: React.FC = () => {
  const dragging = useRef(false);
  const lastPos = useRef({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);

  const handleMouseDown = (e: React.MouseEvent) => {
    dragging.current = true;
    setIsDragging(true);
    lastPos.current = { x: e.clientX, y: e.clientY };

    const panel = document.getElementById('Global');
    if (panel) {
      // Remove transition while dragging for instant movement
      panel.style.transition = 'none';

      // Ensure panel has absolute position
      const computedStyle = window.getComputedStyle(panel);
      if (computedStyle.position !== 'absolute' && computedStyle.position !== 'fixed') {
        panel.style.position = 'absolute';
      }

      // Initialize top/left if empty or non-pixel values
      if (!panel.style.top || !panel.style.top.endsWith('px')) panel.style.top = panel.offsetTop + 'px';
      if (!panel.style.left || !panel.style.left.endsWith('px')) panel.style.left = panel.offsetLeft + 'px';
    }

    window.addEventListener('mousemove', handleMouseMove);
    window.addEventListener('mouseup', handleMouseUp);
  };

  const handleMouseMove = (e: MouseEvent) => {
    if (!dragging.current) return;

    const dx = e.clientX - lastPos.current.x;
    const dy = e.clientY - lastPos.current.y;
    lastPos.current = { x: e.clientX, y: e.clientY };

    const panel = document.getElementById('Global');
    if (panel) {
      const style = panel.style;
      const currentTop = parseInt(style.top || '0', 10);
      const currentLeft = parseInt(style.left || '0', 10);

      panel.style.top = `${currentTop + dy}px`;
      panel.style.left = `${currentLeft + dx}px`;
    }
  };

  const handleMouseUp = () => {
    dragging.current = false;
    setIsDragging(false);

    const panel = document.getElementById('Global');
    if (panel) {
      // Add smooth transition back once drag ends
      panel.style.transition = 'top 0.5s ease, left 0.5s ease';
    }

    window.removeEventListener('mousemove', handleMouseMove);
    window.removeEventListener('mouseup', handleMouseUp);
  };

  return (
    <div
      onMouseDown={handleMouseDown}
      style={{
        position: 'absolute',
        top: '700rem', // original rem position
        left: '160rem', // original rem position
        transform: 'translateX(-50%)',
        width: '56px',
        height: '56px',
        borderRadius: '14px',
        backgroundColor: 'rgba(255, 255, 255, 0.15)', // transparent white almost gray
        backdropFilter: 'blur(10px)',
        WebkitBackdropFilter: 'blur(10px)',
        border: '1px solid rgba(255, 255, 255, 0.3)',
        boxShadow: '0 8px 32px 0 rgba(31, 38, 135, 0.1)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        cursor: isDragging ? 'grabbing' : 'grab',
        userSelect: 'none',
        transition: 'box-shadow 0.3s ease, background-color 0.3s ease',
        zIndex: 1000,
      }}
      onMouseLeave={handleMouseUp}
      onMouseUp={handleMouseUp}
    >
<svg
  xmlns="http://www.w3.org/2000/svg"
  height="24"
  width="40"  // wider to fill more horizontal space
  viewBox="0 0 40 24"
  fill="rgba(255,255,255,0.8)"
  style={{ filter: 'drop-shadow(0 0 1px rgba(0,0,0,0.1))' }}
>
  {/* 3 horizontal bars */}
  <rect x="0" y="4" width="40" height="3" rx="1.5" />
  <rect x="0" y="10.5" width="40" height="3" rx="1.5" />
  <rect x="0" y="17" width="40" height="3" rx="1.5" />
</svg>
    </div>
  );
};

export default DragButton;
