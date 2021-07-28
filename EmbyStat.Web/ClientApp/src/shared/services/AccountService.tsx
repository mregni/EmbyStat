import jwt from 'jwt-decode';
import moment from 'moment';
import { BehaviorSubject } from 'rxjs';
import { axiosInstance } from './axiosInstance';

import { LoginView, AuthenticateResponse, User, ChangeUserNameRequest, ChangePasswordRequest, JwtPayloadCustom } from '../models/login';
import { AxiosResponse } from 'axios';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';
import i18n from '../../i18n';

const domain = 'account/';
const accessTokenStr = 'accessToken';
const refreshTokenStr = 'refreshToken';

export const userLoggedIn$ = new BehaviorSubject<boolean>(false);

export const login = (login: LoginView): Promise<boolean> => {
  return axiosInstance
    .post<AuthenticateResponse>(`${domain}login`, login)
    .then((response) => {
      setLocalStorage(response.data);
      userLoggedIn$.next(true);
      return true;
    })
    .catch(() => {
      return false;
    });
};

export const logout = () => {
  return axiosInstance.post<boolean>(`${domain}logout`).finally(() => {
    clearLocalStorage();
    userLoggedIn$.next(false);
  });
};

export const register = (register: LoginView): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}register`, register)
    .then((response) => {
      return response.data;
    })
    .catch((response) => {
      if (response.status === 401) {
        SnackbarUtils.error(i18n.t('WIZARD.ADMINCREATEFAILED'));
      }
      return Promise.reject();
    });
};

export const forcedRefreshLogin = async () => {
  if (isUserLoggedIn()) {
    const accessToken = localStorage.getItem(accessTokenStr);
    const refreshToken = localStorage.getItem(refreshTokenStr);

    if (accessToken != null && refreshToken != null) {
      await refreshLogin(accessToken, refreshToken);
    }
  }
}

export const refreshLogin = (
  accessToken: string,
  refreshToken: string
): Promise<boolean> => {
  const refresh = { accessToken, refreshToken };
  return axiosInstance
    .post<AuthenticateResponse>(`${domain}refreshtoken`, refresh)
    .then((response) => {
      if (response.data === null || response.data === undefined
        || response.data.accessToken === null || response.data.accessToken === undefined) {
        return false;
      }
      setLocalStorage(response.data);
      return true;
    })
    .catch(() => {
      return false;
    });
};

export const anyAdmins = (): Promise<AxiosResponse<boolean>> => {
  return axiosInstance.get<boolean>(`${domain}any`);
}

export const resetPassword = (username: string): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}reset/password/${username}`)
    .then(response => response.data)
    .catch(() => false)
}

export const changePassword = (request: ChangePasswordRequest): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}change/password`, request)
    .then(response => response.data)
    .catch(() => false);
}

export const changeUserName = (request: ChangeUserNameRequest): Promise<boolean> => {
  return axiosInstance
    .post<boolean>(`${domain}change/username`, request)
    .then(response => response.data)
    .catch(() => false);
}

export const isUserLoggedIn = async (): Promise<boolean> => {
  let accessToken = localStorage.getItem(accessTokenStr) ?? "";
  let refreshToken = localStorage.getItem(refreshTokenStr);

  if (
    ['undefined', undefined, null, ""].includes(accessToken) ||
    ['undefined', undefined, null, ""].includes(refreshToken)
  ) {
    userLoggedIn$.next(false);
    return false;
  }

  let tokenExpiration = jwt<JwtPayloadCustom>(accessToken).exp ?? 0;
  let tokenExpirationTimeInSeconds = tokenExpiration - moment().unix();

  if (tokenExpirationTimeInSeconds < 250) {
    const result = await refreshLogin(
      accessToken as string,
      refreshToken as string
    );
    if (!result) {
      userLoggedIn$.next(false);
      return false;
    }

    accessToken = localStorage.getItem(accessTokenStr) ?? "";
    refreshToken = localStorage.getItem(refreshTokenStr);
    tokenExpiration = jwt<JwtPayloadCustom>(accessToken).exp ?? 0;
    tokenExpirationTimeInSeconds = tokenExpiration - moment().unix();

    const newTokenIsValid = tokenExpirationTimeInSeconds > 120;
    userLoggedIn$.next(newTokenIsValid);
    return tokenExpirationTimeInSeconds > 120;
  }

  userLoggedIn$.next(true);
  return true;
};

export const getUserInfo = (): User | null => {
  if (isUserLoggedIn()) {
    const accessToken = localStorage.getItem(accessTokenStr) ?? "";
    const tokenInfo = jwt<JwtPayloadCustom>(accessToken);
    return {
      username: tokenInfo.sub ?? "",
    };
  }

  return null;
};

export const checkUserRoles = (roles: string[]): boolean => {
  const accessToken = localStorage.getItem(accessTokenStr) ?? "";
  if (['undefined', undefined, null, ""].includes(accessToken)) {
    return false;
  }

  const userRoles = jwt<JwtPayloadCustom>(accessToken).roles as string[];
  const duplicates = userRoles.filter((x) => roles.includes(x));
  return duplicates.length > 0;
};

const setLocalStorage = (tokenInfo: AuthenticateResponse) => {
  localStorage.setItem(accessTokenStr, tokenInfo.accessToken);
  localStorage.setItem(refreshTokenStr, tokenInfo.refreshToken);
};

const clearLocalStorage = () => {
  localStorage.removeItem(accessTokenStr);
  localStorage.removeItem(refreshTokenStr);
};
