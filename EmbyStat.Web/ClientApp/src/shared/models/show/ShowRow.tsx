export interface ShowRow {
  id: string;
  name: string;
  sortName: string;
  status: string;
  seasonCount: number;
  episodeCount: number;
  specialEpisodeCount: number;
  missingEpisodeCount: number;
  runTime: number;
  cumulativeRunTime: number;
  genres: string[];
  officialRating: string;
  sizeInMb: number;
}
