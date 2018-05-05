import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardTimespanComponent } from './card-timespan.component';

describe('CardTimespanComponent', () => {
  let component: CardTimespanComponent;
  let fixture: ComponentFixture<CardTimespanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardTimespanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardTimespanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
