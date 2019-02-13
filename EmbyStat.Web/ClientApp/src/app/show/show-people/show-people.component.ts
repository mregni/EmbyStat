import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';

import { ShowService } from '../service/show.service';
import { PersonStats } from '../../shared/models/person-stats';

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

    this._selectedCollections = collection;
    this.stats$ = this.showService.getPersonStats(collection);
  }

  public stats$: Observable<PersonStats>;

  constructor(private showService: ShowService) { }

  ngOnInit() {
  }

}
