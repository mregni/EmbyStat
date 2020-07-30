import { axiosInstance } from './axiosInstance';
import { MovieStatistics } from '../models/movie';
import { Movie } from '../models/common';

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
}