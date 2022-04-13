import {Grid, Chip, Tooltip} from '@mui/material';
import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

type MultiStringProps = {
  list: string[];
  icon: ReactElement;
  tooltip: string;
}

export const MultiString = (props: MultiStringProps): React.ReactElement => {
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
          {list.map((item) => <Grid item key={item}><Chip size='small' label={item}></Chip></Grid>)}
        </Grid>
      </Grid>
    </Grid>
  );
};
