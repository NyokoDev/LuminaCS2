// SafeColorField.tsx
import React, { useEffect, useState } from "react";
import { Color } from "cs2/bindings";
import { VanillaComponentResolver } from "./VanillaComponentResolver";

type SafeColorFieldProps = {
  value: Color;
  onChange: (color: Color) => void;
  className?: string;
};

export const SafeColorField: React.FC<SafeColorFieldProps> = ({
  value,
  onChange,
  className = "",
}) => {
  const [ColorField, setColorField] = useState<
    ((props: SafeColorFieldProps) => JSX.Element) | null
  >(null);

  useEffect(() => {
    const interval = setInterval(() => {
      const instance = VanillaComponentResolver.instance;
      if (instance && typeof instance.ColorField === "function") {
        setColorField(() => instance.ColorField);
        clearInterval(interval);
      }
    }, 100);

    return () => clearInterval(interval);
  }, []);

  if (!ColorField) return <div className={className}>Loading color picker...</div>;

  return <ColorField value={value} onChange={onChange} className={className} />;
};
