import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-settings-movies',
  templateUrl: './settings-movies.component.html',
  styleUrls: ['./settings-movies.component.scss']
})
export class SettingsMoviesComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  private settings: Settings;

  settingsChangedSub: Subscription;
  newCollectionList: number[];

  formToShort: FormGroup;
  toShortMovieControl: FormControl = new FormControl('', [Validators.required]);

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toaster: ToastService) {
    this.settings$ = this.settingsFacade.getSettings();

    this.formToShort = new FormGroup({
      toShortMovie: this.toShortMovieControl
    });

    this.settingsChangedSub = this.settings$.subscribe((settings: Settings) => {
      this.settings = settings;
      this.toShortMovieControl.setValue(settings.toShortMovie);
    });
  }

  public saveFormToShort() {
    const settings = { ...this.settings };
    settings.toShortMovie = this.toShortMovieControl.value;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.MOVIES');
  }

  public saveFormCollectionTypes() {
    const settings = { ...this.settings };
    settings.movieCollectionTypes = this.newCollectionList;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.MOVIES');
  }

  public onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.settingsChangedSub !== undefined) {
      this.settingsChangedSub.unsubscribe();
    }
  }
}
