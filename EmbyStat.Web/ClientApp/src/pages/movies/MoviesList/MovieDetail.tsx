import React, { useState, useEffect, useContext } from 'react';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import Ratings from 'react-ratings-declarative';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import StorageRoundedIcon from '@material-ui/icons/StorageRounded';
import QueryBuilderRoundedIcon from '@material-ui/icons/QueryBuilderRounded';
import AspectRatioRoundedIcon from '@material-ui/icons/AspectRatioRounded';
import InboxRoundedIcon from '@material-ui/icons/InboxRounded';
import InsertDriveFileRoundedIcon from '@material-ui/icons/InsertDriveFileRounded';
import SubtitlesRoundedIcon from '@material-ui/icons/SubtitlesRounded';
import MusicNoteRoundedIcon from '@material-ui/icons/MusicNoteRounded';
import Chip from '@material-ui/core/Chip';

import { getBackdropImageLink, getItemDetailLink } from '../../../shared/utils/MediaServerUrlUtil';
import PosterCard from '../../../shared/components/cards/PosterCard';
import theme from '../../../styles/theme';
import { useServerType } from '../../../shared/hooks';
import calculateFileSize from '../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../shared/utils/CalculateRunTime';
import Flag from '../../../shared/components/flag';
import { MovieDetailSkeleton } from './MovieDetailSkeleton';
import { getMovieDetails } from '../../../shared/services/MovieService';
import { Movie } from '../../../shared/models/common';
import { MovieRow } from '../../../shared/models/movie';
import { SettingsContext } from '../../../shared/context/settings';
import generateStreamChipLabel from '../../../shared/utils/GenerateVideoStreamLabel';
import { MultipleItem } from '../../../shared/components/detailComponents';
import { Typography } from '@material-ui/core';

const useStyles = makeStyles(() => ({
  container: (props: any) => ({
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url("${props.background}")`,
    backgroundSize: '100%',
    backgroundPositionY: 'center',
    position: 'relative',
    padding: 16
  }),
  movie__title: {
    fontSize: '2rem',
    marginTop: 8,
    color: 'white',
    '&& p': {
      margin: 0,
    },
  },
  movie__links: {
    position: 'absolute',
    top: 8,
    right: 8,
  },
  movie__genres: {
    textTransform: 'uppercase',
    fontSize: '0.75rem',
    marginTop: 0,
  },
}));

interface Props {
  data: MovieRow;
}

export const DetailMovieTemplate = (props: Props) => {
  const { data } = props;
  const [movie, setMovie] = useState<Movie>({} as Movie);
  const { settings } = useContext(SettingsContext);
  const serverType = useServerType();

  useEffect(() => {
    const loadMovie = async () => {
      setMovie(await getMovieDetails(data.id));
    };

    loadMovie();
  }, [data]);

  const getPosterUrl = (): string => {
    return getBackdropImageLink(settings, data.id);
  };

  const classes = useStyles({ background: getPosterUrl() });
  return movie.id != null ? (
    <div >
      <Grid container className={classes.container}>
        <Grid item className="m-r-16">
          <PosterCard
            mediaId={movie.id}
            tag={movie.primary}
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
            className={classes.movie__title}
          >
            <Grid item>
              <p>{movie.originalTitle}</p>
            </Grid>
            <Grid item>
              <Ratings
                rating={(movie.communityRating ?? 0) / 2}
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
          <Grid
            item
            container
            direction="row"
          >
            <Grid
              item
              container
              direction="column"
              spacing={1}
              xs={12}
              md={6}
              lg={4}
              xl={3}
            >
              <Grid item>
                <Grid container spacing={1}>
                  {movie.genres.map(genre =>
                    <Grid key={genre} item>
                      <Chip label={genre} size="small"></Chip>
                    </Grid>)}
                </Grid>
              </Grid>
              <Grid item container alignItems="flex-start" spacing={1}>
                <Grid item>
                  <StorageRoundedIcon />
                </Grid>
                <Grid item>
                  <Typography>{calculateFileSize(movie.mediaSources[0].sizeInMb)}</Typography>
                </Grid>
              </Grid>
              <Grid item container alignItems="flex-start" spacing={1}>
                <Grid item>
                  <QueryBuilderRoundedIcon />
                </Grid>
                <Grid item>
                  <Typography>{calculateRunTime(movie.runTimeTicks)}</Typography>
                </Grid>
              </Grid>
              <Grid item container alignItems="flex-start" spacing={1}>
                <Grid item>
                  <InboxRoundedIcon />
                </Grid>
                <Grid item>
                  <Typography>{movie.container}</Typography>
                </Grid>
              </Grid>
              <Grid item container alignItems="flex-start" spacing={1}>
                <Grid item>
                  <SubtitlesRoundedIcon />
                </Grid>
                <Grid item container xs spacing={1}>
                  {movie.subtitleStreams
                    .filter((x) => x.language != null && x.language !== 'src')
                    .map((x) =>
                      x.language != null ? (
                        <Grid item key={x.id}>
                          <Flag language={x.language} isDefault={x.isDefault} />
                        </Grid>
                      ) : null
                    )}
                  {movie.subtitleStreams.some(
                    (x) => x.language == null || x.language === 'src'
                  ) ? (
                    <Grid item>
                      <Chip size="small" label={`+${movie.subtitleStreams.filter((x) => x.language == null || x.language === 'src').length}`}></Chip>
                    </Grid>
                  ) : null}
                </Grid>
              </Grid>
              <Grid item container alignItems="flex-start">
                <Grid item>
                  <MusicNoteRoundedIcon />
                </Grid>
                <Grid item container xs>
                  {movie.audioStreams
                    .filter((x) => x.language != null && x.language !== 'src')
                    .map((x) =>
                      x.language != null ? (
                        <Grid item key={x.id} className="m-l-8">
                          <Flag language={x.language} isDefault={x.isDefault} />
                        </Grid>
                      ) : null
                    )}
                  {movie.audioStreams.some(
                    (x) => x.language == null || x.language === 'src'
                  ) ? (
                    <Grid item className="m-l-8">
                      +
                      {
                        movie.audioStreams.filter(
                          (x) => x.language == null || x.language === 'src'
                        ).length
                      }
                    </Grid>
                  ) : null}
                </Grid>
              </Grid>
            </Grid>
            <Grid
              item
              container
              direction="column"
              spacing={1}
              xs={12}
              md={6}
              lg={8}
            >
              <MultipleItem
                items={movie.mediaSources.map(source => source.path)}
                icon={<InsertDriveFileRoundedIcon />}
              />
              <MultipleItem
                items={movie.videoStreams.map(stream => generateStreamChipLabel(stream))}
                icon={<AspectRatioRoundedIcon />}
              />
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid
        container
        justify="flex-end"
        spacing={2}
        className={classes.movie__links}
      >
        <Grid item>
          <Button
            variant="outlined"
            color="secondary"
            size="small"
            target="_blank"
            href={getItemDetailLink(settings, movie.id)}
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
            href={`https://www.imdb.com/title/${movie.imdb}`}
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
            href={`https://www.themoviedb.org/movie/${movie.tmdb}`}
            startIcon={<OpenInNewIcon />}
          >
            TMDB
          </Button>
        </Grid>
      </Grid>
    </div>
  ) : (
    <MovieDetailSkeleton />
  );
};
