import React from "react";
import ReactDOM from "react-dom";

export function ModalDialog(props) {
  return ReactDOM.createPortal(
    <div className="modalDialog">{props.children}</div>,
    document.getElementById("modal-container")
  );
}
