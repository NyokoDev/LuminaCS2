import { useEffect } from "react";

export const FixPanelStyles: React.FC = () => {
  useEffect(() => {
    const fixStyles = () => {
      const els = document.querySelectorAll<HTMLElement>('.center-panel-layout_X2p.k45_we_mainPanel');
      console.log(`[FixPanelStyles] Found ${els.length} element(s) with target classes.`);

      els.forEach((el, index) => {
        const beforeZIndex = el.style.zIndex || window.getComputedStyle(el).zIndex;
        const beforePointerEvents = el.style.pointerEvents || window.getComputedStyle(el).pointerEvents;

        el.style.zIndex = "1";
        el.style.pointerEvents = "all";

        const afterZIndex = el.style.zIndex;
        const afterPointerEvents = el.style.pointerEvents;

        console.log(`[FixPanelStyles] Element #${index + 1}:`, el);
        console.log(`  z-index: before='${beforeZIndex}', after='${afterZIndex}'`);
        console.log(`  pointer-events: before='${beforePointerEvents}', after='${afterPointerEvents}'`);
      });
    };

    fixStyles();

    const observer = new MutationObserver(() => {
      fixStyles();
    });

    observer.observe(document.body, { childList: true, subtree: true });

    return () => {
      observer.disconnect();
      console.log("[FixPanelStyles] Mutation observer disconnected.");
    };
  }, []);

  return null;
};
