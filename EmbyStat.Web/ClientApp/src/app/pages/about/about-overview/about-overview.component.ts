import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';

import { environment } from '../../../../environments/environment';
import { About } from '../models/about';
import { AboutService } from '../services/about.service';

@Component({
  selector: 'app-about-overview',
  templateUrl: './about-overview.component.html',
  styleUrls: ['./about-overview.component.scss']
})
export class AboutOverviewComponent implements OnInit {
  about$: Observable<About>;
  environment;

  constructor(private readonly aboutService: AboutService) {
    this.about$ = this.aboutService.getAbout();
    this.environment = environment;
  }

  ngOnInit() {
  }

}
