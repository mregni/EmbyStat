import { Component, OnInit } from '@angular/core';

import { TitleService } from '../../../shared/services/title.service';

@Component({
  selector: 'app-dashboard-overview',
  templateUrl: './dashboard-overview.component.html',
  styleUrls: ['./dashboard-overview.component.scss']
})
export class DashboardOverviewComponent implements OnInit {

  constructor(private readonly titleService: TitleService) {
    this.titleService.updateTitle('Dashboard');
  }

  ngOnInit() {
  }

}
