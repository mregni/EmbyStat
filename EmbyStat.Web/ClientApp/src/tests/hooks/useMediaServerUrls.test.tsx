import React from 'react';

import {useMediaServerUrls} from '../../shared/hooks';

describe('with media server address', () => {
  describe('and with media server type 1', () => {
    beforeEach(() => {
      jest
        .spyOn(React, 'useContext')
        .mockImplementation(() => ({
          userConfig: {
            mediaServer: {
              address: 'https://localhost',
              id: 14,
              type: 1,
            },
          },
        }));
    });

    it('getPrimaryImageLink should return url', () => {
      const {getPrimaryImageLink} = useMediaServerUrls();
      const url = getPrimaryImageLink('12', '321', 'https://fallback');
      expect(url).toBe('https://localhost/Items/12/Images/Primary?maxHeight=350&tag=321&quality=90&enableimageenhancers=false');
    });

    it('getPrimaryUserImageLink should return url', () => {
      const {getPrimaryUserImageLink} = useMediaServerUrls();
      const url = getPrimaryUserImageLink('12', 'https://fallback');
      expect(url).toBe('https://localhost/Users/12/Images/Primary/0?Width=50&EnableImageEnhancers=false');
    });

    it('getItemDetailLink should return url', () => {
      const {getItemDetailLink} = useMediaServerUrls();
      const url = getItemDetailLink('12');
      expect(url).toBe('https://localhost/web/index.html#!/details?id=12&serverId=14');
    });

    it('getUserDetailLink should return url', () => {
      const {getUserDetailLink} = useMediaServerUrls();
      const url = getUserDetailLink('12');
      expect(url).toBe('https://localhost/web/index.html#!/useredit.html?userId=12');
    });

    it('getBackdropImageLink should return url', () => {
      const {getBackdropImageLink} = useMediaServerUrls();
      const url = getBackdropImageLink('12');
      expect(url).toBe('https://localhost/Items/12/Images/Backdrop?quality=90');
    });
  });

  describe('and with media server type 0', () => {
    beforeEach(() => {
      jest
        .spyOn(React, 'useContext')
        .mockImplementation(() => ({
          userConfig: {
            mediaServer: {
              address: 'https://localhost',
              id: 14,
              type: 0,
            },
          },
        }));
    });

    it('getPrimaryImageLink should return url', () => {
      const {getPrimaryImageLink} = useMediaServerUrls();
      const url = getPrimaryImageLink('12', '321', 'https://fallback');
      expect(url).toBe('https://localhost/Items/12/Images/Primary?maxHeight=350&tag=321&quality=90&enableimageenhancers=false');
    });

    it('getPrimaryUserImageLink should return url', () => {
      const {getPrimaryUserImageLink} = useMediaServerUrls();
      const url = getPrimaryUserImageLink('12', 'https://fallback');
      expect(url).toBe('https://localhost/Users/12/Images/Primary/0?Width=50&EnableImageEnhancers=false');
    });

    it('getItemDetailLink should return url', () => {
      const {getItemDetailLink} = useMediaServerUrls();
      const url = getItemDetailLink('12');
      expect(url).toBe('https://localhost/web/index.html#!/item?id=12&serverId=14');
    });

    it('getUserDetailLink should return url', () => {
      const {getUserDetailLink} = useMediaServerUrls();
      const url = getUserDetailLink('12');
      expect(url).toBe('https://localhost/web/index.html#!/users/user?userId=12');
    });

    it('getBackdropImageLink should return url', () => {
      const {getBackdropImageLink} = useMediaServerUrls();
      const url = getBackdropImageLink('12');
      expect(url).toBe('https://localhost/Items/12/Images/Backdrop?quality=90&enableimageenhancers=false');
    });
  });
});

describe('without settings', () => {
  beforeEach(() => {
    jest
      .spyOn(React, 'useContext')
      .mockImplementation(() => ({
        userConfig: null,
      }));
  });

  it('getPrimaryImageLink should return an empty string', () => {
    const {getPrimaryImageLink} = useMediaServerUrls();
    const url = getPrimaryImageLink('12', '321', 'https://fallback');
    expect(url).toBe('');
  });

  it('getPrimaryUserImageLink should return an empty string', () => {
    const {getPrimaryUserImageLink} = useMediaServerUrls();
    const url = getPrimaryUserImageLink('12', 'https://fallback');
    expect(url).toBe('');
  });

  it('getItemDetailLink should return an empty string', () => {
    const {getItemDetailLink} = useMediaServerUrls();
    const url = getItemDetailLink('12');
    expect(url).toBe('');
  });

  it('getUserDetailLink should return an empty string', () => {
    const {getUserDetailLink} = useMediaServerUrls();
    const url = getUserDetailLink('12');
    expect(url).toBe('');
  });

  it('getBackdropImageLink should return an empty string', () => {
    const {getBackdropImageLink} = useMediaServerUrls();
    const url = getBackdropImageLink('12');
    expect(url).toBe('');
  });
});

describe('with media server url being null', () => {
  beforeEach(() => {
    jest
      .spyOn(React, 'useContext')
      .mockImplementation(() => ({
        userConfig: {
          mediaServer: {
            type: 0,
            address: null,
          },
        },
      }));
  });

  it('getPrimaryImageLink should return url', () => {
    const {getPrimaryImageLink} = useMediaServerUrls();
    const url = getPrimaryImageLink('12', '321', 'https://fallback');
    expect(url).toBe('https://fallback/Items/12/Images/Primary?maxHeight=350&tag=321&quality=90&enableimageenhancers=false');
  });

  it('getPrimaryUserImageLink should return url', () => {
    const {getPrimaryUserImageLink} = useMediaServerUrls();
    const url = getPrimaryUserImageLink('12', 'https://fallback');
    expect(url).toBe('https://fallback/Users/12/Images/Primary/0?Width=50&EnableImageEnhancers=false');
  });
});

describe('with media server url being empty', () => {
  beforeEach(() => {
    jest
      .spyOn(React, 'useContext')
      .mockImplementation(() => ({
        userConfig: {
          mediaServer: {
            type: 0,
            address: '',
          },
        },
      }));
  });

  it('getPrimaryImageLink should return url', () => {
    const {getPrimaryImageLink} = useMediaServerUrls();
    const url = getPrimaryImageLink('12', '321', 'https://fallback');
    expect(url).toBe('https://fallback/Items/12/Images/Primary?maxHeight=350&tag=321&quality=90&enableimageenhancers=false');
  });

  it('getPrimaryUserImageLink should return url', () => {
    const {getPrimaryUserImageLink} = useMediaServerUrls();
    const url = getPrimaryUserImageLink('12', 'https://fallback');
    expect(url).toBe('https://fallback/Users/12/Images/Primary/0?Width=50&EnableImageEnhancers=false');
  });
});
