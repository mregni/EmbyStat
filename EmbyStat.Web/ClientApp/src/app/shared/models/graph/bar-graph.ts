import { DataSet } from './data-set';

export class BarGraph<T>
{
  title: string;
  labels: string[];
  datasets: DataSet<T>[];
}
