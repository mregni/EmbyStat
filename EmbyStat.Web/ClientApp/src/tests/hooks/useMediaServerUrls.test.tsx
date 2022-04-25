import {useMediaServerUrls} from '../../shared/hooks';

jest.mock('react', () => {
  const ActualReact = jest.requireActual('react');
  const settings = {mediaServer: {address: 'https://localhost', id: 14}};
  return {
    ...ActualReact,
    useContext: () => ({settings}), // what you want to return when useContext get fired goes here
  };
});


describe('with media server addres', () => {
  it('getPrimaryImageLink should return url', () => {
    const {getPrimaryImageLink} = useMediaServerUrls();
    const url = getPrimaryImageLink('12', '321');
    expect(url).toBe('https://localhost/Items/12/Images/Primary?maxHeight=350&tag=321&quality=90&enableimageenhancers=false');
  });

  it('getPrimaryUserImageLink should return url', () => {
    const {getPrimaryUserImageLink} = useMediaServerUrls();
    const url = getPrimaryUserImageLink('12', '321');
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
