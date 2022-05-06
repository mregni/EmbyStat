import {getUnixTime} from 'date-fns';
import jwt from 'jwt-decode';
import {createContext, useState} from 'react';

import {AuthenticateResponse, JwtPayloadCustom, User} from '../../models/login';
import {
  changePassword as userChangePassword, changeUserName as userChangeUserName, login as userLogin,
  logout as userLogout, refreshLogin as userRefreshLogin, register as userRegister,
} from '../../services/accountService';

const accessTokenStr = 'accessToken';
const refreshTokenStr = 'refreshToken';

const setLocalStorage = (tokenInfo: AuthenticateResponse) => {
  localStorage.setItem(accessTokenStr, tokenInfo.accessToken);
  localStorage.setItem(refreshTokenStr, tokenInfo.refreshToken);
};

const clearLocalStorage = () => {
  localStorage.removeItem(accessTokenStr);
  localStorage.removeItem(refreshTokenStr);
};

const refreshLogin = async (accessToken: string, refreshToken: string): Promise<boolean> => {
  const response = await userRefreshLogin(accessToken, refreshToken);
  if (response === null || response === undefined ||
    response.accessToken === null || response.accessToken === undefined) {
    return false;
  }
  setLocalStorage(response);
  return true;
};

export interface UserContextProps {
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  register: (username: string, password: string) => Promise<boolean>;
  isUserLoggedIn: () => Promise<boolean>;
  checkUserRoles: (roles: string[]) => boolean;
  changeUserName: (username: string) => Promise<boolean>;
  changePassword: (password: string, oldpassword: string) => Promise<boolean>;
}

export const UserContext = createContext<UserContextProps>(null!);

export const useUserContext = (): UserContextProps => {
  const [user, setUser] = useState<User | null>(null);
  const login = async (username: string, password: string): Promise<void> => {
    const loginResponse = await userLogin(username, password);
    setLocalStorage(loginResponse);
    const tokenInfo = jwt<JwtPayloadCustom>(loginResponse.accessToken);
    setUser({
      username: tokenInfo.sub ?? '',
      roles: tokenInfo.roles,
    });
  };

  const logout = async () => {
    await userLogout();
    clearLocalStorage();
  };

  const register = async (username: string, password: string): Promise<boolean> => {
    const registerResponse = await userRegister(username, password);
    if (registerResponse) {
      await login(username, password);
    }

    return registerResponse;
  };

  const isUserLoggedIn = async (): Promise<boolean> => {
    const accessToken = localStorage.getItem(accessTokenStr) ?? '';
    const refreshToken = localStorage.getItem(refreshTokenStr);

    if (
      ['undefined', undefined, null, ''].includes(accessToken) ||
      ['undefined', undefined, null, ''].includes(refreshToken)
    ) {
      return false;
    }

    const tokenInfo = jwt<JwtPayloadCustom>(accessToken);
    const tokenExpiration = tokenInfo.exp ?? 0;
    const tokenExpirationTimeInSeconds = tokenExpiration - getUnixTime(new Date());

    if (tokenExpirationTimeInSeconds >= 250) {
      setUser({
        username: tokenInfo.sub ?? '',
        roles: tokenInfo.roles,
      });
      return true;
    }

    const result = await refreshLogin(accessToken as string, refreshToken as string);
    if (!result) {
      return false;
    }

    const newAccessToken = localStorage.getItem(accessTokenStr) ?? '';
    const newTokenInfo = jwt<JwtPayloadCustom>(newAccessToken);
    if (newTokenInfo !== undefined) {
      const newTokenExpirationTimeInSeconds = (newTokenInfo.exp ?? 0) - getUnixTime(new Date());
      setUser({
        username: newTokenInfo.sub ?? '',
        roles: newTokenInfo.roles,
      });
      return newTokenExpirationTimeInSeconds > 120;
    }

    return false;
  };

  const checkUserRoles = (roles: string[]): boolean => {
    if (user === null) {
      return false;
    }
    const duplicates = user?.roles.filter((x) => roles.includes(x));
    return duplicates.length > 0;
  };

  const changeUserName = async (username: string): Promise<boolean> => {
    if (user !== null) {
      const response = await userChangeUserName(username, user.username);
      if (response) {
        setUser({...user as User, username});
      }

      return response;
    }

    return false;
  };

  const changePassword = (password: string, oldPassword: string)
    : Promise<boolean> => {
    if (user != null) {
      return userChangePassword(oldPassword, password, user.username);
    }
    return Promise.resolve(false);
  };

  return {
    login, logout, register, user, isUserLoggedIn,
    checkUserRoles, changeUserName, changePassword,
  };
};
