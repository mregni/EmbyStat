import React, {useState} from 'react';

import AddToQueueIcon from '@mui/icons-material/AddToQueue';
import {Card, Grid, Zoom} from '@mui/material';

interface Props {
  onClick: () => void;
}


export function NewServerCard(props: Props) {
  const {onClick} = props;
  const [elevation, setElevation] = useState(7);

  return (
    <Grid item xs={12} sm={6} md={4} lg={3} onClick={onClick}>
      <Zoom in style={{transitionDelay: '100ms'}}>
        <Card
          elevation={elevation}
          square
          onMouseEnter={() => setElevation(12)}
          onMouseLeave={() => setElevation(7)}
          sx={{
            'display': 'flex',
            'height': 80,
            'position': 'relative',
            '&:hover': {
              cursor: 'pointer',
            },
            '&:active': {
              boxShadow: (theme) => theme.shadows[7],
            },
          }}
        >
          <Grid container justifyContent="center" alignItems="center">
            <Grid item>
              <AddToQueueIcon style={{fontSize: 35}} />
            </Grid>
          </Grid>
        </Card>
      </Zoom>
    </Grid>
  );
}
