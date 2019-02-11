import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { EmbyToken } from '../../shared/models/emby/emby-token';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-settings-emby',
  templateUrl: './settings-emby.component.html',
  styleUrls: ['./settings-emby.component.scss']
})
export class SettingsEmbyComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  private settings: Settings;

  form: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('', [Validators.required]);
  embyUsernameControl = new FormControl('', [Validators.required]);
  embyPasswordControl = new FormControl('', [Validators.required]);

  settingsChangedSub: Subscription;
  hidePassword = true;

  constructor(private settingsFacade: SettingsFacade, private toaster: ToastService) {
    this.settings$ = this.settingsFacade.getSettings();

    this.form = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.settingsChangedSub = this.settings$.subscribe((settings: Settings) => {
      this.settings = settings;
      this.form.markAsUntouched();
      this.form.setValue({
        embyUsername: settings.emby.userName,
        embyAddress: settings.emby.serverAddress,
        embyPort: settings.emby.serverPort,
        embyProtocol: settings.emby.serverProtocol,
        embyPassword: ''
      });
    });
  }

  public saveForm() {
    const username = this.embyUsernameControl.value;
    const password = this.embyPasswordControl.value;
    const address = this.embyAddressControl.value;
    const port = this.embyPortControl.value;
    const protocol = this.embyProtocolControl.value;

    if (password.length === 0) {
      this.toaster.pushWarning('SETTINGS.EMBY.NOPASSWORD');
    } else {
      const url = (protocol === 0 ? 'http://' : 'https://') + address + ':' + port;
      this.settingsFacade.getToken(username, password, url)
        .subscribe((token: EmbyToken) => {
          if (token.isAdmin) {
            const settings = { ...this.settings };
            settings.emby.userName = username;
            settings.emby.serverAddress = address;
            settings.emby.accessToken = token.token;
            settings.emby.userId = token.id;
            settings.emby.serverPort = port;
            settings.emby.serverProtocol = protocol;
            this.settingsFacade.updateSettings(settings);
            this.toaster.pushSuccess('SETTINGS.SAVED.EMBY');
          }
        });
    }
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.settingsChangedSub !== undefined) {
      this.settingsChangedSub.unsubscribe();
    }
  }
}
