import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';

import { ConfigurationFacade } from '../configuration/state/configuration.facade';
import { EmbyUdpBroadcast } from '../configuration/models/embyUdpBroadcast';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss']
})
export class WizardComponent implements OnInit, OnDestroy {
  public introFormGroup: FormGroup;
  public nameControl: FormControl = new FormControl('', [Validators.required]);
  public languageControl: FormControl = new FormControl('en', [Validators.required]);

  public embyFormGroup: FormGroup;
  public embyAddressControl: FormControl = new FormControl('', [Validators.required]);
  public embyUsernameControl: FormControl = new FormControl('', [Validators.required]);
  public embyPasswordControl: FormControl = new FormControl('', [Validators.required]);

  public languageChangedSub: Subscription;
  public searchEmbySub: Subscription;

  public embyFound: boolean = false;
  public embyServerName: string = "";
  public hidePassword = true;

  constructor(private translate: TranslateService, private configurationFacade: ConfigurationFacade) {
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
  }

  ngOnInit() {
    this.configurationFacade.searchEmby().subscribe((data: EmbyUdpBroadcast) => {
      if (!!data) {
        this.embyFound = true;
        this.embyAddressControl.setValue(data.address);
        this.embyServerName = data.name;
      }
    })
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  ngOnDestroy() {
    this.languageChangedSub.unsubscribe();
    this.searchEmbySub.unsubscribe();
  }
}
