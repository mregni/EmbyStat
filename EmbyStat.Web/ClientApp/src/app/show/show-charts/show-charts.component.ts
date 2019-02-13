import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription ,  Observable } from 'rxjs';

import { ShowChartsService } from '../service/show-charts.service';
import { ShowService } from '../service/show.service';
import { ShowGraphs } from '../models/show-graphs';

@Component({
  selector: 'app-show-charts',
  templateUrl: './show-charts.component.html',
  styleUrls: ['./show-charts.component.scss']
})
export class ShowChartsComponent implements OnInit, OnDestroy {
  private _selectedCollections: string[];
  private previousOnTabValue: boolean;
  private showChartSub: Subscription;
  private onTab = false;

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
    this.graphs$ = undefined;

    if (this.onTab) {
      this.graphs$ = this.showService.getGraphs(this._selectedCollections);
    }
  }

  graphs$: Observable<ShowGraphs>;

  constructor(private showService: ShowService, private showChartsService: ShowChartsService) {
    showChartsService.open.subscribe(value => {
      this.onTab = value;
      if (value && !this.previousOnTabValue) {
        this.previousOnTabValue = value;
        this.graphs$ = this.showService.getGraphs(this._selectedCollections);
      }
    });
  }

  ngOnInit() {

  }

  ngOnDestroy(): void {
    this.showChartsService.changeOpened(false);

    if (this.showChartSub !== undefined) {
      this.showChartSub.unsubscribe();
    }
  }
}
