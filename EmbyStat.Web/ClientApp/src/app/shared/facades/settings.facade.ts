import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';

import {
    LoadSettingsAction, UpdateSettingsAction
} from '../../pages/settings/state/settings.actions';
import { SettingsQuery } from '../../pages/settings/state/settings.reducer';
import { ApplicationState } from '../../states/app.state';
import { EmbyLogin } from '../models/emby/emby-login';
import { EmbyToken } from '../models/emby/emby-token';
import { Language } from '../models/language';
// import { EmbyUdpBroadcast } from '../../../shared/models/emby/emby-udp-broadcast';
import { Settings } from '../models/settings/settings';
import { EmbyService } from '../services/emby.service';
import { SettingsService } from '../services/settings.service';

@Injectable()
export class SettingsFacade {
  constructor(
    private store: Store<ApplicationState>,
    private settingsService: SettingsService) { }

  settings$ = this.store.select(SettingsQuery.getSettings);

  getSettings(): Observable<Settings> {
    this.store.dispatch(new LoadSettingsAction());
    return this.settings$;
  }

  updateSettings(settings: Settings): void {
    this.store.dispatch(new UpdateSettingsAction(settings));
  }

  getLanguages(): Observable<Language[]> {
    return this.settingsService.getLanguages();
  }
}

