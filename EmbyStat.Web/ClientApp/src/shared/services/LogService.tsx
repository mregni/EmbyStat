import { axiosInstance } from './axiosInstance';
import { saveAs } from 'file-saver';

import { LogFile } from '../models/logs';

const domain = 'log/';

export const getLogList = (): Promise<LogFile[]> => {
  return axiosInstance.get(`${domain}list`).then((response) => response.data);
};

export const downloadLogFile = (filename: string, anonymised: boolean): void => {
  axiosInstance.get(`${domain}download/${filename}`, { params: { anonymous: anonymised } }).then((response) => saveFile(response));
};

const saveFile = (response) => {
  if (response.status !== 200) {
    throw new Error(response);
  }

  const filename = response.headers['content-disposition']
    .split(';')
    .find((n) => n.includes('filename='))
    .replace('filename=', '')
    .replace(/"/g, '')
    .trim();

  saveAs(response.data, filename);
};
