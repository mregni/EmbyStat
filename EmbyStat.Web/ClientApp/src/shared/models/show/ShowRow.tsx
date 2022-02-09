export interface ShowRow {
  id: string;
  name: string;
  sortName: string;
  tvdb: string;
  imdb: string;
  status: string;
  seasonCount: number;
  episodeCount: number;
  specialEpisodeCount: number;
  missingEpisodeCount: number;
  runTimeTicks: number;
  cumulativeRunTimeTicks: number;
  genres: string[];
  officialRating: string;
  sizeInMb: number;
}
