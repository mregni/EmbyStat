import React from 'react'
import { useTranslation } from 'react-i18next'
import { Zoom, Paper, Grid, makeStyles, Typography } from '@material-ui/core';
import uuid from 'react-uuid';
import moment from 'moment';

import { TopCard } from '../../models/common';
import { useSelector } from 'react-redux';
import { RootState } from '../../../store/RootReducer';
import getFullMediaServerUrl from '../../utils/GetFullMediaServerUtil';

const useStyles = makeStyles((theme) => ({
  container: (props: any) => ({
    width: 375,
    height: 138,
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.3), rgba(0, 0, 0, 0.6)), url(${props.backdrop})`,
    backgroundPosition: 'top',
    backgroundSize: 'cover',
    backgroundRepeat: 'no-repeat',
  }),
  poster: {
    height: 138,
    '& img': {
      borderTopLeftRadius: 4,
      borderBottomLeftRadius: 4,
    }
  },
  info: {
    marginLeft: 8,
    marginTop: 8,
    width: 'calc(100% - 108px)',
  },
  title: {
    marginBottom: 16,
    textTransform: 'uppercase',
    fontWeight: 700
  },
  details: {
    fontSize: '0.8rem',
    fontStyle: 'italic',
  },
  secondaryColor: {
    color: theme.palette.secondary.main,
  },
  link: {
    cursor: 'pointer',
    '&:hover': {
      color: theme.palette.secondary.main,
    },
    '& a': {
      color: 'inherit',
      textDecoration: 'none',
    },
  },
}));

interface Props {
  data: TopCard;
}

const TopListCard = (props: Props) => {
  const { t } = useTranslation();
  const { data } = props;

  const settings = useSelector((state: RootState) => state.settings);
  const getBackdropUrl = (): string => {
    const fullAddress = getFullMediaServerUrl(settings);
    console.log(`${fullAddress}/emby/Items/${data.mediaId}/Images/Backdrop?EnableImageEnhancers=false`);
    return `${fullAddress}/emby/Items/${data.mediaId}/Images/Backdrop?EnableImageEnhancers=false`;
  }

  const classes = useStyles({ backdrop: getBackdropUrl() });

  const getPosterUrl = (): string => {
    const fullAddress = getFullMediaServerUrl(settings);
    return `${fullAddress}/emby/Items/${data.mediaId}/Images/Primary?maxHeight=200&tag=${data.image}&quality=90&enableimageenhancers=false`;
  }

  const calculateTime = (date: string): string => {
    return moment(date).format('l');
  }

  const calculateMinutes = (ticks: string): number => {
    return Math.round(parseInt(ticks) / 600000000);
  }

  return (
    <Zoom in={true}>
      <Paper elevation={5} className={classes.container}>
        <Grid container direction="row">
          <Grid item className={classes.poster}>
            <img src={getPosterUrl()} alt="poster" width="92" height="138" />
          </Grid>
          <Grid item className={classes.info} container direction="column">
            <Grid item container justify="space-between" className={classes.title}>
              <Grid item>
                {t(data.title)}
              </Grid>
              <Grid item className={classes.secondaryColor}>
                {data.unitNeedsTranslation ? t(data.unit) : data.unit}
              </Grid>
            </Grid>
            {
              data.values.map(pair =>
                <Grid item container justify="space-between" key={uuid()} className={classes.details}>
                  <Grid item className={classes.link}>
                    <a href={`${getFullMediaServerUrl(settings)}/web/index.html#!/item?id=${data.mediaId}&serverId=${settings.mediaServer.serverId}`}
                      target="_blank"
                      rel="noopener noreferrer">
                      {t(pair.label)}
                    </a>
                  </Grid>
                  <Grid item className={classes.secondaryColor}>
                    {data.valueType === 0 ? pair.value : null}
                    {data.valueType === 1 ? calculateMinutes(pair.value) : null}
                    {data.valueType === 2 ? calculateTime(pair.value) : null}
                  </Grid>
                </Grid>
              )
            }
          </Grid>
        </Grid>
      </Paper>
    </Zoom>
  )
}

export default TopListCard;