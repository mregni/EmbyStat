import {AxiosResponse} from 'axios';
import i18n from 'i18next';

import {Language} from '../models/language';
import {Settings} from '../models/settings';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import {axiosInstance} from './axiosInstance';

const domain = 'settings/';

export const getSettings = async (): Promise<Settings> => {
  const response = await axiosInstance.get<Settings>(`${domain}`);
  return response.data;
};

export const updateSettings = (userSettings: Settings): Promise<Settings | void> => {
  const config = {headers: {'Content-Type': 'application/json'}};
  return axiosInstance.put<Settings>(`${domain}`, userSettings, config)
    .then((response) => {
      if (response.status === 200) {
        SnackbarUtils.success(i18n.t('SETTINGS.SAVED'));
      }
      return response.data;
    })
    .catch((x) => {
      SnackbarUtils.error(i18n.t('SETTINGS.SAVEFAILED'));
    });
};

export const getLanguages = (): Promise<AxiosResponse<Language[]>> => {
  return axiosInstance.get<Language[]>(`${domain}languages`);
};