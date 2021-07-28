export interface ShowRow {
  id: string;
  name: string;
  collectedEpisodeCount: number;
  cumulativeRunTimeTicks: number;
  genres: string[];
  missingEpisodesCount: number;
  officialRating: string;
  runTime: number;
  sortName: string;
  specialEpisodeCount: number;
  status: string;
}
