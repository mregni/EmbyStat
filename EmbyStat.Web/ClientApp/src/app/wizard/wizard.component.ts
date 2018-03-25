import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { ConfigurationFacade } from '../configuration/state/facade.configuration';
import { EmbyUdpBroadcast } from '../configuration/models/embyUdpBroadcast';
import { Configuration } from '../configuration/models/configuration';
import { EmbyToken } from '../configuration/models/embyToken';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss']
})
export class WizardComponent implements OnInit, OnDestroy {
  @ViewChild('stepper') private stepper: MatStepper;

  public introFormGroup: FormGroup;
  public nameControl: FormControl = new FormControl('', [Validators.required]);
  public languageControl: FormControl = new FormControl('en', [Validators.required]);

  public embyFormGroup: FormGroup;
  public embyAddressControl: FormControl = new FormControl('', [Validators.required]);
  public embyUsernameControl: FormControl = new FormControl('', [Validators.required]);
  public embyPasswordControl: FormControl = new FormControl('', [Validators.required]);

  public languageChangedSub: Subscription;
  public searchEmbySub: Subscription;
  public configurationSub: Subscription;

  public embyFound: boolean = false;
  public embyServerName: string = "";
  public hidePassword = true;
  public wizardIndex = 0;
  public noAdminAccount: boolean = false;
  public savedConfiguration: boolean = false;
  private configuration: Configuration;

  constructor(private translate: TranslateService,
    private configurationFacade: ConfigurationFacade,
    private router: Router  ) {
    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.embyFormGroup = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.languageChangedSub = this.languageControl.valueChanges.subscribe((value => this.languageChanged(value)));
    this.configurationSub = this.configurationFacade.configuration$.subscribe(config => this.configuration = config);
  }

  ngOnInit() {
    this.configurationFacade.searchEmby().subscribe((data: EmbyUdpBroadcast) => {
      if (!!data.address) {
        this.embyFound = true;
        this.embyAddressControl.setValue(data.address);
        this.embyServerName = data.name;
      }
    });
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  public stepperPageChanged(event) {
    if (event.selectedIndex === 2) {
      this.noAdminAccount = false;
      var username = this.embyFormGroup.get('embyUsername').value;
      var password = this.embyFormGroup.get('embyPassword').value;
      var address = this.embyFormGroup.get('embyAddress').value;
      this.configurationFacade.getToken(username, password, address)
        .subscribe((token: EmbyToken) => {
          if (token.isAdmin) {
            var config = { ...this.configuration };
            config.language = this.introFormGroup.get('language').value;
            config.embyUserName = username;
            config.username = this.introFormGroup.get('name').value;
            config.embyServerAddress = address;
            config.accessToken = token.token;
            config.wizardFinished = true;
            this.configurationFacade.updateConfiguration(config);
            this.savedConfiguration = true;
            this.stepper.next();
          } else {
            this.noAdminAccount = true;
          }
        }, (err) => {
          this.noAdminAccount = true;
        });
    }
  }

  public finishWizard() {
    this.router.navigateByUrl('');
  }

  ngOnDestroy() {
    if (this.languageChangedSub !== undefined) {
      this.languageChangedSub.unsubscribe();
    }

    if (this.searchEmbySub !== undefined) {
      this.searchEmbySub.unsubscribe();
    }

    if (this.configurationSub !== undefined) {
      this.configurationSub.unsubscribe();
    }
  }
}
