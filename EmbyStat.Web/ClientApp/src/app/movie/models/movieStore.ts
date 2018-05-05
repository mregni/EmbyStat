import { MovieStats } from './movieStats';
import { MoviePersonStats } from './moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { Duplicate } from './graphs/duplicate';

export class MovieStore {
  public stats: MovieStats;
  public personStats: MoviePersonStats;
  public collections: Collection[];
  public duplicates: Duplicate[];
}
