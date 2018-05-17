import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ShowFacade } from '../state/facade.show';
import { PersonStats } from '../../shared/models/personStats';

@Component({
  selector: 'app-show-people',
  templateUrl: './show-people.component.html',
  styleUrls: ['./show-people.component.scss']
})
export class ShowPeopleComponent implements OnInit {
  private _selectedCollections: string[];

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    console.log("boe");
    this._selectedCollections = collection;
    this.stats$ = this.showFacade.getPersonStats(collection);
  }

  public stats$: Observable<PersonStats>;

  constructor(private showFacade: ShowFacade) { }

  ngOnInit() {
  }

}
