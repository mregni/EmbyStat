import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { EmbyToken } from '../../shared/models/emby/emby-token';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-configuration-emby',
  templateUrl: './configuration-emby.component.html',
  styleUrls: ['./configuration-emby.component.scss']
})
export class ConfigurationEmbyComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;

  form: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('', [Validators.required]);
  embyUsernameControl = new FormControl('', [Validators.required]);
  embyPasswordControl = new FormControl('', [Validators.required]);

  configChangedSub: Subscription;
  hidePassword = true;

  constructor(private configurationFacade: ConfigurationFacade, private toaster: ToastService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.form = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.form.markAsUntouched();
      this.form.setValue({
        embyUsername: config.embyUserName,
        embyAddress: config.embyServerAddress,
        embyPort: config.embyServerPort,
        embyProtocol: config.embyServerProtocol,
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
      this.toaster.pushWarning('CONFIGURATION.EMBY.NOPASSWORD');
    } else {
      const url = (protocol === 0 ? 'http://' : 'https://') + address + ':' + port;
      console.log(url);
      this.configurationFacade.getToken(username, password, url)
        .subscribe((token: EmbyToken) => {
          if (token.isAdmin) {
            const config = { ...this.configuration };
            config.embyUserName = username;
            config.embyServerAddress = address;
            config.accessToken = token.token;
            config.embyUserId = token.id;
            config.embyServerPort = port;
            config.embyServerProtocol = protocol;
            this.configurationFacade.updateConfiguration(config);
            this.toaster.pushSuccess('CONFIGURATION.SAVED.EMBY');
          }
        });
    }
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
