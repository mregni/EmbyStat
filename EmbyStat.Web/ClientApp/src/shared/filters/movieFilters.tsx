import uuid from 'react-uuid';
import { FilterDefinition } from '../models/filter';

const movieFilters: FilterDefinition[] = [
  {
    field: "Name",
    label: "Title {0} {1}",
    types: [
      { operation: "contains", label: "contains", type: "txt", open: false, id: uuid(), placeholder: "Title" },
      { operation: "!contains", label: "not contains", type: "txt", open: false, id: uuid(), placeholder: "Title" },
      { operation: "==", label: "equals", type: "txt", open: false, id: uuid(), placeholder: "Title" },
      { operation: "startsWith", label: "starts with", type: "txt", open: false, id: uuid(), placeholder: "Title" },
      { operation: "endsWith", label: "ends with", type: "txt", open: false, id: uuid(), placeholder: "Title" }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "IMDB",
    label: "Imdb {0} {1}",
    types: [
      { operation: "==", label: "is", type: "txt", open: false, id: uuid(), placeholder: "Imdb id" },
      { operation: "null", label: "is null", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "TMDB",
    label: "Tmdb {0} {1}",
    types: [
      { operation: "==", label: "is", type: "txt", open: false, id: uuid(), placeholder: "Tmdb id" },
      { operation: "null", label: "is null", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Images",
    label: "Images {0} {1}",
    types: [
      { operation: "!null", label: "has", type: "dropdown", itemType: "static", items: [{ label: "Primary", value: "Primary" }, { label: "Logo", value: "Logo" }], open: false, id: uuid() },
      { operation: "null", label: "has no", type: "dropdown", itemType: "static", items: [{ label: "Primary", value: "Primary" }, { label: "Logo", value: "Logo" }], open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Genres",
    label: "Genres {0} {1}",
    types: [
      { operation: "any", label: "has", type: "dropdown", itemType: "url", itemUrl: "1/genre", open: false, id: uuid() },
      { operation: "!any", label: "has no", type: "dropdown", itemType: "url", itemUrl: "1/genre", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Subtitles",
    label: "Subtitles {0} {1}",
    types: [
      { operation: "empty", label: "has none", type: "none", open: false, id: uuid() },
      { operation: "any", label: "has", type: "dropdown", itemType: "url", itemUrl: "1/subtitle", open: false, id: uuid() },
      { operation: "!any", label: "has no", type: "dropdown", itemType: "url", itemUrl: "1/subtitle", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "CommunityRating",
    label: "Community rating {0} {1}",
    types: [
      { operation: "==", label: "is", type: "number", unit: "", open: false, id: uuid(), placeholder: "Rating" },
      { operation: "between", label: "between", type: "range", unit: "", min: 0, max: 10, step: 0.5, open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "RunTimeTicks",
    label: "Runtime {0} {1}",
    types: [
      { operation: ">", label: "shorter", type: "number", unit: "COMMON.MIN", open: false, id: uuid() },
      { operation: "<", label: "longer", type: "number", unit: "COMMON.MIN", open: false, id: uuid() },
      { operation: "between", label: "between", type: "range", min: 0, max: 400, step: 20, unit: "COMMON.MIN", open: false, id: uuid() },
      { operation: "null", label: "is 0", type: "none", open: false, id: uuid() },
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "SizeInMb",
    label: "Size {0} {1}",
    types: [
      { operation: ">", label: "larger", type: "number", unit: "COMMON.GB", open: false, id: uuid() },
      { operation: "<", label: "smaller", type: "number", unit: "COMMON.GB", open: false, id: uuid() },
      { operation: "between", label: "between", type: "range", min: 0, max: 200, step: 1, unit: "COMMON.GB", open: false, id: uuid() },
      { operation: "null", label: "is 0", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Height",
    label: "Height {0} {1}",
    types: [
      { operation: "==", label: "is", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: ">", label: "larger", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "<", label: "smaller", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "between", label: "between", type: "range", min: 0, max: 8000, step: 1, unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "null", label: "is 0", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Width",
    label: "Width {0} {1}",
    types: [
      { operation: "==", label: "is", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "<", label: "larger", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: ">", label: "smaller", type: "number", unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "between", label: "between", type: "range", min: 0, max: 8000, step: 1, unit: "COMMON.PIXELS", open: false, id: uuid() },
      { operation: "null", label: "is 0", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "AverageFrameRate",
    label: "FrameRate {0} {1}",
    types: [
      { operation: "==", label: "is", type: "number", unit: "COMMON.FPS", open: false, id: uuid() },
      { operation: ">", label: "larger", type: "number", unit: "COMMON.FPS", open: false, id: uuid() },
      { operation: "<", label: "smaller", type: "number", unit: "COMMON.FPS", open: false, id: uuid() },
      { operation: "between", label: "between", type: "range", min: 0, max: 240, step: 1, unit: "COMMON.FPS", open: false, id: uuid() },
      { operation: "null", label: "is 0", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "Container",
    label: "Container {0} {1}",
    types: [
      { operation: "==", label: "is", type: "dropdown", itemType: "url", itemUrl: "1/container", open: false, id: uuid() },
      { operation: "!=", label: "is not", type: "dropdown", itemType: "url", itemUrl: "1/container", open: false, id: uuid() },
      { operation: "null", label: "is null", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  },
  {
    field: "PremiereDate",
    label: "Premiere Date {0} {1}",
    types: [
      { operation: "<", label: "before", type: "date", open: false, id: uuid() },
      { operation: ">", label: "after", type: "date", open: false, id: uuid() },
      { operation: "between", label: "between", type: "dateRange", open: false, id: uuid() },
      { operation: "null", label: "is null", type: "none", open: false, id: uuid() }
    ],
    open: false,
    id: uuid(),
  }
];

export default movieFilters;
