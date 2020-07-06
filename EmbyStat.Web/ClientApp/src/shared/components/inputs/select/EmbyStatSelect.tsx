import React, { ChangeEvent, ReactNode } from 'react';
import { Select, MenuItem } from '@material-ui/core';
import classNames from 'classnames';

import styles from "./style.module.scss";

interface Props {
  className?: string,
  defaultValue?: string,
  onChange: ((event: ChangeEvent<{ name?: string | undefined; value: unknown; }>, child: ReactNode) => void),
  value: string | number,
  menuItems: { id: string | number, value: string | number, label: string }[],
  variant: 'outlined' | 'filled' | 'standard',
}

const EmbyStatSelect = (props: Props) => {
  const {
    className,
    onChange,
    value,
    menuItems,
    variant,
  } = props;

  return (
    <Select
      {...(value !== undefined && { value })}
      onChange={onChange}
      autoWidth={false}
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
    </Select >
  )
}

EmbyStatSelect.defaultProps = {
  variant: 'outlined',
  className: '',
} as Partial<Props>;

export default EmbyStatSelect
