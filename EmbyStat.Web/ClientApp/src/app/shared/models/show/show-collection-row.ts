import { Season } from './season';

export class ShowCollectionRow {
    id: string;
    title: string;
    seasons: number;
    episodes: number;
    missingEpisodeCount: number;
    missingEpisodes: Season[];
    premiereDate: Date;
    status: boolean;
    sortName: string;
    specials: number;
    percentage: number;
    tvdb: string;
    imdb: string;
    size: number;
    banner: string;
}
