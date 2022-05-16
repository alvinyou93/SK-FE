import "react-app-polyfill/ie11";
import "react-app-polyfill/stable";

import { StrictMode } from "react";
import { render } from "react-dom";
import "./index.css";
import { COUNT_UP, BUTTON_COUNT, JSX_IN_ACTION } from "./App";

render(
  <StrictMode>
    <COUNT_UP />
    <BUTTON_COUNT />
    <JSX_IN_ACTION />
  </StrictMode>,
  document.getElementById("root")
);
