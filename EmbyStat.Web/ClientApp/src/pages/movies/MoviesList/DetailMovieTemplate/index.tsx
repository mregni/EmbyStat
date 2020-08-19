import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import { useSelector } from 'react-redux';
import Ratings from 'react-ratings-declarative';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import StorageRoundedIcon from '@material-ui/icons/StorageRounded';
import QueryBuilderRoundedIcon from '@material-ui/icons/QueryBuilderRounded';
import AspectRatioRoundedIcon from '@material-ui/icons/AspectRatioRounded';
import InboxRoundedIcon from '@material-ui/icons/InboxRounded';
import InsertDriveFileRoundedIcon from '@material-ui/icons/InsertDriveFileRounded';
import SubtitlesRoundedIcon from '@material-ui/icons/SubtitlesRounded';
import MusicNoteRoundedIcon from '@material-ui/icons/MusicNoteRounded';
import PaletteRoundedIcon from '@material-ui/icons/PaletteRounded';

import { RootState } from '../../../../store/RootReducer';
import { getBackdropImageLink, getItemDetailLink } from '../../../../shared/utils/MediaServerUrlUtil';
import PosterCard from '../../../../shared/components/cards/PosterCard';
import theme from '../../../../styles/theme';
import { useServerType } from '../../../../shared/hooks';
import calculateFileSize from '../../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../../shared/utils/CalculateRunTime';
import Flag from '../../../../shared/components/flag';
import DetailMovieSkeleton from './DetailMovieSkeleton';
import { getMovieDetails } from '../../../../shared/services/MovieService';
import { Movie } from '../../../../shared/models/common';

const useStyles = makeStyles((theme) => ({
  container: (props: any) => ({
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url("${props.background}")`,
    backgroundSize: '100% auto',
    backgroundPositionY: 'center',
    position: 'relative',
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
  data: any;
}

const DetailMovieTemplate = (props: Props) => {
  const { data } = props;
  const [movie, setMovie] = useState<Movie>({} as Movie);
  const settings = useSelector((state: RootState) => state.settings);
  const serverType = useServerType();

  useEffect(() => {
    const loadMovie = async () => {
      setMovie(await getMovieDetails(data.data.id));
    };

    loadMovie();
  }, [data]);

  const getPosterUrl = (): string => {
    return getBackdropImageLink(settings, data.data.id);
  };

  const classes = useStyles({ background: getPosterUrl() });
  return movie.id != null ? (
    <div className={classes.container}>
      <Grid container className="p-16">
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
                widgetRatedColors={theme.palette.secondary.main}
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
              xs={12}
              md={6}
              lg={4}
              xl={3}
            >
              <Grid item>
                <p className={classes.movie__genres}>{movie.genres.join(', ')}</p>
              </Grid>
              <Grid item container alignItems="center">
                <StorageRoundedIcon />
                <p className="m-l-8">
                  {calculateFileSize(movie.mediaSources[0].sizeInMb)}
                </p>
              </Grid>
              <Grid item container alignItems="center">
                <QueryBuilderRoundedIcon />
                <p className="m-l-8">
                  {calculateRunTime(movie.mediaSources[0].runTimeTicks)}
                </p>
              </Grid>
              <Grid item container alignItems="center">
                <AspectRatioRoundedIcon />
                <p className="m-l-8">
                  {movie.videoStreams[0]?.height ?? 0}x
              {movie.videoStreams[0]?.width ?? 0}
              &nbsp;({movie.videoStreams[0]?.aspectRatio}) @{' '}
                  {Math.round(movie.videoStreams[0]?.averageFrameRate ?? 0)}fps
            </p>
              </Grid>
              <Grid item container alignItems="center">
                <InboxRoundedIcon />
                <p className="m-l-8">{movie.mediaSources[0].container}</p>
              </Grid>
              <Grid item container alignItems="center">
                <InsertDriveFileRoundedIcon />
                <p className="m-l-8">{movie.mediaSources[0].path}</p>
              </Grid>
              <Grid item container alignItems="center">
                <Grid item>
                  <SubtitlesRoundedIcon />
                </Grid>
                <Grid item container xs>
                  {movie.subtitleStreams
                    .filter((x) => x.language != null && x.language !== 'src')
                    .map((x) =>
                      x.language != null ? (
                        <Grid item key={x.id} className="m-l-8">
                          <Flag language={x.language} isDefault={x.isDefault} />
                        </Grid>
                      ) : null
                    )}
                  {movie.subtitleStreams.some(
                    (x) => x.language == null || x.language === 'src'
                  ) ? (
                      <Grid item className="m-l-8">
                        +
                        {
                          movie.subtitleStreams.filter(
                            (x) => x.language == null || x.language === 'src'
                          ).length
                        }
                      </Grid>
                    ) : null}
                </Grid>
              </Grid>
              <Grid item container alignItems="center">
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
              xs={12}
              md={6}
              lg={4}
              xl={3}
            >
              <Grid item container alignItems="center">
                <InboxRoundedIcon />
                <p className="m-l-8">{movie.mediaSources[0].videoRange}</p>
              </Grid>
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
      <DetailMovieSkeleton />
    );
};

export default DetailMovieTemplate;
