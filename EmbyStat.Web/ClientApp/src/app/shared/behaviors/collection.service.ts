import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

import { Collection } from '../models/collection';

@Injectable({
  providedIn: 'root'
})
export class CollectionService {
  collectionsSubject = new BehaviorSubject<Collection[]>([]);
  visibleSubject = new BehaviorSubject<boolean>(false);
  placeholderSubject = new BehaviorSubject<string>('');
  selectedCollectionsSubject = new BehaviorSubject<string[]>([]);
  dropdownBlurredSubject = new BehaviorSubject<boolean>(false);

  constructor() { }

  setCollections(list: Collection[]): void {
    this.collectionsSubject.next(list);
  }

  setVisibility(visible: boolean): void {
    this.visibleSubject.next(visible);
  }

  setPlaceholderSubject(placeholder: string): void {
    this.placeholderSubject.next(placeholder);
  }

  resetDropdownValues(): void {
    this.collectionsSubject.next([]);
    this.visibleSubject.next(false);
    this.placeholderSubject.next('');
  }

  setSelectedCollections(list: string[]) {
    this.selectedCollectionsSubject.next(list);
  }

  fireBlurEvent(value: boolean) {
    this.dropdownBlurredSubject.next(value);
  }
}
