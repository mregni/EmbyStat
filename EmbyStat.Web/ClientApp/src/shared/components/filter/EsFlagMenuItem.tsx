import React from 'react';

import {Stack} from '@mui/material';

import {LabelValuePair} from '../../models/common';
import {EsFlag} from '../esFlag';

type EsFlagMenuItemprops = {
  item: LabelValuePair;
}

export const EsFlagMenuItem = (props: EsFlagMenuItemprops) => {
  const {item} = props;
  return (
    <Stack
      alignItems='center'
      direction='row'
    >
      <EsFlag
        language={item.value}
        height={18}
      />
      {item.label}
    </Stack>
  );
};
