import axios from 'axios';

import i18n from '../../i18n';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';

const axiosInstance = axios.create({
  baseURL: '/api/',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (request) => {
    const accessToken = localStorage.getItem('accessToken');
    if (!['undefined', null].includes(accessToken)) {
      if (request.headers === undefined) {
        request.headers = {};
      }

      request.headers.common['Authorization'] = `Bearer ${accessToken}`;
    }
    return request;
  },
);

axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response == null) {
      SnackbarUtils.error(i18n.t('EXCEPTIONS.UNHANDLED'));
      return Promise.reject(error.response);
    }

    if (error.response?.status === 500) {
      if (error.response.data.isError) {
        SnackbarUtils.error(
          i18n.t(`EXCEPTIONS.${error.response.data.message}`),
        );
      } else {
        SnackbarUtils.error(i18n.t('EXCEPTIONS.UNHANDLED'));
      }
    }

    if (error.response.status === 404) {
      SnackbarUtils.error(i18n.t('EXCEPTIONS.NOT_FOUND'));
    }
    return Promise.reject(error.response);
  },
);

export {axiosInstance};
