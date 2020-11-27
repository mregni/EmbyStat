import { axiosInstance } from './axiosInstance';
import { ShowStatistics } from '../models/show';

const domain = 'show/';

export const getStatistics = (): Promise<ShowStatistics> => {
  return axiosInstance
    .get<ShowStatistics>(`${domain}statistics`)
    .then((response) => {
      return response.data;
    });
};

export const isTypePresent = (): Promise<boolean> => {
  return axiosInstance.get<boolean>(`${domain}typepresent`).then(response => response.data);
}