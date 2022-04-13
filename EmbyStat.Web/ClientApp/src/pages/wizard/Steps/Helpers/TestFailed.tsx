import React, {useContext, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Typography} from '@mui/material';

import {WizardContext} from '../../../../shared/context/wizard/WizardState';

export const TestFailed = () => {
  const {wizard} = useContext(WizardContext);
  const {t} = useTranslation();
  const [type] = useState(wizard.serverType === 0 ? 'Emby' : 'Jellyfin');

  return (
    <Grid container direction="column">
      <Typography variant="body1" className="m-t-16">
        {t('WIZARD.APIKEYFAILED', {type, address: wizard.address, key: wizard.apiKey})}
      </Typography>
      <Typography className="m-t-32">
        {t('WIZARD.ADDRESSUSED', {address: wizard.address})}
      </Typography>
      <Typography>
        {t('WIZARD.APIKEYUSED', {api: wizard.apiKey})}
      </Typography>
    </Grid>
  );
};
