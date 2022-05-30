import {createContext, Dispatch, SetStateAction, useEffect, useState} from 'react';

import {Language} from '../../models/language';
import {Settings, SystemConfig, UserConfig} from '../../models/settings';
import {getLanguages, getSettings, updateSettings} from '../../services';

export interface SettingsContextProps {
  load: (force?: boolean) => Promise<void>;
  save: (settings: UserConfig) => Promise<void>;
  userConfig: UserConfig;
  systemConfig: SystemConfig;
  isLoaded: boolean;
  languages: Language[];
  resetFinished: boolean;
  setResetLogLine: Dispatch<SetStateAction<string>>;
  setResetFinished: Dispatch<SetStateAction<boolean>>;
  resetLogLine: string;
}

export const SettingsContext = createContext<SettingsContextProps>(null!);

export const useSettingsContext = (): SettingsContextProps => {
  const [settings, setSettings] = useState<Settings>(null!);
  const [userConfig, setUserConfig] = useState<UserConfig>(null!);
  const [systemConfig, setSystemConfig] = useState<SystemConfig>(null!);
  const [languages, setLanguages] = useState<Language[]>([]);
  const [isLoaded, setIsLoaded] = useState(false);
  const [resetFinished, setResetFinished] = useState(false);
  const [resetLogLine, setResetLogLine] = useState<string>('');

  useEffect(() => {
    if (resetFinished) {
      setResetFinished(false);
    }
  }, [resetFinished]);

  const load = async (force: boolean = false): Promise<void> => {
    if (!isLoaded || force) {
      const serverSettings = await getSettings();
      setIsLoaded(true);
      setSettings(serverSettings);
      setUserConfig(serverSettings.userConfig);
      setSystemConfig(serverSettings.systemConfig);

      const response = await getLanguages();
      setLanguages(response.data);
    }
  };

  const save = async (userConfig: UserConfig) => {
    await updateSettings(userConfig);
    await load(true);
  };

  return {
    load, save, userConfig, systemConfig, languages,
    setResetFinished, resetFinished, setResetLogLine, resetLogLine,
    isLoaded,
  };
};
