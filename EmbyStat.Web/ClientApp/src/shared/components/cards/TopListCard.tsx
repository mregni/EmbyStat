import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Zoom from '@material-ui/core/Zoom';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import uuid from 'react-uuid';
import moment from 'moment';
import classNames from 'classnames';

import { useSelector } from 'react-redux';
import { TopCard, TopCardItem } from '../../models/common';
import { RootState } from '../../../store/RootReducer';
import getFullMediaServerUrl from '../../utils/GetFullMediaServerUtil';

const useStyles = makeStyles((theme) => ({
  container: {
    width: 375,
    height: 138,
  },
  container__background: (props: any) => ({
    backgroundImage: `linear-gradient( rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.7)), url(${props.backdrop})`,
    backgroundPosition: 'top',
    backgroundSize: 'cover',
    backgroundRepeat: 'no-repeat',
  }),
  poster: {
    height: 138,
    '& img': {
      borderTopLeftRadius: 4,
      borderBottomLeftRadius: 4,
    },
  },
  info: {
    marginLeft: 8,
    marginTop: 8,
    width: 'calc(100% - 108px)',
  },
  title: {
    marginBottom: 16,
    textTransform: 'uppercase',
    fontWeight: 700,
  },
  details: {
    fontSize: '0.8rem',
    fontStyle: 'italic',
  },
  secondaryColor: {
    color: theme.palette.secondary.main,
    fontWeight: 700,
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
  fallbackImg: string;
  enableBackground: boolean;
}

const TopListCard = (props: Props) => {
  const { data, fallbackImg, enableBackground } = props;
  const { t } = useTranslation();
  const [hoveredItem, setHoveredItem] = useState<TopCardItem>(data.values[0]);

  const settings = useSelector((state: RootState) => state.settings);
  const getBackdropUrl = (): string => {
    const fullAddress = getFullMediaServerUrl(settings);
    return `${fullAddress}/emby/Items/${hoveredItem.mediaId}/Images/Backdrop?EnableImageEnhancers=false`;
  };

  const classes = useStyles({ backdrop: getBackdropUrl() });

  const getPosterUrl = (): string => {
    const fullAddress = getFullMediaServerUrl(settings);
    return `${fullAddress}/emby/Items/${hoveredItem.mediaId}/Images/Primary?maxHeight=200&tag=${hoveredItem.image}&quality=90&enableimageenhancers=false`;
  };

  const calculateTime = (date: string): string => {
    return moment(date).format('l');
  };

  const calculateMinutes = (ticks: string): number => {
    return Math.round(parseInt(ticks) / 600000000);
  };

  const addDefaultSrc = (e) => {
    e.target.src = fallbackImg
  }

  return (
    <Zoom in={true}>
      <Paper elevation={5} className={classNames(classes.container, { [classes.container__background]: enableBackground })}>
        <Grid container direction="row">
          <Grid item className={classes.poster}>
            <img
              src={getPosterUrl()}
              alt="poster"
              width="92"
              height="138"
              onError={addDefaultSrc} />
          </Grid>
          <Grid item className={classes.info} container direction="column">
            <Grid
              item
              container
              justify="space-between"
              className={classes.title}
            >
              <Grid item>{t(data.title)}</Grid>
              <Grid item className={classes.secondaryColor}>
                {data.unitNeedsTranslation ? t(data.unit) : data.unit}
              </Grid>
            </Grid>
            {data.values.map((pair) => (
              <Grid
                item
                container
                justify="space-between"
                key={uuid()}
                className={classes.details}
                onMouseOver={() => setHoveredItem(pair)}
              >
                <Grid item className={classes.link}>
                  <a
                    href={`${getFullMediaServerUrl(
                      settings
                    )}/web/index.html#!/item?id=${pair.mediaId}&serverId=${
                      settings.mediaServer.serverId
                      }`}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    {pair.label.length > 35 ? t(pair.label).substr(0, 32) + '...' : t(pair.label)}
                  </a>
                </Grid>
                <Grid item className={classes.secondaryColor}>
                  {data.valueType === 0 ? pair.value : null}
                  {data.valueType === 1 ? calculateMinutes(pair.value) : null}
                  {data.valueType === 2 ? calculateTime(pair.value) : null}
                </Grid>
              </Grid>
            ))}
          </Grid>
        </Grid>
      </Paper>
    </Zoom>
  );
};

TopListCard.defaultProps = {
  enableBackground: false,
}

export default TopListCard;
