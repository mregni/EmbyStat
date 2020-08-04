import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import CardMedia from '@material-ui/core/CardMedia';
import { makeStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { Trans } from 'react-i18next';

import { MediaServerUdpBroadcast } from '../../../../shared/models/mediaServer';
import Emby from '../../../../shared/assets/images/emby.png';
import Jellyfin from '../../../../shared/assets/images/jellyfin.png';

interface Props {
  server: MediaServerUdpBroadcast;
  onClick: Function;
}

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    position: 'relative',
    '&:hover': {
      cursor: 'pointer',
    },
    '&:active': {
      boxShadow: theme.shadows[7],
    },
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
    right: 5,
    top: 5,
  },
}));

const ServerCard = (props: Props) => {
  const { server, onClick } = props;
  const [elevation, setElevation] = useState(7);
  const classes = useStyles();

  const openServer = () => {
    window.open(
      `${server.protocol === 0 ? 'https://' : 'http://'}${server.address}:${
      server.port
      }`,
      '_blank'
    );
  };

  return (
    <Grid item xs={12} sm={6} md={4} lg={3} onClick={() => onClick(server)}>
      <Zoom in={true} style={{ transitionDelay: '100ms' }}>
        <Card
          elevation={elevation}
          square
          onMouseEnter={() => setElevation(12)}
          onMouseLeave={() => setElevation(7)}
          className={classes.root}
        >
          <CardMedia
            className={classes.cover}
            component="img"
            image={server.type === 0 ? Emby : Jellyfin}
            title="Media server logo"
          />
          <Grid item container direction="column" justify="center">
            <Typography
              variant="body1"
              className={classes.server__details__header}
            >
              <Trans i18nKey="COMMON.SERVERNAME" />
            </Typography>
            <Typography
              variant="body1"
              className={classes.server__details__name}
            >
              {server.name}
            </Typography>
          </Grid>

          <OpenInNewIcon
            className={classes.server__details__icon}
            onClick={() => openServer()}
          />
        </Card>
      </Zoom>
    </Grid>
  );
};

export default ServerCard;
