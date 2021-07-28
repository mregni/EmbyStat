import { createContext, useState } from 'react';
import { initialSettingsState, Settings } from '../../models/settings';
import { getSettings } from '../../services/SettingsService';

export interface SettingsContextProps {
  loadSettings: () => Promise<Settings>;
  settings: Settings;
}

export const SettingsContext = createContext<SettingsContextProps>({
  loadSettings: () => Promise.resolve(initialSettingsState),
  settings: initialSettingsState
});

export const useSettingsContext = (): SettingsContextProps => {
  const [settings, setSettings] = useState(initialSettingsState);

  const loadSettings = async (): Promise<Settings> => {
    var serverSettings = await getSettings();
    serverSettings.isLoaded = true;
    setSettings(serverSettings);
    return serverSettings;
  }

  return {
    loadSettings, settings
  };
}