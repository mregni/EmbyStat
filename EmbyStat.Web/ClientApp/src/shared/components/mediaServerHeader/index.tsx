import React from 'react';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import CardMedia from '@material-ui/core/CardMedia';
import CardContent from '@material-ui/core/CardContent';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';
import OpenInNewIcon from '@material-ui/icons/OpenInNewRounded';

import { MediaServerInfo } from '../../models/mediaServer';
import Emby from '../../assets/images/emby.png';
import Jellyfin from '../../assets/images/jellyfin.png';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    padding: '8px',
    position: 'relative',
  },
  content: {
    flex: '1 0 auto',
  },
  cover: {
    width: 80,
    height: 80,
    padding: '10px',
  },
  server__details__header: {
    color:
      theme.palette.type === 'dark'
        ? theme.palette.grey[400]
        : theme.palette.grey[600],
    fontSize: '0.8rem',
  },
  server__details__name: {
    paddingLeft: '8px',
  },
  server__details__icon: {
    width: '20px',
    position: 'absolute',
    right: 10,
    top: 10,
    '&:hover': {
      cursor: 'pointer',
    },
  },
}));

interface Props {
  serverType: number;
  serverInfo: MediaServerInfo;
  serverAddress: string;
}

const MediaServerHeader = (props: Props) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const { serverType, serverInfo, serverAddress } = props;

  const getReleaseString = (): string => {
    if (serverInfo.systemUpdateLevel === 0) {
      return 'Release';
    }
    else if (serverInfo.systemUpdateLevel === 1) {
      return 'Beta';
    }
    return 'Dev';
  }

  const openServer = (): void => {
    window.open(serverAddress, '_blank');
  };

  return (
    <Zoom in={true} style={{ transitionDelay: '100ms' }}>
      <Card elevation={7} square className={classes.root}>
        <CardMedia
          className={classes.cover}
          component="img"
          image={serverType === 0 ? Emby : Jellyfin}
          title="Media server logo"
        />
        <CardContent>
          <Grid item container direction="row">
            <Grid
              item
              md={4}
              container
              direction="column"
              justify="flex-start"
              className="m-l-32"
            >
              <Typography
                variant="body1"
                className={classes.server__details__header}
              >
                {t('COMMON.SERVERNAME')}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__name}
              >
                {serverInfo.serverName}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__header}
              >
                {t('COMMON.VERSION')}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__name}
              >
                {serverInfo.version}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__header}
              >
                {t('COMMON.OS')}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__name}
              >
                {serverInfo.operatingSystem}
              </Typography>
            </Grid>
            <Grid
              item
              md={4}
              container
              direction="column"
              justify="flex-start"
            >
              <Typography
                variant="body1"
                className={classes.server__details__header}
              >
                {t('COMMON.LANADDRESS')}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__name}
              >
                {serverInfo.localAddress}
              </Typography>
              {serverInfo.wanAddress !== null ? (
                <>
                  <Typography
                    variant="body1"
                    className={classes.server__details__header}
                  >
                    {t('COMMON.WANADDRESS')}
                  </Typography>
                  <Typography
                    variant="body1"
                    className={classes.server__details__name}
                  >
                    {serverInfo.wanAddress}
                  </Typography>
                </>
              ) : null}

              <Typography
                variant="body1"
                className={classes.server__details__header}
              >
                {t('COMMON.UPDATELEVEL')}
              </Typography>
              <Typography
                variant="body1"
                className={classes.server__details__name}
              >
                {getReleaseString()}
              </Typography>
            </Grid>
          </Grid>

          <OpenInNewIcon
            className={classes.server__details__icon}
            onClick={() => openServer()}
          />
        </CardContent>
      </Card>
    </Zoom>
  )
}

export default MediaServerHeader
