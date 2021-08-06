import { Language } from "../language";
import { MediaServerUdpBroadcast, Library, MediaServerInfo, MediaServerUser, initialMediaServerInfoState } from "../mediaServer";
import { LibraryContainer } from "../settings";

export interface Wizard {
  serverName: string;
  apiKey: string;
  serverAddress: string;
  serverPort: number | null;
  serverProtocol: 0 | 1;
  serverType: 0 | 1;
  serverId: string;
  userId: string;
  serverBaseurl: string;
  serverBaseUrlNeeded: boolean;
  username: string;
  password: string;
  language: string;
  languages: Language[];
  enableRollbarLogging: boolean;
  foundServers: MediaServerUdpBroadcast[];
  searchedServers: boolean;
  allLibraries: Library[];
  movieLibraries: LibraryContainer[];
  showLibraries: LibraryContainer[];
  mediaServerInfo: MediaServerInfo;
  administrators: MediaServerUser[];
}

export const initialWizardState: Wizard = {
  serverAddress: "",
  serverName: "",
  serverPort: null,
  serverProtocol: 0,
  apiKey: "",
  serverType: 0,
  userId: "",
  serverBaseurl: "",
  serverBaseUrlNeeded: false,
  username: "",
  password: "",
  language: "en-US",
  languages: [],
  enableRollbarLogging: false,
  foundServers: [],
  searchedServers: false,
  allLibraries: [],
  movieLibraries: [],
  showLibraries: [],
  serverId: "",
  mediaServerInfo: initialMediaServerInfoState,
  administrators: []
};