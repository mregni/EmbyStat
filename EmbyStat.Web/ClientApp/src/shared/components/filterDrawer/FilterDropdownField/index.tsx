import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';
import { getFilterValues } from '../../../services/FilterService';
import { LabelValuePair } from '../../../models/common';
import Flag from '../../flag';

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
  },
  'pull-up': {
    marginTop: '-3px',
  },
}));

interface Props {
  onValueChanged: (value: string) => void;
  type: FilterType;
  field: string;
  disableAdd: (disable: boolean) => void;
}

const FilterDropdownField = (props: Props) => {
  const { type, field, onValueChanged, disableAdd } = props;
  const [value, setValue] = useState<string>('u');
  const [values, setValues] = useState<LabelValuePair[]>([]);
  const classes = useStyles();
  const { t } = useTranslation();

  useEffect(() => {
    if (type.itemType === 'url') {
      getFilterValues(type.itemUrl, []).then((response) => {
        setValues(response.values);
      });
    }
  }, [type]);

  useEffect(() => {
    disableAdd(value === 'u');
  }, [disableAdd, value]);

  const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
    setValue(event.target.value as string);
    if (type.itemType === 'url') {
      const labelPair = values.filter((x) => x.value === event.target.value);
      if (labelPair.length !== -1) {
        onValueChanged(`${labelPair[0].value}|${labelPair[0].label}`);
        return;
      }
    }

    onValueChanged(`${event.target.value}|${event.target.value}`);
  };

  return (
    <Select style={{ width: '100%' }} onChange={handleChange} value={value}>
      <MenuItem value="u" disabled>
        {t('COMMON.SELECTVALUE')}
      </MenuItem>
      {type.itemType === 'static'
        ? type.items?.map((x) => (
          <MenuItem key={x.value} value={x.value}>
            {x.label}
          </MenuItem>
        ))
        : values.map((x) => (
          <MenuItem key={x.label} value={x.value}>
            {field === 'Subtitles' ? (
              <Grid container alignItems="center">
                <Grid item>
                  <Flag
                    language={x.value}
                    width={25}
                    height={25}
                    className="m-r-8"
                  />
                </Grid>
                <Grid item className={classes['pull-up']}>
                  {x.label}
                </Grid>
              </Grid>
            ) : (
                x.label
              )}
          </MenuItem>
        ))}
    </Select>
  );
};

export default FilterDropdownField;
