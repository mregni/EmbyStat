import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespanCard';
import { MoviePoster } from '../../shared/models/moviePoster';

export class MovieStats {
  public movieCount: Card ;
  public genreCount: Card ;
  public boxsetCount: Card ;
  public mostUsedContainer: Card ;
  public highestRatedMovie: MoviePoster ;
  public lowestRatedMovie: Card ;
  public longestMovie: Card ;
  public shortestMovie: Card ;
  public oldestPremieredMovie: Card ;
  public youngestPremieredMovie: Card ;
  public youngestAddedMovie: Card ;
  public mostFeaturedMovieActor: Card ;
  public mostFeaturedMovieDirector: Card ;
  public lastPlayedMovie: Card ;
  public totalPlayableTime: TimespanCard ;
}
