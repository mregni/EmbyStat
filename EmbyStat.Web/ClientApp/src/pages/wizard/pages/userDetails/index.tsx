import React, {
  ReactElement,
  useEffect,
  useState,
  forwardRef,
  useImperativeHandle,
} from "react";
import { Trans, useTranslation } from "react-i18next";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import Grid from "@material-ui/core/Grid";
import { useDispatch, useSelector } from "react-redux";

import { RootState } from "../../../../store/RootReducer";
import { ValidationHandle } from "../interfaces";
import { useForm } from "react-hook-form";
import { setUser } from "../../../../store/WizardSlice";

interface Props {
  disableBack: Function;
  disableNext: Function;
}

const UserDetails = forwardRef<ValidationHandle, Props>(
  (props, ref): ReactElement => {
    const { disableBack, disableNext } = props;
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const dispatch = useDispatch();
    const { t } = useTranslation();

    useEffect(() => {
      disableBack(false);
    }, [disableBack]);

    useEffect(() => {
      disableNext(username.length === 0 || password.length === 0);
    }, [disableNext, username, password]);

    const wizard = useSelector((state: RootState) => state.wizard);
    useEffect(() => {
      setUsername(wizard.username);
      setPassword(wizard.password);
    }, [wizard]);

    useImperativeHandle(ref, () => ({
      validate(): Promise<boolean> {
        dispatch(setUser(getValues("username"), getValues("password")));
        return trigger();
      },
    }));

    const { register, trigger, errors, getValues } = useForm({
      mode: "onBlur",
      defaultValues: {
        username: wizard.username,
        password: wizard.password,
      },
    });

    const getPasswordErrorMessage = (): string => {
      switch (errors.password?.type) {
        case "required":
          return t("SETTINGS.ACCOUNT.NOPASSWORD");
        case "minLength":
          return t("SETTINGS.ACCOUNT.PASSWORDMINLENGTH");
        default:
          return "";
      }
    };

    return (
      <Grid container direction="column">
        <Typography variant="h4" color="primary">
          <Trans i18nKey="SETTINGS.ACCOUNT.TITLE" />
        </Typography>
        <Typography variant="body1" className="m-t-32">
          <Trans i18nKey="WIZARD.USERDETAILTEXT" />
        </Typography>
        <Grid
          container
          direction="row"
          item
          xs={12}
          className="m-t-32"
          spacing={4}
        >
          <Grid item xs={12} md={6}>
            <TextField
              inputRef={register({ required: true })}
              label={t("SETTINGS.ACCOUNT.USERNAME")}
              size="small"
              name="username"
              error={!!errors.username}
              helperText={
                errors.username ? t("SETTINGS.ACCOUNT.NOUSERNAME") : ""
              }
              color="primary"
              defaultValue={username}
              onChange={(event) => setUsername(event.target.value as string)}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              inputRef={register({ required: true, minLength: 6 })}
              label={t("SETTINGS.ACCOUNT.PASSWORD")}
              size="small"
              type="password"
              color="primary"
              name="password"
              error={!!errors.password}
              helperText={
                errors.password
                  ? getPasswordErrorMessage()
                  : t("SETTINGS.ACCOUNT.PASSWORDMINLENGTH")
              }
              defaultValue={password}
              onChange={(event) => setPassword(event.target.value as string)}
            />
          </Grid>
        </Grid>
      </Grid>
    );
  }
);

export default UserDetails;
