import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowChartsComponent } from './show-charts.component';

describe('ShowChartsComponent', () => {
  let component: ShowChartsComponent;
  let fixture: ComponentFixture<ShowChartsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowChartsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowChartsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
