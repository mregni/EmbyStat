import React, {
  ReactElement,
  useEffect,
  useState,
  forwardRef,
  useImperativeHandle,
} from "react";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import { Trans, useTranslation } from "react-i18next";
import classNames from "classnames";
import { useSelector, useDispatch } from "react-redux";
import { useForm } from "react-hook-form";

import { searchMediaServers } from "../../../../shared/services/MediaServerService";
import { MediaServerUdpBroadcast } from "../../../../shared/models/mediaServer";
import Loading from "../../../../shared/components/loading";
import ServerResult from "./ServerResult";
import ServerForm from "./ServerForm";
import { RootState } from "../../../../store/RootReducer";
import {
  setServerConfiguration,
  setFoundServers,
  setMovieLibraryStepLoaded,
  setShowLibraryStepLoaded,
} from "../../../../store/WizardSlice";
import { ValidationHandle } from "../interfaces";

const useStyles = makeStyles((theme) => ({
  result__container: {
    minHeight: 115,
  },
}));

interface Props {
  disableNext: Function;
  disableBack: Function;
}

const ConfigureServer = forwardRef<ValidationHandle, Props>(
  (props, ref): ReactElement => {
    const classes = useStyles();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const { disableNext, disableBack } = props;
    const [servers, setServers] = useState<MediaServerUdpBroadcast[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    const wizard = useSelector((state: RootState) => state.wizard);
    useEffect(() => {
      const searchServers = async () => {
        if (!wizard.searchedServers) {
          const newServers = await searchMediaServers();
          setServers(newServers);
          dispatch(setFoundServers(newServers, true));
          setIsLoading(false);
        } else {
          setIsLoading(false);
          setServers(wizard.foundServers);
        }
      };

      searchServers();
    }, [isLoading, wizard.searchedServers, wizard.foundServers, dispatch]);

    useEffect(() => {
      disableBack(false);
      disableNext(false);
    }, [disableNext, disableBack]);

    useEffect(() => {
      dispatch(setMovieLibraryStepLoaded(false));
      dispatch(setShowLibraryStepLoaded(false));
    }, [dispatch]);

    const changeSeletctedServer = async (server: MediaServerUdpBroadcast) => {
      setValue('address', server.address);
      setValue('port', server.port);
      setValue('baseUrl', server.baseUrl);
      setValue('apiKey', "");
      setValue('type', server.type);
      setValue('protocol', server.protocol);
      setValue('baseUrlNeeded', server.baseUrl != null ? true : false);

      dispatch(
        setServerConfiguration(
          server.address,
          server.port,
          server.baseUrl != null ? server.baseUrl : "",
          "",
          server.type,
          server.protocol,
          server.baseUrl != null ? true : false
        )
      );
    };

    const {
      register,
      trigger,
      errors,
      setValue,
      getValues,
    } = useForm({
      mode: "onBlur",
      defaultValues: {
        type: wizard.serverType,
        protocol: wizard.serverProtocol,
        address: wizard.serverAddress,
        port: wizard.serverPort,
        baseUrl: wizard.serverBaseurl,
        apiKey: wizard.apiKey,
        baseUrlNeeded: wizard.serverBaseUrlNeeded,
      },
    });

    useImperativeHandle(ref, () => ({
      validate(): Promise<boolean> {
        const {
          address,
          port,
          baseUrl,
          apiKey,
          protocol,
          type,
          baseUrlNeeded,
        } = getValues();
        dispatch(
          setServerConfiguration(
            address,
            port,
            baseUrl,
            apiKey,
            type,
            protocol,
            baseUrlNeeded
          )
        );
        return trigger();
      },
    }));

    return (
      <Grid container direction="column">
        <Typography variant="h4" color="primary">
          <Trans i18nKey="WIZARD.SERVERCONFIGURATION" />
        </Typography>
        <Loading
          className={classNames("m-t-32", classes.result__container)}
          loading={isLoading}
          label={t("WIZARD.SEARCHSERVERS")}
          Component={ServerResult}
          servers={servers}
          setSelectedServer={changeSeletctedServer}
        />
        <ServerForm
          register={register}
          errors={errors}
          wizard={wizard}
          triggerValidation={trigger}
          className="m-t-32"
          setValue={setValue}
        />
      </Grid>
    );
  }
);

export default ConfigureServer;
