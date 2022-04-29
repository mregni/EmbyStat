import {createContext, Dispatch, SetStateAction, useEffect, useState} from 'react';

import {Language} from '../../models/language';
import {Settings} from '../../models/settings';
import {getLanguages, getSettings, updateSettings} from '../../services';

export interface SettingsContextProps {
  load: (force?: boolean) => Promise<void>;
  save: (settings: Settings) => Promise<void>;
  settings: Settings;
  languages: Language[];
  resetFinished: boolean;
  setResetLogLine: Dispatch<SetStateAction<string>>;
  setResetFinished: Dispatch<SetStateAction<boolean>>;
  resetLogLine: string;
}

export const SettingsContext = createContext<SettingsContextProps>(null!);

export const useSettingsContext = (): SettingsContextProps => {
  const [settings, setSettings] = useState<Settings>(null!);
  const [languages, setLanguages] = useState<Language[]>([]);
  const [resetFinished, setResetFinished] = useState(false);
  const [resetLogLine, setResetLogLine] = useState<string>('');

  useEffect(() => {
    if (resetFinished) {
      setResetFinished(false);
    }
  }, [resetFinished]);

  const load = async (force: boolean = false): Promise<void> => {
    if (!settings?.isLoaded || force) {
      const serverSettings = await getSettings();
      serverSettings.isLoaded = true;
      setSettings(serverSettings);

      const response = await getLanguages();
      setLanguages(response.data);
    }
  };

  const save = async (settings: Settings) => {
    await updateSettings(settings);
    await load(true);
  };

  return {
    load, save, settings, languages,
    setResetFinished, resetFinished, setResetLogLine, resetLogLine,
  };
};
