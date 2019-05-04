import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';

import { CollectionService } from '../../behaviors/collection.service';
import { Collection } from '../../models/collection';

@Component({
  selector: 'app-collection-dropdown',
  templateUrl: './collection-dropdown.component.html',
  styleUrls: ['./collection-dropdown.component.scss']
})
export class CollectionDropdownComponent implements OnInit {
  collections$: Observable<Collection[]>;
  visible$: Observable<boolean>;
  placeholder$: Observable<string>;

  constructor(private readonly collectionSubjectBehavior: CollectionService) {
    this.collections$ = this.collectionSubjectBehavior.collectionsSubject;
    this.visible$ = this.collectionSubjectBehavior.visibleSubject;
    this.placeholder$ = this.collectionSubjectBehavior.placeholderSubject;
  }

  ngOnInit() {
  }

  selectedCollectionsChanged(list: Collection[]): void {
    this.collectionSubjectBehavior.setSelectedCollections(list.map(x => x.id));
  }

  fireBlurEvent() {
    this.collectionSubjectBehavior.fireBlurEvent(true);
  }
}
