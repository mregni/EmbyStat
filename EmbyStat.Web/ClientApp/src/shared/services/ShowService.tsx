import { axiosInstance } from './axiosInstance';
import { ShowRow, ShowStatistics } from '../models/show';
import { Show, TablePage } from '../models/common';

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

export const getShowPage =
  (skip: number, take: number, sortField: string, sortOrder: string, requireTotalCount: boolean, filter: string)
    : Promise<TablePage<ShowRow>> => {
    return axiosInstance
      .get<TablePage<ShowRow>>(`${domain}list?skip=${skip}&take=${take}&sortField=${sortField}&sortOrder=${sortOrder}&requireTotalCount=${requireTotalCount}&filter=${filter}`)
      .then(response => response.data);
  }