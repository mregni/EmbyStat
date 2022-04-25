import {format} from 'date-fns';
import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import AspectRatioIcon from '@mui/icons-material/AspectRatio';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import InboxIcon from '@mui/icons-material/Inbox';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import MusicNoteIcon from '@mui/icons-material/MusicNote';
import QueryBuilderIcon from '@mui/icons-material/QueryBuilder';
import StorageIcon from '@mui/icons-material/Storage';
import SubtitlesIcon from '@mui/icons-material/Subtitles';
import {Box, Chip, Grid, Rating, Stack, Tooltip, Typography} from '@mui/material';

import {EsHyperLinkButton} from '../../../shared/components/buttons';
import {EsPoster} from '../../../shared/components/esPoster';
import {EsDetailRowSkeleton} from '../../../shared/components/table';
import {useLocale, useMediaServerUrls, useServerType} from '../../../shared/hooks';
import {Movie} from '../../../shared/models/movie';
import {getMovieDetails} from '../../../shared/services/movieService';
import {calculateFileSize, generateStreamChipLabel} from '../../../shared/utils';
import {DataLine, MultiString, StreamList} from './Helpers';

type Props = {
  id: string;
}

export function MovieDetailRow(props: Props) {
  const {id} = props;
  const {t} = useTranslation();
  const {getBackdropImageLink, getItemDetailLink} = useMediaServerUrls();
  const [movie, setMovie] = useState<Movie>(null!);
  const {locale} = useLocale();
  const {serverType} = useServerType();

  useEffect(() => {
    const loadMovie = async () => {
      setMovie(await getMovieDetails(id));
    };

    loadMovie();
  }, [id]);

  if (movie === null) {
    return (<EsDetailRowSkeleton/>);
  }

  return (
    <Box
      sx={{
        width: '100%',
        backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), 
      url("${getBackdropImageLink(id)}")`,
        backgroundSize: '100%',
        backgroundPositionY: 'center',
        position: 'relative',
        p: 2,
      }}
    >
      <Box sx={{display: 'flex'}}>
        <Box sx={{flexGrow: 0, width: 200}}>
          <EsPoster mediaId={movie.id} tag={movie.primary} />
        </Box>
        <Box sx={{flexGrow: 1, pl: 2}}>
          <Grid
            container={true}
            direction="column"
            spacing={2}>
            <Grid
              item={true}
              spacing={1}
              container={true}
              direction="column"
            >
              <Grid
                item={true}
                spacing={2}
                direction="row"
                container={true}
                alignItems="center"
              >
                <Grid item={true}>
                  <Typography variant="h5">
                    {movie.originalTitle}
                  </Typography>
                </Grid>
                <Grid item={true}>
                  <Tooltip title={`${movie.communityRating}/10`}>
                    <Box>
                      <Rating defaultValue={(movie.communityRating ?? 0)/2} precision={0.1} readOnly={true} />
                    </Box>
                  </Tooltip>
                </Grid>
              </Grid>
              <Grid item={true}>
                <Stack spacing={1} direction="row">
                  {movie.genres.map((genre) => <Chip label={genre} key={genre} size="small" /> ) }
                </Stack>
              </Grid>
            </Grid>
            <Grid item={true}>
              <Stack direction="row" spacing={2} >
                <Box
                  sx={{
                    minWidth: 200,
                    borderRight: 'solid 1px #aaaaaa',
                  }}>
                  <Grid container={true} direction="column" >
                    <DataLine
                      icon={<StorageIcon />}
                      value={calculateFileSize(movie.mediaSources[0].sizeInMb)}
                      tooltip='COMMON.DISKSPACE'
                    />
                    <DataLine
                      icon={<QueryBuilderIcon />}
                      value={`${movie.runTime} ${t('COMMON.MIN')}`}
                      tooltip='COMMON.RUNTIME'
                    />
                    <DataLine icon={<InboxIcon />} value={movie.container} tooltip='COMMON.CONTAINER' />
                    {
                      movie.premiereDate && (
                        <DataLine
                          icon={<CalendarTodayIcon />}
                          value={format(new Date(movie.premiereDate), 'P', {locale})}
                          tooltip='COMMON.PREMIEREDATE'
                        />
                      )
                    }
                    {
                      movie.dateCreated && (
                        <DataLine
                          icon={<CalendarTodayIcon />}
                          value={format(new Date(movie.dateCreated), 'P', {locale})}
                          tooltip='COMMON.CREATEDDATE'
                        />
                      )
                    }
                  </Grid>
                </Box>
                <Box>
                  <Grid
                    container={true}
                    direction="column"
                    justifyContent="flex-start"
                  >
                    <StreamList icon={<SubtitlesIcon />} list={movie.subtitleStreams} tooltip='COMMON.SUBTITLES' />
                    <StreamList icon={<MusicNoteIcon />} list={movie.audioStreams} tooltip='COMMON.AUDIO' />
                    <MultiString
                      icon={<InsertDriveFileIcon />}
                      list={movie.mediaSources.map((source) => source.path)}
                      tooltip='COMMON.PATHS'
                    />
                    <MultiString
                      icon={<AspectRatioIcon />}
                      list={movie.videoStreams.map((stream) => generateStreamChipLabel(stream))}
                      tooltip='COMMON.STREAMS'
                    />
                  </Grid>
                </Box>
              </Stack>
            </Grid>
          </Grid>
        </Box>
      </Box>
      <Box
        sx={{
          position: 'absolute',
          right: 0,
          top: 0,
        }}>
        <Stack direction="row" spacing={1} sx={{p: 2}}>
          <EsHyperLinkButton label={serverType} href={getItemDetailLink(movie.id)} />
          <EsHyperLinkButton label="IMDB" href={`https://www.imdb.com/title/${movie.imdb}`} />
          <EsHyperLinkButton label="TMDB" href={`https://www.themoviedb.org/movie/${movie.tmdb}`} />
        </Stack>
      </Box>
    </Box>
  );
}
