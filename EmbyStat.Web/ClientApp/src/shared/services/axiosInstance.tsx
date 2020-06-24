import axios from 'axios';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import i18n from '../../i18n';

const axiosInstance = axios.create({
  baseURL: "/api/"
});
axiosInstance.interceptors.response.use(function (response) {
  return response;
}, function (error) {
  if (error.response.status === 500)
    if (error.response.data.isError) {
      console.log(error.response.data);
      SnackbarUtils.error(i18n.t(`EXCEPTIONS.${error.response.data.message}`));
    } else {
      SnackbarUtils.error(i18n.t('EXCEPTIONS.UNHANDLED'));
    }

  if (error.response.status === 404) {
    SnackbarUtils.error(i18n.t('EXCEPTIONS.NOT_FOUND'));
  }
  return Promise.reject(error);
});


export { axiosInstance };