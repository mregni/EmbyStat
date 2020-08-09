export interface FilterItem {
  label: string;
  value: string;
}

export interface FilterType {
  operation:
    | "any"
    | "!any"
    | "null"
    | "!null"
    | "between"
    | "in"
    | "contains"
    | "!contains"
    | "empty"
    | "=="
    | "!="
    | "<"
    | ">"
    | "startsWith"
    | "endsWith";
  label: string;
  type: "txt" | "none" | "dropdown" | "range" | "number" | "date" | "dateRange";
  itemType?: "url" | "static";
  items?: FilterItem[];
  itemUrl?: string;
  min?: number;
  max?: number;
  step?: number;
  unit?: string;
  id: string;
  open: boolean;
  placeholder?: string;
}

export interface FilterDefinition {
  field: string;
  label: string;
  title: string;
  types: FilterType[];
  open: boolean;
  id: string;
}
