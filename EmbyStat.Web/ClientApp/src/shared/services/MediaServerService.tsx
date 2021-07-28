import { axiosInstance } from './axiosInstance';
import axios from 'axios';
import {
  MediaServerUdpBroadcast,
  MediaServerLogin,
  MediaServerInfo,
  MediaServerUser,
  Library,
  MediaServerPlugin,
} from '../models/mediaServer';

const domain = 'mediaserver/';

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
      })
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
    });;
};

export const getServerInfo = (
  forceReSync = false
): Promise<MediaServerInfo | null> => {
  return axiosInstance
    .get<MediaServerInfo>(`${domain}server/info`, {
      params: { forceReSync },
    })
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return null;
    });
};

export const getPlugins = (): Promise<MediaServerPlugin[] | null> => {
  return axiosInstance.get<MediaServerPlugin[]>(`${domain}plugins`)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return null;
    });
}

export const getLibraries = (): Promise<Library[] | null> => {
  return axiosInstance.get<Library[]>(`${domain}server/libraries`)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return null;
    });
};

export const getAdministrators = (): Promise<MediaServerUser[] | null> => {
  return axiosInstance
    .get<MediaServerUser[]>(`${domain}administrators`)
    .then((response) => {
      return response.data;
    })
    .catch(() => {
      return null;
    });
};
