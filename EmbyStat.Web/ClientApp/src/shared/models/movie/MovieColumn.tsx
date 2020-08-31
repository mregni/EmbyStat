export interface MovieColumn {
  id: string;
  name: string;
  container: string;
  subtitles: string[];
  audioLanguages: string[];
  imdb: string;
  tmdb: string;
  tvdb: string;
  runTime: number;
  officialRating: string;
  communityRating: number | null;
  genres: string[];
  banner: string;
  logo: string;
  primary: string;
  thumb: string;
  sortName: string;
  path: string;
  premiereDate: Date | string | null;
  sizeInMb: number;
  bitRate: number;
  height: number | null;
  width: number | null;
  codec: string;
  bitDepth: number | null;
  videoRange: string;
}
