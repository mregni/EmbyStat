import { createContext, useEffect, useMemo, useState } from 'react';

import { Wizard, initialWizardState } from '../../../shared/models/wizard';
import { Library, MediaServer, MediaServerInfo, MediaServerUser } from '../../../shared/models/mediaServer';
import { useSearchMediaServers } from '../../../shared/hooks';
import { Settings } from '../../../shared/models/settings';
import { register } from '../../../shared/services/AccountService';
import { updateSettings } from '../../../shared/services/SettingsService';

export interface WizardContextProps {
  setLanguage: (value: string) => void;
  setMonitoring: (value: boolean) => void;
  setUserDetails: (username: string, password: string) => void;
  getMediaServers: () => void;
  setMediaServerNetworkInfo: (server: MediaServer) => void;
  setMediaServerInfo: (info: MediaServerInfo) => void;
  setAdministrators: (administrators: MediaServerUser[]) => void;
  setLibraries: (libraries: Library[]) => void;
  setAdministrator: (userId: string) => void;
  setMovieLibraries: (libraries: string[]) => void;
  setShowLibraries: (libraries: string[]) => void;
  finishWizard: (settings: Settings) => Promise<boolean>;
  fullServerUrl: string;
  mediaServersLoading: boolean;
  wizard: Wizard;
}

export const WizardContext = createContext<WizardContextProps>({
  setLanguage: () => { },
  setMonitoring: () => { },
  setUserDetails: () => { },
  getMediaServers: () => { },
  setMediaServerNetworkInfo: () => { },
  setMediaServerInfo: () => { },
  setAdministrators: () => { },
  setLibraries: () => { },
  setAdministrator: () => { },
  setMovieLibraries: () => { },
  setShowLibraries: () => { },
  finishWizard: () => Promise.resolve(false),
  fullServerUrl: "",
  mediaServersLoading: true,
  wizard: initialWizardState
});

export const useWizardContext = (): WizardContextProps => {
  const [wizard, setWizard] = useState<Wizard>(initialWizardState);
  const { fetchMediaServers, servers, isLoading: mediaServersLoading } = useSearchMediaServers();

  useEffect(() => {
    setWizard((prev: Wizard) => ({ ...prev, foundServers: servers, searchedServers: true }));
  }, [servers]);

  const setLanguage = (value: string): void => {
    setWizard((prev: Wizard) => ({ ...prev, language: value }));
  }

  const setMonitoring = (value: boolean): void => {
    setWizard((prev: Wizard) => ({ ...prev, enableRollbarLogging: value }));
  }

  const setUserDetails = (username: string, password: string): void => {
    setWizard((prev: Wizard) => ({ ...prev, username: username, password: password }));
  }

  const getMediaServers = async (): Promise<void> => {
    await fetchMediaServers();
  }

  const setMediaServerNetworkInfo = (server: MediaServer): void => {
    setWizard((prev: Wizard) => ({
      ...prev,
      serverId: server.id,
      serverName: server.name,
      serverAddress: server.address,
      serverBaseUrlNeeded: server.baseUrlNeeded,
      serverBaseurl: server.baseUrl,
      serverPort: server.port,
      serverProtocol: server.protocol,
      serverType: server.type,
      apiKey: server.apiKey
    }))
  }

  const setMediaServerInfo = (info: MediaServerInfo): void => {
    setWizard((prev: Wizard) => ({ ...prev, mediaServerInfo: info, serverId: info.id, serverName: info.serverName }));
  }

  const setAdministrators = (administrators: MediaServerUser[]): void => {
    setWizard((prev: Wizard) => ({ ...prev, administrators: administrators }));
  }

  const setAdministrator = (userId: string): void => {
    setWizard((prev: Wizard) => ({ ...prev, userId: userId }));
  }

  const setLibraries = (libraries: Library[]): void => {
    setWizard((prev: Wizard) => ({ ...prev, allLibraries: libraries }));
  }

  const setMovieLibraries = (libraries: string[]): void => {
    setWizard((prev: Wizard) => ({ ...prev, movieLibraries: libraries, movieLibrariesChanged: true }));
  }

  const setShowLibraries = (libraries: string[]): void => {
    setWizard((prev: Wizard) => ({ ...prev, showLibraries: libraries, showLibrariesChanged: true }));
  }

  const fullServerUrl = useMemo((): string => {
    const protocol = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    return `${protocol}${wizard.serverAddress}:${wizard.serverPort}${wizard?.serverBaseurl ?? ''}`
  }, [wizard.serverProtocol, wizard.serverAddress, wizard.serverPort, wizard?.serverBaseurl]);

  const finishWizard = async (settings: Settings): Promise<boolean> => {
    console.log(settings);
    const newSettings: Settings = {
      ...settings,
      mediaServer: {
        ...settings.mediaServer,
        serverId: wizard.serverId,
        apiKey: wizard.apiKey,
        serverAddress: wizard.serverAddress,
        serverBaseUrl: wizard.serverBaseurl != null ? wizard.serverBaseurl : "",
        serverName: wizard.serverName,
        serverPort:
          typeof wizard.serverPort === "number"
            ? wizard.serverPort
            : 8096,
        serverProtocol: wizard.serverProtocol,
        serverType: wizard.serverType,
        userId: wizard.userId,
      },
      movieLibraries: wizard.movieLibraries,
      showLibraries: wizard.showLibraries,
      language: wizard.language,
      enableRollbarLogging: wizard.enableRollbarLogging,
      wizardFinished: true
    }

    try {
      const result = await register({
        username: wizard.username,
        password: wizard.password
      });

      result && await updateSettings(newSettings);
      return true;
    }
    catch {
      return false;
    }
  }

  return {
    setLanguage, getMediaServers, mediaServersLoading,
    setMonitoring, setUserDetails, setMediaServerNetworkInfo,
    setMediaServerInfo, setAdministrators, setLibraries,
    setAdministrator, setMovieLibraries, setShowLibraries,
    fullServerUrl, finishWizard, wizard
  };
} 