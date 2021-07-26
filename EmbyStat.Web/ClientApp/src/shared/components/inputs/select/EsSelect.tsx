import React, { ChangeEvent, ReactNode } from 'react';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import classNames from 'classnames';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
  selector: {
    width: '100%',
  },
}));
interface Props {
  className?: string;
  defaultValue?: string;
  onChange: (
    event: ChangeEvent<{ name?: string | undefined; value: unknown }>,
    child: ReactNode
  ) => void;
  value: string | number;
  menuItems: { id: string | number; value: string | number; label: string }[];
  variant: 'outlined' | 'filled' | 'standard';
}

const EmbyStatSelect = (props: Props) => {
  const { className, onChange, value, menuItems, variant } = props;
  const classes = useStyles();

  return (
    <Select
      {...(value !== undefined && { value })}
      onChange={onChange}
      autoWidth={false}
      className={classNames(className, classes.selector)}
      variant={variant}
    >
      {menuItems.map((x) => (
        <MenuItem key={x.id} value={x.value}>
          {x.label}
        </MenuItem>
      ))}
    </Select>
  );
};

EmbyStatSelect.defaultProps = {
  variant: 'outlined',
  className: '',
} as Partial<Props>;

export default EmbyStatSelect;
