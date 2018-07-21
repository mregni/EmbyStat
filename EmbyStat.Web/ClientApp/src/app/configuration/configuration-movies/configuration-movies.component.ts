import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { ToastService } from '../../shared/services/toast.service';

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

  public formToShort: FormGroup;

  constructor(private configurationFacade: ConfigurationFacade, private toaster: ToastService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.formToShort = new FormGroup({
      toShortMovie: this.toShortMovieControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.formToShort.setValue({ toShortMovie: config.toShortMovie });
    });
  }

  public saveFormToShort() {
    const config = { ...this.configuration };
    config.toShortMovie = this.formToShort.get('toShortMovie').value;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.MOVIES');
  }

  public saveFormCollectionTypes() {
    const config = { ...this.configuration };
    config.toShortMovie = this.formToShort.get('toShortMovie').value;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.MOVIES');
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
