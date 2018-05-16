import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { ShowChartsService } from '../service/show-charts.service';
import { ShowFacade } from '../state/facade.show';
import { ShowGraphs } from '../models/showGraphs';
@Component({
  selector: 'app-show-charts',
  templateUrl: './show-charts.component.html',
  styleUrls: ['./show-charts.component.scss']
})
export class ShowChartsComponent implements OnInit, OnDestroy {
  private _selectedCollections: string[];

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;

    if (this.onTab) {
      this.graphs$ = this.showFacade.getGraphs(this._selectedCollections);
    }
  }

  public graphs$: Observable<ShowGraphs>;
  private showChartSub: Subscription;
  private onTab = false;

  constructor(private showFacade: ShowFacade, private showChartsService: ShowChartsService) {
    showChartsService.open.subscribe(value => {
      this.onTab = value;
      if (value) {
        this.graphs$ = this.showFacade.getGraphs(this._selectedCollections);
      } else {
        this.showFacade.clearGraphs();
        this.graphs$ = undefined;
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
