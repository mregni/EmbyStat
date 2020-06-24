export interface MovieColumn {
  id: string;
  originalTitle: string;
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
  resolutions: string[];
  sizeInMb: number;
}