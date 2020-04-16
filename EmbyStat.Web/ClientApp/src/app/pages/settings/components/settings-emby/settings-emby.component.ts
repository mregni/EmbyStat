import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { MediaServerTypeSelector } from '../../../../shared/helpers/media-server-type-selector';
import { MediaServerLogin } from '../../../../shared/models/media-server/media-server-login';
import { Settings } from '../../../../shared/models/settings/settings';
import { MediaServerService } from '../../../../shared/services/media-server.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'es-settings-emby',
  templateUrl: './settings-emby.component.html',
  styleUrls: ['./settings-emby.component.scss']
})
export class SettingsEmbyComponent implements OnChanges, OnDestroy {
  @Input() settings: Settings;

  embyTokenSub: Subscription;
  isSaving = false;

  embyForm: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl({ value: '0' }, [Validators.required]);
  embyApiKeyControl = new FormControl('', [Validators.required]);

  embyUrl: string;
  hidePassword = true;
  typeText: string;
  newTypeText: string;

  private embyPortControlChange: Subscription;
  private embyAddressControlChange: Subscription;
  private embyProtocolControlChange: Subscription;

  constructor(
    private readonly toastService: ToastService,
    private readonly settingsFacade: SettingsFacade,
    private readonly mediaServerService: MediaServerService
  ) {
    this.embyForm = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyApiKey: this.embyApiKeyControl
    });

    this.embyPortControl.valueChanges.subscribe((value: string) => {
      const url = this.embyAddressControl.value;
      const protocol = this.embyProtocolControl.value;
      this.updateUrl(protocol, url, value);
    });

    this.embyProtocolControl.valueChanges.subscribe((value: number) => {
      const url = this.embyAddressControl.value;
      const port = this.embyPortControl.value;
      this.updateUrl(value, url, port);
    });

    this.embyAddressControl.valueChanges.subscribe((value: string) => {
      const port = this.embyPortControl.value;
      const protocol = this.embyProtocolControl.value;
      this.updateUrl(protocol, value, port);
    });
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.embyAddressControl.setValue(this.settings.mediaServer.serverAddress);
      this.embyPortControl.setValue(this.settings.mediaServer.serverPort);
      this.embyProtocolControl.setValue(this.settings.mediaServer.serverProtocol);
      this.typeText = MediaServerTypeSelector.getServerTypeString(this.settings.mediaServer.serverType);
      this.newTypeText = MediaServerTypeSelector.getOtherServerTypeString(this.settings.mediaServer.serverType);
    }
  }

  private updateUrl(protocol: number, url: string, port: string): void {
    this.embyUrl = (protocol === 0 ? 'https://' : 'http://') + url + ':' + port;
  }

  getPage(): string {
    return MediaServerTypeSelector.getServerApiPage(this.settings.mediaServer.serverType);
  }

  saveEmbyForm(): void {
    for (const i of Object.keys(this.embyForm.controls)) {
      this.embyForm.controls[i].markAsTouched();
      this.embyForm.controls[i].updateValueAndValidity();
    }

    if (this.embyForm.valid) {
      this.isSaving = true;
      const protocol = this.embyProtocolControl.value === 0 ? 'https://' : 'http://';
      const url = `${protocol}${this.embyAddressControl.value}:${this.embyPortControl.value}`;

      const login = new MediaServerLogin(this.embyApiKeyControl.value, url);
      this.embyForm.disable();

      this.embyTokenSub = this.mediaServerService.testApiKey(login).subscribe((result: boolean) => {
        if (result) {
          const settings = { ...this.settings };
          const mediaServer = { ...this.settings.mediaServer };

          mediaServer.serverAddress = this.embyAddressControl.value;
          mediaServer.serverPort = parseInt(this.embyPortControl.value, 10);
          mediaServer.serverName = '';
          mediaServer.serverProtocol = this.embyProtocolControl.value;
          mediaServer.apiKey = this.embyApiKeyControl.value;
          console.log(this.embyApiKeyControl.value);
          settings.mediaServer = mediaServer;

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
        this.embyForm.enable();
      });
    }
  }

  ngOnDestroy(): void {
    if (this.embyTokenSub !== undefined) {
      this.embyTokenSub.unsubscribe();
    }

    if (this.embyPortControlChange !== undefined) {
      this.embyPortControlChange.unsubscribe();
    }

    if (this.embyProtocolControlChange !== undefined) {
      this.embyProtocolControlChange.unsubscribe();
    }

    if (this.embyAddressControlChange !== undefined) {
      this.embyAddressControlChange.unsubscribe();
    }
  }
}
