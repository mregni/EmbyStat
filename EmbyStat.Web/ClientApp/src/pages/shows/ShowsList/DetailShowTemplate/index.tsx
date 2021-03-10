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

import { RootState } from '../../../../store/RootReducer';
import { getBackdropImageLink, getItemDetailLink } from '../../../../shared/utils/MediaServerUrlUtil';
import PosterCard from '../../../../shared/components/cards/PosterCard';
import theme from '../../../../styles/theme';
import { useServerType } from '../../../../shared/hooks';
import calculateFileSize from '../../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../../shared/utils/CalculateRunTime';
import Flag from '../../../../shared/components/flag';
import DetailShowSkeleton from './DetailShowSkeleton';
import { getShowDetails } from '../../../../shared/services/ShowService';
import { Show } from '../../../../shared/models/common';

const useStyles = makeStyles((theme) => ({
  container: (props: any) => ({
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url("${props.background}")`,
    backgroundSize: '100% auto',
    backgroundPositionY: 'center',
    position: 'relative',
  }),
  show__title: {
    fontSize: '2rem',
    marginTop: 8,
    color: 'white',
    '&& p': {
      margin: 0,
    },
  },
  show__links: {
    position: 'absolute',
    top: 8,
    right: 8,
  },
  show__genres: {
    textTransform: 'uppercase',
    fontSize: '0.75rem',
    marginTop: 0,
  },
}));

interface Props {
  data: any;
}

const DetailShowTemplate = (props: Props) => {
  const { data } = props;
  const [show, setShow] = useState<Show>({} as Show);
  const settings = useSelector((state: RootState) => state.settings);
  const serverType = useServerType();

  useEffect(() => {
    const loadShow = async () => {
      setShow(await getShowDetails(data.data.id));
    };

    loadShow();
  }, [data]);

  const getPosterUrl = (): string => {
    return getBackdropImageLink(settings, data.data.id);
  };

  const classes = useStyles({ background: getPosterUrl() });
  return show.id != null ? (
    <div className={classes.container}>
      <Grid container className="p-16">
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
            className={classes.show__title}
          >
            <Grid item>
              <p>{show.name}</p>
            </Grid>
            <Grid item>
                <Ratings
                    rating={(show.communityRating ?? 0) / 2}
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
                <p className={classes.show__genres}>{show.genres.join(', ')}</p>
            </Grid>
            <Grid item container alignItems="center">
                <InsertDriveFileRoundedIcon />
                <p className="m-l-8">{show.path}</p>
            </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </div>
  ) : (
      <DetailShowSkeleton />
    );
};

export default DetailShowTemplate;
