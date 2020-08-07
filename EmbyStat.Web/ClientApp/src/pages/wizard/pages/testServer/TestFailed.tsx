import React, { useState, useEffect } from 'react';
import { Trans } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { Wizard } from '../../../../shared/models/wizard';

interface Props {
  errorMessage: string;
  wizard: Wizard;
}

const TestFailed = (props: Props) => {
  const { errorMessage, wizard } = props;
  const [address, setAddress] = useState('');
  const [type] = useState(wizard.serverType === 0 ? 'Emby' : 'Jellyfin');

  useEffect(() => {
    const protocolTxt = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    const wizardAddress = wizard.serverAddress;
    const port = wizard.serverPort;
    const baseUrl = wizard?.serverBaseurl ?? '';
    setAddress(`${protocolTxt}${wizardAddress}:${port}${baseUrl}`);
  }, [wizard]);

  return (
    <Grid container direction="column">
      <Typography variant="body1" className="m-t-16">
        <Trans
          i18nKey={errorMessage}
          values={{ type, address, key: wizard.apiKey }}
        />
      </Typography>
      <Typography className="m-t-32">
        <Trans i18nKey="WIZARD.ADDRESSUSED" values={{ address }} />
      </Typography>
      <Typography>
        <Trans i18nKey="WIZARD.APIKEYUSED" values={{ api: wizard.apiKey }} />
      </Typography>
    </Grid>
  );
};

export default TestFailed;
