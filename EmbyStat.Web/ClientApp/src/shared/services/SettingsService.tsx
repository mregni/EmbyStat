import { axiosInstance } from './axiosInstance';
import { Settings } from '../models/settings';
import { Language } from '../models/language';

const domain = 'settings/';

export const getSettings = async (): Promise<Settings> => {
  const response = await axiosInstance.get<Settings>(`${domain}`);
  return response.data;
};

export const updateSettings = async (
  userSettings: Settings
): Promise<Settings> => {
  const config = { headers: { 'Content-Type': 'application/json' } };
  const response = await axiosInstance.put<Settings>(
    `${domain}`,
    userSettings,
    config
  );
  return response.data;
};

export const getLanguages = async (): Promise<Language[]> => {
  const response = await axiosInstance.get<Language[]>(`${domain}languages`);
  return response.data;
};
