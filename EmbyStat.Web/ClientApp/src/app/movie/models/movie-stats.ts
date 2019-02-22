import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespan-card';
import { MoviePoster } from '../../shared/models/movie-poster';

export class MovieStats {
  movieCount: Card<number>;
  genreCount: Card<number>;
  boxsetCount: Card<number>;
  mostUsedContainer: Card<string>;
  highestRatedMovie: MoviePoster;
  lowestRatedMovie: Card<string>;
  longestMovie: Card<string>;
  shortestMovie: Card<string>;
  oldestPremieredMovie: Card<string>;
  youngestPremieredMovie: Card<string>;
  youngestAddedMovie: Card<string>;
  mostFeaturedMovieActor: Card<string>;
  mostFeaturedMovieDirector: Card<string>;
  lastPlayedMovie: Card<string>;
  totalPlayableTime: TimespanCard;
}
