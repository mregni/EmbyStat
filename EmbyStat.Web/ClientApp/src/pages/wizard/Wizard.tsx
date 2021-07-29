import React, { useContext, useEffect, useRef, useState } from 'react'
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import Paper from '@material-ui/core/Paper';
import { KeyboardArrowLeft, KeyboardArrowRight } from '@material-ui/icons';
import CircularProgress from '@material-ui/core/CircularProgress';
import MobileStepper from '@material-ui/core/MobileStepper';
import { useHistory } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import { WizardContextProvider } from './Context/WizardContextProvider';
import { useHasAnyAdmins } from '../../shared/hooks';
import PageLoader from "../../shared/components/pageLoader";
import { Intro, MediaServerDetails, SearchMediaServer, UserDetails, TestMediaServer, ConfigureLibrary, Finish } from './Steps';
import { EsButton } from '../../shared/components/buttons';
import { WizardContext } from './Context/WizardState';
import { SettingsContext } from '../../shared/context/settings';
import { updateSettings } from '../../shared/services/SettingsService';

const useStyles = makeStyles((theme) => ({
  root: {
    height: '100vh'
  },
  stepper: {
    height: 50,
    width: '100%',
    backgroundColor: theme.palette.background.paper
  },
  paper: {
    minHeight: 500,
    padding: 32,
    display: 'flex',
    flexDirection: 'row'
  },
  modal: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  paper__modal: {
    backgroundColor: theme.palette.background.paper,
    boxShadow: theme.shadows[5],
    padding: theme.spacing(2, 4, 3),
  },
}));

export const WizardContainer = () => {
  return (
    <WizardContextProvider>
      <Wizard />
    </WizardContextProvider>
  )
}

const Wizard = () => {
  const history = useHistory();
  const classes = useStyles();
  const { t } = useTranslation();
  const theme = useTheme();
  const { finishWizard } = useContext(WizardContext);
  const { settings, load } = useContext(SettingsContext);

  const smDown = useMediaQuery(theme.breakpoints.down('sm'));
  const { hasAdmins, isLoading } = useHasAnyAdmins();
  const [disableControls, setDisableControls] = useState(false);
  const [activeStep, setActiveStep] = useState(0);
  const steps = 8;

  useEffect(() => {
    if (!isLoading && hasAdmins) {
      updateSettings({ ...settings, wizardFinished: true });
      history.push("/login");
    }
  }, [isLoading, hasAdmins, history, settings]);

  const goToNextStep = () => {
    window.scrollTo(0, 0);
    setActiveStep((prevActiveStep) => prevActiveStep + 1 >= steps ? prevActiveStep : prevActiveStep + 1);
  }

  const introRef = useRef<React.ElementRef<typeof Intro>>(null);
  const userDetailRef = useRef<React.ElementRef<typeof UserDetails>>(null);
  const searchMediaServerRef = useRef<React.ElementRef<typeof SearchMediaServer>>(null);
  const mediaServerDetailsRef = useRef<React.ElementRef<typeof MediaServerDetails>>(null);
  const testMediaServerRef = useRef<React.ElementRef<typeof TestMediaServer>>(null);
  const configureMovieLibraryRef = useRef<React.ElementRef<typeof ConfigureLibrary>>(null);
  const configureShowLibraryRef = useRef<React.ElementRef<typeof ConfigureLibrary>>(null);

  const handleNext = async () => {
    setDisableControls(true);
    let validated = true;

    if (activeStep === 0) {
      validated = (await introRef.current?.validate()) ?? false;
      validated && await introRef.current?.saveChanges();
    }

    if (activeStep === 1) {
      validated = (await userDetailRef.current?.validate()) ?? false;
      validated && await userDetailRef.current?.saveChanges();
    }

    if (activeStep === 3) {
      validated = (await mediaServerDetailsRef.current?.validate()) ?? false;
      validated && await mediaServerDetailsRef.current?.saveChanges();
    }

    if (activeStep === 4) {
      validated = (await testMediaServerRef.current?.validate()) ?? false;
      validated && await testMediaServerRef.current?.saveChanges();
    }

    //Last step
    if (activeStep === steps - 1) {
      const result = await finishWizard(settings);
      await load();
      if (result) {
        history.push("/");
        return;
      }
    }

    setDisableControls(false);
    if (validated) {
      goToNextStep();
    }
  }

  const handleBack = async () => {
    setDisableControls(true);

    window.scrollTo(0, 0);
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
    setDisableControls(false);
  };

  return (
    <>
      { isLoading ? <PageLoader /> :
        (
          <Grid container justify="center" alignItems="center" className={classes.root}>
            <Grid item xs={11} md={8} xl={6}>
              <Paper className={classes.paper}>
                <Grid
                  item
                  container
                  direction="column"
                  spacing={2}
                  justify="space-between"
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
                      classes={{
                        root: classes.stepper
                      }}
                      nextButton={
                        <EsButton
                          fullWidth={false}
                          onClick={handleNext}
                          disabled={disableControls || [2].includes(activeStep)}
                        >
                          {
                            disableControls
                              ? (
                                <CircularProgress size={24} />
                              )
                              : (
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
        )
      }
    </>
  )
}
