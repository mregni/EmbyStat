export interface FilterItem {
  label: string;
  value: string;
}

export interface FilterOperation {
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
  label: string;
  type: 'text' | 'none' | 'dropdown' | 'range' | 'number';
  itemType?: 'url' | 'static';
  items?: FilterItem[];
  itemUrl?: string;
  unit?: string;
  id: string;
  open: boolean;
}

export interface FilterDefinition {
  field: string;
  label: string;
  title: string;
  operations: FilterOperation[];
  open: boolean;
  id: string;
}
