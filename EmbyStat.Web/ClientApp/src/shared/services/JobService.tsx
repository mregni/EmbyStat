import { axiosInstance } from './axiosInstance';
import { Job } from '../models/jobs';

const domain = 'job/';

export const getAllJobs = (): Promise<Job[]> => {
  return axiosInstance.get<Job[]>(domain).then((response) => response.data);
};

export const fireJob = (id: string): Promise<void> => {
  return axiosInstance.post(`${domain}fire/${id}`).then((response) => response.data);
};

export const updateTrigger = (id: string, cron: string): Promise<void> => {
  return axiosInstance.patch(
    `${domain}${id}`,
    {},
    {
      params: { cron },
    }
  );
};

export const getJobById = (id: string): Promise<Job> => {
  return axiosInstance.get<Job>(`${domain}${id}`).then((response) => response.data);
}
