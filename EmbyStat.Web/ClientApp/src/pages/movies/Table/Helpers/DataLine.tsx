import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Tooltip, Typography} from '@mui/material';

type DataLineProps = {
  icon: ReactElement;
  value: string | number;
  tooltip: string;
}

export const DataLine = (props: DataLineProps) => {
  const {icon, value, tooltip} = props;
  const {t} = useTranslation();

  return (
    <Grid container item alignItems="flex-start" spacing={1}>
      <Grid item>
        <Tooltip title={t(tooltip) ?? ''}>
          {icon}
        </Tooltip>
      </Grid>
      <Grid item>
        <Typography>{value}</Typography>
      </Grid>
    </Grid>
  );
};
