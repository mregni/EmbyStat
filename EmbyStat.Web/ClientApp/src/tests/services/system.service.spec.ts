import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { SystemService } from '../../app/shared/services/system.service';
import { UpdateResult } from '../../app/shared/models/settings/update-result';

describe('SystemService', () => {
    let injector: TestBed;
    let service: SystemService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [SystemService]
        });
        injector = getTestBed();
        service = injector.get(SystemService);
        httpMock = injector.get(HttpTestingController);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should check for updates', () => {
      const updateMock = new UpdateResult();
      updateMock.isUpdateAvailable = false;

      service.checkForUpdate().subscribe((result: UpdateResult) => {
        expect(result).toEqual(updateMock);
      });

        const req = httpMock.expectOne('/api/system/checkforupdate');
        expect(req.request.method).toBe('GET');
        req.flush(updateMock);
    });

    it('should start backend update', () => {
      service.startUpdate().subscribe();

      const req = httpMock.expectOne('/api/system/startupdate');
      expect(req.request.method).toBe('POST');
    });

    it('should ping the media server', () => {
      service.ping().subscribe();

      const req = httpMock.expectOne('/api/system/ping');
      expect(req.request.method).toBe('GET');
    });

    afterEach(() => {
        httpMock.verify();
    });
});
