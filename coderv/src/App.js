import './App.css';
//import { PrettyPrintCode } from "components";
import React, { useEffect, useState } from 'react';
import { ClassComponent, A11yHidden } from './components';

const initialDocumentTitle = document.title;
const getRandom = (n) => Math.random() * n;

function getRandomMinMax(min = 0, max = 100) {
  if (min > max) {
    throw new Error('min 값이 max 보다 큽니다.');
  }
  return Math.floor(getRandom(max - min) + min);
}

function RandomCountUp({ min = 0, max = 100, step = 1, fps = 10 }) {
  const [targetCount] = useState(() => getRandomMinMax(min, max));

  useEffect(() => {
    document.title = `(${targetCount}) ${initialDocumentTitle}`;
  }, [targetCount]);

  let [isComplete, setIsComplete] = useState(false);
  let [count, setCount] = useState(0);

  useEffect(() => {
    const timeoutID = setTimeout(() => {
      if (count >= targetCount) {
        setIsComplete(true);
        setCount(targetCount);
      } else {
        setCount(count + step);
      }
    }, fps / 1000);

    return () => clearTimeout(timeoutID);
  }, [count]);

  return (
    <div className="randomCountUp">
      <output style={isComplete ? { animationName: 'none' } : null}>
        {count}
      </output>
    </div>
  );
}

export function COUNT_UP() {
  const [reload, setReload] = React.useState(0);

  return (
    <div className="app">
      <RandomCountUp key={reload} min={45} max={92} step={3} />
      <button
        type="button"
        className="reloadButton"
        onClick={() => setReload(reload + 1)}
        title="다시 실행"
      >
        RELOAD
      </button>
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
  var error = null;

  error = {
    message: '네트워크 오류 발생!',
    log() {
      console.log(this.message);
    },
  };

  //자바스크립트(if문)
  // if (error) {
  //   const displayErrorState = (
  //     <div className="container">
  //       <h1 className="headline">{error.message}</h1>
  //     </div>
  //   );
  //   return displayErrorState;
  // }

  // return (
  //   <div className="container">
  //     <h1 className="headline">React조건부렌더링</h1>
  //     <p>오류가 존재하면 렌더링 되도록 코드를 작성합니다.</p>
  //   </div>
  // );

  //react(if문)
  // return (
  //   <div className="App">
  //     <header class="App-header">
  //       <div class="container">
  //         <h1 class="headline">{!error ? "오류없음" : error.message}</h1>
  //       </div>
  //     </header>
  //   </div>
  // );

  //and/or 연산
  // return (
  //   <div className="container">
  //     <h1 className="headline">
  //       {!error ? "react 조건부 렌더링" : error.message}
  //     </h1>
  //     {!error && <p>오류가 존재하면 렌더링 되도록 코드를 작성합니다.</p>}
  //   </div>
  // );

  //null 병합 연산자 & 옵셔널 체이닝
  if (error === null || error === undefined) {
    console.log('현재 앱에는 오류가 발생하지 않았습니다.');
  }
  error ?? console.log('현재 앱에는 오류가 발생하지 않았습니다.');

  //리스트렌더링
  const db = require('./api/db.json');
  const {
    navigation: { items },
  } = db;

  const renderList = (list) => {
    return list.map(({ link, text }) => (
      <li>
        <a href={link}>{text}</a>
      </li>
    ));
  };

  const ListItem = ({ link, text }) => (
    <li>
      <a href={link}>{text}</a>
    </li>
  );

  window.db = db;

  const PrettyPrintCode = (props) =>
    props.code ? <pre>{JSON.stringify(props.code, null, 2)}</pre> : null;

  // const renderList = (list) =>
  //   list.map((item) => (
  //     <li key={item.text}>
  //       <a href="{item.link}">{item.text}</a>
  //     </li>
  //   ));

  return (
    <div className="container">
      <h1 className="headline">React 리스트 렌더링 (배열)</h1>
      {/* 배열 리스트 렌더링 */}
      <nav className="globalNavigation">
        <ul>
          {items.map(({ link, text }) => (
            <ListItem key={text} link={link} text={text} />
          ))}
        </ul>
      </nav>
      {/* 객체 리스트 렌더링 */}
      {/* <dl className="descriptionList">
        {Object.entries(db).map(([key, value]) => {
          let isStringType = typeof value === "string";
          return (
            <Fragment key={key}>
              <dt>{key}</dt>
              <dd>{isStringType ? value : <PrettyPrintCode code={value} />}</dd>
            </Fragment>
          );
        })}
      </dl> */}
    </div>
  );
}
