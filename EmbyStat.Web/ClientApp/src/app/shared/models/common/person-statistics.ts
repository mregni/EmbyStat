import { Card } from './card';
import { PersonPoster } from './person-poster';

export class PersonStatistics {
  totalActorCount: Card<number>;
  totalDirectorCount: Card<number>;
  totalWriterCount: Card<number>;
  mostFeaturedActor: PersonPoster;
  mostFeaturedDirector: PersonPoster;
  mostFeaturedWriter: PersonPoster;
  mostFeaturedActorsPerGenre: PersonPoster[];
}
