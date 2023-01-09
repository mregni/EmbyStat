export interface Chart {
  title: string;
  SeriesCount: number;
  dataSets: SimpleData[];
}

export interface SimpleData {
  label: string;
  value: number;
}

export interface ComplexChart {
  title: string;
  series: string[];
  dataSets: string;
  formatString: string;
}
