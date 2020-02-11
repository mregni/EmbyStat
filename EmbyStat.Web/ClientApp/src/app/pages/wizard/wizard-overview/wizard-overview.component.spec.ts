import { MockComponent, MockPipe } from 'ng-mocks';
import { Observable, of } from 'rxjs';

import { DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgProgressModule } from '@ngx-progressbar/core';
import { TranslateModule, TranslatePipe, TranslateService } from '@ngx-translate/core';

import { LanguageComponent } from '../../../shared/components/language/language.component';
import {
    SideNavigationComponent
} from '../../../shared/components/side-navigation/side-navigation.component';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { CheckBoolean } from '../../../shared/enums/check-boolean-enum';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { MaterialModule } from '../../../shared/material.module';
import { Language } from '../../../shared/models/language';
import { MediaServerLogin } from '../../../shared/models/media-server/media-server-login';
import {
    MediaServerUdpBroadcast
} from '../../../shared/models/media-server/media-server-udp-broadcast';
import { Settings } from '../../../shared/models/settings/settings';
import { CapitalizeFirstPipe } from '../../../shared/pipes/capitalize-first.pipe';
import { JobService } from '../../../shared/services/job.service';
import { MediaServerService } from '../../../shared/services/media-server.service';
import { SideBarService } from '../../../shared/services/side-bar.service';
import { WizardOverviewComponent } from './wizard-overview.component';

class MockedSettingsFacade {
  public settings$: Observable<Settings>;

  constructor() {
    this.settings$ = of(new Settings());
  }

  public getLanguages(): Observable<Language[]> {
    return of([new Language(), new Language()]);
  }

  public updateSettings(settings: Settings): void {

  }
}

class MockedTranslateService {
  public get(key: any): any {
    return of(key);
  }

  public use(lang: string) {

  }
}

class MockedMediaServerService {
  public searchMediaServer(type: number): Observable<MediaServerUdpBroadcast> {
    return of(new MediaServerUdpBroadcast());
  }

  public pingEmby(url: string): Observable<boolean> {
    return of(false);
  }

  public testApiKey(login: MediaServerLogin): Observable<boolean> {
    return of(false);
  }
}

class MockedJobService {
  public fireJob(id: string): Observable<void> {
    return of();
  }
}

class MockedSideBarService {
  public closeMenu() {

  }

  public openMenu() {

  }
}

