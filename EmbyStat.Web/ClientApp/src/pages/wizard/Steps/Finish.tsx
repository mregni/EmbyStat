import React, { useContext } from 'react'
import Grid from '@material-ui/core/Grid'
import Typography from '@material-ui/core/Typography';
import { useTranslation } from 'react-i18next';

import { WizardContext } from '../Context/WizardState';
import { getMediaServerTypeStringFromNumber } from '../../../shared/utils';

export const Finish = () => {
  const { wizard } = useContext(WizardContext);
  const { t } = useTranslation();

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        {t('WIZARD.FINALLABEL')}
      </Typography>
      <Grid container direction="column">
        <Typography variant="body1" className="m-t-16 m-b-16">
          {t('WIZARD.FINISHED', { type: 'EmbyStat' })}
        </Typography>
        <Typography variant="body1">
          {t('WIZARD.FINISHEXPLANATION', { type: getMediaServerTypeStringFromNumber(wizard.serverType) })}
        </Typography>
      </Grid>
    </Grid>
  )
}
