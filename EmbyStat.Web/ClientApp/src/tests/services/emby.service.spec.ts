import { ListContainer } from 'src/app/shared/models/list-container';
import { MediaServerLogin } from 'src/app/shared/models/media-server/media-server-login';
import { MediaServerStatus } from 'src/app/shared/models/media-server/media-server-status';
import {
    MediaServerUdpBroadcast
} from 'src/app/shared/models/media-server/media-server-udp-broadcast';
import { MediaServerUser } from 'src/app/shared/models/media-server/media-server-user';
import { UserId } from 'src/app/shared/models/media-server/user-id';
import { UserMediaView } from 'src/app/shared/models/session/user-media-view';
import { MediaServerService } from 'src/app/shared/services/media-server.service';

import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { MediaServerPlugin } from '../../app/shared/models/media-server/media-server-plugin';
import { ServerInfo } from '../../app/shared/models/media-server/server-info';

describe('EmbyService', () => {
    let injector: TestBed;
    let service: MediaServerService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [MediaServerService]
        });
        injector = getTestBed();
        service = injector.inject(MediaServerService);
        httpMock = injector.inject(HttpTestingController);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should fetch media server plugins', () => {
        const pluginMoc = [
            new MediaServerPlugin(),
            new MediaServerPlugin()
        ];

        service.getPlugins().subscribe((plugins: MediaServerPlugin[]) => {
            expect(plugins).toEqual(pluginMoc);
        });

        const req = httpMock.expectOne('/api/mediaserver/plugins');
        expect(req.request.method).toBe('GET');
        req.flush(pluginMoc);
    });

    it('should fetch media server server info', () => {
        const infoMock = new ServerInfo();
        infoMock.id = '12';

        service.getEmbyServerInfo().subscribe((info: ServerInfo) => {
            expect(info.id).toEqual(infoMock.id);
        });

        const req = httpMock.expectOne('/api/mediaserver/server/info');
        expect(req.request.method).toBe('GET');
        req.flush(infoMock);
    });

    it('should search for an media server server', () => {
        const broadcastMock = new MediaServerUdpBroadcast();
        broadcastMock.id = '123';

        service.searchMediaServer(0).subscribe((result: MediaServerUdpBroadcast) => {
            expect(result).toEqual(broadcastMock);
        });

        const req = httpMock.expectOne('/api/mediaserver/server/search?serverType=0');
        expect(req.request.method).toBe('GET');
        req.flush(broadcastMock);
    });

    it('should get media server server status', () => {
        const embyStatusMock = new MediaServerStatus();
        embyStatusMock.missedPings = 1;

        service.getEmbyStatus().subscribe((result: MediaServerStatus) => {
            expect(result).toEqual(embyStatusMock);
        });

        const req = httpMock.expectOne('/api/mediaserver/server/status');
        expect(req.request.method).toBe('GET');
        req.flush(embyStatusMock);
    });

    it('should fetch media server users', () => {
        const embyUsersMock = [
            new MediaServerUser(),
            new MediaServerUser()
        ];

        service.getUsers().subscribe((result: MediaServerUser[]) => {
            expect(result).toEqual(embyUsersMock);
        });

        const req = httpMock.expectOne('/api/mediaserver/users');
        expect(req.request.method).toBe('GET');
        req.flush(embyUsersMock);
    });

    it('should fetch media server user by id', () => {
        const embyUserMock = new MediaServerUser();
        embyUserMock.id = '123';

        service.getUserById(embyUserMock.id).subscribe((result: MediaServerUser) => {
            expect(result).toEqual(embyUserMock);
        });

        const req = httpMock.expectOne(`/api/mediaserver/users/${embyUserMock.id}`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserMock);
    });

    it('should fetch media server user ids', () => {
        const embyUserIds = [
            new UserId(),
            new UserId()
        ];

        service.getUserIdList().subscribe((result: UserId[]) => {
            expect(result).toEqual(embyUserIds);
        });

        const req = httpMock.expectOne(`/api/mediaserver/ids`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserIds);
    });

    it('should fetch media server user ids', () => {
        const embyUserViewPage = new ListContainer<UserMediaView>();
        embyUserViewPage.data = [
            new UserMediaView(),
            new UserMediaView()
        ];
        embyUserViewPage.totalCount = embyUserViewPage.data.length;

        service.getUserViewsByUserId('1', 0, 10).subscribe((result: ListContainer<UserMediaView>) => {
            expect(result).toEqual(embyUserViewPage);
        });

        const req = httpMock.expectOne(`/api/mediaserver/users/1/views/0/10`);
        expect(req.request.method).toBe('GET');
        req.flush(embyUserViewPage);
    });

    afterEach(() => {
        httpMock.verify();
    });
});
