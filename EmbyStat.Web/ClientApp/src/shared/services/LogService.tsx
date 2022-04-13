import {saveAs} from 'file-saver';

import {LogFile} from '../models/logs';
import {axiosInstance} from './axiosInstance';

const domain = 'log/';

export const getLogList = (): Promise<LogFile[]> => {
  return axiosInstance.get(`${domain}list`).then((response) => response.data);
};

export const downloadLogFile = (filename: string, anonymised: boolean): void => {
  axiosInstance
    .get(`${domain}download/${filename}`, {params: {anonymous: anonymised}})
    .then((response) => saveFile(response));
};

const saveFile = (response: any) => {
  if (response.status !== 200) {
    throw new Error(response);
  }

  const filename = response.headers['content-disposition']
    .split(';')
    .find((n: string) => n.includes('filename='))
    .replace('filename=', '')
    .replace(/"/g, '')
    .trim();
  const blob = new Blob([response.data], {type: 'text/plain;charset=utf-8'});
  saveAs(blob, filename);
};
