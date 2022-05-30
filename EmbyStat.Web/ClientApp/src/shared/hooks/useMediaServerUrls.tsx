import {useContext} from 'react';

import {SettingsContext} from '../context/settings';

export const useMediaServerUrls = () => {
  const {userConfig} = useContext(SettingsContext);

  const getFullMediaServerUrl = (): string => {
    if (userConfig?.mediaServer !== null) {
      return userConfig.mediaServer.address;
    }

    return '';
  };

  const getPrimaryImageLink = (itemId: string, tag: string, fallbackUrl: string = ''): string => {
    if (userConfig !== null) {
      let url = getFullMediaServerUrl();
      if (url === null || url === '') {
        url = fallbackUrl;
      }
      return `${url}/Items/${itemId}/Images/Primary?maxHeight=350&tag=${tag}&quality=90&enableimageenhancers=false`;
    }
    return '';
  };

  const getPrimaryUserImageLink = (userId: string, fallbackUrl: string = ''): string => {
    if (userConfig !== null) {
      let url = getFullMediaServerUrl();
      if (url === null || url === '') {
        url = fallbackUrl;
      }
      return `${url}/Users/${userId}/Images/Primary/0?Width=50&EnableImageEnhancers=false`;
    }
    return '';
  };

  const getItemDetailLink = (itemId: string): string => {
    if (userConfig !== null) {
      const url = getFullMediaServerUrl();
      if (userConfig.mediaServer.type === 0) {
        return `${url}/web/index.html#!/item?id=${itemId}&serverId=${userConfig.mediaServer.id}`;
      }

      return `${url}/web/index.html#!/details?id=${itemId}&serverId=${userConfig.mediaServer.id}`;
    }

    return '';
  };

  const getUserDetailLink = (userId: string): string => {
    if (userConfig !== null) {
      const url = getFullMediaServerUrl();
      if (userConfig.mediaServer.type === 0) {
        return `${url}/web/index.html#!/users/user?userId=${userId}`;
      }

      return `${url}/web/index.html#!/useredit.html?userId=${userId}`;
    }

    return '';
  };

  const getBackdropImageLink = (itemId: string): string => {
    if (userConfig !== null) {
      const url = getFullMediaServerUrl();
      if (userConfig.mediaServer.type === 0) {
        return `${url}/Items/${itemId}/Images/Backdrop?quality=90&enableimageenhancers=false`;
      }

      return `${url}/Items/${itemId}/Images/Backdrop?quality=90`;
    }

    return '';
  };

  return {getPrimaryImageLink, getItemDetailLink,
    getBackdropImageLink, getPrimaryUserImageLink,
    getUserDetailLink};
};
