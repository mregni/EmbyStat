import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespanCard';
import { Poster } from '../../shared/models/poster';

export class MovieStats {
  public movieCount: Card ;
  public genreCount: Card ;
  public boxsetCount: Card ;
  public mostUsedContainer: Card ;
  public highestRatedMovie: Poster ;
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