describe('WizardOverviewComponent', () => {
  let component: WizardOverviewComponent;
  let fixture: ComponentFixture<WizardOverviewComponent>;
  let debugElement: DebugElement;
  let router: Router;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        NgProgressModule,
        MaterialModule,
        FontAwesomeModule,
        NoopAnimationsModule,
        TranslateModule,
        ReactiveFormsModule
      ],
      declarations: [
        WizardOverviewComponent,
        MockComponent(ToolbarComponent),
        MockComponent(SideNavigationComponent),
        MockComponent(LanguageComponent),
        MockPipe(CapitalizeFirstPipe),
        MockPipe(TranslatePipe)
      ],
      providers: [
        { provide: SettingsFacade, useClass: MockedSettingsFacade },
        { provide: TranslateService, useClass: MockedTranslateService },
        { provide: MediaServerService, useClass: MockedMediaServerService },
        { provide: JobService, useClass: MockedJobService },
        { provide: SideBarService, useClass: MockedSideBarService }
      ]
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(WizardOverviewComponent);
        component = fixture.componentInstance;
        debugElement = fixture.debugElement;
        router = TestBed.get(Router);

        fixture.detectChanges();

        fixture.ngZone.run(() => {
          router.initialNavigation();
        });
      });
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should finish wizard', () => {
    const jobService = debugElement.injector.get(JobService);
    const jobServiceFireJobSpy = spyOn(jobService, 'fireJob').and.callThrough();

    const sideBarService = debugElement.injector.get(SideBarService);
    const sideBarServiceOpenMenuSpy = spyOn(sideBarService, 'openMenu').and.callThrough();

    const routerNavigateSpy = spyOn(router, 'navigate').and.callThrough();

    component.finishWizard();

    expect(jobServiceFireJobSpy).toHaveBeenCalledWith('41e0bf22-1e6b-4f5d-90be-ec966f746a2f');
    expect(jobServiceFireJobSpy).toHaveBeenCalledTimes(1);
    expect(sideBarServiceOpenMenuSpy).toHaveBeenCalledTimes(1);
    expect(routerNavigateSpy).toHaveBeenCalledTimes(1);
    expect(routerNavigateSpy).toHaveBeenCalledWith(['']);
  });

  it('should finish wizard and start sync', () => {
    const jobService = debugElement.injector.get(JobService);
    const jobServiceFireJobSpy = spyOn(jobService, 'fireJob').and.callThrough();

    const sideBarService = debugElement.injector.get(SideBarService);
    const sideBarServiceOpenMenuSpy = spyOn(sideBarService, 'openMenu').and.callThrough();

    const routerNavigateSpy = spyOn(router, 'navigate').and.callThrough();

    component.finishWizardAndStartSync();

    expect(jobServiceFireJobSpy).toHaveBeenCalledWith('41e0bf22-1e6b-4f5d-90be-ec966f746a2f');
    expect(jobServiceFireJobSpy).toHaveBeenCalledWith('be68900b-ee1d-41ef-b12f-60ef3106052e');
    expect(jobServiceFireJobSpy).toHaveBeenCalledTimes(2);
    expect(sideBarServiceOpenMenuSpy).toHaveBeenCalledTimes(1);
    expect(routerNavigateSpy).toHaveBeenCalledTimes(1);
    expect(routerNavigateSpy).toHaveBeenCalledWith(['/jobs']);
  });

  it('should accept any user name', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      const nameInput = debugElement.query(By.css('input[formControlName="name"]')).nativeElement;
      nameInput.click();
      fixture.detectChanges();
      fixture.whenStable().then(() => {
        expect(component.nameControl.value).toBe('test user');
      });
    });
  });

  it('should select the proper Emby protocol', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();

    const matSelect = debugElement.queryAll(By.css('.mat-select-trigger'))[1].nativeElement;
    matSelect.click();
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      const matOption = debugElement.query(By.css('.mat-option[value="http://"')).nativeElement;
      matOption.click();
      fixture.detectChanges();
      fixture.whenStable().then(() => {
        expect(component.serverProtocolControl.value).toBe(1);
        expect(component.serverUrl).toBe('http://:');
      });
    });
  });

  it('should accept Emby url', async () => {
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      component.nameControl.setValue('test user');
      const button = debugElement.nativeElement.querySelector('button');
      button.click();

      const urlInput = debugElement.query(By.css('input[formControlName="serverAddress"]')).nativeElement;
      urlInput.value = 'localhost';
      urlInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();
      fixture.whenStable().then(() => {
        expect(component.serverAddressControl.value).toBe('localhost');
        expect(component.serverUrl).toBe('https://localhost:');
      });
    });
  });

  it('should accept Emby port', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();

    const portInput = debugElement.query(By.css('input[formControlName="serverPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.serverPortControl.value).toBe('9000');
      expect(component.serverUrl).toBe('https://:9000');
    });
  });

  it('should accept Emby API key', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();

    const apiInput = debugElement.query(By.css('input[formControlName="serverApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.serverApiKeyControl.value).toBe('srfghyddjsfdgdyhdhsrg');
    });
  });

  it('should fail login if ping to Emby fails', async () => {
    const mediaServerService = debugElement.injector.get(MediaServerService);
    const mediaServerServicePingEmbySpy = spyOn(mediaServerService, 'pingEmby').and.callThrough();

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    component.selectType('emby');

    const urlInput = debugElement.query(By.css('input[formControlName="serverAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="serverPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="serverApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[3].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.serverOnline).toBe(CheckBoolean.false);
      expect(component.apiKeyWorks).toBe(CheckBoolean.busy);
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledTimes(1);
    });
  });

  it('should fail login if API key is wrong', async () => {
    const embyService = debugElement.injector.get(MediaServerService);
    const mediaServerServicePingEmbySpy = spyOn(embyService, 'pingEmby').and.callThrough();
    mediaServerServicePingEmbySpy.and.returnValue(of(true));

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    component.selectType('emby');

    const urlInput = debugElement.query(By.css('input[formControlName="serverAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="serverPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="serverApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[3].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.serverOnline).toBe(CheckBoolean.true);
      expect(component.apiKeyWorks).toBe(CheckBoolean.false);
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledTimes(1);
    });
  });

  it('should update settings', async () => {
    const mediaServerService = debugElement.injector.get(MediaServerService);
    const mediaServerServicePingEmbySpy = spyOn(mediaServerService, 'pingEmby').and.returnValue(of(true));
    const mediaServerServiceTestApiKeySpy = spyOn(mediaServerService, 'testApiKey').and.returnValue(of(true));

    const settingsFacade = debugElement.injector.get(SettingsFacade);
    const settingsFacadeUpdateSettingsSpy = spyOn(settingsFacade, 'updateSettings').and.callThrough();

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    component.selectType('emby');

    const urlInput = debugElement.query(By.css('input[formControlName="serverAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="serverPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="serverApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[3].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.serverOnline).toBe(CheckBoolean.true);
      expect(component.apiKeyWorks).toBe(CheckBoolean.true);
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(mediaServerServicePingEmbySpy).toHaveBeenCalledTimes(1);

      const login = new MediaServerLogin('srfghyddjsfdgdyhdhsrg', 'https://localhost:9000');
      expect(mediaServerServiceTestApiKeySpy).toHaveBeenCalledWith(login);
      expect(mediaServerServiceTestApiKeySpy).toHaveBeenCalledTimes(1);

      expect(settingsFacadeUpdateSettingsSpy).toHaveBeenCalledTimes(1);
    });
  });
});
