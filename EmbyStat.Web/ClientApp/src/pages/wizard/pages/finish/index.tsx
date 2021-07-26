import React, { useContext, useEffect, useState } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { useHistory } from 'react-router-dom';

import Button from '@material-ui/core/Button';
import Checkbox from '@material-ui/core/Checkbox';
import CircularProgress from '@material-ui/core/CircularProgress';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import FormGroup from '@material-ui/core/FormGroup';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';

import { register } from '../../../../shared/services/AccountService';
import { fireJob } from '../../../../shared/services/JobService';
import { RootState } from '../../../../store/RootReducer';
import { saveSettings } from '../../../../store/SettingsSlice';
import { SettingsContext } from '../../../../shared/context/settings';

const useStyles = makeStyles((theme) => ({
  button__loading: {
    color: "#d3d3d3",
  },
  button: {
    height: 36,
    width: 242,
  },
}));

interface Props {
  disableNext: Function;
  disableBack: Function;
}

const Finish = (props: Props) => {
  const { disableNext, disableBack } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const dispatch = useDispatch();
  const history = useHistory();
  const [isLoading, setIsLoading] = useState(false);
  const [fireSync, setFireSync] = useState(false);

  const { settings } = useContext(SettingsContext);
  const wizard = useSelector((state: RootState) => state.wizard);

  const serverType = wizard.serverType === 0 ? "Emby" : "Jellyfin";

  const onFinish = async () => {
    setIsLoading(true);

    const loginView = {
      username: wizard.username,
      password: wizard.password,
    };
    await register(loginView);

    const newSettings = { ...settings };
    const mediaServer = { ...newSettings.mediaServer };
    mediaServer.serverId = wizard.serverId;
    mediaServer.apiKey = wizard.apiKey;
    mediaServer.serverAddress = wizard.serverAddress;
    mediaServer.serverBaseUrl = wizard.serverBaseurl != null ? wizard.serverBaseurl : "";
    mediaServer.serverName = wizard.serverName;
    mediaServer.serverPort =
      typeof wizard.serverPort === "number"
        ? wizard.serverPort
        : 8096;
    mediaServer.serverProtocol = wizard.serverProtocol;
    mediaServer.serverType = wizard.serverType;
    mediaServer.userId = wizard.userId;
    newSettings.mediaServer = mediaServer;
    newSettings.movieLibraries = wizard.movieLibraries;
    newSettings.showLibraries = wizard.showLibraries;
    newSettings.language = wizard.language;
    newSettings.enableRollbarLogging = wizard.enableRollbarLogging;
    newSettings.wizardFinished = true;
    dispatch(saveSettings(newSettings));

    if (fireSync) {
      await fireJob("be68900b-ee1d-41ef-b12f-60ef3106052e");
      history.push("/jobs");
    } else {
      history.push("/");
    }
  };

  useEffect(() => {
    disableNext(true);
    disableBack(true);
  }, [disableNext, disableBack]);

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        <Trans i18nKey="WIZARD.FINALLABEL" />
      </Typography>
      <Grid container direction="column">
        <Typography variant="body1" className="m-t-16 m-b-16">
          <Trans i18nKey="WIZARD.FINISHED" values={{ type: serverType }} />
        </Typography>
        <Typography variant="body1">
          <Trans
            i18nKey="WIZARD.FINISHEXPLANATION"
            values={{ type: serverType }}
          />
        </Typography>
        <Grid item>
          <FormGroup row>
            <FormControlLabel
              control={
                <Checkbox
                  checked={fireSync}
                  onChange={(event) => setFireSync(event.target.checked)}
                  color="primary"
                />
              }
              label={t("WIZARD.RUNSYNC")}
            />
          </FormGroup>
        </Grid>
        <Grid
          item
          container
          direction="row"
          justify="flex-end"
          className="m-t-32"
        >
          <Button
            variant="contained"
            color="primary"
            onClick={onFinish}
            className={classes.button}
            disabled={isLoading}
          >
            {isLoading ? (
              <CircularProgress size={16} className={classes.button__loading} />
            ) : (
                t("COMMON.FINISH")
              )}
          </Button>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default Finish;
