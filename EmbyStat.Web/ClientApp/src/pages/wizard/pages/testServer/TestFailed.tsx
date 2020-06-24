import React, { useState, useEffect } from 'react'
import { Wizard } from '../../../../shared/models/wizard';
import { Trans } from 'react-i18next';
import { Typography, Grid } from '@material-ui/core';

interface Props {
  errorMessage: string,
  wizard: Wizard,
}

const TestFailed = (props: Props) => {
  const {
    errorMessage,
    wizard
  } = props;
  const [address, setAddress] = useState('');
  const [type] = useState(wizard.serverType === 0 ? 'Emby' : 'Jellyfin');

  useEffect(() => {
    const protocolTxt = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    const address = wizard.serverAddress;
    const port = wizard.serverPort;
    const baseUrl = wizard?.serverBaseurl ?? '';
    setAddress(`${protocolTxt}${address}:${port}${baseUrl}`);
  }, [wizard])

  return (
    <Grid container direction="column">
      <Typography variant="body1" className="m-t-16">
        <Trans i18nKey={errorMessage} values={{ type: type, address: address, key: wizard.apiKey }} />
      </Typography>
      <Typography className="m-t-32">
        <Trans i18nKey="WIZARD.ADDRESSUSED" values={{ address: address }} />
      </Typography>
      <Typography>
        <Trans i18nKey="WIZARD.APIKEYUSED" values={{ api: wizard.apiKey }} />
      </Typography>
    </Grid>
  );
}


export default TestFailed
