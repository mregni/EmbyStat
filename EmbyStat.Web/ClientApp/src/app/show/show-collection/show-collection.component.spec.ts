import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowCollectionComponent } from './show-collection.component';

describe('ShowCollectionComponent', () => {
  let component: ShowCollectionComponent;
  let fixture: ComponentFixture<ShowCollectionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowCollectionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowCollectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
