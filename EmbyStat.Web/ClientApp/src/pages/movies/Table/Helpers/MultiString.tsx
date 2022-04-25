import {Grid, Chip, Tooltip} from '@mui/material';
import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

type MultiStringProps = {
  list: string[];
  icon: ReactElement;
  tooltip: string;
}

export function MultiString(props: MultiStringProps): React.ReactElement {
  const {icon, list, tooltip} = props;
  const {t} = useTranslation();

  return (
    <Grid item={true} container={true} alignItems="flex-start" spacing={1}>
      <Grid item={true}>
        <Tooltip title={t(tooltip) ?? ''}>
          {icon}
        </Tooltip>
      </Grid>
      <Grid item={true}>
        <Grid container={true} spacing={1} direction="column">
          {list.map((item) => <Grid item={true} key={item}><Chip size='small' label={item} /></Grid>)}
        </Grid>
      </Grid>
    </Grid>
  );
}
