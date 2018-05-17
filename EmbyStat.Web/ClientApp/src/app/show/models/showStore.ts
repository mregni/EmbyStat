import { Collection } from '../../shared/models/collection';
import { ShowStats } from './showStats';
import { ShowGraphs } from './showGraphs';
import { PersonStats } from '../../shared/models/personStats';

export class ShowStore {
  public collections: Collection[];
  public showStats: ShowStats;
  public graphs: ShowGraphs;
  public personStats: PersonStats;
}
