import React, {
  forwardRef, useContext, useEffect, useImperativeHandle, useRef, useState,
} from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Typography} from '@mui/material';

import {EsLoading} from '../../../shared/components/esLoading';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {MediaServerLogin} from '../../../shared/models/mediaServer';
import {getAdministrators, getLibraries, getServerInfo, testApiKey} from '../../../shared/services';
import {StepProps, ValidationHandleWithSave} from '../Interfaces';
import {TestFailed, TestSuccessFul} from './Helpers';

export const TestMediaServer =
forwardRef<ValidationHandleWithSave, StepProps>(function TestMediaServer(props, ref) {
  const {t} = useTranslation();
  const {wizard, setMediaServerInfo,
    setAdministrators} = useContext(WizardContext);

  const [loadingLabel, setLoadingLabel] = useState('WIZARD.STEPONE');
  const [isLoading, setIsLoading] = useState(true);
  const [currentStep, setCurrentStep] = useState(1);
  const [inError, setInError] = useState(false);

  const successRef = useRef<React.ElementRef<typeof TestSuccessFul>>(null);

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      return await successRef.current?.validate() ?? Promise.resolve(false);
    },
    saveChanges(): void {
      successRef.current?.saveChanges();
    },
  }));

  useEffect(() => {
    const performSteps = async () => {
      if (currentStep === 1) {
        const result = await testApiKey({
          address: wizard.address,
          apiKey: wizard.apiKey,
          type: wizard.serverType,
        } as MediaServerLogin);

        if (result == null) {
          setIsLoading(false);
          return;
        }

        if (result) {
          setCurrentStep(2);
          setLoadingLabel('WIZARD.STEPTWO');
          const serverInfo = await getServerInfo(true);
          if (serverInfo == null) {
            setIsLoading(false);
            return;
          }

          setCurrentStep(3);
          setLoadingLabel('WIZARD.STEPTHREE');
          const admins = await getAdministrators();
          if (admins === []) {
            setIsLoading(false);
            return;
          }

          setCurrentStep(4);
          setLoadingLabel('WIZARD.STEPFOUR');
          const libs = await getLibraries();
          if (libs === []) {
            setIsLoading(false);
            return;
          }
          setMediaServerInfo(serverInfo);
          setAdministrators(admins);
          setIsLoading(false);
        } else {
          setInError(true);
          setIsLoading(false);
        }
      }
    };
    performSteps();
  }, [currentStep, wizard, isLoading, setMediaServerInfo, setAdministrators]);

  return (
    <Grid
      container
      direction="column"
      spacing={2}
    >
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('WIZARD.SERVERCONFIGURATIONTEST')}
        </Typography>
      </Grid>
      <Grid item>
        <EsLoading
          width='100%'
          height='200px'
          loading={isLoading}
          label={t(loadingLabel, {step: currentStep, total: 4})}
        >
          {!inError ? <TestSuccessFul ref={successRef} /> : <TestFailed />}
        </EsLoading>
      </Grid>
    </Grid>
  );
});
