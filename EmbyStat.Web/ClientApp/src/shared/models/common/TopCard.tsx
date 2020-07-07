

export interface TopCardItem {
  mediaId: string;
  image: string;
  label: string;
  value: string;
}

export interface TopCard {
  title: string;
  unit: string;
  unitNeedsTranslation: boolean;
  values: TopCardItem[];
  valueType: number;
}