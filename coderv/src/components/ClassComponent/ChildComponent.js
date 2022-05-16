import { string, func } from "prop-types";
import { child } from "./ClassComponent.module.css";

export function ChildComponent({ brand, onChangeBrand }) {
  return (
    <button type="button" className={child} onClick={onChangeBrand}>
      {brand}
    </button>
  );
}

ChildComponent.propTypes = {
  brand: string.isRequired,
  onChangeBrand: func.isRequired,
};
