import React from 'react';
import Grid from '@material-ui/core/Grid';

import { MovieStatistics } from '../../../shared/models/movie';
import { TopCard, Card } from '../../../shared/models/common';
import BasicCard from '../../../shared/components/cards/BasicCard';
import TopListCard from '../../../shared/components/cards/TopListCard';

interface Props {
  statistics: MovieStatistics;
}

const MoviesGeneral = (props: Props) => {
  const { statistics } = props;

  return (
    <>
      <Grid container spacing={2}>
        {statistics.cards.map((card: Card) => (
          <Grid item key={card.title}>
            <BasicCard card={card} />
          </Grid>
        ))}
      </Grid>
      <Grid container spacing={2} className="m-t-16">
        {statistics.topCards.map((card: TopCard) => (
          <Grid item key={card.title}>
            <TopListCard data={card} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default MoviesGeneral;
