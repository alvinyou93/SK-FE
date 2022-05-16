/* eslint-disable no-unused-vars */

import { parent } from './ClassComponent.module.css';
import React, { Component } from 'react';
import { NestedComponent } from './NestedComponent';
import {
  string,
  number,
  oneOfType,
  oneOf,
  node,
  elementType,
  shape,
  arrayOf,
} from 'prop-types';

export class ClassComponent extends Component {
  static defaultProps = {
    as: 'div',
  };

  static propTypes = {
    as: oneOfType([string, node, elementType]),
  };

  state = {
    brand: 'euid',
  };

  changeBrand = (e) => {
    this.setState(({ brand }) => ({
      brand: brand.includes('euid') ? 'google' : 'euid',
    }));
  };

  render() {
    return (
      <div className={parent}>
        <span>{this.state.brand}</span>
        <NestedComponent
          brand={this.state.brand}
          onChangeBrand={this.changeBrand}
        />
      </div>
    );
  }
}
