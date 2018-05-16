import { Component, OnInit, Input } from '@angular/core';

import { ShowChartsService } from '../service/show-charts.service';
import { ShowFacade } from '../state/facade.show';
@Component({
  selector: 'app-show-charts',
  templateUrl: './show-charts.component.html',
  styleUrls: ['./show-charts.component.scss']
})
export class ShowChartsComponent implements OnInit {
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
  }

  constructor(private showFacade: ShowFacade, private showChartsService: ShowChartsService) {
    showChartsService.open.subscribe(value => {
      if (value) {
        //laad graphs
      }
    });
  }

  ngOnInit() {
  }

}
