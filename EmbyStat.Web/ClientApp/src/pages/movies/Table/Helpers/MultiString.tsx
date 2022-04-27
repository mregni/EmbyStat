import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

import {Chip, Grid, Tooltip} from '@mui/material';

type MultiStringProps = {
  list: string[];
  icon: ReactElement;
  tooltip: string;
}

export function MultiString(props: MultiStringProps): React.ReactElement {
  const {icon, list, tooltip} = props;
  const {t} = useTranslation();

  return (
    <Grid item container alignItems="flex-start" spacing={1}>
      <Grid item>
        <Tooltip title={t(tooltip) ?? ''}>
          {icon}
        </Tooltip>
      </Grid>
      <Grid item>
        <Grid container spacing={1} direction="column">
          {list.map((item) => <Grid item key={item}><Chip size='small' label={item} /></Grid>)}
        </Grid>
      </Grid>
    </Grid>
  );
}
