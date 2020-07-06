import React from 'react';
import { Select, MenuItem } from '@material-ui/core';
import classNames from 'classnames';

import styles from "./style.module.scss";
import { Controller, Control } from 'react-hook-form';

interface Props {
  className?: string,
  value: number,
  menuItems: { id: string | number, value: string | number, label: string }[],
  variant: 'outlined' | 'filled' | 'standard',
  name: string,
  control: Control<Record<string, any>>,
}

const EmbyStatControlledSelect = (props: Props) => {
  const {
    className,
    value,
    menuItems,
    variant,
    name,
    control,
  } = props;

  return (
    <Controller
      as={
        <Select
          autoWidth={false}
          defaultValue={value}
          className={classNames(className, styles.selector)}
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
