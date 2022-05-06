import {format} from 'date-fns';
import React, {useEffect} from 'react';
import {useTranslation} from 'react-i18next';

import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import QueryBuilderIcon from '@mui/icons-material/QueryBuilder';
import StorageIcon from '@mui/icons-material/Storage';
import {Box, Chip, Grid, Rating, Stack, Tooltip, Typography} from '@mui/material';

import {EsHyperLinkButton} from '../../../shared/components/buttons';
import {EsPoster} from '../../../shared/components/esPoster';
import {EsDetailRowSkeleton, EsFetchFailedRow} from '../../../shared/components/table';
import {useLocale, useMediaServerUrls, useServerType, useShowDetails} from '../../../shared/hooks';
import {calculateFileSize} from '../../../shared/utils';
import {DataLine} from '../../movies/Table/Helpers';
import {MissingEpisodeList} from './MissingEpisodeList';

type Props = {
  id: string;
}

export function ShowDetailRow(props: Props) {
  const {id} = props;
  const {t} = useTranslation();
  const {getBackdropImageLink, getItemDetailLink} = useMediaServerUrls();
  const {locale} = useLocale();
  const {serverType} = useServerType();
  const {loading, show, getShowDetails} = useShowDetails();

  useEffect(() => {
    getShowDetails(id);
  }, [id]);

  if (loading) {
    return (<EsDetailRowSkeleton/>);
  }

  if (show === null) {
    return (<EsFetchFailedRow/>);
  }

  const missingEpisodeCount = (): number => {
    return show.missingSeasons.reduce((p, c) => p + c.episodes.length, 0);
  };

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
          <EsPoster mediaId={show.id} tag={show.primary} />
        </Box>
        <Box sx={{flexGrow: 1, pl: 2}}>
          <Grid
            container
            direction="column"
            spacing={2}>
            <Grid
              item
              spacing={1}
              container
              direction="column"
            >
              <Grid
                item
                spacing={2}
                direction="row"
                container
                alignItems="center"
              >
                <Grid item>
                  <Typography variant="h5">
                    {show.name}
                  </Typography>
                </Grid>
                <Grid item>
                  <Tooltip title={`${show.communityRating}/10`}>
                    <Box>
                      <Rating defaultValue={(show.communityRating ?? 0)/2} precision={0.1} readOnly />
                    </Box>
                  </Tooltip>
                </Grid>
              </Grid>
              <Grid item>
                <Stack spacing={1} direction="row">
                  {show.genres.map((genre) => <Chip label={genre} key={genre} size="small" /> ) }
                </Stack>
              </Grid>
            </Grid>
            <Grid item>
              <Stack direction="row" spacing={2} >
                <Box
                  sx={{
                    minWidth: 200,
                    borderRight: 'solid 1px #aaaaaa',
                  }}>
                  <Grid container direction="column" >
                    <DataLine
                      icon={<StorageIcon />}
                      value={calculateFileSize(show.sizeInMb)}
                      tooltip='COMMON.DISKSPACE'
                    />
                    <DataLine
                      icon={<QueryBuilderIcon />}
                      value={`${show.runTime} ${t('COMMON.MIN')}`}
                      tooltip='COMMON.RUNTIME'
                    />
                    {
                      show.premiereDate && (
                        <DataLine
                          icon={<CalendarTodayIcon />}
                          value={format(new Date(show.premiereDate), 'P', {locale})}
                          tooltip='COMMON.PREMIEREDATE'
                        />
                      )
                    }
                    {
                      show.dateCreated && (
                        <DataLine
                          icon={<CalendarTodayIcon />}
                          value={format(new Date(show.dateCreated), 'P', {locale})}
                          tooltip='COMMON.CREATEDDATE'
                        />
                      )
                    }
                    <Typography>
                      {t('COMMON.SEASONS')}: {show.seasonCount}
                    </Typography>
                    <Typography>
                      {t('COMMON.EPISODES')}: {show.episodeCount} / {show.episodeCount + missingEpisodeCount()}
                      {show.specialEpisodeCount !== 0 ? `+ ${show.specialEpisodeCount}` : ''}
                    </Typography>
                  </Grid>
                </Box>
                <Box>
                  <MissingEpisodeList show={show}/>
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
          <EsHyperLinkButton label={serverType} href={getItemDetailLink(show.id)} />
          <EsHyperLinkButton label="IMDB" href={`https://www.imdb.com/title/${show.imdb}`} />
          <EsHyperLinkButton label="TMDB" href={`https://www.themoviedb.org/tv/${show.tmdb}`} />
          <EsHyperLinkButton label="TVDB" href={`https://www.thetvdb.com/?id=${show.tvdb}&tab=series`} />
        </Stack>
      </Box>
    </Box>
  );
}
