import { Card } from './card';
import { PersonPoster } from './person-poster';

export class PersonStats {
  totalActorCount: Card;
  totalDirectorCount: Card;
  totalWriterCount: Card;
  mostFeaturedActor: PersonPoster;
  mostFeaturedDirector: PersonPoster;
  mostFeaturedWriter: PersonPoster;
  mostFeaturedActorsPerGenre: PersonPoster[];
}
