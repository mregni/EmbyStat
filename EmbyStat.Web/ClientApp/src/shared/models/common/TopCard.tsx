export interface LabelValuePair {
  label: string;
  value: string;
}

export interface TopCard {
  mediaId: string;
  title: string;
  image: string;
  unit: string;
  unitNeedsTranslation: boolean;
  values: LabelValuePair[];
  valueType: number;
}