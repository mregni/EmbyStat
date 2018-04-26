import { MovieStats } from './movieStats';
import { MoviePersonStats } from './moviePersonStats';
import { Collection } from '../../shared/models/collection';

export class MovieStore {
  public stats: MovieStats;
  public personStats: MoviePersonStats;
  public collections: Collection[];
}
