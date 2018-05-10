import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormControl } from '@angular/forms';

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

  constructor() { }

  ngOnInit() {
    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });
  }

}
