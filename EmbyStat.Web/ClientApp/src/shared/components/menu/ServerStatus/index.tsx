import React, { useState, useEffect } from 'react'
import Grid from '@material-ui/core/Grid'
import { useSelector } from 'react-redux';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import Button from '@material-ui/core/Button';
import classNames from 'classnames';
import { useTranslation, Trans } from 'react-i18next';
import { useTransition, config, animated } from 'react-spring'

import { RootState } from '../../../../store/RootReducer';
import { useServerType } from '../../../../shared/hooks';
import getFullMediaServerUrl from '../../../utils/MediaServerUrlUtil';
import GreenCircle from '../../../../shared/assets/images/circle-green.png';
import OrangeCircle from '../../../../shared/assets/images/circle-orange.png';
import RedCircle from '../../../../shared/assets/images/circle-red.png';

const useStyles = makeStyles((theme) => ({
  status__text: {
    fontSize: "0.8rem",
    color: theme.palette.grey[400],
    marginTop: 3,
  },
  second__column: {
    width: 'calc( 100% - 28px)',
  },
  first__column: {
    width: 25,
  }
}));

interface Props {

}

const ServerStatus = (props: Props) => {
  const classes = useStyles();
  const serverType = useServerType();
  const { t } = useTranslation();

  const status = useSelector((state: RootState) => state.serverStatus);
  const settings = useSelector((state: RootState) => state.settings);

  const [index, openPage] = useState(0);
  const [image, setImage] = useState(OrangeCircle);
  const pages = [
    ({ style }) => <animated.div style={{ ...style }}><img src={image} alt="server missing" width="20" height="20" /></animated.div>,
    ({ style }) => <animated.div style={{ ...style }}></animated.div>,
  ];
  const transitions = useTransition(index, p => p, {
    from: { position: 'absolute', opacity: 0 },
    enter: { opacity: 1 },
    leave: { opacity: 0 },
    config: config.default,
  });

  useEffect(() => {
    const interval = setInterval(() => {
      openPage(index === 0 ? 1 : 0);
    }, 700);
    return () => clearInterval(interval);
  }, [index]);

  useEffect(() => {
    status.missedPings < 2 ? setImage(OrangeCircle) : setImage(RedCircle);
  }, [status.missedPings])

  return (
    <Grid container direction="column" className="p-16">
      <Grid item>
        <Typography variant="h6" color="secondary">
          <Trans i18nKey="STATUS.TITLE" values={{ type: serverType }} />
        </Typography>
      </Grid>
      {status.missedPings === 0
        ? <Grid item container direction="row" align-items="center" spacing={1}>
          <Grid item>
            <img src={GreenCircle} alt="server online" width="20" height="20" />
          </Grid>
          <Grid item container direction="column" className={classes.second__column}>
            <Grid item className={classNames(classes.status__text, classes.second__column)}>
              {t('STATUS.ONLINE')}
            </Grid>
            <Grid item className={classNames(classes.status__text, classes.second__column, "wordwrap")}>
              {getFullMediaServerUrl(settings)}
            </Grid>
          </Grid>
        </Grid>
        : <Grid item container direction="row" align-items="center" spacing={1}>
          <Grid item className={classes.first__column}>
            {transitions.map(({ item, props, key }) => {
              const Page = pages[item]
              return <Page key={key} style={props} />
            })}
          </Grid>
          <Grid item container direction="column" className={classes.second__column}>
            <Grid item className={classNames(classes.status__text, classes.second__column)}>
              <Trans i18nKey="STATUS.OFFLINE" values={{ count: status.missedPings }} />
            </Grid>
            <Grid item className={classNames(classes.status__text, classes.second__column, "wordwrap")}>
              {getFullMediaServerUrl(settings)}
            </Grid>
          </Grid>
        </Grid>}
      <Grid item container direction="row" justify="flex-end" className="m-t-16">
        <Button
          variant="outlined"
          color="secondary"
          size="small"
          href={getFullMediaServerUrl(settings)}
          target="_blank"
          startIcon={<OpenInNewIcon />}
        >
          {serverType}
        </Button>
      </Grid>
    </Grid>
  )
}

export default ServerStatus
