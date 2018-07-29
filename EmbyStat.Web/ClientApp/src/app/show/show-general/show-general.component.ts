import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ShowStats } from '../models/showStats';
import { ShowFacade } from '../state/facade.show';
import { LoaderFacade } from '../../shared/components/loader/state/facade.loader';

@Component({
  selector: 'app-show-general',
  templateUrl: './show-general.component.html',
  styleUrls: ['./show-general.component.scss']
})

export class ShowGeneralComponent implements OnInit {
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
    this.stats$ = this.showFacade.getGeneralStats(collection);
  }

  public stats$: Observable<ShowStats>;

  constructor(private showFacade: ShowFacade, private loaderFacade: LoaderFacade) { }

  ngOnInit() {
    
  }
}
