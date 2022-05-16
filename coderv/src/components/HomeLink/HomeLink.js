import React from 'react';
import { ReactComponent as LogoSvg } from 'assets/logo.svg';

export function HomeLink() {
  return (
    <a href="/">
      <LogoSvg title="EUID" width={82} height={30} />
    </a>
  );
}
