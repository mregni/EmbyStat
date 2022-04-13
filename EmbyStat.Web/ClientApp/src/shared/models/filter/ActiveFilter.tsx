import {FilterOperation} from './';

export interface ActiveFilter {
  field: string;
  fieldLabel: string;
  fieldValue: string;
  operation: FilterOperation;
  value: string;
  id: string;
}
