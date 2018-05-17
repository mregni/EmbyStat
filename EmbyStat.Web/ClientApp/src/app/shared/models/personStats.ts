import { Card } from './card';
import { PersonPoster } from './personPoster';

export class PersonStats {
  public totalActorCount: Card;
  public totalDirectorCount: Card;
  public totalWriterCount: Card;
  public mostFeaturedActor: PersonPoster;
  public mostFeaturedDirector: PersonPoster;
  public mostFeaturedWriter: PersonPoster;
  public mostFeaturedActorsPerGenre: PersonPoster[];
}
