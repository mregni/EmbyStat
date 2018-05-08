import { MovieStats } from './movieStats';
import { MoviePersonStats } from './moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { MovieGraphs } from '../models/movieGraphs';
import { SuspiciousMovies } from './suspiciousMovies';

export class MovieStore {
  public stats: MovieStats;
  public personStats: MoviePersonStats;
  public collections: Collection[];
  public suspicious: SuspiciousMovies;
  public graphs: MovieGraphs;
}
