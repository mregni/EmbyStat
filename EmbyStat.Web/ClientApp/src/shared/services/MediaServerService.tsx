import axios from 'axios';
import { MediaServerUdpBroadcast, MediaServerLogin, MediaServerInfo, MediaServerUser, Library } from '../models/mediaServer';

const baseUrl = 'http://localhost:6555/api/mediaserver/';

export const searchMediaServers = async (): Promise<MediaServerUdpBroadcast[]> => {
  const embySearch = axios.get<MediaServerUdpBroadcast>(`${baseUrl}server/search?serverType=0`);
  const jellyfinSearch = axios.get<MediaServerUdpBroadcast>(`${baseUrl}server/search?serverType=1`);

  return axios.all([embySearch, jellyfinSearch]).then(axios.spread((...responses) => {

    let servers: MediaServerUdpBroadcast[] = [];
    responses.forEach(response => {
      if (response.status === 200) {
        servers.push(response.data);
      }
    })

    return servers;
  }));
}

export const testApiKey = (login: MediaServerLogin): Promise<boolean> => {
  console.log(login);
  return axios.post<boolean>(`${baseUrl}server/test`, login).then(response => {
    return response.data;
  });
}

export const getServerInfo = (forceReSync = false): Promise<MediaServerInfo> => {
  return axios.get<MediaServerInfo>(`${baseUrl}server/info`, {
    params: { forceReSync: forceReSync }
  }).then(response => {
    return response.data;
  });
}

export const getLibraries = (): Promise<Library[]> => {
  return axios.get<Library[]>(`${baseUrl}server/libraries`).then(response => {
    return response.data;
  });
}

export const getAdministrators = (): Promise<MediaServerUser[]> => {
  return axios.get<MediaServerUser[]>(`${baseUrl}administrators`).then(response => {
    return response.data;
  });
}
