import { axiosInstance } from './axiosInstance';
import { FilterValues } from '../models/filter';

const domain = 'filter/';

export const getFilterValues = (
  filter: string | undefined,
  libraryIds: string[]
): Promise<FilterValues> => {
  return axiosInstance
    .get<FilterValues>(`${domain}${filter}`, { params: { libraryIds } })
    .then((response) => {
      return response.data;
    });
};
