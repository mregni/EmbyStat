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

  configChangedSub: Subscription;
  newCollectionList: number[];

  formToShort: FormGroup;
  toShortMovieControl: FormControl = new FormControl('', [Validators.required]);

  constructor(private configurationFacade: ConfigurationFacade, private toaster: ToastService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.formToShort = new FormGroup({
      toShortMovie: this.toShortMovieControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.toShortMovieControl.setValue(config.toShortMovie);
    });
  }

  public saveFormToShort() {
    const config = { ...this.configuration };
    config.toShortMovie = this.toShortMovieControl.value;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.MOVIES');
  }

  public saveFormCollectionTypes() {
    const config = { ...this.configuration };
    config.movieCollectionTypes = this.newCollectionList;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.MOVIES');
  }

  public onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
