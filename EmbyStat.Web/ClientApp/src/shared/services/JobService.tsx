import { axiosInstance } from './axiosInstance';
import { Job } from '../models/jobs';

const baseUrl = 'http://localhost:6555/api/job/';

export const getAllJobs = (): Promise<Job[]> => {
  return axiosInstance.get<Job[]>(baseUrl).then(response => {
    return response.data;
  });
}

export const fireJob = (id: string): Promise<void> => {
  return axiosInstance.post(`${baseUrl}fire/${id}`).then(response => {
    return response.data;
  });
}

export const updateTrigger = (id: string, cron: string): Promise<void> => {
  return axiosInstance.patch(`${baseUrl}${id}`, {}, {
    params: { cron }
  });
}