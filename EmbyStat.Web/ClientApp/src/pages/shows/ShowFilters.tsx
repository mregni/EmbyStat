import {v4 as uuid} from 'uuid';

import {FilterDefinition} from '../../shared/models/filter';

export const showFilters: FilterDefinition[] = [
  {
    field: 'Name',
    label: 'Title {0} {1}',
    title: 'FILTERS.TITLES.TITLE',
    operations: [
      {
        operation: 'contains',
        label: 'FILTERS.OPERATIONS.CONTAINS',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: '!contains',
        label: 'FILTERS.OPERATIONS.NOTCONTAINS',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: '==',
        label: 'FILTERS.OPERATIONS.EQUAL',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: 'startsWith',
        label: 'FILTERS.OPERATIONS.STARTSWITH',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: 'endsWith',
        label: 'FILTERS.OPERATIONS.ENDSWITH',
        type: 'text',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'IMDB',
    label: 'Imdb {0} {1}',
    title: 'FILTERS.TITLES.IMDB',
    operations: [
      {
        operation: '==',
        label: 'FILTERS.OPERATIONS.IS',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: 'null',
        label: 'FILTERS.OPERATIONS.ISNULL',
        type: 'none',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'TMDB',
    label: 'Tmdb {0} {1}',
    title: 'FILTERS.TITLES.TMDB',
    operations: [
      {
        operation: '==',
        label: 'FILTERS.OPERATIONS.IS',
        type: 'text',
        open: false,
        id: uuid(),
      },
      {
        operation: 'null',
        label: 'FILTERS.OPERATIONS.ISNULL',
        type: 'none',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'Images',
    label: 'Images {0} {1}',
    title: 'FILTERS.TITLES.IMAGES',
    operations: [
      {
        operation: '!null',
        label: 'FILTERS.OPERATIONS.HAS',
        type: 'dropdown',
        itemType: 'static',
        items: [
          {label: 'Primary', value: 'Primary'},
          {label: 'Logo', value: 'Logo'},
        ],
        open: false,
        id: uuid(),
      },
      {
        operation: 'null',
        label: 'FILTERS.OPERATIONS.HASNO',
        type: 'dropdown',
        itemType: 'static',
        items: [
          {label: 'Primary', value: 'Primary'},
          {label: 'Logo', value: 'Logo'},
        ],
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'Genres',
    label: 'Genres {0} {1}',
    title: 'COMMON.GENRES',
    operations: [
      {
        operation: 'any',
        label: 'FILTERS.OPERATIONS.HAS',
        type: 'dropdown',
        itemType: 'url',
        itemUrl: '2/genre',
        open: false,
        id: uuid(),
      },
      {
        operation: '!any',
        label: 'FILTERS.OPERATIONS.HASNO',
        type: 'dropdown',
        itemType: 'url',
        itemUrl: '2/genre',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'CommunityRating',
    label: 'Community rating {0} {1}',
    title: 'COMMON.COMMUNITYRATING',
    operations: [
      {
        operation: '==',
        label: 'FILTERS.OPERATIONS.IS',
        type: 'number',
        unit: '',
        open: false,
        id: uuid(),
      },
      {
        operation: 'between',
        label: 'FILTERS.OPERATIONS.BETWEEN',
        type: 'range',
        unit: '',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: 'RunTimeTicks',
    label: 'Runtime {0} {1}',
    title: 'COMMON.RUNTIME',
    operations: [
      {
        operation: '<',
        label: 'FILTERS.OPERATIONS.SHORTER',
        type: 'number',
        unit: 'COMMON.MIN',
        open: false,
        id: uuid(),
      },
      {
        operation: '>',
        label: 'FILTERS.OPERATIONS.LONGER',
        type: 'number',
        unit: 'COMMON.MIN',
        open: false,
        id: uuid(),
      },
      {
        operation: 'between',
        label: 'FILTERS.OPERATIONS.BETWEEN',
        type: 'range',
        unit: 'COMMON.MIN',
        open: false,
        id: uuid(),
      },
      {
        operation: 'null',
        label: 'FILTERS.OPERATIONS.ISZERO',
        type: 'none',
        open: false,
        id: uuid(),
      },
    ],
    open: false,
    id: uuid(),
  },
];

export default showFilters;