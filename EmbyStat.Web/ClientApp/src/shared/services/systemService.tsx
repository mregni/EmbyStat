import {axiosInstance} from './axiosInstance';

const domain = 'system/';

export const resetEmbyStat = (): Promise<void> => {
  return axiosInstance
    .get<void>(`${domain}reset`)
    .then((response) => {
      return response.data;
    });
};
