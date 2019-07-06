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
  embyUsernameControl = new FormControl('', [Validators.required]);
  embyPasswordControl = new FormControl('', [Validators.required]);

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
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.embyAddressControl.setValue(this.settings.emby.serverAddress);
      this.embyPortControl.setValue(this.settings.emby.serverPort);
      this.embyProtocolControl.setValue(this.settings.emby.serverProtocol);
      this.embyUsernameControl.setValue(this.settings.emby.userName);
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
      const login = new EmbyLogin(this.embyUsernameControl.value, this.embyPasswordControl.value, url);
      this.embyTokenSub = this.embyService.getEmbyToken(login).subscribe((token: EmbyToken) => {
        if (token.isAdmin) {
          const settings = { ...this.settings };
          const emby = { ...this.settings.emby };

          emby.serverAddress = this.embyAddressControl.value;
          emby.serverPort = this.embyPortControl.value;
          emby.serverProtocol = this.embyProtocolControl.value;
          emby.userName = this.embyUsernameControl.value;
          emby.accessToken = token.token;
          emby.userId = token.id;
          settings.emby = emby;

          this.settingsFacade.updateSettings(settings);
          this.toastService.showSuccess('SETTINGS.SAVED.EMBY');
          this.embyPasswordControl.setValue('');
          this.embyPasswordControl.markAsUntouched();
        } else {
          this.toastService.showError('SETTINGS.EMBY.NOADMINUSER');
          this.embyPasswordControl.setValue('');
        }
      },
        error => {
          this.toastService.showError('SETTINGS.EMBY.WRONGPASSWORD');
          this.embyPasswordControl.setValue('');
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
