export interface Chart {
  title: string;
  SeriesCount: number;
  dataSets: SimpleData[];
}

export interface SimpleData {
  label: string;
  value: number;
}
