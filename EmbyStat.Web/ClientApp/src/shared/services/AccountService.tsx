import {BehaviorSubject} from 'rxjs';

import i18n from '../../i18n';
import {AuthenticateResponse} from '../models/login';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import {axiosInstance} from './axiosInstance';

const domain = 'account/';

export const userLoggedIn$ = new BehaviorSubject<boolean>(false);

export const login = (username: string, password: string): Promise<AuthenticateResponse> => {
  return axiosInstance
    .post<AuthenticateResponse>(`${domain}login`, {username, password})
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return Promise.reject(new Error('Authentication failed'));
    });
};

export const logout = () => {
  return axiosInstance.post<boolean>(`${domain}logout`);
};

export const register = (username: string, password: string): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}register`, {username, password})
    .then((response) => {
      return response.data;
    })
    .catch((response) => {
      if (response.status === 401) {
        SnackbarUtils.error(i18n.t('WIZARD.ADMINCREATEFAILED'));
      }
      return Promise.reject(new Error('Registration failed'));
    });
};

export const refreshLogin = (accessToken: string, refreshToken: string): Promise<AuthenticateResponse> => {
  return axiosInstance
    .post<AuthenticateResponse>(`${domain}refreshtoken`, {accessToken, refreshToken})
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return Promise.reject(new Error('Refresh login failed'));
    });
};

export const resetPassword = (username: string): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}reset/password/${username}`)
    .then((response) => response.data)
    .catch(() => false);
};

export const changePassword = (oldPassword: string, newPassword: string, username: string): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}change/password`, {oldPassword, newPassword, username})
    .then((response) => response.data)
    .catch(() => false);
};

export const changeUserName = (userName: string, oldUsername: string): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}change/username`, {userName, oldUsername})
    .then((response) => response.data)
    .catch(() => false);
};

export const anyAdmins = (): Promise<boolean> => {
  return axiosInstance
    .get<boolean>(`${domain}any`)
    .then((response) => response.data);
};
