import i18n from '../../i18n';
import {TablePage} from '../models/common';
import {Library} from '../models/library';
import {Movie, MovieRow, MovieStatistics} from '../models/movie';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import {axiosInstance} from './axiosInstance';

const domain = 'movie/';

export const getStatistics = (): Promise<MovieStatistics> => {
  return axiosInstance
    .get<MovieStatistics>(`${domain}statistics`)
    .then((response) => {
      return response.data;
    });
};

export const getPage =
  (skip: number, take: number, sortField: string, sortOrder: string, requireTotalCount: boolean, filter: string)
    : Promise<TablePage<MovieRow>> => {
    return axiosInstance
      .get<TablePage<MovieRow>>(
        `${domain}list?skip=${skip}&take=${take}
&sortField=${sortField}&sortOrder=${sortOrder}
&requireTotalCount=${requireTotalCount}&filter=${filter}`)
      .then((response) => response.data);
  };

export const getMovieDetails = (id: string): Promise<Movie> => {
  return axiosInstance.get<Movie>(`${domain}${id}`).then((response) => response.data);
};

export const areMoviesPresent = (): Promise<boolean> => {
  return axiosInstance.get<boolean>(`${domain}typepresent`).then((response) => response.data);
};

export const fetchLibraries = (): Promise<Library[]> => {
  return axiosInstance.get<Library[]>(`${domain}libraries`).then((response) => response.data);
};

export const pushLibraries = (libraryIds: string[]): Promise<void> => {
  return axiosInstance
    .post<void>(`${domain}libraries`, libraryIds )
    .then((response) => {
      if (response.status === 200) {
        SnackbarUtils.success(i18n.t('SETTINGS.SAVED'));
      }
      return response.data;
    })
    .catch((x) => {
      SnackbarUtils.error(i18n.t('SETTINGS.SAVEFAILED'));
    });
};
