export interface FilterValue {
  value: string;
  label: string;
}

export interface FilterType {
  id: number;
  label: string;
  field: string;
  type: number;
  values: FilterValue[];
  actions: FilterValue[];
}
