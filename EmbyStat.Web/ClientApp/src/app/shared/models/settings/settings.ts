import { MediaServerSettings } from './media-server-settings';
import { TvdbSettings } from './tvdb-settings';

export class Settings {
  id: string;
  appName: string;
  wizardFinished: boolean;
  username: string;
  language: string;
  toShortMovie: number;
  toShortMovieEnabled: boolean;
  keepLogsCount: number;
  movieLibraryTypes: number[];
  showLibraryTypes: number[];
  autoUpdate: boolean;
  updateTrain: number;
  updateInProgress: boolean;
  version: string;
  mediaServer: MediaServerSettings;
  tvdb: TvdbSettings;
  enableRollbarLogging: boolean ;
  isLoaded: boolean;
  noUpdates: boolean;

  constructor() {
    this.mediaServer = new MediaServerSettings();
    this.tvdb = new TvdbSettings();
  }
}
