import React from 'react'
import { Grid, makeStyles } from '@material-ui/core';
import moment from 'moment';
import TvRoundedIcon from '@material-ui/icons/TvRounded';
import TheatersRoundedIcon from '@material-ui/icons/TheatersRounded';
import classNames from 'classnames';

import { MovieStatistics } from '../../../shared/models/movie';
import { PersonPoster } from '../../../shared/models/person';
import BasicCard from '../../../shared/components/cards/BasicCard';
import PosterCard from '../../../shared/components/cards/PosterCard';
import { Card } from "../../../shared/models/common";
import MostFeaturesActorPerGenre from '../../../shared/components/tables/MostFeaturesActorPerGenre';

const useStyles = makeStyles((theme) => ({
  'icon--small': {
    height: 10,
    marginTop: 2,
    width: 10,
    marginRight: 2,
    marginLeft: 2,
  },
  tableMargin: {
    [theme.breakpoints.down('lg')]: {
      marginTop: 16,
    },
  }
}));

interface Props {
  statistics: MovieStatistics
}

const GeneralFrame = (props: Props) => {
  const { statistics } = props;
  const classes = useStyles();

  return (
    <>
      <Grid container alignItems="flex-start" >
        <Grid item xs={12} xl={6} container >
          <Grid item container spacing={2}>
            {statistics.people.cards.map((card: Card) =>
              <Grid item>
                <BasicCard card={card} />
              </Grid>
            )}
          </Grid>
          <Grid item container spacing={2} className="m-t-16">
            {statistics.people.posters.map((poster: PersonPoster) =>
              <Grid item>
                <PosterCard
                  mediaId={poster.mediaId}
                  tag={poster.tag}
                  name={poster.name}
                  title={poster.title}
                  height={195}
                  details={
                    <>
                      <TheatersRoundedIcon className={classes['icon--small']} />{poster.movieCount} /
                      <TvRoundedIcon className={classNames("m-l-4", classes['icon--small'])} />{poster.showCount} /
                     {poster.birthDate != null
                        ? <span className="m-l-4">{moment(poster.birthDate).format('l')}</span>
                        : null}
                    </>}
                />
              </Grid>
            )}
          </Grid>
        </Grid>
        <Grid item xs={12} xl={6} container spacing={2} className={classes.tableMargin}>
          <Grid item container>
            <MostFeaturesActorPerGenre people={statistics.people.mostFeaturedActorsPerGenre} />
          </Grid>
        </Grid>
      </Grid>
    </>
  )
}

export default GeneralFrame
