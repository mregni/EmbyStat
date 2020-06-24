import React, { useState, useEffect } from 'react'
import { Grid, Typography } from '@material-ui/core';
import { Trans, useTranslation } from 'react-i18next';
import Loading from '../../../../shared/components/loading';

import { testApiKey, getServerInfo, getAdministrators, getLibraries } from '../../../../shared/services/MediaServerService';
import { useSelector } from 'react-redux';
import { RootState } from '../../../../store/RootReducer';
import { MediaServerLogin, MediaServerInfo, MediaServerUser, Library } from '../../../../shared/models/mediaServer';
import TestFailed from './TestFailed';
import TestSuccessful from './TestSuccessFul';

interface Props {
  disableBack: Function,
  disableNext: Function,
}

const TestServer = (props: Props) => {
  const { disableBack, disableNext } = props;
  const { t } = useTranslation();
  const [loadingLabel, setLoadingLabel] = useState('WIZARD.STEPONE');
  const [isLoading, setIsLoading] = useState(true);
  const [currentStep, setCurrentStep] = useState(1);
  const [errorMessage, setErrorMessage] = useState('');
  const [serverInfo, setServerInfo] = useState({} as MediaServerInfo);
  const [administrators, setAdministrators] = useState({} as MediaServerUser[]);
  const [libraries, setLibraries] = useState({} as Library[]);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    const performSteps = async () => {
      const protocolTxt = wizard.serverProtocol === 0 ? 'https://' : 'http://';
      const address = wizard.serverAddress;
      const port = wizard.serverPort;
      const baseUrl = wizard?.serverBaseurl ?? '';
      const fullAddress = `${protocolTxt}${address}:${port}${baseUrl}`;

      if (currentStep === 1) {
        const result = await testApiKey({ address: fullAddress, apiKey: wizard.apiKey, type: wizard.serverType } as MediaServerLogin);
        if (result) {
          setCurrentStep(2);
          setLoadingLabel('WIZARD.STEPTWO');
          const serervInfo = await getServerInfo(true);
          setServerInfo(serervInfo);

          setCurrentStep(3);
          setLoadingLabel('WIZARD.STEPTHREE');
          const admins = await getAdministrators();
          setAdministrators(admins);

          setCurrentStep(4);
          setLoadingLabel('WIZARD.STEPFOUR');
          const libs = await getLibraries();
          setLibraries(libs);
          setIsLoading(false);
        } else {
          console.log('step 1 failed');
          setErrorMessage('WIZARD.APIKEYFAILED');
          setIsLoading(false);
        }
      }
    }
    performSteps();
  }, [currentStep, wizard, isLoading]);

  useEffect(() => {
    disableBack(isLoading);
  }, [isLoading, disableBack]);

  useEffect(() => {
    disableNext(isLoading || errorMessage !== '');
  }, [isLoading, disableNext, errorMessage]);

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.SERVERCONFIGURATIONTEST" />
      </Typography>
      <Loading
        className="m-t-32"
        loading={isLoading}
        label={t(loadingLabel, { step: currentStep, total: 4 })}
        errorMessage={errorMessage}
        serverInfo={serverInfo}
        administrators={administrators}
        wizard={wizard}
        libraries={libraries}
        Component={errorMessage === '' ? TestSuccessful : TestFailed}
      />
    </Grid>
  )
}

export default TestServer
