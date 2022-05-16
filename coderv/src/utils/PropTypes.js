export const PropTypes = {
  string(props, propName, ComponentName) {
    let value = props[propName];
    let valueType = typeof value;
    if (valueType !== 'string') {
      throw new Error(
        `${ComponentName} 컴포넌트에 전달된 ${propName} prop의 기대 타입은 string이지만, 전달된 값의 타입은 ${valueType}입니다.`
      );
    }
  },
  func(props, propName, ComponentName) {
    let value = props[propName];
    let valueType = typeof value;
    if (valueType !== 'function') {
      throw new Error(
        `${ComponentName} 컴포넌트에 전달된 ${propName} prop의 기대 타입은 function이지만, 전달된 값의 타입은 ${valueType}입니다.`
      );
    }
  },
};
