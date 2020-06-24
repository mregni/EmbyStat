import React, { useState, useEffect } from 'react'
import { Grid, Typography, Button } from '@material-ui/core'
import { Trans, useTranslation } from 'react-i18next'
import Loading from '../../../../shared/components/loading';
import { useSelector } from 'react-redux';
import { RootState } from '../../../../store/RootReducer';

import { getSettings, updateSettings } from '../../../../shared/services/SettingsService';

const Result = ({ wizard }) => {
  const { t } = useTranslation();
  const serverType = wizard.serverType === 0 ? 'Emby' : 'Jellyfin';
  return (
    <Grid container direction="column">
      <Typography variant="body1" className="m-t-16 m-b-16">
        <Trans i18nKey="WIZARD.FINISHED" values={{ type: serverType }} />
      </Typography>
      <Typography variant="body1">
        <Trans i18nKey="WIZARD.FINISHEXPLANATION" values={{ type: serverType }} />
      </Typography>
      <Grid item container direction="row" justify="flex-end" className="m-t-32">
        <Button color="secondary" className="m-r-16">{t('COMMON.FINISH')}</Button>
        <Button variant="contained" color="primary">{t('WIZARD.FINISHWITHSYNC')}</Button>
      </Grid>
    </Grid>
  );
}

interface Props {
  disableNext: Function,
  disableBack: Function,
}

const Finish = (props: Props) => {
  const { disableNext, disableBack } = props;
  const { t } = useTranslation();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    disableNext(true);
    disableBack(true);
  }, [disableNext, disableBack]);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    const safeSettings = async () => {
      console.log("SAFE SETTINGS");
      const settings = await getSettings();
      settings.mediaServer.serverId = wizard.serverId;
      settings.mediaServer.apiKey = wizard.apiKey;
      settings.mediaServer.serverAddress = wizard.serverAddress;
      settings.mediaServer.serverBaseurl = wizard.serverBaseurl;
      settings.mediaServer.serverName = wizard.serverName;
      settings.mediaServer.serverPort = typeof wizard.serverPort === 'number' ? wizard.serverPort : parseInt(wizard.serverPort, 10);
      settings.mediaServer.serverProtocol = wizard.serverProtocol;
      settings.mediaServer.serverType = wizard.serverType;
      settings.mediaServer.userId = wizard.userId;
      settings.movieLibraries = wizard.movieLibraries;
      settings.showLibraries = wizard.showLibraries;
      settings.wizardFinished = true;
      settings.language = wizard.language;
      settings.enableRollbarLogging = wizard.enableRollbarLogging
      await updateSettings(settings);
      setIsLoading(false);
    }

    safeSettings();
  }, [wizard])


  return (
    <Grid container direction="column">
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.FINALLABEL" />
      </Typography>
      <Loading
        className="m-t-32"
        loading={isLoading}
        label={t('WIZARD.SAVING')}
        Component={Result}
        wizard={wizard}
      />
    </Grid>
  );
};

export default Finish
