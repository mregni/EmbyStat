import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CollectionSelectorComponent } from './collection-selector.component';

describe('CollectionSelectorComponent', () => {
  let component: CollectionSelectorComponent;
  let fixture: ComponentFixture<CollectionSelectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CollectionSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CollectionSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
