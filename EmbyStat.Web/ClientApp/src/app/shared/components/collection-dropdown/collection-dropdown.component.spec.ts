import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CollectionDropdownComponent } from './collection-dropdown.component';

describe('CollectionDropdownComponent', () => {
  let component: CollectionDropdownComponent;
  let fixture: ComponentFixture<CollectionDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CollectionDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CollectionDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
