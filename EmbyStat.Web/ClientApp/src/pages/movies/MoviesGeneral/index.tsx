import React from 'react'
import { Grid } from '@material-ui/core';

import { MovieStatistics } from '../../../shared/models/movie';
import { TopCard } from '../../../shared/models/common';
import BasicCard from '../../../shared/components/cards/BasicCard';
import TopListCard from '../../../shared/components/cards/TopListCard';
import { Card } from "../../../shared/models/common";

interface Props {
  statistics: MovieStatistics
}

const MoviesGeneral = (props: Props) => {
  const { statistics } = props;

  return (
    <>
      <Grid container spacing={2}>
        {statistics.cards.map((card: Card) =>
          <Grid item key={card.title}>
            <BasicCard card={card} />
          </Grid>
        )}
      </Grid>
      <Grid container spacing={2} className="m-t-16">
        {
          statistics.topCards.map((card: TopCard) =>
            <Grid item>
              <TopListCard data={card} />
            </Grid>
          )
        }
        {/* {statistics.posters.map((poster: MoviePoster) =>
          <Grid item>

            <PosterCard
              mediaId={poster.mediaId}
              tag={poster.tag}
              name={poster.name}
              title={poster.title}
              details={
                <>
                  {poster.durationMinutes != null ? `${poster.durationMinutes} min / ` : null}
                  {poster.officialRating != null ? `${poster.officialRating} / ` : null}
                  {poster.year != null && poster.year > 0 ? `${poster.year} / ` : null}
                  {poster.communityRating != null ? `${poster.communityRating}` : null}
                </>}
            />
          </Grid>
        )} */}
      </Grid>
    </>
  )
}

export default MoviesGeneral
