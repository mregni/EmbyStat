export interface ActiveFilter {
  field: string;
  fieldLabel: string;
  operation:
  | 'any'
  | '!any'
  | 'null'
  | '!null'
  | 'between'
  | 'in'
  | 'contains'
  | '!contains'
  | 'empty'
  | '=='
  | '!='
  | '<'
  | '>'
  | 'startsWith'
  | 'endsWith';
  operationLabel: string;
  value: string;
  valueLabel: string;
  id: string;
  visible: boolean;
  unit?: string;
}
