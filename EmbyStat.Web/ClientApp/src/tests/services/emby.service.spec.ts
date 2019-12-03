import { EmbyLogin } from 'src/app/shared/models/emby/emby-login';
import { EmbyStatus } from 'src/app/shared/models/emby/emby-status';
import { EmbyUdpBroadcast } from 'src/app/shared/models/emby/emby-udp-broadcast';
import { EmbyUser } from 'src/app/shared/models/emby/emby-user';
import { UserId } from 'src/app/shared/models/emby/user-id';
import { ListContainer } from 'src/app/shared/models/list-container';
import { UserMediaView } from 'src/app/shared/models/session/user-media-view';
import { EmbyService } from 'src/app/shared/services/emby.service';

import { HttpResponse } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { EmbyPlugin } from '../../app/shared/models/emby/emby-plugin';
import { ServerInfo } from '../../app/shared/models/emby/server-info';

describe('EmbyService', () => {
    let injector: TestBed;
    let service: EmbyService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [EmbyService]
        });
        injector = getTestBed();
        service = injector.get(EmbyService);
        httpMock = injector.get(HttpTestingController);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should fetch Emby plugins', () => {
        const pluginMoc = [
            new EmbyPlugin(),
            new EmbyPlugin()
        ];

        service.getPlugins().subscribe((plugins: EmbyPlugin[]) => {
            expect(plugins).toEqual(pluginMoc);
        });

        const req = httpMock.expectOne('/api/emby/plugins');
        expect(req.request.method).toBe('GET');
        req.flush(pluginMoc);
    });

    it('should fetch Emby server info', () => {
        const infoMock = new ServerInfo();
        infoMock.id = '12';

        service.getEmbyServerInfo().subscribe((info: ServerInfo) => {
            expect(info.id).toEqual(infoMock.id);
        });

        const req = httpMock.expectOne('/api/emby/server/info');
        expect(req.request.method).toBe('GET');
        req.flush(infoMock);
    });

    it('should search for an Emby server', () => {
        const broadcastMock = new EmbyUdpBroadcast();
        broadcastMock.id = '123';

        service.searchEmby().subscribe((result: EmbyUdpBroadcast) => {
            expect(result).toEqual(broadcastMock);
        });

        const req = httpMock.expectOne('/api/emby/server/search');
        expect(req.request.method).toBe('GET');
        req.flush(broadcastMock);
    });

    it('should get Emby server status', () => {
        const embyStatusMock = new EmbyStatus();
        embyStatusMock.missedPings = 1;

        service.getEmbyStatus().subscribe((result: EmbyStatus) => {
            expect(result).toEqual(embyStatusMock);
        });

        const req = httpMock.expectOne('/api/emby/server/status');
        expect(req.request.method).toBe('GET');
        req.flush(embyStatusMock);
    });

    it('should fetch Emby users', () => {
        const embyUsersMock = [
            new EmbyUser(),
            new EmbyUser()
        ];

        service.getUsers().subscribe((result: EmbyUser[]) => {
            expect(result).toEqual(embyUsersMock);
        });

        const req = httpMock.expectOne('/api/emby/users');
        expect(req.request.method).toBe('GET');
        req.flush(embyUsersMock);
    });

    it('should fetch Emby user by id', () => {
        const embyUserMock = new EmbyUser();
        embyUserMock.id = '123';

        service.getUserById(embyUserMock.id).subscribe((result: EmbyUser) => {
            expect(result).toEqual(embyUserMock);
        });

        const req = httpMock.expectOne(`/api/emby/users/${embyUserMock.id}`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserMock);
    });

    it('should fetch Emby user ids', () => {
        const embyUserIds = [
            new UserId(),
            new UserId()
        ];

        service.getUserIdList().subscribe((result: UserId[]) => {
            expect(result).toEqual(embyUserIds);
        });

        const req = httpMock.expectOne(`/api/emby/ids`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserIds);
    });

    it('should fetch Emby user ids', () => {
        const embyUserViewPage = new ListContainer<UserMediaView>();
        embyUserViewPage.data = [
            new UserMediaView(),
            new UserMediaView()
        ];
        embyUserViewPage.totalCount = embyUserViewPage.data.length;

        service.getUserViewsByUserId('1', 0, 10).subscribe((result: ListContainer<UserMediaView>) => {
            expect(result).toEqual(embyUserViewPage);
        });

        const req = httpMock.expectOne(`/api/emby/users/1/views/0/10`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserViewPage);
    });

    afterEach(() => {
        httpMock.verify();
    });
});
