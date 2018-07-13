import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { TranslateService } from '@ngx-translate/core';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';

@Component({
  selector: 'app-configuration-general',
  templateUrl: './configuration-general.component.html',
  styleUrls: ['./configuration-general.component.scss']
})
export class ConfigurationGeneralComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;
  public languageChangedSub: Subscription;
  public configChangedSub: Subscription;

  public form: FormGroup;
  public nameControl: FormControl = new FormControl('', [Validators.required]);
  public languageControl: FormControl = new FormControl('en-US', [Validators.required]);

  constructor(private configurationFacade: ConfigurationFacade, private translate: TranslateService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.form = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.form.setValue({ name: config.username, language: config.language });
    });

    this.languageChangedSub = this.languageControl.valueChanges
      .subscribe((value => this.languageChanged(value)));
  }

  public saveForm() {
    const config = { ...this.configuration };
    config.language = this.form.get('language').value;
    config.username = this.form.get('name').value;
    this.configurationFacade.updateConfiguration(config);
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.languageChangedSub !== undefined) {
      this.languageChangedSub.unsubscribe();
    }

    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
