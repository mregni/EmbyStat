import { Subscription } from 'rxjs';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { PersonPoster } from '../../../models/common/person-poster';
import { Settings } from '../../../models/settings/settings';
import { SettingsService } from '../../../services/settings.service';

@Component({
  selector: 'app-person-list',
  templateUrl: './person-list.component.html',
  styleUrls: ['./person-list.component.scss']
})
export class PersonListComponent implements OnInit, OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() posters: PersonPoster[];

  opts = {
    position: 'right',
    alwaysVisible: true
  };

  constructor(private settingsService: SettingsService) {
    this.settingsSub = settingsService.getSettings().subscribe(data => this.settings = data);
  }

  ngOnInit() {

  }

  getPoster(tag: string, mediaId: string) {
    if (this.settings === undefined) {
      return '';
    }

    if (tag === null) {
      return '../../../../assets/images/backgrounds/defaultPerson.png';
    }

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${fullAddress}/emby/Items/${mediaId}/Images/Primary?maxHeight=350&tag=${tag}&quality=90&enableimageenhancers=false`;
  }

  openPerson(mediaId: string): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${mediaId}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
