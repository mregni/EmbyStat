import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { EmbyToken } from '../models/embyToken';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-configuration-emby',
  templateUrl: './configuration-emby.component.html',
  styleUrls: ['./configuration-emby.component.scss']
})
export class ConfigurationEmbyComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;

  public form: FormGroup;
  public embyAddressControl: FormControl = new FormControl('', [Validators.required]);
  public embyUsernameControl: FormControl = new FormControl('', [Validators.required]);
  public embyPasswordControl: FormControl = new FormControl('', [Validators.required]);

  public configChangedSub: Subscription;

  public hidePassword = true;

  constructor(private configurationFacade: ConfigurationFacade, private toaster: ToastService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.form = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.form.markAsUntouched();
      this.form.setValue({ embyUsername: config.embyUserName, embyAddress: config.embyServerAddress, embyPassword: '' });
    });
  }

  public saveForm() {
    const username = this.form.get('embyUsername').value;
    const password = this.form.get('embyPassword').value;
    const address = this.form.get('embyAddress').value;
    this.configurationFacade.getToken(username, password, address)
      .subscribe((token: EmbyToken) => {
        if (token.isAdmin) {
          const config = { ...this.configuration };
          config.embyUserName = username;
          config.embyServerAddress = address;
          config.accessToken = token.token;
          config.embyUserId = token.id;
          this.configurationFacade.updateConfiguration(config);
          this.toaster.pushSuccess('CONFIGURATION.SAVED.EMBY');
        }
      });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
