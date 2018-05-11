import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormControl } from '@angular/forms';

import { ShowFacade } from '../state/facade.show';
import { Collection } from '../../shared/models/collection';

@Component({
  selector: 'app-show-overview',
  templateUrl: './show-overview.component.html',
  styleUrls: ['./show-overview.component.scss']
})
export class ShowOverviewComponent implements OnInit {
  public collections$: Observable<Collection[]>;
  public selectedCollections: string[];

  public collectionsFormControl = new FormControl('', { updateOn: 'blur' });

  constructor(private showFacade: ShowFacade) {
    this.collections$ = this.showFacade.getCollections();
  }

  ngOnInit() {
    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });
  }

}
