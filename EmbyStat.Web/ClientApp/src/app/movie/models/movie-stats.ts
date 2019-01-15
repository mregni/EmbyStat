import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespan-card';
import { MoviePoster } from '../../shared/models/movie-poster';

export class MovieStats {
  movieCount: Card ;
  genreCount: Card ;
  boxsetCount: Card ;
  mostUsedContainer: Card ;
  highestRatedMovie: MoviePoster ;
  lowestRatedMovie: Card ;
  longestMovie: Card ;
  shortestMovie: Card ;
  oldestPremieredMovie: Card ;
  youngestPremieredMovie: Card ;
  youngestAddedMovie: Card ;
  mostFeaturedMovieActor: Card ;
  mostFeaturedMovieDirector: Card ;
  lastPlayedMovie: Card ;
  totalPlayableTime: TimespanCard ;
}
