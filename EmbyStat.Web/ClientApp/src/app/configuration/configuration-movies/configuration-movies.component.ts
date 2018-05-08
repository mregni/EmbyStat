import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';

@Component({
  selector: 'app-configuration-movies',
  templateUrl: './configuration-movies.component.html',
  styleUrls: ['./configuration-movies.component.scss']
})
export class ConfigurationMoviesComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;
  public configChangedSub: Subscription;

  public toShortMovieControl: FormControl = new FormControl('', [Validators.required]);

  public form: FormGroup;

  constructor(private configurationFacade: ConfigurationFacade) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.form = new FormGroup({
      toShortMovie: this.toShortMovieControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.form.setValue({ toShortMovie: config.toShortMovie });
    });
  }

  public saveForm() {
    const config = { ...this.configuration };
    config.toShortMovie = this.form.get('toShortMovie').value;
    this.configurationFacade.updateConfiguration(config);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
