import React, { useEffect, useState, useCallback, useContext } from 'react'
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import Ratings from 'react-ratings-declarative';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useTranslation } from 'react-i18next';
import StorageRoundedIcon from '@material-ui/icons/StorageRounded';
import QueryBuilderRoundedIcon from '@material-ui/icons/QueryBuilderRounded';
import Chip from '@material-ui/core/Chip';

import { ShowRow } from '../../../shared/models/show';
import { useServerType } from '../../../shared/hooks';
import { getBackdropImageLink, getItemDetailLink } from '../../../shared/utils/MediaServerUrlUtil';
import { Show } from '../../../shared/models/common';
import { getShowDetails } from '../../../shared/services/ShowService';
import theme from '../../../styles/theme';
import PosterCard from '../../../shared/components/cards/PosterCard';
import calculateFileSize from '../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../shared/utils/CalculateRunTime';
import { ShowDetailSkeleton } from './ShowDetailSkeleton';
import { SettingsContext } from '../../../shared/context/settings';

const useStyles = makeStyles(() => ({
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
  }
}));

interface Props {
  data: ShowRow
}

export const ShowDetail = (props: Props) => {
  const { data } = props;
  const { t } = useTranslation();
  const [show, setShow] = useState<Show | null>(null);
  const { settings } = useContext(SettingsContext);
  const serverType = useServerType();

  const loadShow = useCallback(async () => {
    setShow(await getShowDetails(data.id));
  }, [data.id]);

  useEffect(() => {
    loadShow();
  }, [data, loadShow]);

  const getPosterUrl = (): string => {
    return getBackdropImageLink(settings, data.id);
  };

  const classes = useStyles({ background: getPosterUrl() });
  if (show === null || show === undefined) {
    return (<ShowDetailSkeleton />);
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
            <Grid container spacing={1}>
              {show.genres.map(genre =>
                <Grid key={genre} item>
                  <Chip label={genre} size="small"></Chip>
                </Grid>)
              }
            </Grid>
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
                <p>{t('SHOWS.TOTALEPISODES')}: {show.episodeCount}</p>
                <p>{t('SHOWS.TOTALMISSINGEPISODES')}: {show.missingSeasons.flatMap(x => x.episodes).length}</p>
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
                show.missingSeasons.map(season => (
                  season.episodes.map(episode => (
                    <Grid item key={`${season.indexNumber}${episode.indexNumber}`}>
                      <p>S{season.indexNumber < 10 && "0"}{season.indexNumber}xE{episode.indexNumber < 10 && "0"}{episode.indexNumber} - {episode.name}</p>
                    </Grid>
                  ))
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
