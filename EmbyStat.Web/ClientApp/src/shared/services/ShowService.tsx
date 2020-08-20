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

// export const getMovieDetails = (id: string): Promise<Movie> => {
//   return axiosInstance.get<Movie>(`${domain}${id}`).then(response => response.data);
// };

export const isTypePresent = (): Promise<boolean> => {
  return axiosInstance.get<boolean>(`${domain}typepresent`).then(response => response.data);
}