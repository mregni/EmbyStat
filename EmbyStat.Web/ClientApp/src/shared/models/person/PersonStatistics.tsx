import { Card } from '../common';
import { PersonPoster } from '.';

export interface PersonStatistics {
  cards: Card[];
  posters: PersonPoster[];
  mostFeaturedActorsPerGenre: PersonPoster[];
}
