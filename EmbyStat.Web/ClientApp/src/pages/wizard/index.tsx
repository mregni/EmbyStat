import React, { ReactElement, useState, useEffect, useRef } from "react";
import { Trans, useTranslation } from "react-i18next";
import { useDispatch, useSelector } from "react-redux";
import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import MobileStepper from "@material-ui/core/MobileStepper";
import KeyboardArrowLeft from "@material-ui/icons/KeyboardArrowLeft";
import KeyboardArrowRight from "@material-ui/icons/KeyboardArrowRight";

import UserDetails from "./pages/userDetails";
import Intro from "./pages/intro";
import ConfigureServer from "./pages/configureServer";
import { anyAdmins } from "../../shared/services/AccountService";
import TestServer from "./pages/testServer";
import ConfigureLibraries from "./pages/configureLibraries";
import Finish from "./pages/finish";
import { withRouter, RouteComponentProps } from "react-router-dom";
import { Settings } from "../../shared/models/settings";
import PageLoader from "../../shared/components/pageLoader";
import { saveSettings } from "../../store/SettingsSlice";
import { RootState } from "../../store/RootReducer";

const useStyles = makeStyles((theme) => ({
  root: {
    display: "flex",
    height: "100vh",
  },
  content: {
    padding: theme.spacing(3),
    marginTop: 56,
    [theme.breakpoints.up("sm")]: {
      marginTop: 64,
    },
    width: "100%",
    zIndex: 1,
  },
  wizard: {
    maxWidth: 1440,
  },
  wizard__stepper: {
    maxWidth: 600,
  },
  wizard__step: {
    minHeight: 650,
    [theme.breakpoints.up("md")]: {
      padding: 16,
    },
  },
}));

interface Props extends RouteComponentProps<any> {
  settings: Settings;
}

const Wizard = (props: Props): ReactElement => {
  const { settings, history } = props;
  const [activeStep, setActiveStep] = useState(0);
  const [disableNext, setDisableNext] = useState(false);
  const [disableBack, setDisableBack] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const dispatch = useDispatch();
  const classes = useStyles();
  const { t } = useTranslation();

  const userDetailRef = useRef<React.ElementRef<typeof UserDetails>>(null);
  const configureServiceRef = useRef<React.ElementRef<typeof ConfigureServer>>(
    null
  );

  useEffect(() => {
    const loadAnyAdmin = async () => {
      var result = await anyAdmins();
      if (result) {
        const newSettings = { ...settings };
        newSettings.wizardFinished = true;
        dispatch(saveSettings(newSettings));
        history.push("/login");
      }
      setIsLoading(false);
    };

    loadAnyAdmin();
  }, [settings, history, dispatch]);

  const handleNext = async () => {
    let validated = true;
    if (activeStep === 1) {
      validated = (await userDetailRef.current?.validate()) ?? false;
    } else if (activeStep === 2) {
      validated = (await configureServiceRef.current?.validate()) ?? false;
    }

    if (validated) {
      setActiveStep((prevActiveStep) => prevActiveStep + 1);
    }
  };

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  return (
    <>
      {isLoading ? (
        <PageLoader />
      ) : (
          <div className={classes.root}>
            <main className={classes.content}>
              <Grid container direction="row" justify="center">
                <Grid item xs={12} md={10} lg={8} className={classes.wizard}>
                  <Card>
                    <CardContent>
                      <Grid container direction="column">
                        <Grid item className={classes.wizard__step}>
                          {activeStep === 0 ? (
                            <Intro disableBack={setDisableBack} />
                          ) : null}
                          {activeStep === 1 ? (
                            <form autoComplete="off">
                              <UserDetails
                                disableBack={setDisableBack}
                                disableNext={setDisableNext}
                                ref={userDetailRef}
                              />
                            </form>
                          ) : null}
                          {activeStep === 2 ? (
                            <form autoComplete="off">
                              <ConfigureServer
                                disableBack={setDisableBack}
                                disableNext={setDisableNext}
                                ref={configureServiceRef}
                              />
                            </form>
                          ) : null}
                          {activeStep === 3 ? (
                            <TestServer
                              disableBack={setDisableBack}
                              disableNext={setDisableNext}
                            />
                          ) : null}
                          {activeStep === 4 ? (
                            <ConfigureLibraries type="movie" />
                          ) : null}
                          {activeStep === 5 ? (
                            <ConfigureLibraries type="show" />
                          ) : null}
                          {activeStep === 6 ? (
                            <Finish
                              disableNext={setDisableNext}
                              disableBack={setDisableBack}
                            />
                          ) : null}
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
                                  {t("COMMON.BACK")}
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
            </main>
          </div>
        )}
    </>
  );
};

export default withRouter(Wizard);
