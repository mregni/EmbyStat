import { MovieStats } from './movieStats';
import { MoviePersonStats } from './moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { Graph } from '../../shared/models/graph';
import { Duplicate } from './duplicate';

export class MovieStore {
  public stats: MovieStats;
  public personStats: MoviePersonStats;
  public collections: Collection[];
  public duplicates: Duplicate[];
  public graphs: Graph[];
}
