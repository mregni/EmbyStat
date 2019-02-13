import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { AboutFacade } from '../state/facade.about';
import { About } from '../models/about';

@Component({
  selector: 'app-about-overview',
  templateUrl: './about-overview.component.html',
  styleUrls: ['./about-overview.component.scss']
})
export class AboutOverviewComponent implements OnInit {
  public about$: Observable<About>;

  constructor(private aboutFacade: AboutFacade) {
    this.about$ = this.aboutFacade.getAbout();
  }

  ngOnInit() {
  }

}
