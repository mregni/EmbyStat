import axios from 'axios';

import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import i18n from '../../i18n';

const axiosInstance = axios.create({
  baseURL: '/api/',
  timeout: 60000,
  headers: {
    'Content-Type': 'application/json'
  }
});

axiosInstance.interceptors.request.use(
  function (request) {
    var accessToken = localStorage.getItem('accessToken');
    if (!['undefined', null].includes(accessToken)) {
      request.headers.common['Authorization'] = `Bearer ${accessToken}`;
    }
    return request;
  }
);

axiosInstance.interceptors.response.use(
  function (response) {
    return response;
  },
  function (error) {
    console.log("axios timeout");
    console.log(error);

    if (error.response == null) {
      SnackbarUtils.error(i18n.t('EXCEPTIONS.UNHANDLED'));
      return Promise.reject(error.response);
    }

    if (error.response?.status === 500)
      if (error.response.data.isError) {
        console.log(error.response.data);
        SnackbarUtils.error(
          i18n.t(`EXCEPTIONS.${error.response.data.message}`)
        );
      } else {
        SnackbarUtils.error(i18n.t('EXCEPTIONS.UNHANDLED'));
      }

    if (error.response.status === 404) {
      SnackbarUtils.error(i18n.t('EXCEPTIONS.NOT_FOUND'));
    }
    return Promise.reject(error.response);
  }
);

export { axiosInstance };
