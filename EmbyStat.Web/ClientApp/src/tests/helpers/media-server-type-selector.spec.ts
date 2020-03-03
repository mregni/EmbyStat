import { MediaServerTypeSelector } from '../../app/shared/helpers/media-server-type-selector';

describe('MediaServerTypeSelector', () => {
    describe('when getServerTypeString is called with 0', () => {
        it('should return Emby', () => {
            const helper = MediaServerTypeSelector.getServerTypeString(0);
          expect(helper).toEqual('Emby');
        });
    });

    describe('when getServerTypeString is called with 1', () => {
        it('should return Jellyfin', () => {
            const helper = MediaServerTypeSelector.getServerTypeString(1);
          expect(helper).toEqual('Jellyfin');
        });
    });
});
