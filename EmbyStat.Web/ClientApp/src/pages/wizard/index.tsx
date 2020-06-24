import React, { ReactElement, useState } from 'react';
import { Trans, useTranslation } from 'react-i18next';

import { Card, CardContent } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import { useDispatch } from 'react-redux';
import Grid from '@material-ui/core/Grid';
import MobileStepper from '@material-ui/core/MobileStepper';
import KeyboardArrowLeft from '@material-ui/icons/KeyboardArrowLeft';
import KeyboardArrowRight from '@material-ui/icons/KeyboardArrowRight';
import { useForm } from "react-hook-form";

import UserDetails from './pages/userDetails';
import Intro from './pages/intro';
import ConfigureServer from './pages/configureServer';

import { setUserName, setServerConfiguration } from '../../store/WizardSlice';
import { makeStyles } from '@material-ui/core';
import TestServer from './pages/testServer';
import ConfigureLibraries from './pages/configureLibraries';
import Finish from './pages/finish';

const useStyles = makeStyles((theme) => ({
  wizard: {
    maxWidth: 1440,
  },
  wizard__stepper: {
    maxWidth: 600,
  },
  wizard__step: {
    minHeight: 650,
    [theme.breakpoints.up('md')]: {
      padding: 16,
    }
  }
}));

const Wizard = (): ReactElement => {
  const [activeStep, setActiveStep] = useState(0);
  const [disableNext, setDisableNext] = useState(false);
  const [disableBack, setDisableBack] = useState(false);
  const dispatch = useDispatch();
  const classes = useStyles();
  const { t } = useTranslation();

  const handleNext = async () => {
    //User details form validation
    if (activeStep === 1) {
      if (await triggerValidation()) {
        dispatch(setUserName(getValues("username")));
        setActiveStep((prevActiveStep) => prevActiveStep + 1);
      }
    } else if (activeStep === 2) {
      if (await triggerValidation()) {
        const { address, port, baseUrl, apiKey } = getValues();
        dispatch(setServerConfiguration(address, parseInt(port, 10), baseUrl, apiKey));
        setActiveStep((prevActiveStep) => prevActiveStep + 1);
      }
    } else {
      setActiveStep((prevActiveStep) => prevActiveStep + 1);
    }
  };

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  const { register, triggerValidation, errors, getValues, reset } = useForm(
    {
      mode: 'onBlur',
      defaultValues: {
        type: 0,
        protocol: 0,
        address: '',
        port: '',
        baseUrl: '',
        apiKey: '',
        username: '',
      }
    });

  return (
    <Grid container direction="row" justify="center">
      <Grid item xs={12} md={10} lg={8} className={classes.wizard}>
        <Card>
          <CardContent>
            <Grid container direction="column">
              <Grid item className={classes.wizard__step}>
                {activeStep === 0 ? <Intro disableBack={setDisableBack} /> : null}
                {activeStep === 1 ?
                  <form autoComplete="off">
                    <UserDetails
                      register={register}
                      errors={errors}
                      disableBack={setDisableBack}
                      disableNext={setDisableNext} />
                  </form> : null}
                {activeStep === 2 ?
                  <form autoComplete="off">
                    <ConfigureServer
                      register={register}
                      errors={errors}
                      reset={reset}
                      disableBack={setDisableBack}
                      disableNext={setDisableNext} />
                  </form> : null}
                {activeStep === 3 ?
                  <TestServer
                    disableBack={setDisableBack}
                    disableNext={setDisableNext} /> : null}
                {activeStep === 4 ? <ConfigureLibraries type="movie" /> : null}
                {activeStep === 5 ? <ConfigureLibraries type="show" /> : null}
                {activeStep === 6 ? <Finish
                  disableNext={setDisableNext}
                  disableBack={setDisableBack}
                /> : null}
              </Grid>

              <Grid item container direction="row" justify="center">
                <Grid item xs={12} className={classes.wizard__stepper}>
                  <MobileStepper
                    variant="dots"
                    steps={7}
                    position="static"
                    activeStep={activeStep}
                    nextButton={
                      <Button
                        size="small"
                        onClick={handleNext}
                        disabled={disableNext}
                      >
                        <Trans i18nKey="COMMON.NEXT" />
                        <KeyboardArrowRight />
                      </Button>
                    }
                    backButton={
                      <Button
                        size="small"
                        onClick={handleBack}
                        disabled={disableBack}
                      >
                        <KeyboardArrowLeft />
                        {t('COMMON.BACK')}
                      </Button>
                    }
                  />
                </Grid>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );
};

export default Wizard;
