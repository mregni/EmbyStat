import { Episode } from './episode';

export class ShowCollectionRow {
    id: string;
    title: string;
    seasons: number;
    episodes: number;
    missingEpisodes: Episode[];
    premiereDate: Date;
    status: boolean;
    sortName: string;
    specials: number;
    percentage: number;
  }
