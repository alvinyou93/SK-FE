import { Component } from 'react';

export class Container extends Component {
  constructor(props) {
    super(props);

    this.state = {
      count: props.count ?? 20,
      loading: true,
      error: null,
      makeupData: null,
    };
  }

  static getDerivedStateFromProps(props, state) {
    return {
      isComplete: state.count >= props.targetCount,
    };
  }

  shouldComponentUpdate(nextProps, nextState) {
    return true;
  }

  render() {
    const { loading, error, makeupData, isComplete } = this.state;

    // console.log('render: ', document.querySelector('.Container button'));

    // console.log(loading);

    console.log(makeupData);

    if (loading) {
      return <div role="alert">로딩 중...</div>;
    }

    if (error) {
      return <div role="alert">{error.message}</div>;
    }

    return (
      <div className="Container">
        <output style={{ animationName: isComplete ? 'none' : null }}>
          <button type="button" onClick={this.handleClick}>
            {makeupData.title}
          </button>
        </output>
      </div>
    );
  }

  handleClick = (e) => {
    this.setState({
      count: this.state.count + 1,
      isComplete: true,
    });
  };

  handler = () => {};

  async fetchMakeupData() {
    try {
      const response = await fetch(
        'https://jsonplaceholder.typicode.com/todos/1'
      );
      const json = await response.json();

      this.setState({
        loading: false,
        makeupData: json,
      });
    } catch (error) {
      this.setState({
        loading: false,
        error,
      });
    }
  }

  componentDidMount() {
    this.fetchMakeupData();

    // console.log('mounted container component');
    // side effects
    // DOM 접근/조작
    // 외부 라이브러리 코드 활용 (예: jQuery)
    // 데이터 패치 (서버 통신)
  }

  // getSnapshotBeforeUpdate(prevProps, prevState) {
  //   return {
  //     width: document.querySelector('.Container button').getBoundingClientRect()
  //       .height,
  //   };
  // }

  componentDidUpdate(prevProps, prevState, snapshot) {
    // console.log('updated');

    if (snapshot) {
      // UI 튕기는 문제 해결
    }
  }

  componentWillUnmount() {
    // console.log('component unmount');
  }
}
