import { Graph } from '../../shared/models/graph/graph';
import { SimpleGraphValue } from '../../shared/models/graph/simpleGraphValue';


export class ShowGraphs {
  public barGraphs: Graph<SimpleGraphValue>;
  public pieGraphs: Graph<SimpleGraphValue>;
}
