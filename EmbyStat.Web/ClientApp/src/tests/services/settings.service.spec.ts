import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { SettingsService } from '../../app/shared/services/settings.service';
import { Language } from '../../app/shared/models/language';
import { Settings } from '../../app/shared/models/settings/settings';

describe('SettingsService', () => {
    let injector: TestBed;
    let service: SettingsService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [SettingsService]
        });
        injector = getTestBed();
        service = injector.get(SettingsService);
        httpMock = injector.get(HttpTestingController);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should return list of languages', () => {
      const listMock = [
        new Language(),
        new Language()
      ];

      service.getLanguages().subscribe((list: Language[]) => {
        expect(list).toEqual(listMock);
      });

        const req = httpMock.expectOne('/api/settings/languages');
        expect(req.request.method).toBe('GET');
        req.flush(listMock);
    });

    it('should return the apps settings', () => {
      const settingsMock = new Settings();
      settingsMock.id = '1';

      service.getSettings().subscribe((settings: Settings) => {
        expect(settings).toEqual(settingsMock);
      });

      const req = httpMock.expectOne('/api/settings');
      expect(req.request.method).toBe('GET');
      req.flush(settingsMock);
    });

    it('should update the app settings and return them', () => {
      const settingsMock = new Settings();
      settingsMock.id = '1';

      service.updateSettings(settingsMock).subscribe((settings: Settings) => {
        expect(settings).toEqual(settingsMock);
      });

      const req = httpMock.expectOne('/api/settings');
      expect(req.request.method).toBe('PUT');
      req.flush(settingsMock);
    });

    afterEach(() => {
        httpMock.verify();
    });
});
