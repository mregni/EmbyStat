import { MockComponent, MockPipe } from 'ng-mocks';
import { Observable, of } from 'rxjs';
import { debug } from 'util';

import { DebugElement, Input } from '@angular/core';
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
import { EmbyLogin } from '../../../shared/models/emby/emby-login';
import { EmbyUdpBroadcast } from '../../../shared/models/emby/emby-udp-broadcast';
import { Language } from '../../../shared/models/language';
import { Settings } from '../../../shared/models/settings/settings';
import { CapitalizeFirstPipe } from '../../../shared/pipes/capitalize-first.pipe';
import { EmbyService } from '../../../shared/services/emby.service';
import { JobService } from '../../../shared/services/job.service';
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

class MockedEmbyService {
  public searchEmby(): Observable<EmbyUdpBroadcast> {
    return of(new EmbyUdpBroadcast());
  }

  public pingEmby(url: string): Observable<boolean> {
    return of(false);
  }

  public testApiKey(login: EmbyLogin): Observable<boolean> {
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
        { provide: EmbyService, useClass: MockedEmbyService },
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
        expect(component.embyProtocolControl.value).toBe(1);
        expect(component.embyUrl).toBe('http://:');
      });
    });
  });

  it('should accept Emby url', async () => {
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      component.nameControl.setValue('test user');
      const button = debugElement.nativeElement.querySelector('button');
      button.click();

      const urlInput = debugElement.query(By.css('input[formControlName="embyAddress"]')).nativeElement;
      urlInput.value = 'localhost';
      urlInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();
      fixture.whenStable().then(() => {
        expect(component.embyAddressControl.value).toBe('localhost');
        expect(component.embyUrl).toBe('https://localhost:');
      });
    });
  });

  it('should accept Emby port', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();

    const portInput = debugElement.query(By.css('input[formControlName="embyPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.embyPortControl.value).toBe('9000');
      expect(component.embyUrl).toBe('https://:9000');
    });
  });

  it('should accept Emby API key', async () => {
    component.nameControl.setValue('test user');
    const button = debugElement.nativeElement.querySelector('button');
    button.click();

    const apiInput = debugElement.query(By.css('input[formControlName="embyApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.embyApiKeyControl.value).toBe('srfghyddjsfdgdyhdhsrg');
    });
  });

  it('should fail login if ping to Emby fails', async () => {
    const embyService = debugElement.injector.get(EmbyService);
    const embyServicePingEmbySpy = spyOn(embyService, 'pingEmby').and.callThrough();

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    const urlInput = debugElement.query(By.css('input[formControlName="embyAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="embyPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="embyApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[2].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.embyOnline).toBe(CheckBoolean.false);
      expect(component.apiKeyWorks).toBe(CheckBoolean.busy);
      expect(embyServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(embyServicePingEmbySpy).toHaveBeenCalledTimes(1);
    });
  });

  it('should fail login if API key is wrong', async () => {
    const embyService = debugElement.injector.get(EmbyService);
    const embyServicePingEmbySpy = spyOn(embyService, 'pingEmby').and.callThrough();
    embyServicePingEmbySpy.and.returnValue(of(true));

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    const urlInput = debugElement.query(By.css('input[formControlName="embyAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="embyPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="embyApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[2].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.embyOnline).toBe(CheckBoolean.true);
      expect(component.apiKeyWorks).toBe(CheckBoolean.false);
      expect(embyServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(embyServicePingEmbySpy).toHaveBeenCalledTimes(1);
    });
  });

  it('should update settings', async () => {
    const embyService = debugElement.injector.get(EmbyService);
    const embyServicePingEmbySpy = spyOn(embyService, 'pingEmby').and.returnValue(of(true));
    const embyServiceTestApiKeySpy = spyOn(embyService, 'testApiKey').and.returnValue(of(true));

    const settingsFacade = debugElement.injector.get(SettingsFacade);
    const settingsFacadeUpdateSettingsSpy = spyOn(settingsFacade, 'updateSettings').and.callThrough();

    component.nameControl.setValue('test user');
    let button = debugElement.queryAll(By.css('button'))[0].nativeElement;
    button.click();

    const urlInput = debugElement.query(By.css('input[formControlName="embyAddress"]')).nativeElement;
    urlInput.value = 'localhost';
    urlInput.dispatchEvent(new Event('input'));

    const portInput = debugElement.query(By.css('input[formControlName="embyPort"]')).nativeElement;
    portInput.value = '9000';
    portInput.dispatchEvent(new Event('input'));

    const apiInput = debugElement.query(By.css('input[formControlName="embyApiKey"]')).nativeElement;
    apiInput.value = 'srfghyddjsfdgdyhdhsrg';
    apiInput.dispatchEvent(new Event('input'));

    button = debugElement.queryAll(By.css('button'))[2].nativeElement;
    button.click();

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(component.embyOnline).toBe(CheckBoolean.true);
      expect(component.apiKeyWorks).toBe(CheckBoolean.true);
      expect(embyServicePingEmbySpy).toHaveBeenCalledWith('https://localhost:9000');
      expect(embyServicePingEmbySpy).toHaveBeenCalledTimes(1);

      const login = new EmbyLogin('srfghyddjsfdgdyhdhsrg', 'https://localhost:9000');
      expect(embyServiceTestApiKeySpy).toHaveBeenCalledWith(login);
      expect(embyServiceTestApiKeySpy).toHaveBeenCalledTimes(1);

      expect(settingsFacadeUpdateSettingsSpy).toHaveBeenCalledTimes(1);
    });
  });
});
