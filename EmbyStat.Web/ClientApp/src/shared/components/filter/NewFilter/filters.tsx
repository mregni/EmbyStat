import { FilterType } from '../../../models/filter';

const contain = 'contain';
const notContain = 'does not contain';
const equal = 'equal';
const notEqual = 'does not equal';

const filters: FilterType[] = [
  {
    id: 0,
    field: 'Subtitles',
    label: 'Subtitles',
    type: 0,
    values: [
      { value: 'any', label: 'any' },
      { value: 'en', label: 'English' },
      { value: 'dut', label: 'Dutch' }
    ],
    actions: [
      { value: contain, label: 'contain' },
      { value: notContain, label: 'does not contain' }
    ],
  },
  {
    id: 1,
    field: 'Genres',
    label: 'Genres',
    type: 0,
    values: [
      { value: 'comedy', label: 'Comedy' },
      { value: 'action', label: 'Action' }
    ],
    actions: [
      { value: contain, label: 'contain' },
      { value: notContain, label: 'does not contain' }
    ],
  },
  {
    id: 2,
    field: 'Title',
    label: 'Title',
    type: 1,
    actions: [
      { value: contain, label: 'contain' },
      { value: notContain, label: 'does not contain' },
      { value: equal, label: 'equal' },
      { value: notEqual, label: 'does not equal' }
    ],
    values: [],
  },
  {
    id: 3,
    field: 'Images',
    label: 'Images',
    type: 0,
    values: [
      { value: '0', label: 'Primary' },
      { value: '3', label: 'Banner' },
      { value: '4', label: 'Logo' },
      { value: '5', label: 'Thumb' }
    ],
    actions: [
      { value: contain, label: 'contain' },
      { value: notContain, label: 'does not contain' }
    ],
  },
];

export default filters;