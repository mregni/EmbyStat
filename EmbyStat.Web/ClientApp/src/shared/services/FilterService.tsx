import { axiosInstance } from './axiosInstance';
import { Job } from '../models/jobs';
import { FilterValues } from '../models/filter';

const baseUrl = 'http://localhost:6555/api/filter/';

export const getFilterValues = (filter: string | undefined, libraryIds: string[]): Promise<FilterValues> => {
  return axiosInstance.get<FilterValues>(`${baseUrl}${filter}`, { params: { libraryIds: libraryIds } })
    .then(response => {
      return response.data;
    });
}