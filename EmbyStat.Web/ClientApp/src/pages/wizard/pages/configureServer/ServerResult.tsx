import React from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { Trans } from 'react-i18next';
import { MediaServerUdpBroadcast } from '../../../../shared/models/mediaServer';
import ServerCard from './ServerCard';

interface Props {
  servers: MediaServerUdpBroadcast[];
  className: string;
  setSelectedServer: (server: MediaServerUdpBroadcast) => void;
}

const ServerResult = (props: Props) => {
  const { servers, className, setSelectedServer } = props;

  const onServerCardClick = (server: MediaServerUdpBroadcast) => {
    setSelectedServer(server);
  };

  if (servers.length === 0) {
    return (
      <Typography variant="body1" className={className}>
        <Trans i18nKey="WIZARD.NOTFOUNDTEXT" />
      </Typography>
    );
  }

  return (
    <Grid container direction="column" className={className}>
      <Typography variant="body1" className="m-b-32">
        <Trans i18nKey="WIZARD.FOUNDTEXT" />
      </Typography>

      <Grid item container direction="row" xs={12} spacing={2}>
        {servers.map((x: MediaServerUdpBroadcast) => (
          <ServerCard server={x} key={x.id} onClick={onServerCardClick} />
        ))}
      </Grid>
    </Grid>
  );
};

export default ServerResult;
