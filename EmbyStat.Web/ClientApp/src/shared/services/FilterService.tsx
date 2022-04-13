import {FilterValues} from '../models/filter';
import {axiosInstance} from './axiosInstance';

const domain = 'filter/';

export const getFilterValues = (filter: string | undefined): Promise<FilterValues> => {
  console.log(filter);
  return axiosInstance
    .get<FilterValues>(`${domain}${filter}`)
    .then((response) => {
      return response.data;
    });
};
