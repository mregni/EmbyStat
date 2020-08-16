import React from 'react';
import Grid from '@material-ui/core/Grid';
import uuid from 'react-uuid';

import { ShowStatistics } from '../../../shared/models/show'
import { TopCard, Card } from '../../../shared/models/common';
import BasicCard from '../../../shared/components/cards/BasicCard';
import TopListCard from '../../../shared/components/cards/TopListCard';
import NoPosterHigh from '../../../shared/assets/images/no-poster.png';
import NoProfileHigh from '../../../shared/assets/images/no-profile-high.png';

interface Props {
  statistics: ShowStatistics;
}

const ShowsGeneral = (props: Props) => {
  const { statistics } = props;

  return (
    <Grid container direction="column" spacing={4}>
      <Grid item container spacing={2}>
        {statistics.cards != null
          ? statistics.cards.map((card: Card) => (
            <Grid item key={card.title}>
              <BasicCard card={card} />
            </Grid>
          )) : null}
        {statistics.people.cards != null
          ? statistics.people.cards.map((card: Card) => (
            <Grid item key={uuid()}>
              <BasicCard card={card} />
            </Grid>
          )) : null}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.topCards != null
          ? statistics.topCards.map((card: TopCard) => (
            <Grid item key={card.title}>
              <TopListCard data={card} fallbackImg={NoPosterHigh} enableBackground />
            </Grid>
          )) : null}
      </Grid>
    </Grid>
  )
}

export default ShowsGeneral
