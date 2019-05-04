import { Card } from '../common/card';
import { PersonPoster } from '../common/person-poster';

export class MoviePeopleStatistics {
    totalActorCount: Card<number>;
    totalDirectorCount: Card<number>;
    totalWriterCount: Card<number>;
    mostFeaturedActor: PersonPoster;
    mostFeaturedDirector: PersonPoster;
    mostFeaturedWriter: PersonPoster;
    mostFeaturedActorsPerGenre: PersonPoster[];
}
