import React, { ChangeEvent, ReactNode } from 'react';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import classNames from 'classnames';

import { Controller, Control } from 'react-hook-form';
import styles from './style.module.scss';

interface Props {
  className?: string;
  value: number;
  menuItems: { id: string | number; value: string | number; label: string }[];
  variant: 'outlined' | 'filled' | 'standard';
  name: string;
  control: Control<Record<string, any>>;
  onChange: (
    event: ChangeEvent<{ name?: string | undefined; value: unknown }>,
    child: ReactNode
  ) => void;
}

const EmbyStatControlledSelect = (props: Props) => {
  const { className, value, menuItems, variant, name, control, onChange } = props;

  return (
    <Controller
      as={
        <Select
          autoWidth={false}
          defaultValue={value}
          className={classNames(className, styles.selector)}
          variant={variant}
          onChange={onChange}
        >
          {menuItems.map((x) => (
            <MenuItem key={x.id} value={x.value}>
              {x.label}
            </MenuItem>
          ))}
        </Select>
      }
      name={name}
      control={control}
      defaultValue={value}
    />
  );
};

EmbyStatControlledSelect.defaultProps = {
  variant: 'outlined',
  className: '',
} as Partial<Props>;

export default EmbyStatControlledSelect;
