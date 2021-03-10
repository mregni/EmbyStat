import uuid from 'react-uuid';

import { FilterDefinition } from '../models/filter';

const showFilters: FilterDefinition[] = [
  {
    field: "Name",
    label: "Title {0} {1}",
    title: "FILTERS.TITLES.TITLE",
    types: [
      {
        operation: "contains",
        label: "FILTERS.CONTAINS",
        type: "txt",
        open: false,
        id: uuid(),
        placeholder: "FILTERS.TITLES.TITLE",
      },
      {
        operation: "!contains",
        label: "FILTERS.NOTCONTAINS",
        type: "txt",
        open: false,
        id: uuid(),
        placeholder: "FILTERS.TITLES.TITLE",
      },
      {
        operation: "==",
        label: "FILTERS.EQUAL",
        type: "txt",
        open: false,
        id: uuid(),
        placeholder: "FILTERS.TITLES.TITLE",
      },
      {
        operation: "startsWith",
        label: "FILTERS.STARTSWITH",
        type: "txt",
        open: false,
        id: uuid(),
        placeholder: "FILTERS.TITLES.TITLE",
      },
      {
        operation: "endsWith",
        label: "FILTERS.ENDSWITH",
        type: "txt",
        open: false,
        id: uuid(),
        placeholder: "FILTERS.TITLES.TITLE",
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "CommunityRating",
    label: "Community rating {0} {1}",
    title: "COMMON.COMMUNITYRATING",
    types: [
      {
        operation: "==",
        label: "FILTERS.IS",
        type: "number",
        unit: "",
        open: false,
        id: uuid(),
        placeholder: "COMMON.COMMUNITYRATING",
      },
      {
        operation: "between",
        label: "FILTERS.BETWEEN",
        type: "range",
        unit: "",
        min: 0,
        max: 10,
        step: 0.5,
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "SizeInMb",
    label: "Size {0} {1}",
    title: "COMMON.SIZE",
    types: [
      {
        operation: ">",
        label: "FILTERS.LARGER",
        type: "number",
        unit: "COMMON.GB",
        open: false,
        id: uuid(),
      },
      {
        operation: "<",
        label: "FILTERS.SMALLER",
        type: "number",
        unit: "COMMON.GB",
        open: false,
        id: uuid(),
      },
      {
        operation: "between",
        label: "FILTERS.BETWEEN",
        type: "range",
        min: 0,
        max: 200,
        step: 1,
        unit: "COMMON.GB",
        open: false,
        id: uuid(),
      },
      {
        operation: "null",
        label: "FILTERS.ISZERO",
        type: "none",
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
];

export default showFilters;
