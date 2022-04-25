import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Tooltip, Typography} from '@mui/material';

type DataLineProps = {
  icon: ReactElement;
  value: string | number;
  tooltip: string;
}

export function DataLine(props: DataLineProps) {
  const {icon, value, tooltip} = props;
  const {t} = useTranslation();

  return (
    <Grid container={true} item={true} alignItems="flex-start" spacing={1}>
      <Grid item={true}>
        <Tooltip title={t(tooltip) ?? ''}>
          {icon}
        </Tooltip>
      </Grid>
      <Grid item={true}>
        <Typography>{value}</Typography>
      </Grid>
    </Grid>
  );
}
