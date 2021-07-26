import React, { useEffect, useState, useCallback, useContext } from 'react'
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import Ratings from 'react-ratings-declarative';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useTranslation } from 'react-i18next';
import StorageRoundedIcon from '@material-ui/icons/StorageRounded';
import QueryBuilderRoundedIcon from '@material-ui/icons/QueryBuilderRounded';

import { ShowRow } from '../../../../shared/models/show';
import { useServerType } from '../../../../shared/hooks';
import { getBackdropImageLink, getItemDetailLink } from '../../../../shared/utils/MediaServerUrlUtil';
import { Show } from '../../../../shared/models/common';
import { getShowDetails } from '../../../../shared/services/ShowService';
import theme from '../../../../styles/theme';
import PosterCard from '../../../../shared/components/cards/PosterCard';
import calculateFileSize from '../../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../../shared/utils/CalculateRunTime';
import DetailMovieSkeleton from '../../../movies/MoviesList/DetailMovieTemplate/DetailMovieSkeleton';
import { SettingsContext } from '../../../../shared/context/settings';

const useStyles = makeStyles((theme) => ({
  container: (props: any) => ({
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url("${props.background}")`,
    backgroundSize: '100%',
    backgroundPositionY: 'center',
    position: 'relative',
    padding: 16
  }),
  title: {
    fontSize: '2rem',
    color: 'white',
    '&& p': {
      margin: 0,
    },
  },
  links: {
    position: 'absolute',
    top: 8,
    right: 8,
  },
  genres: {
    textTransform: 'uppercase',
    fontSize: '0.75rem',
    marginTop: 0,
  },
}));

interface Props {
  data: ShowRow
}

export const DetailShowTemplate = (props: Props) => {
  const { data } = props;
  const { t } = useTranslation();
  const [show, setShow] = useState<Show | null>(null);
  const { settings } = useContext(SettingsContext);
  const serverType = useServerType();

  const loadShow = useCallback(async () => {
    setShow(await getShowDetails(data.id));
  }, []);

  useEffect(() => {
    loadShow();
  }, [data, loadShow]);

  const getPosterUrl = (): string => {
    return getBackdropImageLink(settings, data.id);
  };

  const classes = useStyles({ background: getPosterUrl() });
  if (show === null || show == undefined) {
    return (<DetailMovieSkeleton />);
  }

  return (
    <div >
      <Grid container className={classes.container}>
        <Grid item className="m-r-16">
          <PosterCard
            mediaId={show.id}
            tag={show.primary}
            noGradient
            noOpen
          />
        </Grid>
        <Grid
          item
          container
          direction="column"
          xs
          spacing={1}
          justify="flex-start"
        >
          <Grid
            item
            container
            spacing={2}
            alignItems="center"
            className={classes.title}
          >
            <Grid item>
              <p>{show.name}</p>
            </Grid>
            <Grid item>
              <Ratings
                rating={(show.communityRating ?? 0) / 2}
                widgetRatedColors={theme.palette.primary.main}
                widgetEmptyColors="black"
                widgetDimensions="20px"
                widgetSpacings="3px"
              >
                <Ratings.Widget />
                <Ratings.Widget />
                <Ratings.Widget />
                <Ratings.Widget />
                <Ratings.Widget />
              </Ratings>
            </Grid>
          </Grid>
          <Grid item>
            <p className={classes.genres}>{show.genres.join(', ')}</p>
          </Grid>
          <Grid
            item
            container
            direction="row"
          >
            <Grid
              item
              container
              direction="column"
              xs={12}
              md={6}
              lg={4}
              xl={3}
            >

              <Grid item>
                <p>{t('SHOWS.TOTALEPISODES')}: {show.collectedEpisodeCount}</p>
                <p>{t('SHOWS.TOTALMISSINGEPISODES')}: {show.missingEpisodes.length}</p>
                <p>{t('SHOWS.TOTALSPECIALEPISODES')}: {show.specialEpisodeCount}</p>
              </Grid>
              <Grid item container alignItems="center">
                <StorageRoundedIcon />
                <p className="m-l-8">
                  {calculateFileSize(show.sizeInMb)}
                </p>
              </Grid>
              <Grid item container alignItems="center">
                <QueryBuilderRoundedIcon />
                <p className="m-l-8">
                  {calculateRunTime(show.runTimeTicks)}
                </p>
              </Grid>
            </Grid>
            <Grid
              item
              container
              direction="column"
              xs={12}
              md={6}
              lg={8}
            >
              {
                show.missingEpisodes.map(x => (
                  <Grid item key={x.id}>
                    <p>S{x.seasonNumber < 10 && "0"}{x.seasonNumber}xE{x.episodeNumber < 10 && "0"}{x.episodeNumber} - {x.name}</p>
                  </Grid>
                ))
              }
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid
        container
        justify="flex-end"
        spacing={2}
        className={classes.links}
      >
        <Grid item>
          <Button
            variant="outlined"
            color="secondary"
            size="small"
            target="_blank"
            href={getItemDetailLink(settings, show.id)}
            startIcon={<OpenInNewIcon />}
          >
            {serverType}
          </Button>
        </Grid>
        <Grid item>
          <Button
            variant="outlined"
            color="secondary"
            size="small"
            target="_blank"
            href={`https://www.imdb.com/title/${show.imdb}`}
            startIcon={<OpenInNewIcon />}
          >
            IMDB
        </Button>
        </Grid>
        <Grid item>
          <Button
            variant="outlined"
            color="secondary"
            size="small"
            target="_blank"
            href={`https://www.themoviedb.org/movie/${show.tmdb}`}
            startIcon={<OpenInNewIcon />}
          >
            TMDB
        </Button>
        </Grid>
      </Grid>
    </div>
  )
}
