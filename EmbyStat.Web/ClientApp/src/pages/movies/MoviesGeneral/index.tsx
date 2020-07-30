import React from 'react';
import Grid from '@material-ui/core/Grid';
import uuid from 'react-uuid';

import { MovieStatistics } from '../../../shared/models/movie';
import { TopCard, Card } from '../../../shared/models/common';
import BasicCard from '../../../shared/components/cards/BasicCard';
import TopListCard from '../../../shared/components/cards/TopListCard';
import NoProfileHigh from '../../../shared/assets/images/no-profile-high.png';
import NoPosterHigh from '../../../shared/assets/images/no-poster.png';

interface Props {
  statistics: MovieStatistics;
}

const MoviesGeneral = (props: Props) => {
  const { statistics } = props;

  return (
    <Grid container direction="column" spacing={4}>
      <Grid item container spacing={2}>
        {statistics.cards.map((card: Card) => (
          <Grid item key={card.title}>
            <BasicCard card={card} />
          </Grid>
        ))}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.topCards.map((card: TopCard) => (
          <Grid item key={card.title}>
            <TopListCard data={card} fallbackImg={NoPosterHigh} enableBackground />
          </Grid>
        ))}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.cards.map((card: Card) => (
          <Grid item key={uuid()}>
            <BasicCard card={card} />
          </Grid>
        ))}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.globalCards.map((card: TopCard) => (
          <Grid item key={card.title}>
            <TopListCard data={card} fallbackImg={NoProfileHigh} />
          </Grid>
        ))}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.mostFeaturedActorsPerGenreCards.map((card: TopCard) => (
          <Grid item key={card.title}>
            <TopListCard data={card} fallbackImg={NoProfileHigh} />
          </Grid>
        ))}
      </Grid>
    </Grid>
  );
};

export default MoviesGeneral;
