import { Chip, Grid } from '@material-ui/core'
import React, { ReactNode } from 'react'

interface Props {
  items: string[];
  icon: ReactNode;
}

export const MultipleItem = (props: Props): React.ReactElement => {
  const { icon, items } = props;
  return (
    <Grid item container alignItems="flex-start" spacing={1}>
      <Grid item>
        {icon}
      </Grid>
      <Grid item>
        <Grid container spacing={1} direction="column">
          {items.map(item => <Grid item key={item}><Chip size='small' label={item}></Chip></Grid>)}
        </Grid>
      </Grid>
    </Grid>
  )
}