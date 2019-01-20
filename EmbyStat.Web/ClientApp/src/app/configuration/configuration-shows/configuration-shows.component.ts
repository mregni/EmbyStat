import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-configuration-shows',
  templateUrl: './configuration-shows.component.html',
  styleUrls: ['./configuration-shows.component.scss']
})
export class ConfigurationShowsComponent implements OnInit {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;

  configChangedSub: Subscription;
  newCollectionList: number[];

  constructor(private configurationFacade: ConfigurationFacade, private toaster: ToastService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
    });
  }

  public saveFormCollectionTypes() {
    const config = { ...this.configuration };
    config.showCollectionTypes = this.newCollectionList;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.SHOWS');
  }

  public onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnInit() {
  }

}
