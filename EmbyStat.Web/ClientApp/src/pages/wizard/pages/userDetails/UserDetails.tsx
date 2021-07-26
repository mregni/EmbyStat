import React, {
  ReactElement,
  useEffect,
  useState,
  forwardRef,
  useImperativeHandle,
} from "react";
import { Trans, useTranslation } from "react-i18next";
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import { useDispatch, useSelector } from "react-redux";

import { RootState } from "../../../../store/RootReducer";
import { ValidationHandle } from "../interfaces";
import { useForm } from "react-hook-form";
import { setUser } from "../../../../store/WizardSlice";
import { EsTextInput } from "../../../../shared/components/esTextInput";

interface Props {
  disableBack: Function;
  disableNext: Function;
}

export const UserDetails = forwardRef<ValidationHandle, Props>(
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

    const { register, trigger, getValues, formState: { errors } } = useForm({
      mode: "onBlur",
      defaultValues: {
        username: wizard.username,
        password: wizard.password,
      },
    });

    const usernameRegister = register('username', { required: true });
    const passwordRegister = register('password', { required: true, minLength: 6 });

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
            <EsTextInput
              inputRef={usernameRegister}
              label={t("SETTINGS.ACCOUNT.USERNAME")}
              error={errors.username}
              defaultValue={getValues('username')}
              onChange={(value: string) => setUsername(value)}
              errorText={{
                required: t("SETTINGS.ACCOUNT.NOUSERNAME")
              }}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <EsTextInput
              inputRef={passwordRegister}
              label={t("SETTINGS.ACCOUNT.PASSWORD")}
              error={errors.password}
              type="password"
              defaultValue={getValues('password')}
              onChange={(value: string) => setPassword(value)}
              errorText={{
                required: t("SETTINGS.ACCOUNT.NOPASSWORD"),
                minLength: t("SETTINGS.ACCOUNT.PASSWORDMINLENGTH")
              }}
            />
          </Grid>
        </Grid>
      </Grid>
    );
  }
);
