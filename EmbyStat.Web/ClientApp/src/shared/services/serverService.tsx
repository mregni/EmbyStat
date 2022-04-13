import axios from 'axios';

import {Library} from '../models/library';
import {
  MediaServerInfo, MediaServerLogin, MediaServerPlugin, MediaServerUdpBroadcast, MediaServerUser,
} from '../models/mediaServer';
import {axiosInstance} from './axiosInstance';

const domain = 'mediaserver/';

export const getServerInfo = (forceReSync = false): Promise<MediaServerInfo> => {
  return axiosInstance
    .get<MediaServerInfo>(`${domain}server/info`, {params: {forceReSync}})
    .then((response) => response.data);
};

export const getAdministrators = (): Promise<MediaServerUser[]> => {
  return axiosInstance
    .get<MediaServerUser[]>(`${domain}administrators`)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return [];
    });
};

export const getLibraries = (): Promise<Library[]> => {
  return axiosInstance.get<Library[]>(`${domain}server/libraries`)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return [];
    });
};

export const getPlugins = (): Promise<MediaServerPlugin[]> => {
  return axiosInstance
    .get<MediaServerPlugin[]>(`${domain}plugins`)
    .then((response) => response.data);
};

export const searchMediaServers = async (): Promise<MediaServerUdpBroadcast[]> => {
  const embySearch = axiosInstance.get<MediaServerUdpBroadcast[]>(`${domain}server/search?serverType=0`);
  const jellyfinSearch = axiosInstance.get<MediaServerUdpBroadcast[]>(`${domain}server/search?serverType=1`);

  return axios.all([embySearch, jellyfinSearch])
    .then(
      axios.spread((...responses) => {
        let servers: MediaServerUdpBroadcast[] = [];
        responses.forEach((response) => {
          if (response.status === 200) {
            servers = [...servers, ...response.data];
          }
        });

        return servers;
      }),
    );
};

export const testApiKey = (login: MediaServerLogin): Promise<boolean | null> => {
  return axiosInstance
    .post<boolean>(`${domain}server/test`, login)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return null;
    }); ;
};
