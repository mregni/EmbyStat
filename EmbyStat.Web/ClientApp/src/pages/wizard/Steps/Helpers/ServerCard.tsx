import React, {useState} from 'react';

import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import {Card, CardMedia, Grid, Typography, Zoom} from '@mui/material';

import Emby from '../../../../shared/assets/images/emby.png';
import Jellyfin from '../../../../shared/assets/images/jellyfin.png';
import {MediaServerUdpBroadcast} from '../../../../shared/models/mediaServer';

interface Props {
  server: MediaServerUdpBroadcast;
  onClick: (server: MediaServerUdpBroadcast) => void;
}

export const ServerCard = (props: Props) => {
  const {server, onClick} = props;
  const [elevation, setElevation] = useState(7);

  const openServer = () => {
    window.open(
      `http${server.protocol === 0 ? 's' : ''}://${server.address}:${server.port}`,
      '_blank',
    );
  };

  return (
    <Grid item xs={12} sm={6} md={4} lg={3} onClick={() => onClick(server)}>
      <Zoom in={true} style={{transitionDelay: '100ms'}}>
        <Card
          elevation={elevation}
          square
          onMouseEnter={() => setElevation(12)}
          onMouseLeave={() => setElevation(7)}
          sx={{
            'display': 'flex',
            'position': 'relative',
            '&:hover': {
              cursor: 'pointer',
            },
            '&:active': {
              boxShadow: (theme) => theme.shadows[7],
            },
          }}
        >
          <CardMedia
            sx={{
              width: 80,
              height: 80,
              padding: 2,
            }}
            component="img"
            image={server.type === 0 ? Emby : Jellyfin}
            title="Media server logo"
          />
          <Grid item container direction="column" justifyContent="center">
            <Typography
              variant="body1"
              sx={{
                color: (theme) => theme.palette.grey[400],
                fontSize: '0.8rem',
              }}
            >
              {server.name}
            </Typography>
            <Typography variant="body1">
              {server.address}
            </Typography>
          </Grid>

          <OpenInNewIcon
            sx={{
              width: '15px',
              position: 'absolute',
              right: 5,
              top: 5,
            }}
            onClick={() => openServer()}
          />
        </Card>
      </Zoom>
    </Grid>
  );
};
