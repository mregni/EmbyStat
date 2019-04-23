import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { TitleService } from '../../../shared/services/title.service';
import { About } from '../models/about';
import { AboutService } from '../services/about.service';

@Component({
  selector: 'app-about-overview',
  templateUrl: './about-overview.component.html',
  styleUrls: ['./about-overview.component.scss']
})
export class AboutOverviewComponent implements OnInit {
  about$: Observable<About>;

  constructor(
    private readonly translate: TranslateService,
    private readonly titleService: TitleService,
    private readonly aboutService: AboutService) {
      this.translate.get('MENU.ABOUT').subscribe((translation: string) => {
        console.log(translation);
        this.titleService.updateTitle(translation);
    });

    this.about$ = this.aboutService.getAbout();
  }

  ngOnInit() {
  }

}
