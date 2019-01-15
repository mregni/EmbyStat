export class Configuration {
  id: string;
  wizardFinished: boolean;
  accessToken: string;
  embyUserName: string;
  embyServerAddress: string;
  username: string;
  language: string;
  serverName: string;
  isLoaded: boolean;
  embyUserId: string;
  toShortMovie: number;
  movieCollectionTypes: number[];
  showCollectionTypes: number[];
  tvdbApiKey: string;
  embyServerPort: number;
  embyServerProtocol: number;
  lastTvdbUpdate: Date;
  keepLogsCount: number;
  autoUpdate: boolean;
  updateTrain: number;
  updateInProgress: boolean;
}
