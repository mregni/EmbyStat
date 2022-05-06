import {Language} from '../language';
import {MediaServerInfo, MediaServerUdpBroadcast, MediaServerUser} from '../mediaServer';

export interface Wizard {
  serverName: string;
  apiKey: string;
  address: string;
  serverType: 0 | 1;
  serverId: string;
  userId: string;
  username: string;
  password: string;
  language: string;
  languages: Language[];
  enableRollbarLogging: boolean;
  foundServers: MediaServerUdpBroadcast[];
  searchedServers: boolean;
  movieLibraryIds: string[];
  showLibraryIds: string[];
  mediaServerInfo: MediaServerInfo;
  administrators: MediaServerUser[];
}
