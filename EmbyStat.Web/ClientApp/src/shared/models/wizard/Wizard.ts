import { MediaServerUdpBroadcast, Library } from "../mediaServer";

export interface Wizard {
  serverName: string;
  apiKey: string;
  serverAddress: string;
  serverPort: number | string;
  serverProtocol: number;
  serverType: number;
  serverId: string;
  userId: string;
  serverBaseurl: string;
  serverBaseUrlNeeded: boolean;
  username: string;
  password: string;
  language: string;
  enableRollbarLogging: boolean;
  foundServers: MediaServerUdpBroadcast[];
  searchedServers: boolean;
  allLibraries: Library[];
  movieLibraries: string[];
  showLibraries: string[];
  loadedMovieLibraryStep: boolean;
  loadedShowLibraryStep: boolean;
}
