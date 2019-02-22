import { EmbySettings } from './emby-settings';
import { TvdbSettings } from './tvdb-settings';

export class Settings {
  id: string;
  appName: string;
  wizardFinished: boolean;
  username: string;
  language: string;
  toShortMovie: number;
  keepLogsCount: number;
  movieCollectionTypes: number[];
  showCollectionTypes: number[];
  autoUpdate: boolean;
  updateTrain: number;
  updateInProgress: boolean;
  version: string;
  emby: EmbySettings;
  tvdb: TvdbSettings;
  enableRollbarLogging: boolean ;
  isLoaded: boolean;
}
