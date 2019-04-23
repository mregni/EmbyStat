import { TitleService } from 'src/app/shared/services/title.service';

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-settings-container',
  templateUrl: './settings-container.component.html',
  styleUrls: ['./settings-container.component.scss']
})
export class SettingsContainerComponent implements OnInit {

  constructor(private readonly titleService: TitleService) {
    this.titleService.updateTitle('Settings');
  }

  ngOnInit() {
  }

}
