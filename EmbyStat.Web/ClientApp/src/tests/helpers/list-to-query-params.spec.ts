import { ListToQueryParam } from '../../app/shared/helpers/list-to-query-param';

describe('ListToQueryParams', () => {
    describe('when a list is passed and list is the first parameter', () => {
        it('should map the list', () => {
            const helper = ListToQueryParam.convert('test', ['obj1', 'obj2']);
          expect(helper).toEqual('?test=obj1&test=obj2');
        });
    });

    describe('when a list is passed and list is not the first parameter', () => {
        it('should map the list', () => {
            const helper = ListToQueryParam.convert('test', ['obj1', 'obj2'], false);
          expect(helper).toEqual('&test=obj1&test=obj2');
        });
    });
});
