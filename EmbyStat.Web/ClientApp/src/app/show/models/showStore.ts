import { Collection } from '../../shared/models/collection';
import { ShowStats } from './showStats';
import { ShowGraphs } from './showGraphs';

export class ShowStore {
  public collections: Collection[];
  public showStats: ShowStats;
  public graphs: ShowGraphs;
}
