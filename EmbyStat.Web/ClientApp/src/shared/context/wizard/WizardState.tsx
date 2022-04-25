import {createContext, useEffect, useState} from 'react';

import {useSearchMediaServers} from '../../../shared/hooks';
import {Settings} from '../../../shared/models/settings';
import {Wizard} from '../../../shared/models/wizard';
import {MediaServer, MediaServerInfo, MediaServerUser} from '../../models/mediaServer';
import {register, updateSettings} from '../../services';
import {pushLibraries as pushMovieLibraries} from '../../services/movieService';
import {pushLibraries as pushShowLibraries} from '../../services/showService';

export interface WizardContextProps {
  setLanguage: (value: string) => void;
  setMonitoring: (value: boolean) => void;
  setUserDetails: (username: string, password: string) => void;
  getMediaServers: () => void;
  setMediaServerNetworkInfo: (server: MediaServer) => void;
  setMediaServerInfo: (info: MediaServerInfo) => void;
  setAdministrators: (administrators: MediaServerUser[]) => void;
  setAdministrator: (userId: string) => void;
  setMovieLibraryIds: (libraries: string[]) => Promise<void>;
  setShowLibraryIds: (libraries: string[]) => Promise<void>;
  finishWizard: (settings: Settings) => Promise<boolean>;
  mediaServersLoading: boolean;
  wizard: Wizard;
}

export const WizardContext = createContext<WizardContextProps>(null!);

export const useWizardContext = (): WizardContextProps => {
  const [wizard, setWizard] = useState<Wizard>(null!);
  const {fetchMediaServers, servers, isLoading: mediaServersLoading} = useSearchMediaServers();

  useEffect(() => {
    setWizard((prev: Wizard) => ({...prev, foundServers: servers, searchedServers: true}));
  }, [servers]);

  const setLanguage = (value: string): void => {
    setWizard((prev: Wizard) => ({...prev, language: value}));
  };

  const setMonitoring = (value: boolean): void => {
    setWizard((prev: Wizard) => ({...prev, enableRollbarLogging: value}));
  };

  const setUserDetails = (username: string, password: string): void => {
    setWizard((prev: Wizard) => ({...prev, username, password}));
  };

  const getMediaServers = async (): Promise<void> => {
    await fetchMediaServers();
  };

  const setMediaServerNetworkInfo = (server: MediaServer): void => {
    setWizard((prev: Wizard) => ({
      ...prev,
      serverId: server.id,
      serverName: server.name,
      address: server.address,
      serverType: server.type,
      apiKey: server.apiKey,
    }));
  };

  const setMediaServerInfo = (info: MediaServerInfo): void => {
    setWizard((prev: Wizard) => ({...prev, mediaServerInfo: info, serverId: info.id, serverName: info.serverName}));
  };

  const setAdministrators = (administrators: MediaServerUser[]): void => {
    setWizard((prev: Wizard) => ({...prev, administrators}));
  };

  const setAdministrator = (userId: string): void => {
    setWizard((prev: Wizard) => ({...prev, userId}));
  };

  const setMovieLibraryIds = (libraries: string[]): Promise<void> => {
    setWizard((prev: Wizard) => ({...prev, movieLibraryIds: libraries, movieLibrariesChanged: true}));
    return Promise.resolve();
  };

  const setShowLibraryIds = (libraries: string[]): Promise<void> => {
    setWizard((prev: Wizard) => ({...prev, showLibraryIds: libraries, showLibrariesChanged: true}));
    return Promise.resolve();
  };

  const finishWizard = async (settings: Settings): Promise<boolean> => {
    const newSettings: Settings = {
      ...settings,
      mediaServer: {
        ...settings.mediaServer,
        id: wizard.serverId,
        apiKey: wizard.apiKey,
        address: wizard.address,
        name: wizard.serverName,
        type: wizard.serverType,
        userId: wizard.userId,
      },
      language: wizard.language,
      enableRollbarLogging: wizard.enableRollbarLogging,
      wizardFinished: true,
    };

    try {
      const result = await register(wizard.username, wizard.password);

      result && await updateSettings(newSettings);
      result && await pushMovieLibraries(wizard.movieLibraryIds);
      result && await pushShowLibraries(wizard.showLibraryIds);

      return true;
    } catch {
      return false;
    }
  };

  return {
    setLanguage, setMonitoring, setUserDetails,
    getMediaServers, setMediaServerNetworkInfo,
    finishWizard, wizard, mediaServersLoading,
    setMovieLibraryIds, setShowLibraryIds,
    setAdministrators, setMediaServerInfo, setAdministrator,
  };
};
