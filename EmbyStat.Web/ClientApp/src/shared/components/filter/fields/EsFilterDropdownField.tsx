import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, LinearProgress, MenuItem, Select, SelectChangeEvent} from '@mui/material';

import {EsFlagMenuItem} from '..';
import {LabelValuePair} from '../../../models/common';
import {FilterOperation} from '../../../models/filter';
import {getFilterValues} from '../../../services/filterService';

type Props = {
  onValueChanged: (val0: string) => void;
  operation: FilterOperation;
}

export function EsFilterDropdownField(props: Props) {
  const {onValueChanged, operation} = props;
  const [value, setValue] = useState<string>('u');
  const [values, setValues] = useState<LabelValuePair[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const {t} = useTranslation();

  useEffect(() => {
    if (operation.itemType === 'url') {
      setIsLoading(true);
      getFilterValues(operation.itemUrl).then((response) => {
        setValues(response.values);
        setIsLoading(false);
      });
    }
  }, [operation]);

  const handleChange = ((event: SelectChangeEvent<string>) => {
    setValue(event.target.value);
    if (operation.itemType === 'url') {
      const labelPair = values.filter((x) => x.value === event.target.value);
      if (labelPair.length !== -1) {
        onValueChanged(`${labelPair[0].value}|${labelPair[0].label}`);
        return;
      }
    }

    onValueChanged(`${event.target.value}|${event.target.value}`);
  });

  if (isLoading) {
    return (
      <Box sx={{width: 100, pt: '27px'}}>
        <LinearProgress variant='indeterminate' />
      </Box>
    );
  }

  return (
    <Select
      onChange={handleChange}
      value={value}
      variant="standard"
      size='small'
    >
      <MenuItem value="u" disabled={true}>
        {t('COMMON.SELECTVALUE')}
      </MenuItem>
      {operation.itemType === 'static' ?
        operation.items?.map((x) => (
          <MenuItem key={x.value} value={x.value}>
            {x.label}
          </MenuItem>
        )) :
        values.map((x) => (
          <MenuItem key={x.label} value={x.value}>
            {operation.itemUrl?.includes('subtitle') ?
              ( <EsFlagMenuItem item={x} /> ) :
              ( x.label )}
          </MenuItem>
        ))}
    </Select>
  );
}

