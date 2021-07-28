import { axiosInstance } from './axiosInstance';
import { MovieRow, MovieStatistics } from '../models/movie';
import { Movie, TablePage } from '../models/common';

const domain = 'movie/';

export const getStatistics = (): Promise<MovieStatistics> => {
  return axiosInstance
    .get<MovieStatistics>(`${domain}statistics`)
    .then((response) => {
      return response.data;
    });
};

export const getMovieDetails = (id: string): Promise<Movie> => {
  return axiosInstance.get<Movie>(`${domain}${id}`).then(response => response.data);
};

export const isTypePresent = (): Promise<boolean> => {
  return axiosInstance.get<boolean>(`${domain}typepresent`).then(response => response.data);
};

export const getMoviePage =
  (skip: number, take: number, sortField: string, sortOrder: string, requireTotalCount: boolean, filter: string)
    : Promise<TablePage<MovieRow>> => {
    return axiosInstance
      .get<TablePage<MovieRow>>(`${domain}list?skip=${skip}&take=${take}&sortField=${sortField}&sortOrder=${sortOrder}&requireTotalCount=${requireTotalCount}&filter=${filter}`)
      .then(response => response.data);
  }