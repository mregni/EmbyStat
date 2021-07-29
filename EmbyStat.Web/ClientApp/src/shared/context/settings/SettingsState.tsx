import { createContext, useState } from 'react';
import { initialSettingsState, Settings } from '../../models/settings';
import { getSettings, updateSettings } from '../../services/SettingsService';

export interface SettingsContextProps {
  load: () => Promise<Settings>;
  save: (settings: Settings) => Promise<void>;
  settings: Settings;
}

export const SettingsContext = createContext<SettingsContextProps>({
  load: () => Promise.resolve(initialSettingsState),
  save: () => Promise.resolve(),
  settings: initialSettingsState
});

export const useSettingsContext = (): SettingsContextProps => {
  const [settings, setSettings] = useState(initialSettingsState);

  const load = async (): Promise<Settings> => {
    var serverSettings = await getSettings();
    serverSettings.isLoaded = true;
    setSettings(serverSettings);
    return serverSettings;
  }

  const save = async (settings: Settings) => {
    await updateSettings(settings);
  }

  return {
    load, save, settings
  };
}