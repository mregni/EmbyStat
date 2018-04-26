import { Card } from '../../shared/models/card';
import { PersonPoster } from '../../shared/models/personPoster';

export class MoviePersonStats {
  public totalActorCount: Card;
  public totalDirectorCount: Card;
  public totalWriterCount: Card;
  public mostFeaturedActor: PersonPoster;
  public mostFeaturedDirector: PersonPoster;
  public mostFeaturedWriter: PersonPoster;
}
