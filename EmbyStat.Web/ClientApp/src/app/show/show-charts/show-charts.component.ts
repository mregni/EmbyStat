import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';

import { ShowChartsService } from '../service/show-charts.service';
import { ShowFacade } from '../state/facade.show';
import { ShowGraphs } from '../models/showGraphs';
import { LoaderFacade } from '../../shared/components/loader/state/facade.loader';

@Component({
  selector: 'app-show-charts',
  templateUrl: './show-charts.component.html',
  styleUrls: ['./show-charts.component.scss']
})
export class ShowChartsComponent implements OnInit, OnDestroy {
  private _selectedCollections: string[];
  private previousOnTabValue: boolean;

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
      this.graphs$ = this.showFacade.getGraphs(this._selectedCollections);
    }
  }

  public graphs$: Observable<ShowGraphs>;
  private showChartSub: Subscription;
  private onTab = false;

  constructor(private showFacade: ShowFacade,
    private showChartsService: ShowChartsService,
    private loaderFacade: LoaderFacade) {
    showChartsService.open.subscribe(value => {
      this.onTab = value;
      if (value && !this.previousOnTabValue) {
        this.previousOnTabValue = value;
        this.graphs$ = this.showFacade.getGraphs(this._selectedCollections);
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
