import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { UpdateResult } from '../../shared/models/update-result';
import { ToastService } from '../../shared/services/toast.service';
import { SystemService } from '../../shared/services/system.service';

@Component({
  selector: 'app-configuration-updates',
  templateUrl: './configuration-updates.component.html',
  styleUrls: ['./configuration-updates.component.scss']
})
export class ConfigurationUpdatesComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  updateResult$: Observable<UpdateResult>;
  configuration: Configuration;
  configChangedSub: Subscription;
  updatingSub: Subscription;
  onMaster = true;

  form: FormGroup;
  autoUpdateControl = new FormControl('', [Validators.required] );
  trainControl = new FormControl('', [Validators.required]);

  constructor(
    private configurationFacade: ConfigurationFacade,
    private toaster: ToastService,
    private systemService: SystemService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.updateResult$ = this.systemService.checkForUpdate();

    this.form = new FormGroup({
      autoUpdate: this.autoUpdateControl,
      train: this.trainControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.autoUpdateControl.setValue(config.autoUpdate);
      this.trainControl.setValue(config.updateTrain);
      this.onMaster = config.updateTrain === 2;
    });
  }

  save() {
    const config = { ...this.configuration };
    config.updateTrain = this.trainControl.value;
    config.autoUpdate = this.autoUpdateControl.value;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.UPDATES');
  }

  ngOnInit() {
  }

  startUpdate() {
    this.systemService.checkAndStartUpdate().subscribe((newVersion: boolean) => {
      this.setUpdateState(newVersion);
    });
  }

  private setUpdateState(state: boolean) {
    const config = { ...this.configuration };
    config.updateInProgress = state;
    this.configurationFacade.updateConfiguration(config);
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }

    if (this.updatingSub !== undefined) {
      this.updatingSub.unsubscribe();
    }
  }
}
