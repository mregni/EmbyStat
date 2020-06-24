import React, { ChangeEvent, ReactNode } from 'react';
import { Select, MenuItem } from '@material-ui/core';

import styles from "./style.module.scss";
import { Controller, Control } from 'react-hook-form';

interface Props {
  className?: string,
  value: number,
  menuItems: { id: string | number, value: string | number, label: string }[],
  variant: 'outlined' | 'filled' | 'standard',
  name: string,
  control: Control<Record<string, any>>,
  onChange: any,
}

const EmbyStatControlledSelect = (props: Props) => {
  const {
    className,
    value,
    menuItems,
    variant,
    name,
    control,
    onChange,
  } = props;

  console.log(name + " " + value);

  const interalClassNames = className?.split(' ') ?? [];
  interalClassNames.push(styles.selector);

  return (
    <Controller
      as={
        <Select
          autoWidth={false}
          defaultValue={value}
          className={interalClassNames.join(' ')}
          variant={variant}>
          {
            menuItems.map((x) => (
              <MenuItem
                key={x.id}
                value={x.value}>
                {x.label}
              </MenuItem>
            ))
          }
        </Select>}
      name={name}
      control={control}
      defaultValue={value}
    />
  )
}

EmbyStatControlledSelect.defaultProps = {
  variant: 'outlined',
  className: '',
} as Partial<Props>;

export default EmbyStatControlledSelect
