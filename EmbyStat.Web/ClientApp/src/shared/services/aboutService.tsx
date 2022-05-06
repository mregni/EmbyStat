import {About} from '../models/about';
import {axiosInstance} from './axiosInstance';

const domain = 'about/';

export const getAbout = (): Promise<About> => {
  return axiosInstance
    .get<About>(domain)
    .then((response) => {
      return response.data;
    });
};
