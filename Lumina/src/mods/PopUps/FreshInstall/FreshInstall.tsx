import React from "react";
import "./FreshInstall.scss";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../../../mod.json";
import LuminaSVG from "../../../img/Lumina.svg";
import { useLocalization } from "cs2/l10n";

 

const freshInstall$ = bindValue<boolean>(
  mod.id,
  "FreshInstall"
);

export const FreshInstall: React.FC = () => {
        //Use localization
      const { translate } = useLocalization();
  const isFreshInstall = useValue(freshInstall$);

  return (
    <div
      className={`fresh-install ${
        isFreshInstall ? "fresh-install--visible" : "fresh-install--hidden"
      }`}
    >
      <div className="fresh-install__logo" aria-hidden="true">
        <img
          className="fresh-install__logo-svg"
          src={LuminaSVG}
          alt=""
        />
      </div>

      <div className="fresh-install__content">
        <p className="fresh-install__message">
          {translate("LUMINA.thankyou") ?? "Thank you for installing Lumina."}
        </p>

        <button
          className="fresh-install__button"
          type="button"
          onClick={() => trigger(mod.id, "StopFreshInstall")}
        >
          {translate("LUMINA.getstarted") ?? "Get Started"}
        </button>
      </div>
    </div>
  );
};