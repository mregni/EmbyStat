import React, {useContext} from 'react';

import {Box, Grid} from '@mui/material';

import {EsBasicCard} from '../../../shared/components/cards';
import {MediaServerUserContext} from '../../../shared/context/mediaServerUser';
import {Card} from '../../../shared/models/common';

type Props = {}

export function EsUserStatistics(props: Props) {
  const {loaded, statistics} = useContext(MediaServerUserContext);

  if (!loaded) {
    return (null);
  }

  return (
    <Box>
      <Grid container={true} spacing={2}>
        {statistics?.cards != null && statistics.cards.length > 0 ?
          statistics.cards.map((card: Card) => (
            <Grid item={true} key={card.title} xs={12} sm={6} md={4} lg={3} xl={2}>
              <EsBasicCard card={card} />
            </Grid>
          )) : null}
      </Grid>
    </Box>
  );
}
