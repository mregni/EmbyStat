import React, { useState, useEffect } from "react";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import { Trans, useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import { makeStyles } from "@material-ui/core";

import {
  testApiKey,
  getServerInfo,
  getAdministrators,
  getLibraries,
} from "../../../../shared/services/MediaServerService";
import { RootState } from "../../../../store/RootReducer";
import {
  MediaServerLogin,
  MediaServerInfo,
  MediaServerUser,
  Library,
} from "../../../../shared/models/mediaServer";
import TestFailed from "./TestFailed";
import TestSuccessful from "./TestSuccessFul";
import Loading from "../../../../shared/components/loading";

const useStyles = makeStyles((theme) => ({
  full_height: {
    height: "100%",
  },
}));

interface Props {
  disableBack: Function;
  disableNext: Function;
}

const TestServer = (props: Props) => {
  const { disableBack, disableNext } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [loadingLabel, setLoadingLabel] = useState("WIZARD.STEPONE");
  const [isLoading, setIsLoading] = useState(true);
  const [currentStep, setCurrentStep] = useState(1);
  const [errorMessage, setErrorMessage] = useState("");
  const [serverInfo, setServerInfo] = useState({} as MediaServerInfo);
  const [administrators, setAdministrators] = useState({} as MediaServerUser[]);
  const [libraries, setLibraries] = useState({} as Library[]);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    const performSteps = async () => {
      const protocolTxt = wizard.serverProtocol === 0 ? "https://" : "http://";
      const address = wizard.serverAddress;
      const port = wizard.serverPort;
      const baseUrl = wizard?.serverBaseurl ?? "";
      const fullAddress = `${protocolTxt}${address}:${port}${baseUrl}`;

      if (currentStep === 1) {
        const result = await testApiKey({
          address: fullAddress,
          apiKey: wizard.apiKey,
          type: wizard.serverType,
        } as MediaServerLogin);

        if (result == null) {
          setIsLoading(false);
          return;
        }

        if (result) {
          setCurrentStep(2);
          setLoadingLabel("WIZARD.STEPTWO");
          const serverInfo = await getServerInfo(true);
          if (serverInfo == null) {
            setIsLoading(false);
            return;
          }
          setServerInfo(serverInfo);

          setCurrentStep(3);
          setLoadingLabel("WIZARD.STEPTHREE");
          const admins = await getAdministrators();
          if (admins == null) {
            setIsLoading(false);
            return;
          }
          setAdministrators(admins);

          setCurrentStep(4);
          setLoadingLabel("WIZARD.STEPFOUR");
          const libs = await getLibraries();
          if (libs == null) {
            setIsLoading(false);
            return;
          }
          setLibraries(libs);
          setIsLoading(false);
        } else {
          setErrorMessage("WIZARD.APIKEYFAILED");
          setIsLoading(false);
        }
      }
    };
    performSteps();
  }, [currentStep, wizard, isLoading]);

  useEffect(() => {
    disableBack(isLoading);
  }, [isLoading, disableBack]);

  useEffect(() => {
    disableNext(isLoading || errorMessage !== "");
  }, [isLoading, disableNext, errorMessage]);

  return (
    <>
      <Typography variant="h4" color="primary">
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
        Component={errorMessage === "" ? TestSuccessful : TestFailed}
      />
    </>
  );
};

export default TestServer;
