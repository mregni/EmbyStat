/* eslint-disable no-unused-vars */
// TODO: remove eslint disable lines

import {t} from 'i18next';
import React, {useContext, useEffect, useRef, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {useNavigate} from 'react-router';

import {KeyboardArrowLeft, KeyboardArrowRight} from '@mui/icons-material';
import {CircularProgress, Grid, MobileStepper, Paper, Stack, useMediaQuery} from '@mui/material';
import {useTheme} from '@mui/material/styles';

import {EsButton} from '../../shared/components/buttons';
import {SettingsContext} from '../../shared/context/settings';
import {WizardContextProvider} from '../../shared/context/wizard/WizardContextProvider';
import {WizardContext} from '../../shared/context/wizard/WizardState';
import {useHasAnyAdmins} from '../../shared/hooks/useHasAnyAdmins';
import {updateSettings} from '../../shared/services';
import {
  ConfigureLibrary, Finish, Intro, MediaServerDetails, SearchMediaServer, TestMediaServer,
  UserDetails,
} from './Steps';

const WizardContainer = () => {
  const {t} = useTranslation();
  const {hasAdmins, isLoading} = useHasAnyAdmins();
  const {finishWizard} = useContext(WizardContext);
  const {settings, load} = useContext(SettingsContext);
  const navigate = useNavigate();
  const theme = useTheme();
  const smDown = useMediaQuery(theme.breakpoints.down('sm'));

  const [activeStep, setActiveStep] = useState(0);
  const [disableControls, setDisableControls] = useState(false);

  const steps = 8;

  useEffect(() => {
    if (!isLoading && hasAdmins) {
      updateSettings({...settings, wizardFinished: true});
      navigate('/login', {replace: true});
    }
  }, [isLoading, hasAdmins, history, settings]);

  const introRef = useRef<React.ElementRef<typeof Intro>>(null);
  const userDetailRef = useRef<React.ElementRef<typeof UserDetails>>(null);
  const searchMediaServerRef = useRef<React.ElementRef<typeof SearchMediaServer>>(null);
  const mediaServerDetailsRef = useRef<React.ElementRef<typeof MediaServerDetails>>(null);
  const testMediaServerRef = useRef<React.ElementRef<typeof TestMediaServer>>(null);
  const configureMovieLibraryRef = useRef<React.ElementRef<typeof ConfigureLibrary>>(null);
  const configureShowLibraryRef = useRef<React.ElementRef<typeof ConfigureLibrary>>(null);

  const goToNextStep = () => {
    window.scrollTo(0, 0);
    setActiveStep((prevActiveStep) => prevActiveStep + 1 >= steps ? prevActiveStep : prevActiveStep + 1);
  };

  const handleNext = async () => {
    setDisableControls(true);
    let validated = true;

    if (activeStep === 0) {
      validated = (await introRef.current?.validate()) ?? false;
      validated && introRef.current?.saveChanges();
    }

    if (activeStep === 1) {
      validated = (await userDetailRef.current?.validate()) ?? false;
      validated && userDetailRef.current?.saveChanges();
    }

    if (activeStep === 3) {
      validated = (await mediaServerDetailsRef.current?.validate()) ?? false;
      validated && mediaServerDetailsRef.current?.saveChanges();
    }

    if (activeStep === 4) {
      validated = (await testMediaServerRef.current?.validate()) ?? false;
      validated && testMediaServerRef.current?.saveChanges();
    }

    if (activeStep === 5) {
      await configureMovieLibraryRef.current?.validate();
    }

    if (activeStep === 6) {
      await configureShowLibraryRef.current?.validate();
    }


    // Last step
    if (activeStep === steps - 1) {
      const result = await finishWizard(settings);
      if (result) {
        await load(true);
        navigate('/', {replace: true});
        return;
      }
    }

    setDisableControls(false);
    if (validated) {
      goToNextStep();
    }
  };

  const handleBack = async () => {
    setDisableControls(true);

    window.scrollTo(0, 0);
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
    setDisableControls(false);
  };


  return (
    <Grid container justifyContent="center" sx={{mt: 8}}>
      <Grid item xs={12} md={10} lg={8} xl={6}>
        <Paper sx={{minHeight: 500, p: 4}}>
          <Grid
            container
            direction="column"
            spacing={2}
            justifyContent="space-between"
            sx={{minHeight: 500}}
          >
            <Grid item>
              {activeStep === 0 && <Intro ref={introRef} />}
              {activeStep === 1 && <UserDetails ref={userDetailRef} />}
              {activeStep === 2 && <SearchMediaServer handleNext={handleNext} ref={searchMediaServerRef} />}
              {activeStep === 3 && <MediaServerDetails ref={mediaServerDetailsRef} />}
              {activeStep === 4 && <TestMediaServer ref={testMediaServerRef} />}
              {activeStep === 5 && <ConfigureLibrary type="movie" ref={configureMovieLibraryRef} />}
              {activeStep === 6 && <ConfigureLibrary type="show" ref={configureShowLibraryRef} />}
              {activeStep === 7 && <Finish />}
            </Grid>
            <Grid item>
              <MobileStepper
                steps={steps}
                position="static"
                variant={smDown ? 'text' : 'dots'}
                activeStep={activeStep}
                sx={{
                  height: 50,
                  width: '100%',
                  backgroundColor: 'rgba(0, 0, 0, 0)',
                }}
                nextButton={
                  <EsButton
                    fullWidth={false}
                    onClick={handleNext}
                    disabled={disableControls || [2].includes(activeStep)}
                  >
                    {
                      disableControls ?
                        (<CircularProgress size={24} />) :
                        (
                          <>
                            <span>{activeStep !== (steps - 1) ? t('next') : t('finish')}</span>
                            <KeyboardArrowRight />
                          </>
                        )
                    }
                  </EsButton>
                }
                backButton={
                  <EsButton
                    onClick={handleBack}
                    fullWidth={false}
                    disabled={disableControls || [0, 7].includes(activeStep)}
                  >
                    <KeyboardArrowLeft />
                    {t('back')}
                  </EsButton>
                }
              />
            </Grid>
          </Grid>
        </Paper>
      </Grid>
    </Grid>
  );
};

export const Wizard = () => {
  return (
    <WizardContextProvider>
      <WizardContainer />
    </WizardContextProvider>
  );
};

