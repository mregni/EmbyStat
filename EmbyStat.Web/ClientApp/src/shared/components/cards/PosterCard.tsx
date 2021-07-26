import React, { ReactNode, useContext } from 'react';
import Paper from '@material-ui/core/Paper';
import Zoom from '@material-ui/core/Zoom';
import Button from '@material-ui/core/Button';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import { useSelector } from 'react-redux';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';
import { RootState } from '../../../store/RootReducer';

import { getItemDetailLink, getPrimaryImageLink } from '../../utils/MediaServerUrlUtil';
import { SettingsContext } from '../../context/settings';

const useStyles = makeStyles((theme) => ({
  card: {
    width: 200,
    height: 300,
  },
  card__container: {
    backgroundSize: 'contain',
    width: '100%',
    height: 300,
    zIndex: 10,
    display: 'block',
    borderRadius: 4,
  },
  'card__container--mask': {
    background:
      '-webkit-linear-gradient(top,  rgba(255,255,255,0) 38%,rgba(255,255,255,0.06) 39%,rgba(30,27,38,0.88) 53%,rgba(30,27,38,1) 55%)',
  },
  poster: (props: any) => ({
    width: '100%',
    height: '100%',
    backgroundImage: `url(${props.poster})`,
    backgroundPosition: 'top',
    backgroundSize: 'cover',
    backgroundRepeat: 'no-repeat',
    zIndex: 111,
    borderRadius: 4,
    position: 'relative',
  }),
  'poster--mask': (props: any) => ({
    height: props.height,
    '-webkit-mask-image': `-webkit-gradient(linear, left top, left bottom, 
    color-stop(0.00,  rgba(0,0,0,1)),
    color-stop(0.35,  rgba(0,0,0,1)),
    color-stop(0.50,  rgba(0,0,0,1)),
    color-stop(0.65,  rgba(0,0,0,1)),
    color-stop(0.85,  rgba(0,0,0,0.6)),
    color-stop(1.00,  rgba(0,0,0,0)))`,
  }),
  card__text: (props: any) => ({
    padding: 10,
    height: 300 - props.height,
  }),
  card__name: {
    fontWeight: 700,
    fontSize: '0.9rem',
  },
  card__title: {
    textTransform: 'uppercase',
    fontWeight: 300,
    fontSize: '0.75rem',
  },
  card__details: {
    fontSize: '0.7rem',
    marginBottom: 4,
    color: theme.palette.grey[600],
    fontWeight: 500,
  },
}));

interface Props {
  mediaId: string;
  tag: string;
  name?: string;
  title?: string;
  height?: number;
  details?: ReactNode;
  noGradient?: boolean;
  noOpen?: boolean;
}

const PosterCard = (props: Props) => {
  const {
    mediaId,
    tag,
    name = null,
    title = null,
    details = null,
    height = 175,
    noGradient = false,
    noOpen = false,
  } = props;
  const { t } = useTranslation();
  const { settings } = useContext(SettingsContext);
  const getPosterUrl = (): string => {
    return getPrimaryImageLink(settings, mediaId, tag);
  };

  const classes = useStyles({ poster: getPosterUrl(), height });

  const openMovie = () => {
    window.open(getItemDetailLink(settings, mediaId), '_blank');
  };

  return (
    <Zoom in={true}>
      <Paper elevation={5} className={classes.card}>
        <div
          className={classNames(classes.card__container, {
            [classes['card__container--mask']]: !noGradient,
          })}
        >
          <div
            className={classNames(classes.poster, {
              [classes['poster--mask']]: !noGradient,
            })}
          />
          <Grid
            container
            className={classes.card__text}
            direction="column"
            justify="space-between"
          >
            <Grid item container direction="column">
              {title != null ? (
                <Grid item className={classes.card__title}>
                  {t(title)}
                </Grid>
              ) : null}
              {name != null ? (
                <Grid item className={classes.card__name}>
                  {name}
                </Grid>
              ) : null}
            </Grid>
            <Grid item container direction="column">
              {details != null ? (
                <Grid
                  item
                  container
                  alignItems="center"
                  className={classes.card__details}
                >
                  {details}
                </Grid>
              ) : null}
              <Grid item>
                {!noOpen ? (
                  <Button
                    variant="outlined"
                    color="secondary"
                    size="small"
                    startIcon={<OpenInNewIcon />}
                    onClick={() => openMovie()}
                  >
                    {t('COMMON.OPEN')}
                  </Button>
                ) : null}
              </Grid>
            </Grid>
          </Grid>
        </div>
      </Paper>
    </Zoom>
  );
};

export default PosterCard;
