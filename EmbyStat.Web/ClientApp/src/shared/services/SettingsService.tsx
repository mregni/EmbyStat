import axios from 'axios';
import { Settings } from '../../shared/models/settings';
import { Language } from '../../shared/models/language';


const baseUrl = 'http://localhost:6555/api/settings/';

export const getSettings = async (): Promise<Settings> => {
  const response = await axios.get<Settings>(`${baseUrl}`);
  return response.data;
}

export const updateSettings = async (userSettings: Settings): Promise<Settings> => {
  const config = { headers: { 'Content-Type': 'application/json' } };
  const response = await axios.put<Settings>(`${baseUrl}`, userSettings, config);
  return response.data;
}

export const getLanguages = async (): Promise<Language[]> => {
  const response = await axios.get<Language[]>(`${baseUrl}languages`);
  return response.data;
}