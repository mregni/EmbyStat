import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { EmbyLogin } from '../../../../shared/models/emby/emby-login';
import { EmbyToken } from '../../../../shared/models/emby/emby-token';
import { Settings } from '../../../../shared/models/settings/settings';
import { EmbyService } from '../../../../shared/services/emby.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-emby',
  templateUrl: './settings-emby.component.html',
  styleUrls: ['./settings-emby.component.scss']
})
export class SettingsEmbyComponent implements OnInit, OnChanges, OnDestroy {
  @Input() settings: Settings;

  embyTokenSub: Subscription;

  embyForm: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('0', [Validators.required]);
  embyApiKeyControl = new FormControl('', [Validators.required]);

  isSaving = false;
  hidePassword = true;

  constructor(
    private readonly toastService: ToastService,
    private readonly settingsFacade: SettingsFacade,
    private readonly embyService: EmbyService
  ) {
    this.embyForm = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyApiKey: this.embyApiKeyControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.embyAddressControl.setValue(this.settings.emby.serverAddress);
      this.embyPortControl.setValue(this.settings.emby.serverPort);
      this.embyProtocolControl.setValue(this.settings.emby.serverProtocol);
    }
  }

  saveEmbyForm() {
    for (const i of Object.keys(this.embyForm.controls)) {
      this.embyForm.controls[i].markAsTouched();
      this.embyForm.controls[i].updateValueAndValidity();
    }

    if (this.embyForm.valid) {
      this.isSaving = true;
      const url = (this.embyProtocolControl.value === 0 ? 'https://' : 'http://') + this.embyAddressControl.value + ':' + this.embyPortControl.value;
      const login = new EmbyLogin(this.embyApiKeyControl.value, url);
      this.embyTokenSub = this.embyService.testApiKey(login).subscribe((result: boolean) => {
        if (result) {
          const settings = { ...this.settings };
          const emby = { ...this.settings.emby };

          emby.serverAddress = this.embyAddressControl.value;
          emby.serverPort = this.embyPortControl.value;
          emby.serverName = '';
          emby.serverProtocol = this.embyProtocolControl.value;
          emby.apiKey = this.embyApiKeyControl.value;
          settings.emby = emby;

          this.settingsFacade.updateSettings(settings);
          this.toastService.showSuccess('SETTINGS.SAVED.EMBY');
          this.embyApiKeyControl.setValue('');
          this.embyApiKeyControl.markAsUntouched();
        } else {
          this.toastService.showError('SETTINGS.EMBY.WRONGAPIKEY');
          this.embyApiKeyControl.setValue('');
        }
      },
        error => {
          this.toastService.showError('SETTINGS.EMBY.WRONGAPIKEY');
          this.embyApiKeyControl.setValue('');
        });

      this.embyTokenSub.add(() => {
        this.isSaving = false;
      });
    }
  }

  ngOnDestroy() {
    if (this.embyTokenSub !== undefined) {
      this.embyTokenSub.unsubscribe();
    }
  }
}
