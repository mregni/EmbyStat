import { axiosInstance } from './axiosInstance';
import { ShowStatistics } from '../models/show';
import { Show } from '../models/common';

const domain = 'show/';

export const getStatistics = (): Promise<ShowStatistics> => {
  return axiosInstance
    .get<ShowStatistics>(`${domain}statistics`)
    .then((response) => {
      return response.data;
    });
};

export const getShowDetails = (id: string): Promise<Show> => {
  return axiosInstance.get<Show>(`${domain}${id}`).then(response => response.data);
 };

export const isTypePresent = (): Promise<boolean> => {
  return axiosInstance.get<boolean>(`${domain}typepresent`).then(response => response.data);
}