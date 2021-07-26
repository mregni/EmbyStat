import React from 'react';
import Grid from '@material-ui/core/Grid';
import uuid from 'react-uuid';

import { MovieStatistics } from '../../shared/models/movie';
import { TopCard, Card } from '../../shared/models/common';
import BasicCard from '../../shared/components/cards/BasicCard';
import TopListCard from '../../shared/components/cards/TopListCard';
import NoProfileHigh from '../../shared/assets/images/no-profile-high.png';
import NoPosterHigh from '../../shared/assets/images/no-poster.png';

interface Props {
  statistics: MovieStatistics;
}

export const MoviesGeneral = (props: Props) => {
  const { statistics } = props;

  return (
    <Grid container direction="column" spacing={4}>
      <Grid item container spacing={2}>
        {statistics.cards != null && statistics.cards.length > 0
          ? statistics.cards.map((card: Card) => (
            <Grid item key={card.title}>
              <BasicCard card={card} />
            </Grid>
          )) : null}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.topCards != null && statistics.topCards.length > 0
          ? statistics.topCards.map((card: TopCard) => (
            <Grid item key={card.title}>
              <TopListCard data={card} fallbackImg={NoPosterHigh} enableBackground />
            </Grid>
          )) : null}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.cards != null && statistics.people.cards.length > 0
          ? statistics.people.cards.map((card: Card) => (
            <Grid item key={uuid()}>
              <BasicCard card={card} />
            </Grid>
          )) : null}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.globalCards != null && statistics.people.globalCards.length > 0
          ? statistics.people.globalCards.map((card: TopCard) => (
            <Grid item key={card.title}>
              <TopListCard data={card} fallbackImg={NoProfileHigh} />
            </Grid>
          )) : null}
      </Grid>
      <Grid item container spacing={2}>
        {statistics.people.mostFeaturedActorsPerGenreCards != null
          && statistics.people.mostFeaturedActorsPerGenreCards.length > 0
          ? statistics.people.mostFeaturedActorsPerGenreCards.map((card: TopCard) => (
            <Grid item key={card.title}>
              <TopListCard data={card} fallbackImg={NoProfileHigh} />
            </Grid>
          )) : null}
      </Grid>
    </Grid>
  );
};
