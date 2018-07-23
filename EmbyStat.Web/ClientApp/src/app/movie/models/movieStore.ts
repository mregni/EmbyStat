import { MovieStats } from './movieStats';
import { PersonStats } from '../../shared/models/personStats';
import { Collection } from '../../shared/models/collection';
import { MovieGraphs } from '../models/movieGraphs';
import { SuspiciousMovies } from './suspiciousMovies';

export class MovieStore {
  public stats: MovieStats;
  public personStats: PersonStats;
  public collections: Collection[];
  public suspicious: SuspiciousMovies;
  public graphs: MovieGraphs;
}
