import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';

import {Stack, Typography} from '@mui/material';

import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {useServerType} from '../../../shared/hooks';

export const Finish = () => {
  const {wizard} = useContext(WizardContext);
  const {t} = useTranslation();
  const {getMediaServerTypeStringFromNumber} = useServerType();

  return (
    <Stack spacing={2}>
      <Typography variant="h4" color="primary">
        {t('WIZARD.FINALTITLE')}
      </Typography>
      <Typography variant="body1" className="m-t-16 m-b-16">
        {t('WIZARD.FINISHED')}
      </Typography>
      <Typography variant="body1">
        {t('WIZARD.FINISHEXPLANATION', {type: getMediaServerTypeStringFromNumber(wizard.serverType)})}
      </Typography>
    </Stack>
  );
};
