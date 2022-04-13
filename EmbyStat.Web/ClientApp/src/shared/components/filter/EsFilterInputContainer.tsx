import React from 'react';

import {FilterOperation} from '../../models/filter';
import {EsFilterNumberField, EsFilterRangeField, EsFilterTextField} from './';
import {EsFilterDropdownField} from './fields/EsFilterDropdownField';

type EsFilterInputContainerProps = {
  operation: FilterOperation;
  errors: any;
  register: Function;
  onValueChanged: (value: string) => void;
}

export const EsFilterInputContainer = (props: EsFilterInputContainerProps) => {
  const {operation, errors, register, onValueChanged} = props;

  switch (operation.type) {
  case 'text':
    return (<EsFilterTextField
      errors={errors}
      register={register}
      onValueChanged={onValueChanged}
    />);
  case 'number':
    return (<EsFilterNumberField
      errors={errors}
      register={register}
      onValueChanged={onValueChanged}
      unit={operation.unit}
    />);
  case 'range':
    return (<EsFilterRangeField
      errors={errors}
      register={register}
      onValueChanged={onValueChanged}
      unit={operation.unit}
    />);
  case 'dropdown':
    return (<EsFilterDropdownField onValueChanged={onValueChanged} operation={operation} />);
  case 'none':
  default: return <></>;
  }
};
