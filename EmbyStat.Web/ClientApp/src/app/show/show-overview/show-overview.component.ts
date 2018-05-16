import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormControl } from '@angular/forms';

import { ShowFacade } from '../state/facade.show';
import { Collection } from '../../shared/models/collection';
import { ShowChartsService } from '../service/show-charts.service'

@Component({
  selector: 'app-show-overview',
  templateUrl: './show-overview.component.html',
  styleUrls: ['./show-overview.component.scss']
})
export class ShowOverviewComponent implements OnInit {
  public collections$: Observable<Collection[]>;
  public selectedCollections: string[];

  public collectionsFormControl = new FormControl('', { updateOn: 'blur' });

  constructor(private showFacade: ShowFacade, private showChartsService: ShowChartsService) {
    this.collections$ = this.showFacade.getCollections();
  }

  ngOnInit() {
    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });
  }

  onTabChanged(event): void {
    if (event.index === 1) {
      this.showChartsService.changeOpened(true);
    } else {
      this.showChartsService.changeOpened(false);
    }
  }
}
