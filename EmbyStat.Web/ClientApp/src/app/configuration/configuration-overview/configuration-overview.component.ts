import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-configuration-overview',
  templateUrl: './configuration-overview.component.html',
  styleUrls: ['./configuration-overview.component.scss']
})
export class ConfigurationOverviewComponent implements OnInit, OnDestroy {
  private readonly paramSub: Subscription;

  selected = 0;

  constructor(private readonly activatedRoute: ActivatedRoute) {
    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const tab = params['tab'];
      switch (tab) {
        case "emby":
          this.selected = 1;
          break;
        case "movies":
          this.selected = 2;
          break;
        case "shows":
          this.selected = 3;
          break;
        case "updates":
          this.selected = 4;
          break;
        default:
          this.selected = 0;
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }
  }
}
