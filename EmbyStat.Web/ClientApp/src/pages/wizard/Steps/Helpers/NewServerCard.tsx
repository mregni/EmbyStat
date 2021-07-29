import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import { makeStyles } from '@material-ui/core/styles';
import AddToQueueRoundedIcon from '@material-ui/icons/AddToQueueRounded';

interface Props {
  onClick: () => void;
}

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    height: 80,
    position: 'relative',
    '&:hover': {
      cursor: 'pointer',
    },
    '&:active': {
      boxShadow: theme.shadows[7],
    },
  },
  content: {
    flex: '1 0 auto',
  },
}));

export const NewServerCard = (props: Props) => {
  const { onClick } = props;
  const [elevation, setElevation] = useState(7);
  const classes = useStyles();

  return (
    <Grid item xs={12} sm={6} md={4} lg={3} onClick={onClick}>
      <Zoom in={true} style={{ transitionDelay: '100ms' }}>
        <Card
          elevation={elevation}
          square
          onMouseEnter={() => setElevation(12)}
          onMouseLeave={() => setElevation(7)}
          className={classes.root}
        >
          <Grid container justify="center" alignItems="center">
            <Grid item>
              <AddToQueueRoundedIcon style={{ fontSize: 35 }} />
            </Grid>
          </Grid>
        </Card>
      </Zoom>
    </Grid>
  );
};
