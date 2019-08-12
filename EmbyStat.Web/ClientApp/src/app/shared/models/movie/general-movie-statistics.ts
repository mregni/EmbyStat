import { Card } from '../common/card';
import { TimespanCard } from '../common/timespan-card';
import { MoviePoster } from './movie-poster';

export class GeneralMovieStatistics {
  movieCount: Card<number>;
  genreCount: Card<number>;
  boxsetCount: Card<number>;
  mostUsedContainer: Card<string>;
  highestRatedMovie: MoviePoster;
  lowestRatedMovie: Card<string>;
  longestMovie: Card<string>;
  shortestMovie: Card<string>;
  oldestPremieredMovie: Card<string>;
  newestPremieredMovie: Card<string>;
  latestAddedMovie: Card<string>;
  mostFeaturedMovieActor: Card<string>;
  mostFeaturedMovieDirector: Card<string>;
  lastPlayedMovie: Card<string>;
  totalPlayableTime: TimespanCard;
}
