import { string, func } from 'prop-types';
import { ChildComponent } from './ChildComponent';

export const NestedComponent = ({ brand, onChangeBrand }) => (
  <>
    <h2
      style={{
        color: '#fff',
      }}
    >
      Nested Component
    </h2>
    <ChildComponent brand={brand} onChangeBrand={onChangeBrand} />
  </>
);

NestedComponent.defaultProps = {
  brand: 'euid',
};

NestedComponent.propTypes = {
  brand: string,
  onChangeBrand: func.isRequired,
};
