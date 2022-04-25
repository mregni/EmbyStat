import {OptionsObject, useSnackbar, WithSnackbarProps} from 'notistack';
import React from 'react';

interface IProps {
  setUseSnackbarRef: (showSnackbar: WithSnackbarProps) => void;
}

const InnerSnackbarUtilsConfigurator: React.FC<IProps> = (props: IProps) => {
  props.setUseSnackbarRef(useSnackbar());
  return null;
};

let useSnackbarRef: WithSnackbarProps;
const setUseSnackbarRef = (useSnackbarRefProp: WithSnackbarProps) => {
  useSnackbarRef = useSnackbarRefProp;
};

export function SnackbarUtilsConfigurator() {
  return (
    <InnerSnackbarUtilsConfigurator setUseSnackbarRef={setUseSnackbarRef} />
  );
}

export default {
  success(msg: string, options: OptionsObject = {}) {
    this.toast(msg, {...options, variant: 'success'});
  },
  warning(msg: string, options: OptionsObject = {}) {
    this.toast(msg, {...options, variant: 'warning'});
  },
  info(msg: string, options: OptionsObject = {}) {
    this.toast(msg, {...options, variant: 'info'});
  },
  error(msg: string, options: OptionsObject = {}) {
    this.toast(msg, {...options, variant: 'error'});
  },
  toast(msg: string, options: OptionsObject = {}) {
    useSnackbarRef.enqueueSnackbar(msg, options);
  },
};
