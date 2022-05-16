import { ClassComponent, A11yHidden } from "./components";
import logo from "./logo.svg";
import "./App.css";

export function COUNT_UP() {
  return (
    <div className="App">
      <header className="App-header"></header>
    </div>
  );
}

export function BUTTON_COUNT() {
  return (
    <div className="App">
      <header className="App-header">
        <ClassComponent />
        <A11yHidden>Hey</A11yHidden>
      </header>
    </div>
  );
}

export function JSX_IN_ACTION() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}
