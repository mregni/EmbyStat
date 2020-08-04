import i18n from 'i18next';

import { axiosInstance } from './axiosInstance';
import { Settings } from '../models/settings';
import { Language } from '../models/language';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';

const domain = 'settings/';

export const getSettings = async (): Promise<Settings> => {
  const response = await axiosInstance.get<Settings>(`${domain}`);
  return response.data;
};

export const updateSettings = async (
  userSettings: Settings
): Promise<Settings> => {
  const config = { headers: { 'Content-Type': 'application/json' } };
  return await axiosInstance.put<Settings>(
    `${domain}`,
    userSettings,
    config
  ).then((response) => {
    if (response.status === 200) {
      SnackbarUtils.success(i18n.t('SETTINGS.SAVED'))
    }
    return response.data;
  });
};

export const getLanguages = async (): Promise<Language[]> => {
  return await axiosInstance.get<Language[]>(`${domain}languages`).then(response => response.data);
};
